using System.Collections.Generic;
using UnityEngine;
using static HedgehogBase;

public class Test : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] HedgehogBase hedgehog;
    [SerializeField] float deceleratSpeed; // 減速スピード
    [SerializeField] float applyForce;     // 加える力
    [SerializeField] float runSpeedThreshold; // 走るアニメーションに切り替えるスピードの閾値
    [SerializeField] float rotationSpeed = 10f; // インスペクターで設定

    // ────────── Joy-Con設定 ──────────
    List<Joycon> joycons;
    Joycon joyconL;
    Joycon joyconR;

    [SerializeField] float tiltSensitivity = 2f;   // 傾き感度
    [SerializeField] float smoothing = 5f;         // 入力のなめらかさ
    [SerializeField] float deadZone = 0.5f;        // デッドゾーン（小さな傾き無視）
    [SerializeField] float maxSpeed = 5f;          // 最大速度制限
    Vector3 smoothedInput = Vector3.zero;

    //ジャンプ用の調整パラメータ
    [SerializeField] float jumpForce = 5.0f;         // ジャンプ力
    [SerializeField] float jumpThreshold = 0.8f;     // 振り上げを検知するY加速度のしきい値
    [SerializeField] float jumpCooldown = 1.0f;      // 次のジャンプまでの待機時間（秒）

    bool isSphere;
    float dx, dz;
    float lastJumpTime = 0f;                       // 最後にジャンプした時間
    bool canControl = true;
    bool isGrounded = false;                         // 地面に触れているか
    public bool CanControl { get { return canControl; } set { canControl = value; } }

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
            Debug.LogWarning("Joy-Conが見つかりませんでした！ → WASDで操作します");
        }

    }

    void Update()
    {
        if (!canControl) { dx = dz = 0; return; }

        // Joy-Conが接続されていればJoy-Con入力優先
        if ((joyconL != null && joyconL.state != null) || (joyconR != null && joyconR.state != null))
        {
            Vector3 accelL = joyconL != null ? joyconL.GetAccel() : Vector3.zero;
            Vector3 accelR = joyconR != null ? joyconR.GetAccel() : Vector3.zero;

            // 両方の平均（片方しかない場合はそのまま）
            Vector3 accel = (accelL + accelR) / ((joyconL != null && joyconR != null) ? 2f : 1f);

            // ── 振り上げでジャンプ（左右どちらでもOK） ──
            bool jumpDetected =
                (joyconL != null && joyconL.GetAccel().y > jumpThreshold) ||
                (joyconR != null && -joyconR.GetAccel().y > jumpThreshold);

            if (jumpDetected && isGrounded && Time.time - lastJumpTime > jumpCooldown)
            {
                Jump();
            }

            // ノイズ除去
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
            dx = canControl ? Input.GetAxis("Horizontal") : 0;
            dz = canControl ? Input.GetAxis("Vertical") : 0;
        }
    }

    private void FixedUpdate()
    {
        AddForce();
    }

    void Jump()
    {
        // 下向き速度リセットして上に力を加える
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

        isGrounded = false;
        lastJumpTime = Time.time;
    }

    public void AddForce()
    {
        if (dx == 0 && dz == 0) return;
        UpdateMovementAnimation();

        var movement = new Vector3(dx, 0, dz).normalized;
        rb.AddForce(movement * applyForce, ForceMode.Force);

        if (isSphere)
        {
            //rb.constraints &= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

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
                Quaternion targetRotation = Quaternion.LookRotation(-movement);
                transform.rotation = targetRotation;

                // X, Z軸の傾きを強制的に0にリセットし、Y軸の向きを確定
                Vector3 currentEuler = transform.eulerAngles;
                transform.rotation = Quaternion.Euler(0f, currentEuler.y, 0f);
            }

            rb.AddForce(movement * applyForce, ForceMode.Force);
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

    //地面判定
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
