using UnityEngine;
/// <summary>
/// 
/// </summary>
public class KeyBoardBallContoroller : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] float moveSpeed;// スピード
    [SerializeField] float smoothing;// 入力のなめらかさ
    [SerializeField] float drag;// 減衰

    Vector3 smoothedInput = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = drag;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        Vector3 input = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            input.z += 1;
        if (Input.GetKey(KeyCode.S))
            input.z -= 1;
        if (Input.GetKey(KeyCode.A))
            input.x -= 1;
        if (Input.GetKey(KeyCode.D))
            input.x += 1;

        // 
        if (input.sqrMagnitude > 1)
            input.Normalize();

        // 
        smoothedInput = Vector3.Lerp(smoothedInput, input, Time.fixedDeltaTime * smoothing);

        // 
        rb.AddForce(smoothedInput * moveSpeed);
    }
}
