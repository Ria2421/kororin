//----------------------------------------------------------
// ギミックマネージャー [ GimmickManager.cs ]
// Author:Rui Enomoto
// Editor:Kenta Nakamoto
//----------------------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using Kororin.Shared.Interfaces.StreamingHubs;

public class GimmickManager : MonoBehaviour
{
    //-------------------
    // フィールド

    // シーン内に存在するギミック
    [SerializeField] List<GimmickBase> gimmicks = new List<GimmickBase>();

    // マネージャーで管理しているギミック
    Dictionary<string, GimmickBase> managedGimmicks = new Dictionary<string, GimmickBase>();
    public Dictionary<string, GimmickBase> ManagedGimmicks { get { return managedGimmicks; } private set { managedGimmicks = value; } }

    #region インスタンス

    static GimmickManager instance;
    public static GimmickManager Instance
    {
        get
        {
            return instance;
        }
    }

    #endregion

    //-------------------
    // メソッド

    // 起動処理
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            // 識別用IDを設定
            for (int i = 0; i < gimmicks.Count; i++)
            {
                gimmicks[i].UniqueId = $"{gimmicks[i].name}_{i}";
                managedGimmicks.Add(gimmicks[i].UniqueId, gimmicks[i]);
            }
        }
        else
        {
            // インスタンスが複数存在しないように、既に存在していたら自身を消去する
            Destroy(gameObject);
        }
    }

    // 初期処理
    private void Start()
    {
        if (!RoomModel.Instance) return;
        RoomModel.Instance.OnBootedGimmick += this.OnBootGimmick;
        RoomModel.Instance.OnChangedMasterClient += this.OnChangedMasterClient;
    }

    // 破棄処理
    private void OnDisable()
    {
        if (!RoomModel.Instance) return;
        RoomModel.Instance.OnBootedGimmick -= this.OnBootGimmick;
        RoomModel.Instance.OnChangedMasterClient -= this.OnChangedMasterClient;
    }

    /// <summary>
    /// GimmickDataに加工して返す
    /// </summary>
    /// <returns></returns>
    public List<GimmickData> GetGimmickDatas()
    {
        var gimmickDatas = new List<GimmickData>();
        foreach (var gimmick in managedGimmicks)
        {
            if (gimmick.Value != null)
            {
                var gimmickData = new GimmickData()
                {
                    UniqueID = gimmick.Key,
                    Position = gimmick.Value.transform.position,
                    Rotation = gimmick.Value.transform.rotation,
                    Scale = gimmick.Value.transform.localScale,
                };

                gimmickDatas.Add(gimmickData);
            }
        }
        return gimmickDatas;
    }

    /// <summary>
    /// 一括でギミックを更新する
    /// </summary>
    /// <param name="gimmickDatas"></param>
    public void UpdateGimmicks(List<GimmickData> gimmickDatas)
    {
        foreach (var data in gimmickDatas)
        {
            if (managedGimmicks.ContainsKey(data.UniqueID))
            {
                managedGimmicks[data.UniqueID].UpdateGimmick(data);
            }
        }
    }

    /// <summary>
    /// ギミック起動通知
    /// </summary>
    /// <param name="uniqueId"></param>
    void OnBootGimmick(string uniqueId, bool triggerOnce)
    {
        if (managedGimmicks.ContainsKey(uniqueId))
        {
            managedGimmicks[uniqueId].TurnOnPower();
            if (triggerOnce) managedGimmicks.Remove(uniqueId);
        }
    }

    /// <summary>
    /// 自身がマスタクライアントになったとき
    /// </summary>
    void OnChangedMasterClient()
    {
        foreach (var gimmick in managedGimmicks.Values)
        {
            gimmick.Reactivate();
        }
    }

    /// <summary>
    /// 新たなギミックをリストに追加
    /// </summary>
    /// <param name="uniqueId"></param>
    /// <param name="gimmick"></param>
    public void AddGimmickFromList(string uniqueId, GimmickBase gimmick)
    {
        managedGimmicks.Add(uniqueId, gimmick);
    }
}
