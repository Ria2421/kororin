//----------------------------------------------------------
// 試験用トップ画面マネージャー [ TestTopManager.cs ]
// Author:Kenta Nakamoto
//----------------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestTopManager : MonoBehaviour
{
    //-------------------
    // フィールド

    //-------------------
    // メソッド

    // 起動処理
    private void Awake()
    {
        if (!LoadingManager.Instance) SceneManager.LoadScene("01_UI_Loading", LoadSceneMode.Additive);
    }

    #region テスト用

    //// ユーザー登録テスト用
    //[SerializeField] private string testName = "";

    //// ランキング登録テスト用
    //[SerializeField] private int testUsId = 0;
    //[SerializeField] private int testStId = 0;
    //[SerializeField] private float testTime = 0;

    //===================
    // 試験用関数

    //public async void TestUserRegist()
    //{
    //    var i = await APIModel.Instance.RegistUserAsync(testName);
    //    Debug.Log(i);
    //}

    //public async void TestRankingRegist()
    //{
    //    var i = await APIModel.Instance.RegistRankingAsync(testUsId,testStId, testTime);
    //    Debug.Log(i);
    //}

    //public async void TestGetRanking()
    //{
    //    var i = await APIModel.Instance.GetRankingAsync(testStId);

    //    string op = "";
    //    for (var j = 0; j < i.Count ; j++)
    //    {
    //        op +=(j+1).ToString() + ":" + i[j].user_name + "\n";
    //    }
    //    Debug.Log(op);
    //}
    #endregion
}
