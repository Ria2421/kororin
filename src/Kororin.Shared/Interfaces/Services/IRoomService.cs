﻿using MagicOnion;
using Kororin.Shared.Interfaces.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kororin.Shared.Interfaces.Services
{
    public interface IRoomService : IService<IRoomService>
    {
        //ルームををユーザーの名前指定で取得
        UnaryResult<Room> GetRoom(string user_name);

        //ステージ一覧を取得
        UnaryResult<Room[]> GetAllRoom();

        //ルームを作成
        UnaryResult<Room> RegistRoom(string room_name,string user_name,string pass,int game_mode);

        //ルームを開始状態にする
        UnaryResult<Room> StartRoom(string user_name);

        //ルームを削除
        UnaryResult<Room> RemoveRoom(string room_name);

    }
}
