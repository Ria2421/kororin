//------------------------------------------------------------------------
// Enum管理 [ EnumManager.cs ]
// Author：Kenta Nakamoto
//------------------------------------------------------------------------
using MessagePack;

namespace Kororin.Shared.Interfaces.StreamingHubs
{
    [MessagePackObject]
    public class EnumManager
    {
        #region システム関連

        /// <summary>
        /// ステージ種類
        /// </summary>
        public enum STAGE_TYPE
        {
            Rndom = 0,
            Stage01,
            Stage02,
        }
        public const int STAGE_TYPE_MAX = 2;

        /// <summary>
        /// アイテムの種類
        /// </summary>
        public enum ITEM_TYPE
        {
            Relic,
            DataCube,
            DataBox
        }

        #endregion
    }
}
