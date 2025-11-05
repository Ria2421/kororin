//------------------------------------------------------
// キャラクターのデータクラス [ CharacterData.cs ]
// Author:Kenta Nakamoto
//------------------------------------------------------
using MessagePack;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace Shared.Interfaces.StreamingHubs
{
    [MessagePackObject]
    public class CharacterData
    {
        /// <summary>
        /// 接続ID
        /// </summary>
        [Key(0)]
        public Guid ConnectionID { get; set; }

        [Key(1)]
        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 Position { get; set; } = Vector3.zero;

        [Key(2)]
        /// <summary>
        /// スケール
        /// </summary>
        public Vector3 Scale { get; set; }

        [Key(3)]
        /// <summary>
        /// 向き
        /// </summary>
        public Quaternion Rotation { get; set; } = Quaternion.identity;

        [Key(4)]
        /// <summary>
        /// アニメーションID
        /// </summary>
        public int AnimationId { get; set; }
    }
}
