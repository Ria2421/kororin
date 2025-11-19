//------------------------------------------
// プレスギミック [ Press.cs ]
// Author:Souma Ueno
//------------------------------------------

using DG.Tweening;
using UnityEngine;

public class Press : GimmickBase
{
    [Header("移動時間")]
    [SerializeField] float moveTime;      // プレスにかかる時間

    [Header("戻る時間")]
    [SerializeField] float resetMoveTime; // 初期位置に戻る時間

    [Header("停止時間")]
    [SerializeField] float stopTime;      // 動いた後に止まる時間

    [Header("ループにかかる時間")]
    [SerializeField] int roopTime;

    // 初期位置保存用
    float initPosZ;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initPosZ = transform.localPosition.z;

        // オフライン時
        if (RoomModel.Instance == null)
        {
            StartPressLoop();
        }
        else
        {// オンライン時
            if (RoomModel.Instance.IsMaster)
            {
                StartPressLoop();
            }
        }
    }

    /// <summary>
    /// プレスの移動
    /// </summary>
    private Tween MovePosZ()
    {
        return transform.DOLocalMoveZ(0, moveTime);
    }

    /// <summary>
    /// 初期位置に戻る
    /// </summary>
    private Tween ResetPosZ()
    {
        return transform.DOLocalMoveZ(initPosZ, resetMoveTime);
    }

    /// <summary>
    /// プレスのループ
    /// </summary>
    private void StartPressLoop()
    {
        DOTween.Kill(transform);

        Sequence pressSequence = DOTween.Sequence();

        pressSequence.Append(MovePosZ());

        // 一定時間停止
        pressSequence.AppendInterval(stopTime);

        pressSequence.Append(ResetPosZ());

        // 一定時間停止
        pressSequence.AppendInterval(stopTime);

        // ループ
        pressSequence.SetLoops(-roopTime);
    }
}
