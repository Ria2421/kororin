//-----------------------------------------------------------
// マルチロビーマネージャーテスト用 [ TestMultiLobby.cs ]
// Author:Kenta Nakamoto
//-----------------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestMultiLobbyManager : MonoBehaviour
{
    //-------------------
    // フィールド

    // 通信確立フラグ
    private bool isConnect = false;
    public bool IsConnected { get; private set; }

    // ローカルテスト用
    [SerializeField] int testId = 0;        // 仮ユーザーID
    [SerializeField] string testName = "";  // 仮ユーザー名

    [SerializeField] List<Transform> generatePos = new List<Transform>();   // プレイヤー生成位置

    #region インスタンス

    private static TestMultiLobbyManager instance;
    public static TestMultiLobbyManager Instance
    {
        get { return instance; }
    }

    #endregion

    //-------------------
    // メソッド

    // 起動処理
    private async void Awake()
    {
        if (!LoadingManager.Instance) SceneManager.LoadScene("01_UI_Loading", LoadSceneMode.Additive);

        CharacterManager.Instance.StartPoints = generatePos;

        await RoomModel.Instance.ConnectAsync();
    }

    // 初期処理
    private async void Start()
    {
        await RoomModel.Instance.JoinedAsync("Kororin", testId, testName);

        RoomModel.Instance.OnJoinedUser += OnJoinedUser;
        RoomModel.Instance.OnCreatedRoom += OnCreatedRoom;
        RoomModel.Instance.OnLeavedUser += OnLeavedUser;
    }

    // 更新処理
    void Update()
    {
        
    }

    #region 通知処理

    // ルーム作成通知
    public void OnCreatedRoom()
    {
        Debug.Log("入室");
        isConnect = true;
        CharacterManager.Instance.GenerateAllCharacters();
    }

    // 入室完了通知
    public void OnJoinedUser(JoinedUser joinedUser)
    {
        Debug.Log(joinedUser.UserName + "が入室しました。");
        CharacterManager.Instance.GenerateCharacters(joinedUser.ConnectionId);
    }

    // 退室通知
    public void OnLeavedUser(JoinedUser leavedUser)
    {
        Debug.Log(leavedUser.UserName + "が退室しました。");
    }

    #endregion
}
