using Shared.Interfaces.StreamingHubs;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float deceleratSpeed; // �����X�s�[�h
    [SerializeField] private float applyForce;     // �������
    [SerializeField] protected Animator animator;

    private float dx, dz;

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

    /// <summary>
    /// �A�j���[�V����ID�擾����
    /// </summary>
    /// <returns></returns>
    public int GetAnimId()
    {
        return animator != null ? animator.GetInteger("animation_id") : 0;
    }
}
