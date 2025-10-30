using UnityEngine;

public class Ball : HedgehogBase
{
    private Rigidbody rb;

    [SerializeField] float deceleratSpeed; // 減速スピード
    [SerializeField] float applyForce;     // 加える力
    [SerializeField] float runSpeedThreshold; // ★追加：走るアニメーションに切り替えるスピードの閾値
    [SerializeField] float rotationSpeedFactor;// ★追加：回転の速さを調整する係数
    [SerializeField] Transform meshTransform;

    float dx, dz;

    void Start()
    {
        // Rigidbodyコンポーネントを取得
        rb = GetComponent<Rigidbody>();

        // 減速のためのDrag（抵抗）を設定
        rb.linearDamping = deceleratSpeed;
    }

    void Update()
    {
        // プレイヤーの入力を取得
        dx = Input.GetAxis("Horizontal");
        dz = Input.GetAxis("Vertical");

        AddForce();
    }

    public void AddForce()
    {
        // 入力に基づいて移動方向を計算
        var movement = new Vector3(dx, 0, dz);

        // 球体に力を加える
        rb.AddForce(movement * applyForce);

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

        // 現在再生中のアニメーションIDを取得
        Anim_Id currentAnimId = (Anim_Id)GetAnimId();

        // スピードが閾値を超えているか判定
        if (currentSpeed >= runSpeedThreshold)
        {
            // 閾値を超えていて、現在のIDがRun_Ball以外ならRun_Ballに切り替え
            if (currentAnimId != Anim_Id.Run)
            {
                SetAnimId((int)Anim_Id.Run);
            }
        }
        else if (currentSpeed >= runSpeedThreshold * 2)
        {
            // 閾値を超えていて、現在のIDがRun_Ball以外ならRun_Ballに切り替え
            if (currentAnimId != Anim_Id.Run_Ball)
            {
                SetAnimId((int)Anim_Id.Run_Ball);
            }
        }
        else
        {
            // 閾値未満で、現在のIDがIdle_Ball以外ならIdle_Ballに切り替え
            // ただし、Jumpなどのアクションアニメーション中は切り替えない方が良い
            if (currentAnimId == Anim_Id.Run_Ball || currentAnimId == Anim_Id.Idle_Ball)
            {
                if (currentAnimId != Anim_Id.Idle_Ball)
                {
                    SetAnimId((int)Anim_Id.Idle_Ball);
                }
            }
        }
    }
}
