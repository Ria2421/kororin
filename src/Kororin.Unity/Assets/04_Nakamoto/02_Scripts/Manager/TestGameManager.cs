//-------------------------------------------------------------
// 試験用ゲームマネージャー [ TestGameManager.cs ]
// Author:中本健太
//-------------------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestGameManager : MonoBehaviour
{
    //-------------------
    // フィールド

    [SerializeField] private Text cntTxt;           // カウントダウン用テキスト
    [SerializeField] private GameObject gateObj;    // スタートゲートオブジェ
    [SerializeField] private List<Transform> generatePos = new List<Transform>();   // プレイヤー生成位置

    //-----------------------
    // メソッド

    /// <summary>
    /// 初期処理
    /// </summary>
    async void  Start()
    {
        // 生成位置の設定
        CharacterManager.Instance.StartPoints = generatePos;

        // キャラ生成
        CharacterManager.Instance.GenerateAllCharacters();

        // 接続フラグオン
        RoomModel.Instance.IsConnect = true;

        // 通知処理の登録
        RoomModel.Instance.OnStartedCount += OnStartedCount;
        RoomModel.Instance.OnOpenedGate += OnOpenGate;
        RoomModel.Instance.OnLeavedUser += OnLeavedUser;
        RoomModel.Instance.OnChangedMasterClient += OnChangedMasterClient;
        RoomModel.Instance.OnResulted += OnResulted;

        // 遷移完了状態を送信
        await RoomModel.Instance.TransitionInGameAsync();
    }

    /// <summary>
    /// 切断時の処理
    /// </summary>
    private void OnDisable()
    {
        if (!RoomModel.Instance) return;
        StopAllCoroutines();

        // 通知処理の解除
        RoomModel.Instance.OnStartedCount -= OnStartedCount;
        RoomModel.Instance.OnOpenedGate -= OnOpenGate;
        RoomModel.Instance.OnLeavedUser -= OnLeavedUser;
        RoomModel.Instance.OnChangedMasterClient -= OnChangedMasterClient;
        RoomModel.Instance.OnResulted -= OnResulted;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update()
    {
        
    }

    #region 通知処理

    /// <summary>
    /// カウント開始通知
    /// </summary>
    public void OnStartedCount()
    {
        // カウントコルーチン呼び出し
        StartCoroutine(StartCountCoroutine());
    }

    /// <summary>
    /// ゲート開放通知
    /// </summary>
    public void OnOpenGate()
    {
        Destroy(gateObj);
    }

    /// <summary>
    /// リザルト通知
    /// </summary>
    public void OnResulted(Dictionary<Guid,JoinedUser> joindUsers)
    {

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

    #endregion

    /// <summary>
    /// カウントダウン
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartCountCoroutine()
    {
        int count = 3;
        cntTxt.text = count.ToString();

        yield return new WaitForSeconds(1);

        while (true)
        {
            count--;
            cntTxt.text = count.ToString();
            yield return new WaitForSeconds(1);

            if(count == 1)
            {
                // スタート関数呼び出し
                DisplayStart();

                cntTxt.text = "Start!!";
                yield return new WaitForSeconds(1);
                cntTxt.text = "";

                yield break;
            }
        }
    }

    async private void DisplayStart()
    {
        await RoomModel.Instance.CountEndAsync();
    }
}
