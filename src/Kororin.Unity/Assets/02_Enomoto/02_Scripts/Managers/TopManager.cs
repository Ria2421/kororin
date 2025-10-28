using UnityEngine;
using static Kororin.Shared.Interfaces.StreamingHubs.EnumManager;

public class TopManager : ManagerBase
{
    public static TopManager Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null) Instance = this;
    }

    public void OnSelectStage(STAGE_TYPE stage)
    {
        switch (stage)
        {
            case STAGE_TYPE.Stage01:
                LoadingManager.Instance.StartSceneLoad("01_Stage");
                break;
            case STAGE_TYPE.Stage02:
                LoadingManager.Instance.StartSceneLoad("02_Stage");
                break;
        }
    }
}
