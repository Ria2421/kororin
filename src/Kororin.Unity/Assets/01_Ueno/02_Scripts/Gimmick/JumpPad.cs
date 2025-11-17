//------------------------------------------
// ジャンプギミック [ JumpPad.cs ]
// Author:Souma Ueno
//------------------------------------------
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    // ジャンプ力
    public float launchForce = 30f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>();

            playerRb.AddForce(Vector3.up * launchForce, ForceMode.Impulse);

            //transform.position = 
            //    new Vector3(transform.position.x, 
            //    -0.5f,
            //    transform.position.z);
        }
    }
}
