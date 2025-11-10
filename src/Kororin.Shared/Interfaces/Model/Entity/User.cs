//------------------------------------------------------------------------
// ユーザーデータ [ User.cs ]
// Author：Kenta Nakamoto
//------------------------------------------------------------------------

using MessagePack;
using System;

namespace Kororin.Shared.Interfaces.Model.Entity
{
    [MessagePackObject]
    /// <summary>
    /// ユーザーのカラム設定(Public)
    /// </summary>
    public class User
    {
        [Key(0)]
        public int id { get; set; }                 // ユーザーID
        [Key(1)]
        public string name { get; set; }            // 名前
        [Key(2)]
        public DateTime Created_at { get; set; }    // 生成日時
        [Key(3)]
        public DateTime Updated_at { get; set; }    // 更新日時
    }
}
