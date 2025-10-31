using UnityEngine;
using static HedgehogBase;

public class Ball : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] GameObject playerObj;
    [SerializeField] float deceleratSpeed; // 減速スピード
    [SerializeField] float applyForce;     // 加える力
    [SerializeField] float runSpeedThreshold; // 走るアニメーションに切り替えるスピードの閾値
    [SerializeField] float rotationSpeed = 10f; // インスペクターで設定

    HedgehogBase hedgehog;

    bool isMoveType;

    float dx, dz;

    void Start()
    {
        // Rigidbodyコンポーネントを取得
        rb = GetComponent<Rigidbody>();

        // 減速のためのDrag（抵抗）を設定
        rb.linearDamping = deceleratSpeed;

        hedgehog = playerObj.GetComponent<HedgehogBase>();

        isMoveType = false;
    }

    void Update()
    {
        // プレイヤーの入力を取得
        dx = Input.GetAxis("Horizontal");
        dz = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        AddForce();
    }

    public void AddForce()
    {
        var movement = new Vector3(dx, 0, dz).normalized;
        rb.AddForce(movement * applyForce, ForceMode.Force);

        if (isMoveType)
        {
            rb.constraints &= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            // 1. ターゲットとなる回転を作成
            
            // 入力に基づいて移動方向を計算
            movement = new Vector3(dx, 0, dz);
            // 球体に力を加える
            rb.AddForce(movement * applyForce);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            movement = new Vector3(dx, 0, dz).normalized;

            //// inputDirectionを「前」とする回転
            //Quaternion targetRotation = Quaternion.LookRotation(movement);

            //// 2. 現在の回転からターゲットの回転へ滑らかに補間
            //transform.rotation = Quaternion.Slerp(
            //    transform.rotation,
            //    targetRotation,
            //    rotationSpeed * Time.fixedDeltaTime
            //);


            rb.AddForce(movement * applyForce, ForceMode.Force);
        }

        UpdateMovementAnimation();
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

        //// 現在再生中のアニメーションIDを取得
        //Anim_Id currentAnimId = (Anim_Id)GetAnimId();

        // スピードが閾値を超えているか判定
        if (currentSpeed >= runSpeedThreshold * 2)
        {
            hedgehog.SetAnimId((int)Anim_Id.Run_Ball);
            isMoveType = true;
        }
        else if (currentSpeed >= 0.5f)
        {
            hedgehog.SetAnimId((int)Anim_Id.Run);
            isMoveType = false;
        }
        else
        {
            hedgehog.SetAnimId((int)Anim_Id.Idle);
        }
    }
}
