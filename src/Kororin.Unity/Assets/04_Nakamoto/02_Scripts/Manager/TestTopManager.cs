//--------------------------------------------------
// トップ画面通信テスト用 [ TestTopManager.cs ]
// Author:Kenta Nakamoto
//--------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestTopManager : MonoBehaviour
{
    [SerializeField] int testId = 0;
    [SerializeField] string testName = "";

    public static TestTopManager Instance { get; private set; }

    // 起動処理
    private async void Awake()
    {
        if (!LoadingManager.Instance) SceneManager.LoadScene("01_UI_Loading", LoadSceneMode.Additive);
        if (Instance == null) Instance = this;

        await RoomModel.Instance.ConnectAsync();
    }

    // 初期処理
    void Start()
    {
        RoomModel.Instance.OnJoinedUser += OnJoinedUser;
        RoomModel.Instance.OnCreatedRoom += OnCreatedRoom;
        RoomModel.Instance.OnLeavedUser += OnLeavedUser;
    }

    // シングルモード遷移
    public void TransitionSinglScene()
    {
        LoadingManager.Instance.StartSceneLoad("01_Stage");
    }

    // マルチモード遷移
    public async void TransitionMultiScene()
    {
        await RoomModel.Instance.JoinedAsync("Kororin",testId,testName);
    }

    #region 通知処理

    // ルーム作成通知
    public void OnCreatedRoom()
    {
        Debug.Log("入室");
    }

    // 入室完了通知
    public void OnJoinedUser(JoinedUser joinedUser)
    {
        Debug.Log(joinedUser.UserName + "が入室しました。");
    }

    // 退室通知
    public void OnLeavedUser(JoinedUser leavedUser)
    {
        Debug.Log(leavedUser.UserName + "が退室しました。");
    }

    #endregion

}
