using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Joy-Con専用で操作する用のスクリプト
/// </summary>
public class JoyConBallContoroller : MonoBehaviour
{
    Rigidbody rb;

    //JoyConリスト(接続されたすべてのJoyCon)
    List<Joycon> joycons;

    //左右のJoyCon
    Joycon joyconL;
    Joycon joyconR;

    // 傾きの感度、移動速度などの調整用パラメータ
    [SerializeField] float tiltSensitivity;  // 傾きに対する感度
    [SerializeField] float moveSpeed;        // ボールの転がりスピード
    [SerializeField] float smoothing;        // 入力のなめらかさ
    [SerializeField] float drag;             // 摩擦（止まりやすさ）


    // 変化量をなめらかにするための補間用の変数
    Vector3 smoothedInput = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //摩擦の調整
        rb.linearDamping = drag;

        // Joy-Conのリストを取得
        joycons = JoyconManager.Instance.j;

        // Joy-Conが接続されているか確認
        if (joycons.Count > 0)
        {
            // 左のJoy-Conを取得
            joyconL = joycons.Find(c => c.isLeft);

            //右のJoy-Conを取得
            /*joyconR = joycons.Find(c => !c.isLeft);*/
        }
        else
        {
            Debug.LogWarning("Joy-Conが見つかりませんでした！");
        }
    }

    //毎フレーム更新
    void FixedUpdate()
    {

        // Joy-Conが接続されていない場合、処理を終了
        if (joycons.Count == 0) return;

        // 加速度（左と右のJoy-Con用）
        Vector3 accelL = Vector3.zero;
        Vector3 accelR = Vector3.zero;

        // Joy-Conが接続されている数をカウント
        int activeCount = 0;

        // 左のJoy-Conが接続されていれば、その加速度を取得
        if (joyconL != null)
        {
            accelL = joyconL.GetAccel();// 加速度取得
            activeCount++;// 左が有効ならカウントを増やす
        }

        // 右のJoy-Conが接続されていれば、その加速度を取得
        if (joyconR != null)
        {
            accelR = joyconR.GetAccel();// 加速度取得
            activeCount++;// 右が有効ならカウントを増やす
        }

        // どちらかが有効なら平均を取る
        Vector3 accel = (accelL + accelR) / Mathf.Max(activeCount, 1);

        // 軸調整
        Vector3 input = new Vector3(accel.x, 0, -accel.y) * tiltSensitivity;

        // スムーズ補間
        smoothedInput = Vector3.Lerp(smoothedInput, input, Time.fixedDeltaTime * smoothing);

        // 力を加える
        rb.AddForce(smoothedInput * moveSpeed);
    }
}
