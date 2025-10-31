using UnityEngine;
using Pixeye.Unity;
using UnityEngine.EventSystems;
using UnityEditor.Animations;

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
    #endregion

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

    #endregion
}
