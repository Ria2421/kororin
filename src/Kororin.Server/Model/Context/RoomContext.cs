//=============================
// ルームコンテキストスクリプト
// Author:木田晃輔
//=============================
using Cysharp.Runtime.Multicast;
using Korirn.Server.Model.Context;
using Kororin.Shared.Interfaces.StreamingHubs;
using Shared.Interfaces.StreamingHubs;

namespace Kororin.Server.Model.Context
{
    public class RoomContext
    {
        #region RoomContext基本構造
        /// <summary>
        /// コンテキストID
        /// Author:Kida
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// ルーム名
        /// Author:Kida
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// パスワード
        /// Author:Kida
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// 現在のステージ
        /// Author:Nishiura
        /// </summary>
        public EnumManager.STAGE_TYPE NowStage { get; set; }

        /// <summary>
        /// ステージ進行リクエスト変数
        /// Author:Nishiura
        /// </summary>
        public bool isAdvanceRequest;

        /// <summary>
        /// グループ
        /// Author:Kida
        /// </summary>
        public IMulticastSyncGroup<Guid, IRoomHubReceiver> Group { get; }

        /// <summary>
        /// ゲームスタート
        /// </summary>
        public bool IsStartGame { get; set; } = false;

        /// <summary>
        /// マスタデータを読み込み済みかどうか
        /// </summary>
        public bool IsLoadMasterDatas { get; private set; } = false;

        #endregion

        #region コンテキストに保存する情報のリスト一覧

        /// <summary>
        /// 参加者の情報リスト
        /// Author:Kida
        /// </summary>
        public Dictionary<Guid, JoinedUser> JoinedUserList { get; } = new Dictionary<Guid, JoinedUser>();

        /// <summary>
        /// 参加者のキャラデータリスト
        /// </summary>
        public Dictionary<Guid,CharacterData> CharacterDataList { get; } = new Dictionary<Guid, CharacterData>();
        
        #endregion

        // RoomContextの定義
        public RoomContext(IMulticastGroupProvider groupProvider, string roomName , string pass)
        {
            Id = Guid.NewGuid();
            Name = roomName;
            PassWord = pass;
            Group =
                groupProvider.GetOrAddSynchronousGroup<Guid, IRoomHubReceiver>(roomName);
        }

        #region 独自関数

        /// <summary>
        /// グループ退室処理
        /// Author:木田晃輔
        /// </summary>
        public void Dispose()
        {
            Group.Dispose();
        }


        /// <summary>
        /// ユーザーの退出処理
        /// Aughter:Kida
        /// </summary>
        /// <returns></returns>
        public void RemoveUser(Guid guid)
        {
            int joinOrder = 1;
            if (JoinedUserList != null)
            { //参加者リストが存在している場合
                // 退出したユーザーを特定して削除
                JoinedUserList.Remove(guid);
                foreach (var joinUser in JoinedUserList)
                {
                    joinUser.Value.JoinOrder = joinOrder;
                    joinOrder++;
                }
            }
        }

        #endregion
    }
}
