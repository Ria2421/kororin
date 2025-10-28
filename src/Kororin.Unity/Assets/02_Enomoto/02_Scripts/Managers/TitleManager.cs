using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : ManagerBase
{
    public static TitleManager Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        if(Instance == null) Instance = this;
    }

    public void OnButton()
    {
        LoadingManager.Instance.StartSceneLoad("02_Top");
    }
}
