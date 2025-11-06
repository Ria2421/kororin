//-------------------------------------------------------------
// 試験用ゲームマネージャー [ TestGameManager.cs ]
// Author:中本健太
//-------------------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using System.Collections.Generic;
using UnityEngine;

public class TestGameManager : MonoBehaviour
{
    //-------------------
    // フィールド

    [SerializeField] private GameObject gateObj;    // スタートゲートオブジェ
    [SerializeField] private List<Transform> generatePos = new List<Transform>();   // プレイヤー生成位置

    //-----------------------
    // メソッド

    /// <summary>
    /// 初期処理
    /// </summary>
    void Start()
    {
        // 生成位置の設定
        CharacterManager.Instance.StartPoints = generatePos;

        // キャラ生成
        CharacterManager.Instance.GenerateAllCharacters();

        RoomModel.Instance.IsConnect = true;

        RoomModel.Instance.OnLeavedUser += OnLeavedUser;
        RoomModel.Instance.OnChangedMasterClient += OnChangedMasterClient;
    }

    /// <summary>
    /// 切断時の処理
    /// </summary>
    private void OnDisable()
    {
        if (!RoomModel.Instance) return;
        StopAllCoroutines();

        // シーン遷移したときに登録した通知処理を解除
        RoomModel.Instance.OnLeavedUser -= OnLeavedUser;
        RoomModel.Instance.OnChangedMasterClient -= OnChangedMasterClient;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        
    }

    #region 通知処理

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

    #endregion
}
