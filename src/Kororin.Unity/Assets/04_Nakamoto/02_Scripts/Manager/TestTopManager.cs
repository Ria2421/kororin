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
    // メソッド

    // 起動処理
    private void Awake()
    {
        if (!LoadingManager.Instance) SceneManager.LoadScene("01_UI_Loading", LoadSceneMode.Additive);
    }
}
