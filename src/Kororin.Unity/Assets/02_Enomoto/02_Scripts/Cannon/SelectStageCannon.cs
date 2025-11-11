using UnityEngine;
using static Kororin.Shared.Interfaces.StreamingHubs.EnumManager;

public class SelectStageCannon : CannonBase
{
    [SerializeField] STAGE_TYPE stageType;
    GameObject player;

    private void Awake()
    {
        if (stageType == STAGE_TYPE.Rndom)
        {
            int rndId = Random.Range((int)STAGE_TYPE.Stage01, STAGE_TYPE_MAX + 1);
            switch (rndId)
            {
                case (int)STAGE_TYPE.Stage01:
                    stageType = STAGE_TYPE.Stage01;
                    break;
                case (int)STAGE_TYPE.Stage02:
                    stageType = STAGE_TYPE.Stage02;
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            player = other.gameObject;
            player.transform.localScale = Vector3.one * 0.1f;
            player.GetComponent<Ball>().ResetVelocitys(false);
            player.transform.position = transform.position;
            PlayEnterAnim();
        }
    }

    void CallOnSelectStageMethod()
    {
        if (TopManager.Instance)
        {
            TopManager.Instance.OnSelectStage(stageType);
        }
        if (MultiSceneManager.Instance)
        {
            MultiSceneManager.Instance.OnSelectStage(stageType);
        }
    }

    public override void OnEnterAnim()
    {
        PlayFireAnim();
    }

    public override void OnFireAnim()
    {
        player.transform.localScale = Vector3.one;
        Rigidbody rigidbody = player.GetComponent<Rigidbody>();
        rigidbody.useGravity = true;
        rigidbody.AddForce(transform.forward * firePower, ForceMode.Impulse);
        player = null;
        Invoke("CallOnSelectStageMethod", 1);
    }
}
