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
        public int Id { get; set; }                 // ランキングID
        [Key(1)]
        public int UserId { get; set; }             // ユーザーID
        [Key(2)]
        public int StageId { get; set; }            // ステージID
        [Key(3)]
        public float ClearTime { get; set; }        // クリアタイム
        [Key(4)]
        public DateTime Created_at { get; set; }    // 生成日時
        [Key(5)]
        public DateTime Updated_at { get; set; }    // 更新日時
    }
}
