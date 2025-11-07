using System;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            var ball = other.GetComponent<Ball>();
            if (ball)
            {
                ball.CanControl = false;
                GameUIManager.Instance.PlayFade(() => { ball.Respawn(); });
            }
        }
    }
}
