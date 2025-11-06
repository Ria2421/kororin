using UnityEngine;
using static Kororin.Shared.Interfaces.StreamingHubs.EnumManager;


public class MultiSceneManager : ManagerBase
{
    public static MultiSceneManager Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null) Instance = this;
    }

    public void OnSelectStage(STAGE_TYPE stage)
    {
        switch (stage)
        {
            case STAGE_TYPE.TopScene:
                LoadingManager.Instance.StartSceneLoad("02_Top");
                break;
            case STAGE_TYPE.Stage01:
                LoadingManager.Instance.StartSceneLoad("01_Stage");
                break;
            case STAGE_TYPE.Stage02:
                LoadingManager.Instance.StartSceneLoad("02_Stage");
                break;
        }
    }

}
