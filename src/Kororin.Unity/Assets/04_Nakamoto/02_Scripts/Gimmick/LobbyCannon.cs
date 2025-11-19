using UnityEngine;
using static Kororin.Shared.Interfaces.StreamingHubs.EnumManager;

public class LobbyCannon : CannonBase
{
    [SerializeField] string sceneName;
    GameObject player;

    private async void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            player = other.gameObject;
            player.transform.localScale = Vector3.one * 0.1f;
            player.GetComponent<NakamotoBall>().CanControl = false;
            player.GetComponent<NakamotoBall>().ResetVelocitys(false);
            player.transform.position = transform.position;
            await RoomModel.Instance.StandbyAsync();
            //PlayEnterAnim();
        }
    }

    void CallOnSelectStageMethod()
    {
        RoomModel.Instance.IsConnect = false;
        LoadingManager.Instance.StartSceneLoad(sceneName);
    }

    public override void OnEnterAnim()
    {
        PlayFireAnim();
    }

    public override void OnFireAnim()
    {
        if (player == null) return;

        player.transform.localScale = Vector3.one;
        Rigidbody rigidbody = player.GetComponent<Rigidbody>();
        rigidbody.useGravity = true;
        rigidbody.AddForce(transform.forward * firePower, ForceMode.Impulse);
        player = null;
        Invoke("CallOnSelectStageMethod", 1);
    }
}
