using UnityEngine;
using Pixeye.Unity;
using UnityEngine.EventSystems;
using UnityEditor.Animations;

public class HedgehogBase : MonoBehaviour
{
    #region �A�j���[�V�����֘A

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
    Avatar ballAvatar;
    [Foldout("�A�j���[�V�����֘A")]
    [SerializeField]
    Avatar defaultAvatar;

    // �G�t�F�N�g�̐����ʒu
    [Foldout("�A�j���[�V�����֘A")]
    [SerializeField]
    protected Transform reference;
    public Transform Reference { get { return reference; } }
    #endregion

    #region �p�[�e�B�N���֘A

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    [ContextMenu("Test")]
    public void Test()
    {
        int nextId = GetAnimId() == (int)Anim_Id.Love ? 0 : GetAnimId() + 1;
        SetAnimId(nextId);
    }

    protected virtual void Move()
    {

    }

    #region �A�j���[�V�����֘A

    /// <summary>
    /// �{�[���Ɍ`�ԕω�
    /// </summary>
    public void ChangeToBallAvatar()
    {
        if (animator.avatar == ballAvatar) return;
        animator.avatar = ballAvatar;
        animator.Rebind();

        InvokeRepeating("Move", 0, 0.1f);
    }

    /// <summary>
    /// �f�t�H���g�Ɍ`�ԕω�
    /// </summary>
    public void ChangeToDefaultAvatar()
    {
        if (animator.avatar == defaultAvatar) return;
        animator.avatar = defaultAvatar;
        animator.Rebind();

        CancelInvoke("Move");
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
            ChangeToBallAvatar();

        animator.SetInteger("animation_id", id);
    }

    #endregion
}
