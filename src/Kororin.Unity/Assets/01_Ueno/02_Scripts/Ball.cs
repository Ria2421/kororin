using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using static HedgehogBase;

public class Ball : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] HedgehogBase hedgehog;
    [SerializeField] float deceleratSpeed; // 減速スピード
    [SerializeField] float applyForce;     // 加える力
    [SerializeField] float runSpeedThreshold; // 走るアニメーションに切り替えるスピードの閾値
    [SerializeField] float jumpForce;         // ジャンプ力
    [SerializeField] float knockbackForce;

    private bool canMove; // 移動を許可するかどうかのフラグ
    bool isSphere;        // ボール状態かどうか
    float dx, dz;
    float raycastLength = 1f; // 地面判定用のRayの長さ（Colliderの下端から）
    bool isGrounded = false;

    bool canControl = true;
    public bool CanControl { get { return canControl; } set { canControl = value; } }

    CheckPoint checkPoint;
    public CheckPoint CheckPoint { get { return checkPoint; } set { checkPoint = value; } }

    void Start()
    {
        // Rigidbodyコンポーネントを取得
        rb = GetComponent<Rigidbody>();

        // 減速のためのDrag（抵抗）を設定
        rb.linearDamping = deceleratSpeed;

        isSphere = false;
        canMove = true;
    }

    void Update()
    {
        // プレイヤーの入力を取得
        dx = canControl ? Input.GetAxis("Horizontal") : 0;
        dz = canControl ? Input.GetAxis("Vertical") : 0;

        CheckIsGround();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        if (canMove) AddForce();
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

    public void Jump()
    {
        //if (isSphere)
        //{
        //    hedgehog.SetAnimId((int)Anim_Id.Jump_Ball);
        //}
        //else
        //{
        //    hedgehog.SetAnimId((int)Anim_Id.Jump);
        //}

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    /// <summary>
    /// リスポーン処理
    /// </summary>
    public void OnRespawn()
    {
        transform.eulerAngles = Vector3.zero;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        hedgehog.SetAnimId((int)Anim_Id.Land);

        const float scaleDuration = 0.3f;
        transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutBack);
    }

    /// <summary>
    /// デッドゾーンに触れたら
    /// </summary>
    public void OnDeadZone()
    {
        canControl = false;
        transform.localScale = Vector3.zero;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
    }

    /// <summary>
    /// 当たり判定
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Wall")
        {
            canMove = false;

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

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Wall")
        {
            canMove = true;
        }
    }

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
}
