//------------------------------------------------------------------------
// クライアント返却用ランキングデータ [ RankingDto.cs ]
// Author：Kenta Nakamoto
//------------------------------------------------------------------------
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kororin.Shared.Interfaces.Model.Entity
{
    [MessagePackObject]
    public class RankingDto
    {
        [Key(0)]
        public int user_id { get; set; }        // ユーザーID
        [Key(1)]
        public string user_name { get; set; }   // ユーザー名
        [Key(2)]
        public float clear_time { get; set; }   // クリアタイム
    }
}
