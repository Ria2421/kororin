using DG.Tweening;
using UnityEngine;

public class Gimmck : GimmickBase
{
    [SerializeField] float angle = 45f;        // 振れる角度
    [SerializeField] float duration = 1.5f;    // 片道の時間

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        float startAngle = -angle;

        transform.rotation = Quaternion.Euler(0, 0, startAngle);
        // 左右に揺れるアニメーション（無限ループ）
        transform
            .DORotate(new Vector3(0, 0, angle), duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
