//-----------------------------------------------------------------
// マスタクライアントが同期する際に使用 [ MasterClientData.cs ]
// Author:Kenta Nakamoto
//-----------------------------------------------------------------
using Kororin.Shared.Interfaces.StreamingHubs;
using MessagePack;
using Shared.Interfaces.StreamingHubs;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace Shared.Interfaces.StreamingHubs
{
    [MessagePackObject]
    /// <summary>
    /// マスタークライアント用のデータクラス
    /// </summary>
    public class MasterClientData
    {
        [Key(0)]
        /// <summary>
        /// 自身の操作キャラを同期させる情報
        /// </summary>
        public CharacterData CharacterData { get; set; }

        [Key(1)]
        /// <summary>
        /// 全ての存在し続けるギミックを同期させる情報
        /// </summary>
        public List<GimmickData> GimmickDatas { get; set; }
    }
}
