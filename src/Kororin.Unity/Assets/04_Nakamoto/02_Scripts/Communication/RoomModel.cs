//-------------------------------------------------------------
// RoomHubへの接続管理 [ RoomModel.cs ]
// Author:中本健太
//-------------------------------------------------------------
#region using一覧
using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion;
using MagicOnion.Client;
using Kororin.Shared.Interfaces.StreamingHubs;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static Kororin.Shared.Interfaces.StreamingHubs.EnumManager;
using Vector2 = UnityEngine.Vector2;
using System.Runtime.InteropServices;
#endregion

public class RoomModel : BaseModel, IRoomHubReceiver
{
    //-----------------------
    // フィールド

    private GrpcChannel channel;  // サーバーURL
    private IRoomHub roomHub;     // roomHubの関数を呼び出す時に使う

    // 通信確立フラグ
    private bool isConnect = false;
    public bool IsConnect { get { return isConnect; } set { isConnect = value; } }

    // マスタークライアントかどうか
    public bool IsMaster { get; set; }

    // 接続ID
    public Guid ConnectionId { get; private set; }

    // ユーザーID
    [SerializeField] private int userID = 0;
    public int UserId { get { return userID; } set { userID = value; } }

    // ユーザー名
    [SerializeField] private string userName = "";
    public string UserName { get { return userName; } set { userName = value; } }

    // 現在の参加者情報
    public Dictionary<Guid, JoinedUser> joinedUserList { get; private set; } = new Dictionary<Guid, JoinedUser>();

    // 現在のルーム情報
    public RoomData[] roomDataList { get; set; }

    #region 通知定義一覧

    #region システム

    // ルーム検索通知
    public Action<List<string>, List<string>, List<string>> OnSearchedRoom { get; set; }

    // ルーム生成通知
    public Action OnCreatedRoom { get; set; }

    // ユーザー接続通知
    public Action<JoinedUser> OnJoinedUser { get; set; }

    // 入室失敗通知
    public Action<int> OnFailedJoinSyn { get; set; }

    // ユーザー退室通知
    public Action<JoinedUser> OnLeavedUser { get; set; }

    // 準備完了通知
    public Action<Guid> OnStoodby { get; set; }

    // ゲーム開始通知
    public Action<int> OnStartedGame { get; set; }

    // 易度上昇通知
    public Action<int> OnAscendDifficultySyn { get; set; }

    // 次ステージ進行通知
    public Action<STAGE_TYPE> OnAdanceNextStageSyn { get; set; }

    // ステージ進行通知
    public Action OnAdvancedStageSyn { get; set; }

    ////ゲーム終了通知
    //public Action<ResultData> OnGameEndSyn { get; set; }

    #endregion

    #region インゲームシステム

    // カウント開始通知
    public Action OnStartedCount { get; set; }

    // ゲート開放通知
    public Action OnOpenedGate { get; set; }

    // ゴール通知
    public Action<Guid> OnGoaledPlayer { get; set; }

    // リザルト通知
    public Action<Dictionary<Guid,JoinedUser>> OnResulted {  get; set; }

    #endregion

    #region プレイヤー・マスタクライアント

    //マスタークライアントの変更通知
    public Action OnChangedMasterClient { get; set; }

    ////マスタークライアントの更新通知
    public Action<MasterClientData> OnUpdatedMasterClient { get; set; }

    //プレイヤー位置回転通知
    public Action<CharacterData> OnUpdatedCharacter { get; set; }

    //プレイヤーダウン通知
    public Action<Guid> OnPlayerDeadSyn { get; set; }

    public Action<Guid, bool> OnBeamEffectActived { get; set; }

    #endregion

    #region ギミック

    //ギミックの起動通知
    public Action<string, bool> OnBootedGimmick { get; set; }

    #endregion

    #endregion

    #region RoomModelインスタンス生成

    private static RoomModel instance;
    public static RoomModel Instance
    {
        get
        {
            // GETプロパティを呼ばれたときにインスタンスを作成する(初回のみ)
            if (instance == null)
            {
                GameObject gameObj = new GameObject("RoomModel" + DateTime.Now.ToString());
                instance = gameObj.AddComponent<RoomModel>();
                DontDestroyOnLoad(gameObj);
            }
            return instance;
        }
    }

    #endregion

    //-----------------------
    // メソッド

    /// <summary>
    /// 起動処理
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // インスタンスが複数存在しないように、既に存在していたら自身を消去する
            Destroy(gameObject);
        }
    }

    #region MagicOnion接続・切断処理

    /// <summary>
    /// MagicOnion接続処理
    /// </summary>
    /// <returns></returns>
    public async UniTask ConnectAsync()
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        roomHub = await StreamingHubClient.ConnectAsync<IRoomHub, IRoomHubReceiver>(channel, this);
    }

    /// <summary>
    /// MagicOnion切断処理
    /// </summary>
    /// <returns></returns>
    public async UniTask DisconnectAsync()
    {
        if (roomHub != null) await roomHub.DisposeAsync();
        if (channel != null) await channel.ShutdownAsync();
        roomHub = null; channel = null;
    }

    /// <summary>
    /// 破棄処理
    /// </summary>
    async void OnDestroy()
    {
        DisconnectAsync();
        instance = null;
    }

    #endregion

    #region 通知の処理

    #region 入室・退室・準備完了通知

    /// <summary>
    /// ルーム生成通知
    /// </summary>
    /// <returns></returns>
    public void OnRoom()
    {
        OnCreatedRoom();
    }

    /// <summary>
    /// 入室通知(IRoomHubReceiverインターフェイスの実装)
    /// </summary>
    /// <param name="joinedUser"></param>
    public void Onjoin(JoinedUser joinedUser)
    {
        // データ被りを回避
        if (!joinedUserList.ContainsKey(joinedUser.ConnectionId))
            joinedUserList.Add(joinedUser.ConnectionId, joinedUser);

        //入室通知
        OnJoinedUser(joinedUser);
    }

    public void OnFailedJoin(int errorId)
    {
        OnFailedJoinSyn(errorId);
    }

    /// <summary>
    /// 退室通知
    /// </summary>
    /// <param name="user"></param>
    public void OnLeave(Dictionary<Guid, JoinedUser> joinedUser, Guid targetUser)
    {
        int i = 1;
        JoinedUser leaveUser = joinedUser[targetUser];
        joinedUserList = joinedUser;
        joinedUserList.Remove(targetUser);
        foreach (var user in joinedUserList)
        {
            user.Value.JoinOrder = i;
            i++;
        }

        OnLeavedUser(leaveUser);
    }

    /// <summary>
    /// 準備完了通知
    /// </summary>
    /// <param name="conID"></param>
    public void OnStandby(Guid guid)
    {
        joinedUserList[guid].IsReady = true;
        OnStoodby(guid);
    }

    #endregion

    #region インゲームシステム通知

    /// <summary>
    /// カウント開始通知
    /// </summary>
    public void OnStartCount()
    {
        OnStartedCount();
    }

    /// <summary>
    /// ゲート開放通知
    /// </summary>
    public void OnOpenGate()
    {
        OnOpenedGate();
    }

    /// <summary>
    /// ゴール通知
    /// </summary>
    /// <param name="connectionID"></param>
    public void OnGoalPlayer(Guid connectionID)
    {
        OnGoaledPlayer(connectionID);
    }

    /// <summary>
    /// リザルト通知
    /// </summary>
    /// <param name="result"></param>
    public void OnResult(Dictionary<Guid, JoinedUser> result)
    {
        isConnect = false;
        joinedUserList = result;
        OnResulted(result);
    }

    #endregion

    #region プレイヤー通知関連

    /// <summary>
    /// プレイヤーの移動通知
    /// </summary>
    /// <param name="user"></param>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    /// <param name="animID"></param>
    public void OnUpdateCharacter(CharacterData charaData)
    {
        OnUpdatedCharacter(charaData);
    }

    /// <summary>
    /// マスタークライアントの変更通知
    /// </summary>
    public void OnChangeMasterClient()
    {
        OnChangedMasterClient();
        Debug.Log("あなたがマスタークライアントになりました");
        IsMaster = true;
    }

    /// <summary>
    /// マスタークライアントの更新通知
    /// </summary>
    /// <param name="masterClientData"></param>
    public void OnUpdateMasterClient(MasterClientData masterClientData)
    {
        OnUpdatedMasterClient(masterClientData);
    }

    #endregion

    #region ゲーム内UI・仕様の同期関連

    /// <summary>
    /// ゲーム開始通知
    /// </summary>
    public void OnStartGame(int id)
    {
        OnStartedGame(id);
    }

    /// <summary>
    /// ギミックの起動通知
    /// </summary>
    /// <param name="gimmickData"></param>
    public void OnBootGimmick(string uniqueID, bool triggerOnce)
    {
        OnBootedGimmick(uniqueID, triggerOnce);
    }

    /// <summary>
    /// アイテム獲得通知
    /// </summary>
    public void OnGetItem(Guid conId, string itemID, int nowLevel, int nowExp, int nextLevelExp)
    {
        //OnGetItemSyn(conId, itemID, nowLevel, nowExp, nextLevelExp);
    }

    #endregion

    #endregion

    #region リクエスト関連

    #region 入室からゲーム開始まで

    /// <summary>
    /// 入室同期
    /// </summary>
    /// <returns></returns>
    public async UniTask JoinedAsync(string roomName, int userId, string userName)
    {
        joinedUserList = await roomHub.JoinedAsync(roomName,userId,userName);
        if (joinedUserList == null) return;
        foreach (var user in joinedUserList)
        {
            if (user.Value.UserId == userId)
            {
                this.ConnectionId = user.Value.ConnectionId;
                this.IsMaster = user.Value.IsMaster;
            }
            Debug.Log(user.Value.UserName + "が参加");
        }

        CharacterManager.Instance.GenerateAllCharacters();
    }

    /// <summary>
    /// 退室の同期
    /// </summary>
    /// <returns></returns>
    public async UniTask LeavedAsync()
    {
        await roomHub.LeavedAsync(false);
        this.IsMaster = false;
        //自分をリストから消す
        joinedUserList.Clear();
    }

    /// <summary>
    /// 準備完了同期
    /// </summary>
    /// <returns></returns>
    public async UniTask StandbyAsync()
    {
        await roomHub.StandbyAsync();
    }

    #endregion

    #region インゲーム

    #region システム

    /// <summary>
    /// インゲーム遷移完了
    /// </summary>
    /// <returns></returns>
    public async UniTask TransitionInGameAsync()
    {
        await roomHub.TransitionInGameAsync();
    }

    /// <summary>
    /// カウント終了
    /// </summary>
    /// <returns></returns>
    public async UniTask CountEndAsync()
    {
        await roomHub.CountEndAsync();
    }

    /// <summary>
    /// ゴール到達
    /// </summary>
    /// <returns></returns>
    public async UniTask ArrivalGoalAsync()
    {
        await roomHub.ArrivalGoalAsync();
    }

    #endregion

    #region プレイヤー関連
    /// <summary>
    /// キャラの更新同期
    /// </summary>
    /// <param name="playerData"></param>
    /// <returns></returns>
    public async UniTask UpdateCharacterAsync(CharacterData charaData)
    {
        await roomHub.UpdateCharacterAsync(charaData);
    }

    /// <summary>
    /// マスタークライアントの更新同期
    /// </summary>
    /// <param name="masterClient"></param>
    /// <returns></returns>
    public async UniTask UpdateMasterClientAsync(MasterClientData masterClient)
    {
        await roomHub.UpdateMasterClientAsync(masterClient);
    }

    #endregion

    #region ゲーム内UI、仕様関連

    /// <summary>
    /// ギミックの起動同期
    /// </summary>
    /// <param name="uniqueID"></param>
    /// <returns></returns>
    public async UniTask BootGimmickAsync(string uniqueID, bool triggerOnce)
    {
        await roomHub.BootGimmickAsync(uniqueID, triggerOnce);
    }

    ///// <summary>
    ///// オブジェクト生成リクエスト
    ///// </summary>
    ///// <returns></returns>
    //public async UniTask SpawnObjectAsync(OBJECT_TYPE type, Vector2 spawnPos)
    //{
    //    await roomHub.SpawnObjectAsync(type, spawnPos);
    //}

    #endregion

    #endregion

    #endregion
}
