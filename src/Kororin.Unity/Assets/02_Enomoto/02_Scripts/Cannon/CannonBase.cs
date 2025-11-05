using UnityEngine;

public class CannonBase : MonoBehaviour
{
    [SerializeField] protected float firePower = 20;

    #region アニメーション関連
    [SerializeField]
    protected Animator animator;
    protected readonly string enterAnimName = "Enter";
    protected readonly string fireAnimName = "Fire";
    #endregion

    #region アニメーション関連

    public void PlayEnterAnim()
    {
        animator.Play(enterAnimName, 0, 0);
    }

    public void PlayFireAnim()
    {
        animator.Play(fireAnimName, 0, 0);
    }

    #region アニメーションイベントからの呼び出し

    public virtual void OnEnterAnim() { }

    public virtual void OnFireAnim() { }
    #endregion

    #endregion
}
