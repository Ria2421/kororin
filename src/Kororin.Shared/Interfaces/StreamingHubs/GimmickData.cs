using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Kororin.Shared.Interfaces.StreamingHubs
{
    [MessagePackObject]
    public class GimmickData
    {
        [Key(0)]
        /// <summary>
        /// ギミックID
        /// </summary>
        public string UniqueID { get; set; }

        [Key(1)]
        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 Position { get; set; }

        [Key(2)]
        /// <summary>
        /// 回転
        /// </summary>
        public Quaternion Rotation { get; set; }


        [Key(3)]
        /// <summary>
        /// 向き
        /// </summary>
        public Vector3 Scale { get; set; }
    }
}
