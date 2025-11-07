using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : ManagerBase
{
    public static GameManager Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        SceneManager.LoadScene("03_UI_Game", LoadSceneMode.Additive);
        if (Instance == null) Instance = this;
    }

    public void EndGame()
    {
        LoadingManager.Instance.StartSceneLoad("02_Top");
    }
}
