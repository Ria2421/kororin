//------------------------------------------------------------------------
// 参加者の情報 [ JoinedUser.cs ]
// Author：Kenta Nakamoto
//------------------------------------------------------------------------
using System;
using MessagePack;

namespace Shared.Interfaces.StreamingHubs
{
    [MessagePackObject]
    public class JoinedUser
    {
        /// <summary>
        /// 接続ID
        /// </summary>
        [Key(0)]
        public Guid ConnectionId { get; set; }

        /// <summary>
        /// ユーザーID
        /// </summary>
        [Key(1)]
        public int UserId { get; set; }

        /// <summary>
        /// ユーザ名
        /// </summary>
        [Key(2)]
        public string UserName { get; set; }
    
        /// <summary>
        /// 参加順
        /// </summary>
        [Key(3)]
        public int JoinOrder { get; set; }

        /// <summary>
        /// マスタクライアント判定
        /// </summary>
        [Key(4)]
        public bool IsMaster { get; set; }

        /// <summary>
        /// 準備完了判定
        /// </summary>
        [Key(5)]
        public bool IsReady { get; set; }

        /// <summary>
        /// 順位
        /// </summary>
        [Key(6)]
        public int Rank { get; set; }
    }

}
