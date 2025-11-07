using System;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    const float invokeTime = 0.5f;
    Ball ball;
    CheckPoint checkPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            var ball = other.GetComponent<Ball>();
            if (ball)
            {
                ball.OnDeadZone();
                this.ball = ball;
                this.checkPoint = ball.CheckPoint;
                Invoke("CallRespawnMethod", invokeTime);
            }
        }
    }

    void CallRespawnMethod()
    {
        if(checkPoint == null) return;
        checkPoint.Respawn(ball);
    }
}
