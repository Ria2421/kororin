//------------------------------------------
// ジャンプギミック [ JumpPad.cs ]
// Author:Souma Ueno
//------------------------------------------
using DG.Tweening;
using UnityEngine;

public class JumpPad : GimmickBase
{
    // ジャンプ力
    [SerializeField] float launchForce = 30f;

    [SerializeField] float downAmount;      // どれくらい沈むか
    [SerializeField] float cooldown;        // クールタイム
    [SerializeField] float upAmount;        // どれくらい飛び出すか

    [SerializeField] float downTime;       // 下がる速さ
    [SerializeField] float upTime;         // 上に飛び出す速さ
    [SerializeField] float returnTime;      // 元に戻る速さ

    private float defaultY;

    private bool iscooldown=false;


    private void Start()
    {
        defaultY = transform.localPosition.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (iscooldown) return;
       
        if(other.CompareTag("Player"))
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>();

            iscooldown = true;
            Invoke(nameof(ResetCoolDown), cooldown);

            playerRb.AddForce(Vector3.up * launchForce, ForceMode.Impulse);

            //transform.position = 
            //    new Vector3(transform.position.x, 
            //    -0.5f,
            //    transform.position.z);

            JumpPadAnimation();
        }
    }

    private void ResetCoolDown()
    {
        iscooldown = false;
    }

    private void JumpPadAnimation()
    {
        // 一旦、下にへこむ
        transform.DOLocalMoveY(defaultY - downAmount, downTime)
        .SetEase(Ease.InQuad)
        .OnComplete(() =>
        {
            // すぐに上に飛び出す
            transform.DOLocalMoveY(defaultY + upAmount, upTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // 最終的に元の位置へ戻る
                transform.DOLocalMoveY(defaultY, returnTime)
                .SetEase(Ease.OutBack);
            });
        });
    }
}
