//-----------------------------------------------------------
// 試験用マルチロビーマネージャー [ TestMultiLobby.cs ]
// Author:Kenta Nakamoto
//-----------------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestMultiLobbyManager : MonoBehaviour
{
    //-------------------
    // フィールド

    // スタンバイフラグ
    private bool isStandby = false;
    public bool IsStandby { get { return isStandby; } set { isStandby = value; } }

    [SerializeField] private List<Transform> generatePos = new List<Transform>();   // プレイヤー生成位置
    [SerializeField] private Text rankText; // 順位表示用テキスト

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
        // 順位非表示
        rankText.text = "";

        // 生成位置の設定
        CharacterManager.Instance.StartPoints = generatePos;

        RoomModel.Instance.OnJoinedUser += OnJoinedUser;
        RoomModel.Instance.OnCreatedRoom += OnCreatedRoom;
        RoomModel.Instance.OnLeavedUser += OnLeavedUser;
        RoomModel.Instance.OnStoodby += OnStoodby;
        RoomModel.Instance.OnStartedGame += OnStartedGame;
        RoomModel.Instance.OnChangedMasterClient += OnChangedMasterClient;

        if (RoomModel.Instance.joinedUserList.Count == 0)
        {
            await RoomModel.Instance.ConnectAsync();

#if UNITY_EDITOR
            await RoomModel.Instance.JoinedAsync("Kororin", RoomModel.Instance.UserId, RoomModel.Instance.UserName);
#else
            await RoomModel.Instance.JoinedAsync("Kororin", APIModel.Instance.Id, APIModel.Instance.Name);
#endif
        }
        else
        {
            // 順位表示
            string[] strings = new string[RoomModel.Instance.joinedUserList.Count];
            foreach (var user in RoomModel.Instance.joinedUserList)
            {
                strings[user.Value.Rank - 1] = user.Value.UserName;
            }
            for (int i=0; i < RoomModel.Instance.joinedUserList.Count;i++)
            {
                rankText.text = rankText.text + (i+1).ToString() + "位：" + strings[i] + "　"; 
            }

            // キャラ生成
            CharacterManager.Instance.GenerateAllCharacters();
            // 接続フラグオン
            RoomModel.Instance.IsConnect = true;
        }
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
    public void OnStartedGame(int id)
    {
        // インゲームシーンに移動
        RoomModel.Instance.IsConnect = false;
        Debug.Log("ステージ" + id + "：ゲーム開始");

        //++ そのうち引数のidのステージに遷移

        LoadingManager.Instance.StartSceneLoad("04_SampleStage");
    }

    #endregion
}
