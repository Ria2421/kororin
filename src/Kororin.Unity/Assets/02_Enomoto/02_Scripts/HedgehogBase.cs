using UnityEngine;
using Pixeye.Unity;
using UnityEngine.EventSystems;
using UnityEditor.Animations;

public class HedgehogBase : MonoBehaviour
{
    #region �A�j���[�V�����֘A

    /// <summary>
    /// �A�j���[�V����ID
    /// </summary>
    public enum Anim_Id
    {
        Idle = 0,
        Idle_Ball,
        Run,
        Run_Ball,
        Jump,
        Jump_Ball,
        Land,   // ���X�|�[�������Ƃ��̃A�j���[�V����
        
        Celebrate,  // �j���G���[�g
        Defeated,   // �s�k�G���[�g
        No,
        Yes,
        Dance,
        Playing,
        Love
    }

    [Foldout("�A�j���[�V�����֘A")]
    [SerializeField]
    protected Animator animator;
    [Foldout("�A�j���[�V�����֘A")]
    [SerializeField]
    protected Avatar ballAvatar;
    [Foldout("�A�j���[�V�����֘A")]
    [SerializeField]
    protected Avatar defaultAvatar;
    #endregion

    #region �A�j���[�V�����֘A

    /// <summary>
    /// �{�[���Ɍ`�ԕω�
    /// </summary>
    public void ChangeToBallAvatar()
    {
        if (animator.avatar == ballAvatar) return;
        animator.avatar = ballAvatar;
        animator.Rebind();
    }

    /// <summary>
    /// �f�t�H���g�Ɍ`�ԕω�
    /// </summary>
    public void ChangeToDefaultAvatar()
    {
        if (animator.avatar == defaultAvatar) return;
        animator.avatar = defaultAvatar;
        animator.Rebind();
    }

    /// <summary>
    /// �Đ����̃A�j���[�V����ID�擾
    /// </summary>
    /// <returns></returns>
    public int GetAnimId()
    {
        return animator.GetInteger("animation_id");
    }

    /// <summary>
    /// �w�肵���A�j���[�V�������Đ�
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
