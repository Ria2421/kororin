//------------------------------------------------------------------------
// ユーザーAPIインターフェース [ IUserService.cs ]
// Author：Kenta Nakamoto
//------------------------------------------------------------------------
using MagicOnion;
using Kororin.Shared.Interfaces.Model.Entity;
using System.Collections.Generic;

namespace Kororin.Shared.Interfaces.Services
{
    /// <summary>
    /// ユーザーのインターフェースの追加(Shared)
    /// </summary>
    public interface IRoomService : IService<IRoomService>
    {
        // ユーザーの登録
        UnaryResult<int> RegistUserAsync(string name);

        // ユーザーのid指定取得
        UnaryResult<User> GetUserAsync(int id);

        // ランキング登録
        UnaryResult RegistRankingAsync(int userID, int stageID, float clearTime);

        // 指定ステージIDのランキングを取得
        UnaryResult<List<RankingDto>> GetRankingAsync(int stageID);
    }
}
