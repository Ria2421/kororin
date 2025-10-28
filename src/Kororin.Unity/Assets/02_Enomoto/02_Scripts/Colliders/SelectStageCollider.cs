using UnityEngine;
using static Kororin.Shared.Interfaces.StreamingHubs.EnumManager;
using Kororin.Shared.Interfaces.StreamingHubs;

public class SelectStageCollider : MonoBehaviour
{
    [SerializeField] STAGE_TYPE stageType;

    private void Awake()
    {
        if(stageType == STAGE_TYPE.Rndom)
        {
            int rndId = Random.Range(0, STAGE_TYPE_MAX + 1);
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
        TopManager.Instance.OnSelectStage(stageType);
    }
}
