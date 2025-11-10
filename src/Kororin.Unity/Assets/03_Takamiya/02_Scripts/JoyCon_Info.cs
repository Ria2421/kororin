using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
/// <summary>
/// Joy-Conで操作した際にJoy-Conのデータを表示するスクリプト
/// </summary>
public class JoyCon_Info : MonoBehaviour
{
    // Joy-Conのボタンを列挙型で定義
    private static readonly Joycon.Button[] m_buttons =
     Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    // Joy-Conのリスト、左・右Joy-Con、押されたボタンの情報を保持
    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon.Button? m_pressedButtonL;
    private Joycon.Button? m_pressedButtonR;

    private void Start()
    {
        // Joy-Conのリストを取得
        m_joycons = JoyconManager.Instance.j;

        // Joy-Conが接続されていない場合は処理を終了
        if (m_joycons == null || m_joycons.Count <= 0) return;

        // 左と右のJoy-Conを取得
        m_joyconL = m_joycons.Find(c => c.isLeft);// Joy-Con (L)
        m_joyconR = m_joycons.Find(c => !c.isLeft);// Joy-Con (R)
    }

    private void Update()
    {
        //// フレームごとに押下状態をリセット
        //m_pressedButtonL = null;
        //m_pressedButtonR = null;

        //// Joy-Con未接続時は処理をスキップ
        //if (m_joycons == null || m_joycons.Count <= 0) return;

        //// --- 各ボタンの押下を確認 ---
        //foreach (var button in m_buttons)
        //{
        //    // 左Joy-Conのボタンが押された場合
        //    if (m_joyconL != null && m_joyconL.GetButton(button))
        //    {
        //        m_pressedButtonL = button;
        //    }

        //    // 右Joy-Conのボタンが押された場合
        //    if (m_joyconR != null && m_joyconR.GetButton(button))
        //    {
        //        m_pressedButtonR = button;
        //    }
        //}

        //// --- デバッグ用：Z / Xキーで振動テスト ---
        //if (Input.GetKeyDown(KeyCode.Z) && m_joyconL != null)
        //{
        //    // 引数：(lowFreq, highFreq, 振動の強さ, 継続時間[ms])
        //    m_joyconL.SetRumble(160, 320, 0.6f, 200);
        //}

        //if (Input.GetKeyDown(KeyCode.X) && m_joyconR != null)
        //{
        //    m_joyconR.SetRumble(160, 320, 0.6f, 200);
        //}
    }

    /// <summary>
    /// GUIに情報を表示
    /// </summary>
    private void OnGUI()
    {
        var style = GUI.skin.GetStyle("label");
        style.fontSize = 40;

        if (m_joycons == null || m_joycons.Count <= 0)
        {
            GUILayout.Label("Joy-Con が接続されていません!");
            return;
        }

        GUILayout.BeginHorizontal(GUILayout.Width(700));

        // 左Joy-Conがある場合のみ表示
        if (m_joyconL != null)
            ShowJoyConInfo(m_joyconL, "Joy-Con (L)");

        // 右Joy-Conがある場合のみ表示
        if (m_joyconR != null)
            ShowJoyConInfo(m_joyconR, "Joy-Con (R)");

        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Joy-Con の情報を1つ分表示する共通関数
    /// </summary>
    private void ShowJoyConInfo(Joycon joycon, string name)
    {
        var gyro = joycon.GetGyro();          // ジャイロの値
        var accel = joycon.GetAccel();        // 加速度
        var orientation = joycon.GetVector(); // 傾きのベクトル

        GUILayout.BeginVertical(GUILayout.Width(1000));
        GUILayout.Label(name);                    // Joy-Con名（LかR）
        GUILayout.Label($"ジャイロ：{gyro}");     // ジャイロ
        GUILayout.Label($"加速度：{accel}");      // 加速度
        GUILayout.Label($"傾き：{orientation}");  // 傾き
        GUILayout.EndVertical();
    }
}
