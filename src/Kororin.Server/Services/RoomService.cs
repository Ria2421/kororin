//------------------------------------------------------------------------
// 使用API [ RoomService.cs ]
// Author：Kenta Nakamoto
//------------------------------------------------------------------------
using Korirn.Server.Model.Context;
using Kororin.Shared.Interfaces.Model.Entity;
using Kororin.Shared.Interfaces.Services;
using MagicOnion;
using MagicOnion.Server;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Kororin.Server.Services
{
    public class RoomService : ServiceBase<IRoomService>, IRoomService
    {
        /// <summary>
        /// ユーザー登録
        /// </summary>
        /// <returns></returns>
        public async UnaryResult<int> RegistUserAsync(string name)
        {
            //データベースを取得
            using var context = new GameDbContext();

            //テーブルにレコードを追加
            User user = new User();
            user.name = name;
            user.Created_at = DateTime.Now;     //生成日時
            user.Updated_at = DateTime.Now;     //更新日時
            context.Users.Add(user);            //データベースに格納
            await context.SaveChangesAsync();   //データベースを保存する
            return user.id;                     //ユーザーのidを返す
        }

        /// <summary>
        /// ID指定取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async UnaryResult<User> GetUserAsync(int id)
        {
            // DBを取得
            using var context = new GameDbContext();
            User user = new User();

            // idが異常な値か確認
            if (context.Users.Count() < id || id <= 0)
            {
                //400エラー表示
                throw new ReturnStatusException(Grpc.Core.StatusCode.InvalidArgument,
                    "そのIDのユーザーは登録されていません");
            }

            // ID指定でデータを取得
            user = context.Users.Where(user => user.id == id).First();

            // ユーザーのデータを返す
            return user;
        }

        /// <summary>
        /// ランキング登録
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="stageID"></param>
        /// <param name="clearTime"></param>
        /// <returns></returns>
        public async UnaryResult RegistRankingAsync(int userID, int stageID, float clearTime)
        {
            using var context = new GameDbContext();

            // 追加データの作成
            Ranking ranking = new Ranking();
            ranking.user_id = userID;
            ranking.stage_id = stageID;
            ranking.clear_time = clearTime;
            ranking.Created_at = DateTime.Now;     //生成日時
            ranking.Updated_at = DateTime.Now;     //更新日時

            // データベースを保存する
            context.Rankings.Add(ranking);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// 指定ステージIDのランキングを取得
        /// </summary>
        /// <param name="stageID"></param>
        /// <returns></returns>
        public async UnaryResult<List<RankingDto>> GetRankingAsync(int stageID)
        {
            using var context = new GameDbContext();

            // 指定IDの上位５つのデータを取得
            var topRankings = await context.Rankings
            .Where(r => r.stage_id == stageID)
            .OrderBy(r => r.clear_time)
            .Join(
                context.Users,
                r => r.user_id,
                u => u.id,
                (r, u) => new RankingDto
                {
                    user_id = r.user_id,
                    user_name = u.name,
                    clear_time = r.clear_time,
                })
            .Take(5)
            .ToListAsync();

            return topRankings;
        }
    }
}
