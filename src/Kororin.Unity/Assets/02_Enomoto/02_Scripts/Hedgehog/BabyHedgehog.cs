using UnityEngine;
using DG.Tweening;

public class BabyHedgehog : HedgehogBase
{
    [SerializeField]
    UITextWindow textWindow;

    [SerializeField]
    Transform ctrls;

    Vector3 defaultAngles = Vector3.zero;
    Transform target;
    Tween tween = null;

    private void Start()
    {
        defaultAngles = ctrls.localEulerAngles;
    }

    private void OnTriggerEnter(Collider other)
    {
        textWindow.ShowTextWindow();
        if (!target)
        {
            if(tween != null) tween.Kill();
            target = other.transform;
            InvokeRepeating("LookAtTarget", 0, 0.1f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        target = null;
        CancelInvoke("LookAtTarget");
        ResetAngles();
    }

    void LookAtTarget()
    {
        Vector3 dir = target.position - ctrls.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        tween = ctrls.transform.DORotate(lookRot.eulerAngles, 0.5f).SetEase(Ease.Linear);
    }

    void ResetAngles()
    {
        tween = ctrls.transform.DOLocalRotate(defaultAngles, 0.5f).SetEase(Ease.Linear);
    }
}
