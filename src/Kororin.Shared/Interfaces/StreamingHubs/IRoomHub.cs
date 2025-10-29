//------------------------------------------------------------------------
// クライアントからサーバーへの通信を管理するスクリプト [ IRoomHub.cs ]
// Author：Kenta Nakamoto
//------------------------------------------------------------------------
using MagicOnion;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kororin.Shared.Interfaces.StreamingHubs
{
    public interface IRoomHub:IStreamingHub<IRoomHub,IRoomHubReceiver>
    {
        //ここにクライアント～サーバー定義

        #region 入室からゲーム開始まで
        /// <summary>
        /// ユーザー入室
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<Guid, JoinedUser>> JoinedAsync(string roomName, int userId, string userName);

        /// <summary>
        /// ユーザー退室
        /// </summary>
        /// <returns></returns>
        Task LeavedAsync(bool isEnd);

        /// <summary>
        /// 準備完了
        /// </summary>
        /// <returns></returns>
        Task ReadyAsync(int characterID);

        #endregion

        #region ゲーム内

        #region プレイヤー関連
        #endregion

        #region ゲーム内UI、仕様関連
        #endregion

        #endregion
    }
}
