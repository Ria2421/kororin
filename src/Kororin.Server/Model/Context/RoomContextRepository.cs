﻿using Cysharp.Runtime.Multicast;
using Kororin.Server.Model.Context;
using System.Collections.Concurrent;

namespace Korirn.Server.Model.Context
{
    public class RoomContextRepository(IMulticastGroupProvider groupProvider)
    {
        private readonly ConcurrentDictionary<string, RoomContext> contexts = new();

        #region ゲームコンテキストを使用するための関数一覧

        // ゲームコンテキストの作成
        public RoomContext CreateContext(string roomName , string pass)
        {
            var context = new RoomContext(groupProvider, roomName,pass);
            contexts[roomName] = context;
            return context;
        }

        // ゲームコンテキストの取得
        public RoomContext GetContext(string roomName)
        {
            if(contexts.ContainsKey(roomName))
            {
                return contexts[roomName];
            }
            else
            {
                return null;
            }
            
        }

        // ゲームコンテキストの全取得
        public ConcurrentDictionary<string, RoomContext> GetALLContext()
        {
            if(contexts != null)
            {
                return contexts;
            }
            else
            {
                return null;
            }
        }

        // ゲームコンテキストの削除
        public void RemoveContext(string roomName)
        {
            if(contexts.Remove(roomName, out var roomContext))
            {
                roomContext?.Dispose();
            }
        }

        #endregion
    }
}
