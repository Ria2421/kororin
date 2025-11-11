//-------------------------------------------------------------
// クライアント～サーバー間の通信に関するスクリプト
// Author:中本健太
//-------------------------------------------------------------

#region using一覧
using MagicOnion.Server.Hubs;
using Kororin.Server.Model.Context;
using Kororin.Shared.Interfaces.Model.Entity;
using Kororin.Shared.Interfaces.StreamingHubs;
using UnityEngine;
using Korirn.Server.Model.Context;
using Shared.Interfaces.StreamingHubs;
using System.Runtime.InteropServices.Marshalling;
#endregion

namespace StreamingHubs
{
    public class RoomHub(RoomContextRepository roomContextRepository) : StreamingHubBase<IRoomHub, IRoomHubReceiver>, IRoomHub
    {
        //-----------------------
        // フィールド

        // コンテキスト定義
        private RoomContext roomContext;
        RoomContextRepository roomContextRepos;
        Dictionary<Guid, JoinedUser> JoinedUsers { get; set; }

        // 最大参加可能人数
        private const int MAX_JOINABLE_PLAYERS = 2;

        // 最大ステージ数 (乱数に用いるので最大数+1)
        private const int MAX_STAGE_NUM = 6;

        //-----------------------
        // メソッド

        #region 接続・切断処理
        // 接続した場合
        protected override ValueTask OnConnected()
        {
            roomContextRepos = roomContextRepository;
            return default;
        }

        // 切断された場合
        protected override ValueTask OnDisconnected()
        {
            // 退室処理を実行
            LeavedAsync(false); return CompletedTask;
        }

        #endregion

        #region マッチングしてからゲーム開始までの処理

        /// <summary>
        /// 入室処理
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Dictionary<Guid, JoinedUser>> JoinedAsync(string roomName,int userId ,string userName)
        {
            lock (roomContextRepository)
            { //同時に生成しないように排他制御

                // ルームに参加＆ルームを保持
                this.roomContext = roomContextRepository.GetContext(roomName);

                if (this.roomContext == null)
                {   // 無い時は新規作成
                    this.roomContext = roomContextRepository.CreateContext(roomName,"");
                }

                // ルームにユーザーを追加
                this.roomContext.Group.Add(this.ConnectionId, Client);

                // グループストレージにユーザーデータを格納
                var joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId, UserId = userId , UserName = userName };

                if (roomContext.JoinedUserList.Count == 0)
                {   // 最初の1人目の時

                    // 参加順番の初期化
                    joinedUser.JoinOrder = 1;

                    // 1人目をマスタークライアントにする
                    joinedUser.IsMaster = true;
                }
                else
                {
                    // 参加順番の設定
                    joinedUser.JoinOrder = roomContext.JoinedUserList.Count + 1;
                }

                // ルームコンテキストに参加ユーザーを保存
                this.roomContext.JoinedUserList[this.ConnectionId] = joinedUser;

                // 多分、自身に入室できたことを通知してる
                this.roomContext.Group.Only([this.ConnectionId]).OnRoom();
                
                // 自身以外に参加者を通知
                this.roomContext.Group.Except([this.ConnectionId]).Onjoin(roomContext.JoinedUserList[this.ConnectionId]);

                // 参加中のユーザー情報を返す
                return this.roomContext.JoinedUserList;
            }
        }

        /// <summary>
        /// 退室処理
        /// </summary>
        /// <returns></returns>
        public async Task LeavedAsync(bool isEnd)
        {
            lock (roomContextRepository) // 排他制御
            {
                // Nullチェック入れる
                if (this.roomContext==null) return;
                if (!this.roomContext.JoinedUserList.ContainsKey(this.ConnectionId)) return;

                GameDbContext context = new GameDbContext();

                // 退室するユーザーを取得
                var joinedUser = this.roomContext.JoinedUserList[this.ConnectionId];

                if(isEnd == false)
                {
                    // マスタークライアントだったら次の人に譲渡する
                    if (joinedUser.IsMaster == true)
                    {
                        MasterChange(this.ConnectionId);
                        foreach (var user in this.roomContext.JoinedUserList)
                        {
                            if (user.Value.IsMaster == true)
                            {
                                this.roomContext.Group.Only([user.Key]).OnChangeMasterClient();
                            }
                        }
                    }
                }

                // ルーム参加者全員に、ユーザーの退室通知を送信
                this.roomContext.Group.All.OnLeave(roomContext.JoinedUserList, this.ConnectionId);

                //　ルームから退室
                this.roomContext.Group.Remove(this.ConnectionId);

                //コンテキストからユーザーを削除
                roomContext.RemoveUser(this.ConnectionId);

                //ゲームが始まっていないなら実行しない
                if (roomContext.IsStartGame == false) return;

                // 全滅判定変数
                //bool isAllDead = true;

                //foreach (var player in this.roomContext.characterDataList)
                //{
                //    if (player.Value.IsDead == false) // もし誰かが生きていた場合
                //    {
                //        isAllDead = false;
                //        break;
                //    }
                //}

                //// 全滅した場合、ゲーム終了通知を全員に出す
                //if (isAllDead)
                //{
                //    Result();
                //}
            }
        }

        /// <summary>
        /// ロビー準備完了
        /// </summary>
        /// <returns></returns>
        public async Task StandbyAsync()
        {
            bool canStartGame = true; // ゲーム開始可能判定変数

            // 自身のデータを取得
            var joinedUser = roomContext.JoinedUserList[this.ConnectionId];
            joinedUser.IsReady = true;  // 準備完了にする

            // ルーム参加者全員に、自分が準備完了した通知を送信
            this.roomContext.Group.All.OnStandby(this.ConnectionId);

            // 最低参加人数がいない時点で早期リターン
            if (this.roomContext.JoinedUserList.Count < MAX_JOINABLE_PLAYERS) return;

            foreach (var user in this.roomContext.JoinedUserList)
            { // 現在の参加者数分ループ
                if (!user.Value.IsReady) canStartGame = false; // もし一人でも準備完了していなかった場合、開始させない
            }

            // ゲームが開始できる場合、開始通知をする
            if (canStartGame)
            {
                Random rnd = new Random();
                var id = rnd.Next(1, MAX_STAGE_NUM);   // 1以上最大ステージ数未満の値がランダムに出力

                this.roomContext.Group.All.OnStartGame(id);
                this.roomContext.IsStartGame = true;

                // フラグリセット
                foreach (var user in this.roomContext.JoinedUserList)
                {
                    user.Value.IsReady = false;
                }
            }
        }

        #endregion

        #region ゲーム内での処理

        /// <summary>
        /// インゲーム遷移完了処理
        /// </summary>
        /// <returns></returns>
        public async Task TransitionInGameAsync()
        {
            bool canCountdown = true;   // カウントダウン開始判定

            // 自身を遷移完了にする
            var joinedUser = roomContext.JoinedUserList[this.ConnectionId];
            joinedUser.IsTransition = true;

            // 全員が遷移完了してるかチェック
            foreach (var user in this.roomContext.JoinedUserList)
            {
                if (!user.Value.IsTransition) canCountdown = false;
            }

            // 遷移完了してたらカウント開始
            if (canCountdown)
            {
                this.roomContext.Group.All.OnStartCount();

                // フラグリセット
                foreach (var user in this.roomContext.JoinedUserList)
                {
                    user.Value.IsTransition = false;
                }
            }

        }

        /// <summary>
        /// カウント終了処理
        /// </summary>
        /// <returns></returns>
        public async Task CountEndAsync()
        {
            bool canOpenGate = true;    // カウントダウン開始判定

            // 自身をカウント終了状態にする
            var joinedUser = roomContext.JoinedUserList[this.ConnectionId];
            joinedUser.IsCountEnd = true;

            // 全員がカウント終了してるかチェック
            foreach (var user in this.roomContext.JoinedUserList)
            {
                if (!user.Value.IsCountEnd) canOpenGate = false;
            }

            // 遷移完了してたらカウント開始
            if (canOpenGate)
            {
                this.roomContext.Group.All.OnOpenGate();

                // フラグリセット
                foreach (var user in this.roomContext.JoinedUserList)
                {
                    user.Value.IsCountEnd = false;
                }
            }
        }

        /// <summary>
        /// ゴール到着処理
        /// </summary>
        /// <returns></returns>
        public async Task ArrivalGoalAsync()
        {
            lock (roomContextRepository)    // 排他制御
            {
                bool canTransResult = true; // 全員ゴールしたか判別

                int rank = 1;

                foreach(var user in this.roomContext.JoinedUserList)
                {
                    if(user.Value.Rank != -1) rank++;
                }

                // 順位を保存
                roomContext.JoinedUserList[this.ConnectionId].Rank = rank;

                // 全員が遷移完了してるかチェック
                foreach (var user in this.roomContext.JoinedUserList)
                {
                    if (user.Value.Rank == -1) canTransResult = false;
                }

                // 遷移完了してたらカウント開始
                if (canTransResult)
                {
                    this.roomContext.Group.All.OnResult(roomContext.JoinedUserList);

                    // フラグリセット
                    foreach (var user in this.roomContext.JoinedUserList)
                    {
                        user.Value.Rank = -1;
                    }
                }
            }
        }

        /// <summary>
        /// マスタークライアント譲渡処理
        /// </summary>
        /// <param name="conID"></param>
        /// <returns></returns>
        void MasterChange(Guid conID)
        {
            // 参加者リストをループ
            foreach (var user in this.roomContext.JoinedUserList)
            {
                // 対象がマスタークライアントでない場合
                if (user.Value.IsMaster == false)
                {
                    // その対象をマスタークライアントとしループを抜ける
                    user.Value.IsMaster = true;
                    break;
                }
            }

            // マスタークライアントを剥奪
            this.roomContext.JoinedUserList[conID].IsMaster = false;
        }

        /// <summary>
        /// プレイヤーの更新
        /// </summary>
        /// <param name="playerData"></param>
        /// <returns></returns>
        public async Task UpdateCharacterAsync(CharacterData charaData)
        {
            // キャラクターデータリストに自身のデータがない場合
            if (!this.roomContext.CharacterDataList.ContainsKey(this.ConnectionId))
            {
                // 新たなキャラクターデータを追加
                this.roomContext.CharacterDataList.Add(this.ConnectionId, charaData);
            }
            else // 既に存在している場合
            {
                // キャラクターデータを更新
                this.roomContext.CharacterDataList[this.ConnectionId] = charaData;
            }

            // ルームの自分以外に、ユーザ情報通知を送信
            this.roomContext.Group.Except([this.ConnectionId]).OnUpdateCharacter(charaData);
        }

        ///// <summary>
        ///// マスタークライアントの更新
        ///// </summary>
        ///// <param name="masterClientData"></param>
        ///// <returns></returns>
        //public async Task UpdateMasterClientAsync(MasterClientData masterClientData)
        //{
        //    lock (roomContextRepository) // 排他制御
        //    {
        //        // ルームデータから敵のリストを取得し、該当する要素を更新する
        //        var gottenEnemyDataList = this.roomContext.enemyDataList;
        //        foreach (var enemyData in masterClientData.EnemyDatas)
        //        {
        //            if (gottenEnemyDataList.ContainsKey(enemyData.UniqueId))
        //            {
        //                gottenEnemyDataList[enemyData.UniqueId] = enemyData;
        //            }
        //        }

        //        // ルームデータから端末情報を取得し、アクティブ状態の端末を更新
        //        if(masterClientData.TerminalDatas != null)
        //        {
        //            foreach (var termData in masterClientData.TerminalDatas)
        //            {
        //                if(termData.State == TERMINAL_STATE.Active)
        //                {
        //                    var data = roomContext.terminalList.FirstOrDefault(t => t.ID == termData.ID);

        //                    if (data != null)
        //                    {
        //                        data.Time = termData.Time;
        //                    }
        //                }
        //            }
        //        }

        //        foreach (var item in masterClientData.GimmickDatas)
        //        {
        //            // すでにルームコンテキストにギミックが含まれている場合
        //            if (this.roomContext.gimmickList.ContainsKey(item.UniqueID))
        //            {
        //                // そのギミックを更新する
        //                this.roomContext.gimmickList[item.UniqueID] = item;
        //            }
        //            else // 含まれていない場合
        //            {
        //                // そのギミックを追加する
        //                this.roomContext.gimmickList.Add(item.UniqueID, item);
        //            }
        //        }

        //        // キャラクターデータリストに自身のデータがない場合
        //        if (!this.roomContext.characterDataList.ContainsKey(this.ConnectionId))
        //        {
        //            // 新たなキャラクターデータを追加
        //            this.roomContext.AddCharacterData(this.ConnectionId, masterClientData.PlayerData);
        //        }
        //        else // 既に存在している場合
        //        {
        //            // キャラクターデータを更新
        //            this.roomContext.characterDataList[this.ConnectionId] = masterClientData.PlayerData;
        //        }

        //        // ルームの自分以外に、マスタークライアントの状態の更新通知を送信
        //        this.roomContext.Group.Except([this.ConnectionId]).OnUpdateMasterClient(masterClientData);
        //    }
        //}

        ///// <summary>
        ///// ギミック起動同期処理
        ///// </summary>
        ///// <param name="gimID">ギミック識別ID</param>
        ///// <returns></returns>
        //public async Task BootGimmickAsync(string uniqueID, bool triggerOnce)
        //{
        //    lock (roomContextRepository)
        //    {
        //        // 対象ギミックが存在している場合
        //        if (this.roomContext.gimmickList.ContainsKey(uniqueID))
        //        {
        //            if (triggerOnce)
        //            {
        //                this.roomContext.gimmickList.Remove(uniqueID);
        //            }

        //            // 参加者全員にギミック情報を通知
        //            this.roomContext.Group.All.OnBootGimmick(uniqueID, triggerOnce);
        //        }
        //    }
        //}

        ///// <summary>
        ///// オブジェクト生成処理
        ///// </summary>
        ///// <returns></returns>
        //public async Task SpawnObjectAsync(OBJECT_TYPE type, Vector2 spawnPos)
        //{
        //    lock (roomContextRepository)
        //    {
        //        string uniqueId = Guid.NewGuid().ToString();
        //        GimmickData gimmickData = new GimmickData()
        //        {
        //            UniqueID = uniqueId,
        //            Position = spawnPos,
        //        };
        //        this.roomContext.gimmickList.Add(uniqueId, gimmickData);
        //        this.roomContext.Group.All.OnSpawnObject(type, spawnPos, uniqueId);
        //    }
        //}

        #endregion
    }
}
