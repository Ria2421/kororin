//---------------------------------------------------
// キャラクター管理 [ CharacterManager.cs ]
//  Author:Kenta Nakamoto
//---------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    //-----------------------
    // フィールド

    /// <summary>
    /// 参加者のプレイヤーオブジェリスト
    /// </summary>
    private Dictionary<Guid, GameObject> playerObjs = new Dictionary<Guid, GameObject>();

    public Dictionary<Guid, GameObject> PlayerObjs { get { return playerObjs; } }

    [SerializeField] private GameObject playerPrefab;   // 生成するプレイヤーオブジェ

    [SerializeField] private GameObject playerObjSelf;  // ローカル用に属性付与

    //------------
    // 定数

    private const float UPDATE_SEC = 0.1f;  // 通信頻度

    //-----------------------
    // 通信関連

    /// <summary>
    /// 入室者のプレイヤーオブジェの生成
    /// </summary>
    /// <param name="connectionID"></param>
    public void GenerateCharacters(Guid connectionID)
    {
        // 開始位置の設定
        //var point = startPoints[0];
        //startPoints.RemoveAt(0);

        var playerObj = Instantiate(playerPrefab, playerPrefab.transform.position, Quaternion.identity);
        playerObjs.Add(connectionID, playerObj);
    }

    /// <summary>
    /// 参加しているユーザー情報を元に、全員のプレイヤーを生成する
    /// </summary>
    private void GenerateAllCharacters()
    {
        foreach (var joinduser in RoomModel.Instance.joinedUserList)
        {
            // 開始位置の設定
            //var point = startPoints[0];
            //startPoints.RemoveAt(0);

            var playerObj = Instantiate(playerPrefab, playerPrefab.transform.position, Quaternion.identity);
            playerObjs.Add(joinduser.Key, playerObj);

            // 自身のプレイヤーを生成した場合
            if (joinduser.Key == RoomModel.Instance.ConnectionId)
            {
                playerObjSelf = playerObj;

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
            if (TestTopManager.Instance.IsConnected)
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
}
