//=============================
// クライアントからサーバーへの通信を管理するスクリプト
// Author:木田晃輔
//=============================

using MagicOnion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Kororin.Shared.Interfaces.StreamingHubs
{
    public interface IRoomHub:IStreamingHub<IRoomHub,IRoomHubReceiver>
    {
        //ここにクライアント～サーバー定義

        #region 入室からゲーム開始まで
        /// <summary>
        /// ユーザー退室
        /// Author:Kida
        /// </summary>
        /// <returns></returns>
        Task LeavedAsync(bool isEnd);

        /// <summary>
        /// キャラクター変更
        /// </summary>
        /// <returns></returns>
        Task ChangeCharacterAsync(int CharacterId);

        /// <summary>
        /// 準備完了
        /// Author:Nishiura
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
