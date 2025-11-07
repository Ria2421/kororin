using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField]
    GameObject shockWaveParticle;

    [SerializeField]
    Vector3 respawnPos;
    public Vector3 RespawnPos { get { return respawnPos; } }

    private void Awake()
    {
        respawnPos = respawnPos == Vector3.zero ? transform.position : respawnPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 3)
        {
            var ball = other.GetComponent<Ball>();
            if(ball && ball.CheckPoint != this)
            {
                ball.CheckPoint = this;
                var particle = Instantiate(shockWaveParticle);
                particle.transform.position = ball.transform.position;
            }
        }
    }
}
