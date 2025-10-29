//-------------------------------------------------------------
// RoomHub�ւ̐ڑ��Ǘ�
// Aughter:���{����
//-------------------------------------------------------------
#region using�ꗗ
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
#endregion

public class RoomModel : BaseModel, IRoomHubReceiver
{
    private GrpcChannel channel;  // �T�[�o�[URL
    private IRoomHub roomHub;     // roomHub�̊֐����Ăяo�����Ɏg��

    // �}�X�^�[�N���C�A���g���ǂ���
    public bool IsMaster { get; set; }

    // �ڑ�ID
    public Guid ConnectionId { get; private set; }

    // ���[�U�[ID
    public int userId { get; set; }

    // ���݂̎Q���ҏ��
    public Dictionary<Guid, JoinedUser> joinedUserList { get; private set; } = new Dictionary<Guid, JoinedUser>();

    // ���݂̃��[�����
    public RoomData[] roomDataList { get; set; }

    #region �ʒm��`�ꗗ

    #region �V�X�e��

    // ���[�������ʒm
    public Action<List<string>, List<string>, List<string>> OnSearchedRoom { get; set; }

    // ���[�������ʒm
    public Action OnCreatedRoom { get; set; }

    // ���[�U�[�ڑ��ʒm
    public Action<JoinedUser> OnJoinedUser { get; set; }

    // �������s�ʒm
    public Action<int> OnFailedJoinSyn { get; set; }

    // ���[�U�[�ގ��ʒm
    public Action<JoinedUser> OnLeavedUser { get; set; }

    // �L�����N�^�[�ύX�ʒm
    public Action<Guid, int> OnChangedCharacter { get; set; }

    // ���������ʒm
    public Action<Guid> OnReadySyn { get; set; }

    // �Q�[���J�n�ʒm
    public Action OnStartedGame { get; set; }

    // �Փx�㏸�ʒm
    public Action<int> OnAscendDifficultySyn { get; set; }

    // ���X�e�[�W�i�s�ʒm
    public Action<STAGE_TYPE> OnAdanceNextStageSyn { get; set; }

    // �X�e�[�W�i�s�ʒm
    public Action OnAdvancedStageSyn { get; set; }

    ////�Q�[���I���ʒm
    //public Action<ResultData> OnGameEndSyn { get; set; }

    #endregion

    #region �v���C���[�E�}�X�^�N���C�A���g

    //�}�X�^�[�N���C�A���g�̕ύX�ʒm
    public Action OnChangedMasterClient { get; set; }

    ////�}�X�^�[�N���C�A���g�̍X�V�ʒm
    //public Action<MasterClientData> OnUpdateMasterClientSyn { get; set; }

    ////�v���C���[�ʒu��]�ʒm
    //public Action<PlayerData> OnUpdatePlayerSyn { get; set; }

    //�v���C���[�_�E���ʒm
    public Action<Guid> OnPlayerDeadSyn { get; set; }

    public Action<Guid, bool> OnBeamEffectActived { get; set; }

    #endregion

    #region �M�~�b�N

    //�M�~�b�N�̋N���ʒm
    public Action<string, bool> OnBootedGimmick { get; set; }

    #endregion

    #endregion

    #region RoomModel�C���X�^���X����
    private static RoomModel instance;
    public static RoomModel Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // �C���X�^���X���������݂��Ȃ��悤�ɁA���ɑ��݂��Ă����玩�g����������
            Destroy(gameObject);
        }
    }

    //public static RoomModel Instance
    //{
    //    get
    //    {
    //        // GET�v���p�e�B���Ă΂ꂽ�Ƃ��ɃC���X�^���X���쐬����(����̂�)
    //        if (instance == null)
    //        {
    //            GameObject gameObj = new GameObject("RoomModel"+DateTime.Now.ToString());
    //            instance = gameObj.AddComponent<RoomModel>();
    //            DontDestroyOnLoad(gameObj);
    //        }
    //        return instance;
    //    }
    //}
    #endregion

    #region MagicOnion�ڑ��E�ؒf����
    /// <summary>
    /// MagicOnion�ڑ�����
    /// Aughter:�ؓc�W��
    /// </summary>
    /// <returns></returns>
    public async UniTask ConnectAsync()
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        roomHub = await StreamingHubClient.ConnectAsync<IRoomHub, IRoomHubReceiver>(channel, this);
    }

    /// <summary>
    /// MagicOnion�ؒf����
    /// Aughter:�ؓc�W��
    /// </summary>
    /// <returns></returns>
    public async UniTask DisconnectAsync()
    {
        if (roomHub != null) await roomHub.DisposeAsync();
        if (channel != null) await channel.ShutdownAsync();
        roomHub = null; channel = null;
    }
    #endregion

    /// <summary>
    /// �j������
    /// Aughter:�ؓc�W��
    /// </summary>
    async void OnDestroy()
    {
        DisconnectAsync();
        instance = null;
    }

    #region �ʒm�̏���

    ///// <summary>
    ///// �����J�n
    ///// Aughtor:�ؓc�W��
    ///// </summary>
    //public void OnSameStart(List<TerminalData> list)
    //{
    //    OnSameStartSyn(list);
    //}

    ///// <summary>
    ///// �Q�[���I���ʒm
    ///// </summary>
    ///// <param name="result"></param>
    //public async void OnGameEnd(ResultData result)
    //{
    //    OnGameEndSyn(result);
    //    await roomHub.LeavedAsync(true);
    //}

    #region �����E�ގ��E���������ʒm

    /// <summary>
    /// ���[�������ʒm
    /// Aughter:�ؓc�W��
    /// </summary>
    /// <param name="roomName"></param>
    /// <param name="userName"></param>
    public void OnSearchRoom(RoomData[] roomDatas)
    {
        List<string> roomNameList = new List<string>();
        List<string> userNameList = new List<string>();
        List<string> passWordList = new List<string>();

        foreach (RoomData roomData in roomDatas)
        {
            roomNameList.Add(roomData.roomName);
            userNameList.Add(roomData.userName);
            passWordList.Add(roomData.passWord);
        }

        OnSearchedRoom(roomNameList, userNameList, passWordList);
    }

    /// <summary>
    /// ���[�������ʒm
    /// Aughter:�ؓc�W��
    /// </summary>
    /// <returns></returns>
    public void OnRoom()
    {
        OnCreatedRoom();
    }

    /// <summary>
    /// �����ʒm(IRoomHubReceiver�C���^�[�t�F�C�X�̎���)
    /// Aughter:�ؓc�W��
    /// </summary>
    /// <param name="joinedUser"></param>
    public void Onjoin(JoinedUser joinedUser)
    {
        // �f�[�^�������
        if (!joinedUserList.ContainsKey(joinedUser.ConnectionId))
            joinedUserList.Add(joinedUser.ConnectionId, joinedUser);

        //�����ʒm
        OnJoinedUser(joinedUser);
    }

    public void OnFailedJoin(int errorId)
    {
        OnFailedJoinSyn(errorId);
    }

    /// <summary>
    /// �ގ��ʒm
    /// Aughter:�ؓc�W��
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
    /// ���������ʒm
    /// Aughter:�ؓc�W��
    /// </summary>
    /// <param name="conID"></param>
    public void OnReady(JoinedUser joinedUser)
    {
        joinedUserList[joinedUser.ConnectionId] = joinedUser;
        OnReadySyn(joinedUser.ConnectionId);
    }
    #endregion

    #region �v���C���[�ʒm�֘A
    ///// <summary>
    ///// �v���C���[�̈ړ��ʒm
    ///// Aughter:�ؓc�W��
    ///// </summary>
    ///// <param name="user"></param>
    ///// <param name="pos"></param>
    ///// <param name="rot"></param>
    ///// <param name="animID"></param>
    //public void OnUpdatePlayer(PlayerData playerData)
    //{
    //    OnUpdatePlayerSyn(playerData);
    //}

    /// <summary>
    /// �}�X�^�[�N���C�A���g�̕ύX�ʒm
    /// Aughter:�ؓc�W��
    /// </summary>
    public void OnChangeMasterClient()
    {
        OnChangedMasterClient();
        Debug.Log("���Ȃ����}�X�^�[�N���C�A���g�ɂȂ�܂���");
        IsMaster = true;
    }

    ///// <summary>
    ///// �}�X�^�[�N���C�A���g�̍X�V�ʒm
    ///// Aughter:�ؓc�W��
    ///// </summary>
    ///// <param name="masterClientData"></param>
    //public void OnUpdateMasterClient(MasterClientData masterClientData)
    //{
    //    OnUpdateMasterClientSyn(masterClientData);
    //}

    #endregion

    #region �Q�[����UI�E�d�l�̓����֘A
    /// <summary>
    /// �Q�[���J�n�ʒm
    /// Aughter:�ؓc�W��
    /// </summary>
    public void OnStartGame()
    {
        OnStartedGame();
    }

    /// <summary>
    /// �M�~�b�N�̋N���ʒm
    /// Aughter:�ؓc�W��
    /// </summary>
    /// <param name="gimmickData"></param>
    public void OnBootGimmick(string uniqueID, bool triggerOnce)
    {
        OnBootedGimmick(uniqueID, triggerOnce);
    }

    /// <summary>
    /// �A�C�e���l���ʒm
    /// Author:�ؓc�W��
    /// </summary>
    public void OnGetItem(Guid conId, string itemID, int nowLevel, int nowExp, int nextLevelExp)
    {
        //OnGetItemSyn(conId, itemID, nowLevel, nowExp, nextLevelExp);
    }

    #endregion

    #endregion

    #region ���N�G�X�g�֘A

    #region ��������Q�[���J�n�܂�

    /// <summary>
    /// ��������
    /// Aughter:�ؓc�W��
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
            Debug.Log(user.Value.UserName + "���Q��");
        }
    }

    /// <summary>
    /// �ގ��̓���
    /// Aughter:�ؓc�W��
    /// </summary>
    /// <returns></returns>
    public async UniTask LeavedAsync()
    {
        await roomHub.LeavedAsync(false);
        this.IsMaster = false;
        //���������X�g�������
        joinedUserList.Clear();
    }

    /// <summary>
    /// ������������
    /// </summary>
    /// <returns></returns>
    public async UniTask ReadyAsync(int characterId)
    {
        await roomHub.ReadyAsync(characterId);
    }

    ///// <summary>
    ///// �X�^�[�g
    ///// Aughter:�ؓc�W��
    ///// </summary>
    ///// <param name="hostName"></param>
    ///// <returns></returns>
    //public async Task StartRoomAsync(string hostName)
    //{
    //    if (TitleManagerk.GameMode == 0) return;
    //    var handler = new YetAnotherHttpHandler() { Http2Only = true };
    //    var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
    //    var client = MagicOnionClient.Create<IRoomService>(channel);

    //    await client.StartRoom(hostName);
    //}
    #endregion

    #region �Q�[����

    #region �v���C���[�֘A
    ///// <summary>
    ///// �v���C���[�̍X�V����
    ///// </summary>
    ///// <param name="playerData"></param>
    ///// <returns></returns>
    //public async UniTask UpdatePlayerAsync(PlayerData playerData)
    //{
    //    await roomHub.UpdatePlayerAsync(playerData);
    //}

    ///// <summary>
    ///// �}�X�^�[�N���C�A���g�̍X�V����
    ///// Aughter:�ؓc�W��
    ///// </summary>
    ///// <param name="masterClient"></param>
    ///// <returns></returns>
    //public async UniTask UpdateMasterClientAsync(MasterClientData masterClient)
    //{
    //    await roomHub.UpdateMasterClientAsync(masterClient);
    //}

    #endregion

    #region �Q�[����UI�A�d�l�֘A
    ///// <summary>
    ///// �M�~�b�N�̋N������
    /////  Aughter:�ؓc�W��
    ///// </summary>
    ///// <param name="uniqueID"></param>
    ///// <returns></returns>
    //public async UniTask BootGimmickAsync(string uniqueID, bool triggerOnce)
    //{
    //    await roomHub.BootGimmickAsync(uniqueID, triggerOnce);
    //}

    ///// <summary>
    ///// �X�e�[�W�N���A
    ///// Author:Nishiura
    ///// </summary>
    ///// <param name="isAdvance">�X�e�[�W�i�s����</param>
    ///// <returns></returns>
    //public async UniTask StageClear(bool isAdvance)
    //{
    //    await roomHub.StageClear(isAdvance);
    //}

    ///// <summary>
    ///// �X�e�[�W�i�s�����̓���
    ///// </summary>
    ///// <returns></returns>
    //public async UniTask AdvancedStageAsync()
    //{
    //    await roomHub.AdvancedStageAsync();
    //}

    ///// <summary>
    ///// �I�u�W�F�N�g�������N�G�X�g
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
