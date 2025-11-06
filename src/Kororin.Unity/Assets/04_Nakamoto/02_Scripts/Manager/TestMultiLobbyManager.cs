//-----------------------------------------------------------
// 試験用マルチロビーマネージャー [ TestMultiLobby.cs ]
// Author:Kenta Nakamoto
//-----------------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestMultiLobbyManager : MonoBehaviour
{
    //-------------------
    // フィールド

    // スタンバイフラグ
    private bool isStandby = false;
    public bool IsStandby { get { return isStandby; } set { isStandby = value; } }

    // ローカルテスト用
    [SerializeField] private int testId = 0;        // 仮ユーザーID
    [SerializeField] private string testName = "";  // 仮ユーザー名

    [SerializeField] private List<Transform> generatePos = new List<Transform>();   // プレイヤー生成位置

    #region インスタンス

    private static TestMultiLobbyManager instance;
    public static TestMultiLobbyManager Instance
    {
        get { return instance; }
    }

    #endregion

    //-------------------
    // メソッド

    /// <summary>
    /// 起動処理
    /// </summary>
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

    /// <summary>
    /// 初期処理
    /// </summary>
    private async void Start()
    {
        // 生成位置の設定
        CharacterManager.Instance.StartPoints = generatePos;

        RoomModel.Instance.OnJoinedUser += OnJoinedUser;
        RoomModel.Instance.OnCreatedRoom += OnCreatedRoom;
        RoomModel.Instance.OnLeavedUser += OnLeavedUser;
        RoomModel.Instance.OnStoodby += OnStoodby;
        RoomModel.Instance.OnStartedGame += OnStartedGame;
        RoomModel.Instance.OnChangedMasterClient += OnChangedMasterClient;

        await RoomModel.Instance.ConnectAsync();

        await RoomModel.Instance.JoinedAsync("Kororin", testId, testName);
    }

    /// <summary>
    /// 切断時の処理
    /// </summary>
    private void OnDisable()
    {
        if (!RoomModel.Instance) return;
        StopAllCoroutines();

        // シーン遷移したときに登録した通知処理を解除
        RoomModel.Instance.OnJoinedUser -= OnJoinedUser;
        RoomModel.Instance.OnCreatedRoom -= OnCreatedRoom;
        RoomModel.Instance.OnLeavedUser -= OnLeavedUser;
        RoomModel.Instance.OnStoodby -= OnStoodby;
        RoomModel.Instance.OnStartedGame -= OnStartedGame;
        RoomModel.Instance.OnChangedMasterClient -= OnChangedMasterClient;
    }

    #region 通知処理

    /// <summary>
    /// ルーム作成通知
    /// </summary>
    public void OnCreatedRoom()
    {
        Debug.Log("入室");
        RoomModel.Instance.IsConnect = true;
    }

    /// <summary>
    /// 入室完了通知
    /// </summary>
    /// <param name="joinedUser"></param>
    public void OnJoinedUser(JoinedUser joinedUser)
    {
        Debug.Log(joinedUser.UserName + "が入室しました。");
        CharacterManager.Instance.GenerateCharacters(joinedUser);
    }

    /// <summary>
    /// 退室通知
    /// </summary>
    /// <param name="leavedUser"></param>
    public void OnLeavedUser(JoinedUser leavedUser)
    {
        // 退出ユーザーのオブジェを削除
        var leaveObj = CharacterManager.Instance.PlayerObjs[leavedUser.ConnectionId];
        CharacterManager.Instance.PlayerObjs.Remove(leavedUser.ConnectionId);
        Destroy(leaveObj);

        Debug.Log(leavedUser.UserName + "が退室しました。");
    }

    /// <summary>
    /// マスター変更通知
    /// </summary>
    public void OnChangedMasterClient()
    {
        // マスターになった時の処理
    }

    /// <summary>
    /// 準備完了通知
    /// </summary>
    /// <param name="guid"></param>
    public void OnStoodby(Guid guid)
    {
        Debug.Log(RoomModel.Instance.joinedUserList[guid].UserName + "、準備完了！");
    }

    /// <summary>
    /// ゲーム開始通知
    /// </summary>
    public void OnStartedGame()
    {
        // インゲームシーンに移動
        RoomModel.Instance.IsConnect = false;
        Debug.Log("ゲーム開始");
        LoadingManager.Instance.StartSceneLoad("04_SampleStage");
    }

    #endregion
}
