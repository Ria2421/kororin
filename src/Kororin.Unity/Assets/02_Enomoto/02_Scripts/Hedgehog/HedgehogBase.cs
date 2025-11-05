using UnityEngine;
using Pixeye.Unity;
using UnityEngine.EventSystems;
using UnityEditor.Animations;
using DG.Tweening;

public class HedgehogBase : MonoBehaviour
{
    #region アニメーション関連

    /// <summary>
    /// アニメーションID
    /// </summary>
    public enum Anim_Id
    {
        Idle = 0,
        Idle_Ball,
        Run,
        Run_Ball,
        Jump,
        Jump_Ball,
        Land,   // リスポーンしたときのアニメーション
        
        Celebrate,  // 祝うエモート
        Defeated,   // 敗北エモート
        No,
        Yes,
        Dance,
        Playing,
        Love
    }

    [Foldout("アニメーション関連")]
    [SerializeField]
    protected Animator animator;
    [Foldout("アニメーション関連")]
    [SerializeField]
    protected Avatar ballAvatar;
    [Foldout("アニメーション関連")]
    [SerializeField]
    protected Avatar defaultAvatar;

    [Foldout("アニメーション関連")]
    [SerializeField]
    protected bool playSpawnAnim = true;
    #endregion

    private void Start()
    {
        if(playSpawnAnim) PlaySpawnAnim();
    }

    #region アニメーション関連

    /// <summary>
    /// ボールに形態変化
    /// </summary>
    public void ChangeToBallAvatar()
    {
        if (animator.avatar == ballAvatar) return;
        animator.avatar = ballAvatar;
        animator.Rebind();
    }

    /// <summary>
    /// デフォルトに形態変化
    /// </summary>
    public void ChangeToDefaultAvatar()
    {
        if (animator.avatar == defaultAvatar) return;
        animator.avatar = defaultAvatar;
        animator.Rebind();

        var angles = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(angles.x, 0, angles.z);
    }

    /// <summary>
    /// 再生中のアニメーションID取得
    /// </summary>
    /// <returns></returns>
    public int GetAnimId()
    {
        return animator.GetInteger("animation_id");
    }

    /// <summary>
    /// 指定したアニメーションを再生
    /// </summary>
    /// <param name="id"></param>
    public void SetAnimId(int id)
    {
        Anim_Id animId = (Anim_Id)id;
        if (animId == Anim_Id.Idle_Ball || animId == Anim_Id.Run_Ball || animId == Anim_Id.Jump_Ball)
        {
            ChangeToBallAvatar();
        }
        else
        {
            ChangeToDefaultAvatar();
        }   

        animator.SetInteger("animation_id", id);
    }

    /// <summary>
    /// スポーンアニメーション演出
    /// </summary>
    void PlaySpawnAnim()
    {
        var parent = transform.parent.transform;
        Rigidbody rigidbody = parent.GetComponent<Rigidbody>();
        Vector3 startPos = new Vector3(0, 5, -8);
        Vector3 endPos;
        float jumpPower = 3;
        float duration = 2;

        ChangeToBallAvatar();
        SetAnimId((int)Anim_Id.Run_Ball);

        rigidbody.useGravity = false;
        endPos = parent.position;
        startPos += endPos;
        parent.position = startPos;

        var sequence = DOTween.Sequence();
        sequence.Append(parent.DOJump(new Vector3(endPos.x, endPos.y, startPos.z / 2), jumpPower, 1, duration / 2).SetEase(Ease.Linear))
            .Join(parent.DORotate(Vector3.right * 360f * 5f, duration / 2, RotateMode.FastBeyond360).SetEase(Ease.Linear))
            .Append(parent.DOJump(endPos, jumpPower / 2, 1, duration / 2).SetEase(Ease.Linear))
            .Join(parent.DORotate(Vector3.right * 360f, duration / 2, RotateMode.FastBeyond360).SetEase(Ease.Linear));
        sequence.OnComplete(() => 
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.useGravity = true;
            parent.eulerAngles = Vector3.zero;
            SetAnimId((int)Anim_Id.Land);
        });
    }

    #endregion
}
