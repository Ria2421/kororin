using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static HedgehogBase;


public class Test : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] GameObject JoyConManager;  //JoyConManagerのプレハブ

    //==============================
    // 移動の設定
    //==============================
    [SerializeField] HedgehogBase hedgehog;
    [SerializeField] float deceleratSpeed;      // 減速スピード
    [SerializeField] float applyForce;          // 加える力
    [SerializeField] float runSpeedThreshold;   // 走るアニメーションに切り替えるスピードの閾値
    [SerializeField] float rotationSpeed;       // インスペクターで設定

    //==============================
    // Joy-Con関連設定
    //==============================
    List<Joycon> joycons;
    Joycon joyconL;
    Joycon joyconR;
    [SerializeField] float tiltSensitivity;   // 傾き感度
    [SerializeField] float smoothing;         // 入力のなめらかさ
    [SerializeField] float deadZone;          // デッドゾーン（小さな傾き無視）
    Vector3 smoothedInput = Vector3.zero;

    //==============================
    // ゲームパッド関連設定
    //==============================
    [SerializeField] float stickDeadZone;     // デッドゾーン
    [SerializeField] float stopThreshold;     // 転がってからの速度がどのくらい小さくなったら完全に停止するのか

    //==============================
    // ジャンプ関連設定
    //==============================
    [SerializeField] float jumpForce;         // ジャンプ力
    [SerializeField] float jumpThreshold;     // 振り上げを検知するY加速度のしきい値
    [SerializeField] float jumpCooldown;      // 次のジャンプまでの待機時間（秒）

    //==============================
    // 状態管理用変数
    //==============================
    bool isSphere;                            // 球体の状態
    float dx, dz;                             // 入力方向
    float lastJumpTime = 0f;                  // 最後にジャンプした時間
    bool canControl = true;                   // 操作可能かどうか
    bool isGrounded = false;                  // 地面に触れているかフラグ

    public bool CanControl { get { return canControl; } set { canControl = value; } }

    /// <summary>
    /// ゲーム起動時
    /// </summary>
    private void Awake()
    {
        // すでにシーン内に存在しているなら生成しない
        if (JoyconManager.Instance == null)
        {
            GameObject obj = Instantiate(JoyConManager);
            obj.name = "JoyconManager";
            DontDestroyOnLoad(obj);
            Debug.Log("JoyconManagerのPrefabを自動生成しました");
        }
        else
        {
            Debug.Log("既にJoyconManagerが存在しています");
        }
    }

    /// <summary>
    /// ゲーム開始時
    /// </summary>
    void Start()
    {
        // Rigidbodyコンポーネントを取得
        rb = GetComponent<Rigidbody>();

        // 減速のためのDrag（抵抗）を設定
        rb.linearDamping = deceleratSpeed;

        isSphere = false;

        // Joy-Con初期化
        joycons = JoyconManager.Instance.j;
        if (joycons != null && joycons.Count > 0)
        {
            joyconL = joycons.Find(c => c.isLeft);
            joyconR = joycons.Find(c => !c.isLeft);
        }
        else
        {
            Debug.LogWarning("Joy-Conが見つかりませんでした!ゲームパッドで操作してください");
        }

    }

    /// <summary>
    /// 毎フレーム更新
    /// </summary>
    void Update()
    {
        //JoyCon優先
        bool useJoyCon = (joyconL != null && joyconL.state != null) || (joyconR != null && joyconR.state != null);

        if (useJoyCon)
        {
            InputJoyCon();//InputJoyCon関数呼び出し
        }
        else
        {
            InputGamepad();//InputGamepad関数呼び出し
        }

        dx = canControl ? Input.GetAxis("Horizontal") : 0;
        dz = canControl ? Input.GetAxis("Vertical") : 0;
    }

    private void FixedUpdate()
    {
        // 入力がある場合は移動
        if (Mathf.Abs(dx) > 0.05f || Mathf.Abs(dz) > 0.05f)
        {
            AddForce();
        }
        else
        {
            ApplyStopCheck(); // 入力がないとき減速チェック
        }

        // 角速度の制限
        if (rb.angularVelocity.magnitude > 10f)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * 10f;
        }

        // 入力がなくても速度に応じてアニメーション更新
        UpdateMovementAnimation();
    }

    /// <summary>
    /// 減速処理(スティックを離したとき)
    /// </summary>
    void ApplyStopCheck()
    {
        // 入力がないかどうか
        bool noInput = Mathf.Abs(dx) < 0.05f && Mathf.Abs(dz) < 0.05f;

        if (noInput)
        {
            // 横方向の速度を取得
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            // 徐々に減速
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity,
                new Vector3(0, rb.linearVelocity.y, 0),
                Time.fixedDeltaTime * 5f); // 減速

            // ある程度止まったら完全停止
            if (flatVel.magnitude < stopThreshold)
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
                rb.angularVelocity = Vector3.zero;
                hedgehog.SetAnimId((int)Anim_Id.Idle);
            }
        }
    }

    /// <summary>
    /// Joy-Conの入力処理
    /// </summary>
    private void InputJoyCon()
    {
        if (!canControl) { dx = dz = 0; return; }

        // Joy-Conが接続されていればJoy-Con入力優先
        if ((joyconL != null && joyconL.state != null) || (joyconR != null && joyconR.state != null))
        {
            // 左右Joy-Conの加速度を取得（存在しない場合はゼロ）
            Vector3 accelL = joyconL != null ? joyconL.GetAccel() : Vector3.zero;
            Vector3 accelR = joyconR != null ? joyconR.GetAccel() : Vector3.zero;

            // 両方の平均（片方しかない場合はそのまま）
            Vector3 accel = (accelL + accelR) / ((joyconL != null && joyconR != null) ? 2f : 1f);

            // 左右どちらかのJoy-Conがしきい値を超えたらジャンプ
            bool jumpDetected =
                (joyconL != null && joyconL.GetAccel().y > jumpThreshold) ||
                (joyconR != null && -joyconR.GetAccel().y > jumpThreshold);

            // ジャンプ条件：振り上げ検知＆地面に接地＆クールタイム経過
            if (jumpDetected && isGrounded && Time.time - lastJumpTime > jumpCooldown)
            {
                Jump();
            }

            // デッドゾーン処理（小さな傾きは無視）
            if (Mathf.Abs(accel.x) < deadZone) accel.x = 0;
            if (Mathf.Abs(accel.y) < deadZone) accel.y = 0;

            // 軸変換（Joy-Conを傾けた方向に進む）
            Vector3 input = new Vector3(accel.x, 0, -accel.y) * tiltSensitivity;

            // スムージング
            smoothedInput = Vector3.Lerp(smoothedInput, input, Time.deltaTime * smoothing);

            dx = smoothedInput.x;
            dz = smoothedInput.z;
        }
        else
        {
            // プレイヤーの入力を取得
            /*dx = canControl ? Input.GetAxis("Horizontal") : 0;
            dz = canControl ? Input.GetAxis("Vertical") : 0;*/
        }
    }

    /// <summary>
    /// ジャンプ処理
    /// </summary>
    void Jump()
    {

        // 下向き速度リセット
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        //瞬時に上方向に力を加える
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

        isGrounded = false;
        lastJumpTime = Time.time;
    }

    /// <summary>
    /// 力を加えて移動
    /// </summary>
    public void AddForce()
    {
        if (dx == 0 && dz == 0) return;
        UpdateMovementAnimation();

        // 入力方向を正規化
        var movement = new Vector3(dx, 0, dz).normalized;

        //瞬時に力を加える
        rb.AddForce(movement * applyForce, ForceMode.Force);

        if (isSphere)
        {
            // 入力に基づいて移動方向を計算
            movement = new Vector3(dx, 0, dz);
            // 球体に力を加える
            rb.AddForce(movement * applyForce);
        }
        else
        {
            if (isGrounded)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                movement = new Vector3(dx, 0, dz).normalized;

                // 入力がある場合のみ回転
                if (movement.magnitude >= 0.1f)
                {
                    // キー入力方向へ瞬時回転
                    Quaternion targetRotation = Quaternion.LookRotation(movement);
                    transform.rotation = targetRotation;

                    // X, Z軸の傾きを強制的に0にリセットし、Y軸の向きを確定
                    Vector3 currentEuler = transform.eulerAngles;
                    transform.rotation = Quaternion.Euler(0f, currentEuler.y, 0f);
                }
            }
            rb.AddForce(movement * applyForce, ForceMode.Force);
        }
    }

    /// <summary>
    /// 通常のゲームパッド入力処理
    /// </summary>
    void InputGamepad()
    {
        if (Gamepad.current == null) return;

        // 左スティック入力
        Vector2 stick = Gamepad.current.leftStick.ReadValue();

        if (stick.magnitude < stickDeadZone)
        {
            dx = 0;
            dz = 0;
        }
        else
        {
            dx = stick.x;
            dz = stick.y;
        }


        // Aボタンでジャンプ
        if (Gamepad.current.buttonSouth.wasPressedThisFrame && isGrounded && Time.time - lastJumpTime > jumpCooldown)
        {
            Jump();
        }
    }

    /// <summary>
    /// 移動スピードに基づいてアニメーションを更新する
    /// </summary>
    private void UpdateMovementAnimation()
    {
        if (!isGrounded)
        {
            hedgehog.SetAnimId((int)Anim_Id.Run_Ball);
            return;
        }

        // Y軸方向の速度を除外し、平面での移動速度（ベクトルの大きさ）を取得
        // Rigidbody.velocity.magnitude は全体の速度
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        float currentSpeed = horizontalVelocity.magnitude;


        // スピードが閾値を超えているか判定
        if (currentSpeed >= runSpeedThreshold * 1.5f)
        {
            hedgehog.SetAnimId((int)Anim_Id.Run_Ball);
            isSphere = true;
        }
        else if (currentSpeed >= 0.5f)
        {
            hedgehog.SetAnimId((int)Anim_Id.Run);
            isSphere = false;
        }
        else
        {
            hedgehog.SetAnimId((int)Anim_Id.Idle);
        }
    }

    /// <summary>
    /// 地面に接地したときの処理
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    /// <summary>
    /// 地面から離れたときの処理
    /// </summary>
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
