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
    public bool IsConnect {  get { return isConnect; } }

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
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // インスタンスが複数存在しないように、既に存在していたら自身を消去する
            Destroy(gameObject);
        }

        if (!LoadingManager.Instance) SceneManager.LoadScene("01_UI_Loading", LoadSceneMode.Additive);
    }

    // 初期処理
    private async void Start()
    {
        // 生成位置の設定
        CharacterManager.Instance.StartPoints = generatePos;

        RoomModel.Instance.OnJoinedUser += OnJoinedUser;
        RoomModel.Instance.OnCreatedRoom += OnCreatedRoom;
        RoomModel.Instance.OnLeavedUser += OnLeavedUser;

        await RoomModel.Instance.ConnectAsync();

        await RoomModel.Instance.JoinedAsync("Kororin", testId, testName);
    }

    private void OnDisable()
    {
        if (!RoomModel.Instance) return;
        StopAllCoroutines();

        // シーン遷移したときに登録した通知処理を解除
        RoomModel.Instance.OnJoinedUser -= OnJoinedUser;
        RoomModel.Instance.OnCreatedRoom -= OnCreatedRoom;
        RoomModel.Instance.OnLeavedUser -= OnLeavedUser;
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
