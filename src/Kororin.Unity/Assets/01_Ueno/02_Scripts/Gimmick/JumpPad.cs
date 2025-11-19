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


    [SerializeField] float cooldown;        // クールタイム

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
    {// ジャンプ台を 0 の位置まで飛び出させる
        transform.DOLocalMoveY(0f, upTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // 終わったら -2 に戻す
                transform.DOLocalMoveY(-2f, returnTime)
                    .SetEase(Ease.OutBack);
            });
    }
}
