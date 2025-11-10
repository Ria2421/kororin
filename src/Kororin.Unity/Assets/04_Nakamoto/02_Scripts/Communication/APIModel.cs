using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Kororin.Shared.Interfaces.Model.Entity;
using Kororin.Shared.Interfaces.Services;
using MagicOnion.Client;
using System.Collections.Generic;
using UnityEngine;

public class APIModel : BaseModel
{
    //--------------------
    // ƒtƒB[ƒ‹ƒh

    // ƒ†[ƒU[ID
    public int Id { get; set; }

    // ƒ†[ƒU[–¼
    public string Name { get; set; }

    #region ƒCƒ“ƒXƒ^ƒ“ƒX
    
    private static APIModel instance;
    public static APIModel Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObj = new GameObject("UserModel");
                instance = gameObj.AddComponent<APIModel>();
                DontDestroyOnLoad(gameObj);
            }
            return instance;
        }
    }

    #endregion

    //--------------------
    // ƒƒ\ƒbƒh

    /// <summary>
    /// ƒ†[ƒU[“o˜^
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> RegistUserAsync(string name)
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IRoomService>(channel);
        try
        {// “o˜^¬Œ÷
            Id = await client.RegistUserAsync(name);
            Name = name;
            Debug.Log("“o˜^¬Œ÷");
            return true;
        }
        catch (RpcException e)
        {// “o˜^¸”s
            Debug.Log("“o˜^¸”s");
            return false;
        }
    }

    /// <summary>
    /// ƒ‰ƒ“ƒLƒ“ƒO“o˜^
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="stageID"></param>
    /// <param name="clearTime"></param>
    /// <returns></returns>
    public async UniTask<bool> RegistRankingAsync(int userID, int stageID, float clearTime)
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IRoomService>(channel);
        try
        {// “o˜^¬Œ÷
            await client.RegistRankingAsync(userID,stageID,clearTime);
            Debug.Log("“o˜^¬Œ÷");
            return true;
        }
        catch (RpcException e)
        {// “o˜^¸”s
            Debug.Log("“o˜^¸”s");
            return false;
        }
    }

    /// <summary>
    /// w’èID‚Ìƒ‰ƒ“ƒLƒ“ƒOæ“¾
    /// </summary>
    /// <param name="stageID"></param>
    /// <returns></returns>
    public async UniTask<List<RankingDto>> GetRankingAsync(int stageID)
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IRoomService>(channel);
        try
        {// æ“¾¬Œ÷
            var ranking = await client.GetRankingAsync(stageID);
            Debug.Log("æ“¾¬Œ÷");
            return ranking;
        }
        catch (RpcException e)
        {// æ“¾¸”s
            Debug.Log("æ“¾¸”s");
            return null;
        }
    }
}
