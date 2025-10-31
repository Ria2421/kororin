using UnityEngine;
using DG.Tweening;

public class UITextWindow : MonoBehaviour
{
    [SerializeField]
    float tweenDuration;

    [SerializeField]
    float displayTime;

    [SerializeField]
    float endSize = 0.0012f;
    float startSize = 0f;

    [SerializeField]
    float endPosY = 0f;
    float startPosY = -0.5f;

    Sequence sequence = null;

    private void Awake()
    {
        sequence = DOTween.Sequence();
    }

    void Start()
    {
        transform.localScale = Vector3.zero;
        transform.localPosition = Vector3.up * startPosY;
    }

    /// <summary>
    /// テキストウインドウをdisplayTime時間表示する
    /// </summary>
    public void ShowTextWindow()
    {
        if (sequence == null || sequence.IsPlaying()) return;
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(endSize, tweenDuration).SetEase(Ease.OutBack))
            .Join(transform.DOLocalMove(Vector3.up * endPosY, tweenDuration).SetEase(Ease.OutBack))
            .AppendInterval(displayTime)
            .Append(transform.DOScale(startSize, tweenDuration).SetEase(Ease.InBack))
            .Join(transform.DOLocalMove(Vector3.up * startPosY, tweenDuration).SetEase(Ease.InBack));
    }
}
