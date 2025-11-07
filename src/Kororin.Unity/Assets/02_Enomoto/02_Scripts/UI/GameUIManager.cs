using Pixeye.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    #region フェード関連
    [Foldout("フェード関連")]
    [SerializeField]
    Image fadeImg;
    #endregion

    static public GameUIManager Instance {  get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    #region 黒フェード処理

    public void PlayFade(Action onFinished)
    {
        StartCoroutine(FadeScrean(onFinished));
    }

    IEnumerator FadeScrean(Action onFinished)
    {
        const float addAlphaAmount = 0.1f;
        const float waitSec = 0.05f;

        while (fadeImg.color.a < 1)
        {
            var color = fadeImg.color;
            color.a += addAlphaAmount;
            fadeImg.color = color;
            yield return new WaitForSeconds(waitSec);
        }

        yield return new WaitForSeconds(waitSec * 2);

        while (fadeImg.color.a > 0)
        {
            var color = fadeImg.color;
            color.a -= addAlphaAmount;
            fadeImg.color = color;
            yield return new WaitForSeconds(waitSec);
        }

        onFinished?.Invoke();
    }

    #endregion
}