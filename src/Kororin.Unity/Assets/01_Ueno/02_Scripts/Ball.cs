using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] float deceleratSpeed; // �����X�s�[�h
    [SerializeField] float applyForce;     // �������

    float dx, dz;

    void Start()
    {
        // Rigidbody�R���|�[�l���g���擾
        rb = GetComponent<Rigidbody>();

        // �����̂��߂�Drag�i��R�j��ݒ�
        // �l���傫���قǑ�������
        rb.linearDamping = deceleratSpeed;
    }

    void Update()
    {
        // �v���C���[�̓��͂��擾
        dx = Input.GetAxis("Horizontal");
        dz = Input.GetAxis("Vertical");

        AddForce();
    }

    public void AddForce()
    {
        // ���͂Ɋ�Â��Ĉړ��������v�Z
        var movement = new Vector3(dx, 0, dz);

        // ���̂ɗ͂�������
        rb.AddForce(movement * applyForce);
    }
}
