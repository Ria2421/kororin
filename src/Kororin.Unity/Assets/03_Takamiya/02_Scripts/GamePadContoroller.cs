using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ゲームパッドで操作できる用のスクリプト
/// </summary>
public class GamePadContoroller : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] float moveForce;    // スティック入力に対して加える力
    [SerializeField] float maxSpeed;     // 球の最大速度

    [SerializeField] float jumpForce;    // ジャンプ時に加える上方向の力    
    [SerializeField] float jumpCooldown; // 次のジャンプまでの待機時間（秒）  


    private bool isGrounded = false;     // 地面に触れているかフラグ          
    private float lastJumpTime = 0f;     // 最後にジャンプした時間          


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        Move();
        Jump();
    }

    /// <summary>
    /// 左スティックでボールを転がす処理
    /// </summary>
    private void Move()
    {
        // 左スティックの入力値を取得（X：左右, Y：前後）
        Vector2 input = Gamepad.current.leftStick.ReadValue();

        // 入力をワールド座標の方向に変換（Z軸前後）
        Vector3 forceDir = new Vector3(input.x, 0, input.y);

        // スティックが少しでも倒されている場合
        if (forceDir.sqrMagnitude > 0.01f)
        {
            // 速度が最大を超えないよう制限
            if (rb.linearVelocity.magnitude < maxSpeed)
            {
                // 力を加えて転がす
                rb.AddForce(forceDir * moveForce, ForceMode.Force);
            }
        }
    }

    /// <summary>
    /// Aボタンでジャンプ
    /// </summary>
    private void Jump()
    {
        // Aボタン
        bool jumpPressed = Input.GetKeyDown(KeyCode.JoystickButton0);

        // ジャンプ条件：
        // - ボタンが押された
        // - 接地している
        // - クールダウン時間が経過している
        if (jumpPressed && isGrounded && Time.time - lastJumpTime > jumpCooldown)
        {
            // 垂直方向の速度をリセットしてから上に力を加える
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

            // 状態更新
            isGrounded = false;
            lastJumpTime = Time.time;
        }
    }


    /// <summary>
    /// 地面に接地したときの処理
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    /// <summary>
    /// 地面から離れたときの処理
    /// </summary>
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}
