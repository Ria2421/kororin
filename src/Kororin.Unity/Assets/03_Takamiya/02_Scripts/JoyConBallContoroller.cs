using UnityEngine;
using System.Collections.Generic;

public class JoyConBallContoroller : MonoBehaviour
{
    Rigidbody rb;
    List<Joycon> joycons;
    Joycon joyconL;
    Joycon joyconR;

    [SerializeField] float tiltSensitivity;  // �X���ɑ΂��銴�x
    [SerializeField] float moveSpeed;        // �{�[���̓]����X�s�[�h
    [SerializeField] float smoothing;        // ���͂̂Ȃ߂炩��
    [SerializeField] float drag;             // ���C�i�~�܂�₷���j

    Vector3 smoothedInput = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = drag;

        // JoyCon�擾
        joycons = JoyconManager.Instance.j;
        if (joycons.Count > 0)
        {
            joyconL = joycons.Find(c => c.isLeft);
            /*joyconR = joycons.Find(c => !c.isLeft);*/
        }
        else
        {
            Debug.LogWarning("Joy-Con��������܂���ł����I");
        }
    }

    void FixedUpdate()
    {
        if (joycons.Count == 0) return;

        Vector3 accelL = Vector3.zero;
        Vector3 accelR = Vector3.zero;
        int activeCount = 0;

        // ��Joy-Con
        if (joyconL != null)
        {
            accelL = joyconL.GetAccel();
            activeCount++;
        }

        // �EJoy-Con
        if (joyconR != null)
        {
            accelR = joyconR.GetAccel();
            activeCount++;
        }

        // �ǂ��炩���L���Ȃ畽�ς����
        Vector3 accel = (accelL + accelR) / Mathf.Max(activeCount, 1);

        // ������
        Vector3 input = new Vector3(accel.x, 0, -accel.y) * tiltSensitivity;

        // �X���[�Y���
        smoothedInput = Vector3.Lerp(smoothedInput, input, Time.fixedDeltaTime * smoothing);

        // �͂�������
        rb.AddForce(smoothedInput * moveSpeed);
    }
}
