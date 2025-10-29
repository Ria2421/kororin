using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] float deceleratSpeed; // 減速スピード
    [SerializeField] float applyForce;     // 加える力

    float dx, dz;

    void Start()
    {
        // Rigidbodyコンポーネントを取得
        rb = GetComponent<Rigidbody>();

        // 減速のためのDrag（抵抗）を設定
        // 値が大きいほど早く減速
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
    }
}
