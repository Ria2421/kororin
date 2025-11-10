//------------------------------------------------------------------------
// ランキングデータ [ Ranking.cs ]
// Author：Kenta Nakamoto
//------------------------------------------------------------------------
using MessagePack;
using System;

namespace Kororin.Shared.Interfaces.Model.Entity
{
    [MessagePackObject]
    /// <summary>
    /// ランキングのカラム設定(Public)
    /// </summary>
    public class Ranking
    {
        [Key(0)]
        public int id { get; set; }                 // ランキングID
        [Key(1)]
        public int user_id { get; set; }             // ユーザーID
        [Key(2)]
        public int stage_id { get; set; }            // ステージID
        [Key(3)]
        public float clear_time { get; set; }        // クリアタイム
        [Key(4)]
        public DateTime Created_at { get; set; }    // 生成日時
        [Key(5)]
        public DateTime Updated_at { get; set; }    // 更新日時
    }
}
