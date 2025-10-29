//------------------------------------------------------------------------
// ユーザーインターフェース [ IUserService.cs ]
// Author：Kenta Nakamoto
//------------------------------------------------------------------------
using MagicOnion;
using Kororin.Shared.Interfaces.Model.Entity;

namespace Kororin.Shared.Interfaces.Services
{
    /// <summary>
    /// ユーザーのインターフェースの追加(Shared)
    /// </summary>
    public interface IUserService : IService<IUserService>
    {
        //ユーザーの登録
        UnaryResult<int> RegistUserAsync();

        //ユーザーの全取得
        UnaryResult<User[]> GetAllUsersAsync();

        //ユーザーのid指定取得
        UnaryResult<User> GetUserAsync(int id);
    }
}
