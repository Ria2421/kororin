//-------------------------------------------------
// データベースとの接続を管理するスクリプト
// Author:中本健太
//-------------------------------------------------

using Microsoft.EntityFrameworkCore;
using Kororin.Shared.Interfaces.Model.Entity;

namespace Korirn.Server.Model.Context
{
    /// <summary>
    /// データベースの設定、SQLとの接続を行う
    /// </summary>
    public class GameDbContext : DbContext
    {

        #region データベース設定一覧

        // ユーザーのデータベース設定
        public DbSet<User> Users { get; set; }

        // ランキングのデータベース設定
        public DbSet<Ranking> Rankings { get; set; }

        #endregion

#if DEBUG
        //server名;ユーザー名;パスワード指定
        readonly string connectionString = "server=db-ge-07.mysql.database.azure.com;database=kororindb;user=student;password=Yoshidajobi2023;";
#else
        readonly string connectionString = "server=db-ge-07.mysql.database.azure.com;database=kororindb;user=student;password=Yoshidajobi2023;";
#endif

        //SQLと接続
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString,
                                                new MySqlServerVersion(new Version(8, 0)));
        }
    }
}
