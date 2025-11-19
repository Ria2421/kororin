using DG.Tweening;
using Pixeye.Unity;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static HedgehogBase;

public class NakamotoBall : MonoBehaviour
{
    //-------------------
    // フィールド

    #region 基礎設定

    [Foldout("基礎設定")]
    [SerializeField] private NakamotoHedgehogBase hedgehog;
    [Foldout("基礎設定")]
    [SerializeField] private float deceleratSpeed;      // 減速スピード
    [Foldout("基礎設定")]
    [SerializeField] private float applyForce;          // 加える力
    [Foldout("基礎設定")]
    [SerializeField] private float runSpeedThreshold;   // 走るアニメーションに切り替えるスピードの閾値
    [Foldout("基礎設定")]
    [SerializeField] private float knockbackForce;      // ノックバック力
    [Foldout("基礎設定")]
    [SerializeField] private float stopThreshold;       // 転がってからの速度がどのくらい小さくなったら完全に停止するのか

    #endregion

    #region ジャンプ関連

    [Foldout("ジャンプ関連")]
    [SerializeField] private float jumpForce;       // ジャンプ力
    [Foldout("ジャンプ関連")]
    [SerializeField] private float jumpCooldown;    // 次のジャンプまでの待機時間（秒）

    private float lastJumpTime = 0f;                // 最後にジャンプした時間

    #endregion

    #region ゲームパッド関連

    [Foldout("ゲームパッド関連")]
    [SerializeField] float stickDeadZone;     // デッドゾーン

    #endregion

    #region ジョイコン関連

    private List<Joycon> joycons;
    private Joycon joyconL;
    private Joycon joyconR;
    private Vector3 smoothedInput = Vector3.zero;
    [Foldout("ジョイコン関連")]
    [SerializeField] private GameObject JoyConManager;  // JoyConManagerのプレハブ
    [Foldout("ジョイコン関連")]
    [SerializeField] private float tiltSensitivity;     // 傾き感度
    [Foldout("ジョイコン関連")]
    [SerializeField] private float smoothing;           // 入力のなめらかさ
    [Foldout("ジョイコン関連")]
    [SerializeField] private float deadZone;            // デッドゾーン（小さな傾き無視）頭は.2くらい
    [Foldout("ジョイコン関連")]
    [SerializeField] float jumpThreshold;     // 振り上げを検知するY加速度のしきい値

    #endregion

    #region 動作用フラグ

    private bool isSphere;          // ボール状態かどうか
    private bool isGrounded;        // 地面との接触判定
    private bool isGoal = false;    // ゴールしたかどうか

    // 自身を判別する変数
    private bool isSelf = true;
    public bool IsSelf { get { return isSelf; } set { isSelf = value; } }

    // 操作可能かどうか
    private bool canControl = false;
    public bool CanControl { get { return canControl; } set { canControl = value; } }

    #endregion

    #region 環境変数

    private Rigidbody rb;
    private float dx, dz;
    private float raycastLength = 1f; // 地面判定用のRayの長さ（Colliderの下端から）

    // チェックポイント
    CheckPoint checkPoint;
    public CheckPoint CheckPoint { get { return checkPoint; } set { checkPoint = value; } }

    #endregion

    //-------------------
    // メソッド

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
    /// 初期処理
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
    /// 更新処理
    /// </summary>
    void Update()
    {
        // プレイヤーの入力を取得
        dx = canControl ? Input.GetAxis("Horizontal") : 0;
        dz = canControl ? Input.GetAxis("Vertical") : 0;

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

        // 地面接触判定
        CheckIsGround();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && Time.time - lastJumpTime > jumpCooldown)
        {
            Jump();
        }
    }

    /// <summary>
    /// 定期更新処理
    /// </summary>
    private void FixedUpdate()
    {
        if (!canControl) return;

        // 入力がある場合は移動
        if (Mathf.Abs(dx) > 0.05f || Mathf.Abs(dz) > 0.05f)
        {
            AddForce();
        }
        else
        {
            ApplyStopCheck(); // 入力がないとき減速チェック
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
    /// 移動処理
    /// </summary>
    public void AddForce()
    {
        if (dx == 0 && dz == 0) return;
        UpdateMovementAnimation();

        var movement = new Vector3(dx, 0, dz).normalized;
        rb.AddForce(movement * applyForce, ForceMode.Force);

        rb.constraints &= ~RigidbodyConstraints.FreezeRotationX;
        rb.constraints &= ~RigidbodyConstraints.FreezeRotationZ;

        if (isSphere)
        {
            // 入力に基づいて移動方向を計算
            movement = new Vector3(dx, 0, dz);
            // 球体に力を加える
            rb.AddForce(movement * applyForce);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            movement = new Vector3(dx, 0, dz).normalized;

            // 入力がある場合のみ回転
            if (movement.magnitude >= 0.1f)
            {
                // キー入力方向へ瞬時回転
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                transform.rotation = targetRotation;
            }
            // X, Z軸の傾きを強制的に0にリセットし、Y軸の向きを確定
            Vector3 currentEuler = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, currentEuler.y, 0f);

            rb.AddForce(movement * applyForce, ForceMode.Force);
        }
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
    /// 移動スピードに基づいてアニメーションを更新する
    /// </summary>
    private void UpdateMovementAnimation()
    {
        // Y軸方向の速度を除外し、平面での移動速度（ベクトルの大きさ）を取得
        // Rigidbody.velocity.magnitude は全体の速度
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        float currentSpeed = horizontalVelocity.magnitude;

        // スピードが閾値を超えているか判定
        if (currentSpeed >= runSpeedThreshold * 1.2f)
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
    /// ジャンプ
    /// </summary>
    public void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        lastJumpTime = Time.time;
    }

    /// <summary>
    /// リスポーン処理
    /// </summary>
    public void OnRespawn()
    {
        ResetVelocitys(true);
        hedgehog.SetAnimId((int)Anim_Id.Land);

        const float scaleDuration = 0.3f;
        hedgehog.transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutBack);
    }

    /// <summary>
    /// デッドゾーンに触れたら
    /// </summary>
    public void OnDeadZone()
    {
        canControl = false;
        hedgehog.transform.localScale = Vector3.zero;

        ResetVelocitys(false);
    }

    /// <summary>
    /// Rigidbodyの現在の速度をリセットする
    /// </summary>
    public void ResetVelocitys(bool useGravity = true)
    {
        if(rb == null) return;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = useGravity;
        transform.eulerAngles = Vector3.zero;
    }

    #region 当たり判定

    /// <summary>
    /// 当たり判定
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Wall")
        {
            //canControl = false;

            if (rb == null) return;

            rb.constraints |= RigidbodyConstraints.FreezeRotationX;
            rb.constraints |= RigidbodyConstraints.FreezeRotationZ;

            // ★ 速度をゼロにして、衝突による移動慣性を完全に消去
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero; // 回転速度もゼロに

            // ノックバックの方向を計算
            Vector3 knockbackDirection = -transform.forward;

            // Y軸方向のノックバックを抑える
            knockbackDirection.y = 0;

            // ノックバックの力を加える (瞬間的に速度を変える)
            rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode.VelocityChange);

            hedgehog.SetAnimId((int)Anim_Id.Idle);

            rb.rotation = Quaternion.identity;
        }
    }

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Wall")
    //    {
    //        canControl = true;
    //    }
    //}

    /// <summary>
    /// トリガー接触判定
    /// </summary>
    /// <param name="other"></param>
    private async void OnTriggerEnter(Collider other)
    {
        if (!isSelf) return;

        if (!isGoal && other.tag == "Goal")
        {
            isGoal = true;
            await RoomModel.Instance.ArrivalGoalAsync();
        }
    }

    #endregion

    /// <summary>
    /// 地面との接触確認
    /// </summary>
    public void CheckIsGround()
    {
        RaycastHit hit;

        // プレイヤーの中心から真下に向けてRaycastを発射
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastLength))
        {
            // Rayが何かに当たった場合、そのオブジェクトのタグをチェック
            if (hit.collider.CompareTag("Ground"))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }
        else
        {
            isGrounded = false;
        }
    }

    /// <summary>
    /// 再生中のアニメーションID取得
    /// </summary>
    /// <returns></returns>
    public int GetAnimId()
    {
        return hedgehog.GetAnimId();
    }

    /// <summary>
    /// 指定したアニメーションを再生
    /// </summary>
    /// <param name="id"></param>
    public void SetAnimId(int id)
    {
        hedgehog.SetAnimId(id);
    }

    /// <summary>
    /// スポーン状況の取得
    /// </summary>
    /// <returns></returns>
    public bool GetIsSpawn()
    {
        return hedgehog.IsSpawn;
    }
}
