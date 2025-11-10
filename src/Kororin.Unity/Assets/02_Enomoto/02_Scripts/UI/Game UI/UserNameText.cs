using DG.Tweening;
using System.Collections;
using UnityEngine;

public class UserNameText : MonoBehaviour
{
    [SerializeField]
    GameObject target;

    Vector3 defaultScale;
    float offsetPosY = 1f;

    private void Awake()
    {
        defaultScale = transform.localScale;
    }

    private void OnEnable()
    {
        StartCoroutine(TrackingTarget());
    }

    private void OnDisable()
    {
        StopCoroutine(TrackingTarget());
    }

    IEnumerator TrackingTarget()
    {
        const float waitSec = 0.1f;

        while (true)
        {
            if (target)
            {
                transform.localScale = defaultScale;
                transform.DOMove(target.transform.position + Vector3.up * offsetPosY, waitSec).SetEase(Ease.Linear);
            }
            else
            {
                transform.localScale = Vector3.zero;
            }
            yield return new WaitForSeconds(waitSec);
        }
    }
}
