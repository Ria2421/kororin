//---------------------------------------------------
// キャラクター管理 [ CharacterManager.cs ]
// Author:Kenta Nakamoto
//---------------------------------------------------
using DG.Tweening;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CharacterManager : MonoBehaviour
{
    //-----------------------
    // フィールド

    // 各プレイヤーの初期位置
    private List<Transform> startPoints = new List<Transform>();   
    public List<Transform> StartPoints {  get { return startPoints; } set { startPoints = value; } }

    // 参加者のプレイヤーオブジェリスト
    private Dictionary<Guid, GameObject> playerObjs = new Dictionary<Guid, GameObject>();

    public Dictionary<Guid, GameObject> PlayerObjs { get { return playerObjs; } }

    [SerializeField] private GameObject playerPrefab;   // 生成するプレイヤーオブジェ

    [SerializeField] private GameObject playerObjSelf;  // ローカル用に属性付与
    public GameObject PlayerObjSelf { get { return playerObjSelf; } }

    static CharacterManager instance;
    public static CharacterManager Instance
    {
        get
        {
            return instance;
        }
    }

    //------------
    // 定数

    private const float UPDATE_SEC = 0.1f;  // 通信頻度

    //-----------------------
    // メソッド

    // 起動処理
    private void Awake()
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

        // 既にステージに配置されているプレイヤーを削除し、参加人数分プレイヤー生成
        //DestroyExistingPlayer();
        //GenerateCharacters();
    }

    // 初期処理
    private void Start()
    {
        if (!RoomModel.Instance) return;
        StartCoroutine("UpdateCoroutine");

        // 通知処理を登録
        RoomModel.Instance.OnUpdatedCharacter += this.OnUpdateCharacter;
        //RoomModel.Instance.OnUpdateMasterClientSyn += this.OnUpdateMasterClient;
        //RoomModel.Instance.OnLeavedUser += this.OnLeave;
        //RoomModel.Instance.OnEnemyHealthSyn += this.OnHitEnemy;
        //RoomModel.Instance.OnChangedMasterClient += this.ActivateAllEnemies;
        //RoomModel.Instance.OnShootedBullet += this.OnShootedBullet;
        //RoomModel.Instance.OnUpdateStatusSyn += this.OnUpdatePlayerStatus;
        //RoomModel.Instance.OnLevelUpSyn += this.OnLevelup;
        //RoomModel.Instance.OnPlayerDeadSyn += this.OnPlayerDead;
        //RoomModel.Instance.OnBeamEffectActived += this.OnBeamEffectActived;
        //RoomModel.Instance.OnDeleteEnemySyn += this.OnDestroyEnemy;
    }

    private void OnDisable()
    {
        if (!RoomModel.Instance) return;
        StopAllCoroutines();

        // シーン遷移したときに登録した通知処理を解除
        RoomModel.Instance.OnUpdatedCharacter -= this.OnUpdateCharacter;
    }

    /// <summary>
    /// 入室者のプレイヤーオブジェの生成
    /// </summary>
    /// <param name="connectionID"></param>
    public void GenerateCharacters(Guid connectionID)
    {
        // 開始位置の設定
        var point = startPoints[0];
        startPoints.RemoveAt(0);

        var playerObj = Instantiate(playerPrefab, point.position, Quaternion.identity);
        playerObjs.Add(connectionID, playerObj);

        playerObj.GetComponent<Player>().enabled = false;

        StartCoroutine("UpdateCoroutine");
    }

    /// <summary>
    /// 参加しているユーザー情報を元に、全員のプレイヤーを生成する
    /// </summary>
    public void GenerateAllCharacters()
    {
        foreach (var joinduser in RoomModel.Instance.joinedUserList)
        {
            // 開始位置の設定
            var point = startPoints[0];
            startPoints.RemoveAt(0);

            var playerObj = Instantiate(playerPrefab, point.position, Quaternion.identity);
            playerObjs.Add(joinduser.Key, playerObj);

            // 自身のプレイヤーを生成した場合
            if (joinduser.Key == RoomModel.Instance.ConnectionId)
            {
                playerObjSelf = playerObj;

                playerObj.GetComponent<Player>().IsSelf = true;

                // カメラの追従設定
                //if (cinemachineTargetGroup)
                //{
                //    var newTarget = new CinemachineTargetGroup.Target
                //    {
                //        Object = playerObjSelf.transform,
                //        Radius = 1f,
                //        Weight = 1f
                //    };
                //    cinemachineTargetGroup.Targets.Add(newTarget);
                //}
                //else
                //{
                //    var target = new CameraTarget();
                //    target.TrackingTarget = playerObjSelf.transform;
                //    target.LookAtTarget = playerObjSelf.transform;
                //    camera.GetComponent<CinemachineCamera>().Target.TrackingTarget = playerObjSelf.transform;
                //}
            }
            else
            {
                playerObj.GetComponent<Player>().enabled = false;
            }
        }
    }

    /// <summary>
    /// キャラクターの情報更新呼び出し用コルーチン
    /// </summary>
    /// <returns></returns>
    public IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (TestMultiLobbyManager.Instance.IsConnect)
            {
                UpdateCharacterDataRequest();
            }
            yield return new WaitForSeconds(UPDATE_SEC);
        }
    }

    /// <summary>
    /// プレイヤーの情報更新
    /// </summary>
    async void UpdateCharacterDataRequest()
    {
        var playerData = GetCharacterData();

        // プレイヤー情報更新リクエスト
        await RoomModel.Instance.UpdateCharacterAsync(playerData);
    }

    /// <summary>
    /// プレイヤー情報取得
    /// </summary>
    /// <returns></returns>
    CharacterData GetCharacterData()
    {
        if (!playerObjs.ContainsKey(RoomModel.Instance.ConnectionId)) return null;
        var player = playerObjs[RoomModel.Instance.ConnectionId].GetComponent<Player>();
        return new CharacterData()
        {
            Position = player.transform.position,
            Scale = player.transform.localScale,
            Rotation = player.transform.rotation,
            AnimationId = player.GetAnimId(),

            // 以下は専用変数
            ConnectionID = RoomModel.Instance.ConnectionId,
        };
    }

    /// <summary>
    /// キャラの更新処理
    /// </summary>
    /// <param name="characterData"></param>
    void UpdateCharacter(CharacterData characterData,GameObject playerObj)
    {
        playerObj.gameObject.transform.DOMove(characterData.Position, UPDATE_SEC).SetEase(Ease.Linear);
        playerObj.gameObject.transform.localScale = characterData.Scale;
        playerObj.gameObject.transform.DORotateQuaternion(characterData.Rotation, UPDATE_SEC).SetEase(Ease.Linear);
    }

    #region 通知関連

    /// <summary>
    /// キャラ更新通知
    /// </summary>
    /// <param name="characterData"></param>
    public void OnUpdateCharacter(CharacterData characterData)
    {
        if (!playerObjs.ContainsKey(characterData.ConnectionID) || !RoomModel.Instance) return;

        // プレイヤーの情報更新
        var player = playerObjs[characterData.ConnectionID];
        UpdateCharacter(characterData,player);
    }

    #endregion
}
