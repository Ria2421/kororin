using UnityEngine;

public class GameManager : ManagerBase
{
    protected override void Awake()
    {
        base.Awake();
    }

    public void EndGame()
    {
        LoadingManager.Instance.StartSceneLoad("02_Top");
    }
}
