//*********************************************************
// 滑らかにシーン遷移させるスクリプト
// Author:Rui Enomoto
//*********************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] GameObject camera;
    [SerializeField] Animator canvasAnimator;
    [SerializeField] float fadeCompDurarion = 1.0f;

    string sceneName;
    public bool isLoading { get; private set; }

    public static LoadingManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Destroy(camera);
            isLoading = false;
            Instance = this;
            canvasAnimator.gameObject.SetActive(false);
            canvasAnimator.enabled = false;

            // シーン遷移しても破棄しないようにする
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// 非同期のシーン読み込み
    /// </summary>
    /// <returns></returns>
    IEnumerator Load()
    {
        yield return new WaitForSeconds(fadeCompDurarion);

        // シーンを非同期でロードする
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

        // ロードが完了するまで待機する
        while (true)
        {
            yield return null;

            if (async.isDone)
            {
                canvasAnimator.Play("FadeOut", 0, 0);
                break;
            }
        }

        yield return new WaitForSeconds(fadeCompDurarion);
        canvasAnimator.gameObject.SetActive(false);
        canvasAnimator.enabled = false;
        isLoading = false;
    }

    /// <summary>
    /// シーン読み込み開始
    /// </summary>
    /// <param name="sceneName"></param>
    public void StartSceneLoad(string sceneName)
    {
        if (isLoading) return;
        isLoading = true;

        canvasAnimator.gameObject.SetActive(true);
        canvasAnimator.enabled = true;
        canvasAnimator.Play("FadeIn", 0, 0);

        this.sceneName = sceneName;
        StartCoroutine(Load());
    }
}