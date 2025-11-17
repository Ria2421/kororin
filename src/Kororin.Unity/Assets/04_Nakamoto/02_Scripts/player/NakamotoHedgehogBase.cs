using UnityEngine;
using Pixeye.Unity;
using UnityEngine.EventSystems;
using UnityEditor.Animations;
using DG.Tweening;

public class NakamotoHedgehogBase : MonoBehaviour
{
    [SerializeField] NakamotoBall ball;

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

    [Foldout("スポーンアニメーション関連")]
    [SerializeField]
    protected bool playSpawnAnim = true;

    [Foldout("スポーンアニメーション関連")]
    [SerializeField]
    protected float spawnDist = 5;
    #endregion

    private void Start()
    {
        if (playSpawnAnim)
        {
            PlaySpawnAnim();
        }
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

        if (animId == Anim_Id.Land) ball.CanControl = false;

        animator.SetInteger("animation_id", id);
    }

    /// <summary>
    /// スポーンアニメーション演出
    /// </summary>
    void PlaySpawnAnim()
    {
        SetAnimId((int)Anim_Id.Idle_Ball);
        var parent = transform.parent.transform;
        Rigidbody rigidbody = parent.GetComponent<Rigidbody>();
        Vector3 spawnWeight = new Vector3(parent.forward.x * -spawnDist, parent.position.y + spawnDist, parent.forward.z * -spawnDist);
        Vector3 startPos = parent.position + spawnWeight;
        Vector3 endPos;
        float jumpPower = 3;
        float duration = 2;
        float landHeight = 0.25f;

        rigidbody.useGravity = false;
        endPos = parent.position;
        parent.position = startPos;

        ChangeToBallAvatar();
        SetAnimId((int)Anim_Id.Run_Ball);

        var sequence = DOTween.Sequence();
        sequence.Append(parent.DOJump(new Vector3((startPos.x - spawnWeight.x / 2), endPos.y, (startPos.z - spawnWeight.z / 2)), jumpPower, 1, duration / 2).SetEase(Ease.Linear))
            .Join(parent.DORotate(parent.eulerAngles + Vector3.right * 360f * 5f, duration / 2, RotateMode.FastBeyond360).SetEase(Ease.Linear))
            .Append(parent.DOJump(endPos + Vector3.up * landHeight, jumpPower / 2, 1, duration / 2).SetEase(Ease.Linear))
            .Join(parent.DORotate(parent.eulerAngles + Vector3.right * 360f, duration / 2, RotateMode.FastBeyond360).SetEase(Ease.Linear));
        sequence.OnComplete(() =>
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.useGravity = true;
            parent.eulerAngles = new Vector3(0, parent.eulerAngles.y, 0);
            SetAnimId((int)Anim_Id.Land);
        });
    }

    #endregion

    #region アニメーションイベント関連

    /// <summary>
    /// 着地アニメーション終了時
    /// </summary>
    public void OnEndLandAnim()
    {
        ball.CanControl = true;
    }
    #endregion
}
