//-------------------------------------------------------------
// プレイヤーの動作テストスクリプト [ Player.cs ]
// Author:中本健太
//-------------------------------------------------------------
using Shared.Interfaces.StreamingHubs;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 自身かどうか判別
    private bool isSelf = false;
    public bool IsSelf { get { return isSelf; } set { isSelf = value; } }

    // ゴールしたか判別
    private bool isGoal = false;

    private Rigidbody rb;

    [SerializeField] private float deceleratSpeed; // 減速スピード
    [SerializeField] private float applyForce;     // 加える力
    [SerializeField] protected Animator animator;

    private float dx, dz;

    void Start()
    {
        // Rigidbodyコンポーネントを取得
        rb = GetComponent<Rigidbody>();

        // 減速のためのDrag（抵抗）を設定
        // 値が大きいほど早く減速
        rb.linearDamping = deceleratSpeed;

        // オンラインのみ
        if (!RoomModel.Instance) return;
    }

    void Update()
    {
        // プレイヤーの入力を取得
        dx = Input.GetAxis("Horizontal");
        dz = Input.GetAxis("Vertical");

        AddForce();
    }

    public void AddForce()
    {
        // 入力に基づいて移動方向を計算
        var movement = new Vector3(dx, 0, dz);

        // 球体に力を加える
        rb.AddForce(movement * applyForce);
    }

    /// <summary>
    /// アニメーション設定処理
    /// </summary>
    /// <param name="id"></param>
    public virtual void SetAnimId(int id)
    {
        if (animator == null) return;
        animator.SetInteger("animation_id", id);
    }

    /// <summary>
    /// アニメーションID取得処理
    /// </summary>
    /// <returns></returns>
    public int GetAnimId()
    {
        return animator != null ? animator.GetInteger("animation_id") : 0;
    }

    /// <summary>
    /// トリガー接触判定
    /// </summary>
    /// <param name="other"></param>
    private async void OnTriggerEnter(Collider other)
    {
        if(!isSelf) return;

        if(other.tag == "Standby")
        {
            TestMultiLobbyManager.Instance.IsStandby = true;
            await RoomModel.Instance.StandbyAsync();
        }

        if (!isGoal && other.tag == "Goal")
        {
            isGoal = true;
            await RoomModel.Instance.ArrivalGoalAsync();
        }
    }
}
