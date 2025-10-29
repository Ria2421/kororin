using UnityEngine;
using Pixeye.Unity;
using UnityEngine.EventSystems;
using UnityEditor.Animations;

public class HedgehogBase : MonoBehaviour
{
    #region アニメーション関連

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
    Avatar ballAvatar;
    [Foldout("アニメーション関連")]
    [SerializeField]
    Avatar defaultAvatar;

    // エフェクトの生成位置
    [Foldout("アニメーション関連")]
    [SerializeField]
    protected Transform reference;
    public Transform Reference { get { return reference; } }
    #endregion

    #region パーティクル関連

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

    #region アニメーション関連

    /// <summary>
    /// ボールに形態変化
    /// </summary>
    public void ChangeToBallAvatar()
    {
        if (animator.avatar == ballAvatar) return;
        animator.avatar = ballAvatar;
        animator.Rebind();

        InvokeRepeating("Move", 0, 0.1f);
    }

    /// <summary>
    /// デフォルトに形態変化
    /// </summary>
    public void ChangeToDefaultAvatar()
    {
        if (animator.avatar == defaultAvatar) return;
        animator.avatar = defaultAvatar;
        animator.Rebind();

        CancelInvoke("Move");
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
            ChangeToBallAvatar();

        animator.SetInteger("animation_id", id);
    }

    #endregion
}
