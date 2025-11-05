//-------------------------------------------------------------
// サーバーからクライアントへの通信を管理するスクリプト
// Author:Kenta Nakamoto
//-------------------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections.Generic;

namespace Kororin.Shared.Interfaces.StreamingHubs
{
    public interface IRoomHubReceiver
    {
        //ここにサーバー～クライアントの定義

        #region 入室からゲーム開始まで

        /// <summary>
        /// ルーム作成通知
        /// </summary>
        void OnRoom();

        /// <summary>
        /// 参加失敗の通知
        /// </summary>
        void OnFailedJoin(int errorId);

        /// <summary>
        /// ユーザーの入室通知
        /// </summary>
        /// <param name="joindUserList">参加者リスト</param>
        void Onjoin(JoinedUser joindUser);

        /// <summary>
        /// ユーザーの退室通知
        /// </summary>
        /// <param name = "user" > 対象者 </ param >
        void OnLeave(Dictionary<Guid, JoinedUser> user, Guid targetUser);

        /// <summary>
        /// 準備完了通知
        /// </summary>
        /// <param name="conID">接続ID</param>
        void OnStandby(Guid guid);

        /// <summary>
        /// ゲーム開始通知
        /// </summary>
        void OnStartGame();

        #endregion

        #region ゲーム内

        #region プレイヤー関連

        /// <summary>
        /// プレイヤー動作通知
        /// </summary>
        void OnUpdateCharacter(CharacterData charaData);

        #endregion

        #region 敵関連
        #endregion

        #region ゲーム内UI、仕様
        #endregion

        #endregion

        /// <summary>
        /// マスタークライアントの変更通知
        /// </summary>
        void OnChangeMasterClient();
    }
}
