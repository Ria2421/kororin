//------------------------------------------------------------------------
// ランキングインターフェース [ IRankingService.cs ]
// Author：Kenta Nakamoto
//------------------------------------------------------------------------
using MagicOnion;
using Kororin.Shared.Interfaces.Model.Entity;

namespace Kororin.Shared.Interfaces.Services
{
    /// <summary>
    /// ユーザーのインターフェースの追加(Shared)
    /// </summary>
    public interface IRankingService : IService<IRankingService>
    {
        // ランキングの登録
        UnaryResult<int> RegistRankingsAsync();

        // ランキングの全取得
        UnaryResult<User[]> GetAllRankingsAsync();

        // ステージID指定取得
        UnaryResult<User> GetRankingAsync(int stageId);
    }
}