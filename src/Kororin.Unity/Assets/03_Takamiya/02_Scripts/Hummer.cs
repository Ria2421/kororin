using DG.Tweening;
using UnityEngine;

public class Gimmck : GimmickBase
{
    [SerializeField] float angle;        // 振れる角度
    [SerializeField] float duration;    // 片道の時間
    [SerializeField] bool startFromRight = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        float startAngle = startFromRight ? -angle : angle;
        float endAngle = startFromRight ? angle : -angle;

        transform.rotation = Quaternion.Euler(0, 0, startAngle);
        // 左右に揺れるアニメーション（無限ループ）
        transform
            .DORotate(new Vector3(0, 0, endAngle), duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
