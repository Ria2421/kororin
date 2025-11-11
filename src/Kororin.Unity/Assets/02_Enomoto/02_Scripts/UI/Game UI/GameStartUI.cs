using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GameStartUI : MonoBehaviour
{
    [SerializeField]
    List<GameObject> imageObjs = new List<GameObject>();

    private void OnEnable()
    {
        const float duration = 0.3f;
        const float interval = 0.6f;

        var result = DOTween.Sequence();
        foreach (var obj in imageObjs)
        {
            var image = obj.GetComponent<Image>();
            image.color = new Color(1, 1, 1, 0);
            var transform = obj.transform;
            transform.localScale = Vector3.one * 1.5f;

            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(Vector3.one, duration).SetEase(Ease.Linear))
                .Join(image.DOFade(1, duration).SetEase(Ease.Linear));
            result.Append(sequence);
        }

        result.Play();
    }
}
