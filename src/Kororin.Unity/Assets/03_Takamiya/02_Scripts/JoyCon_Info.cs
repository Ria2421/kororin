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
        // 毎フレームで押されたボタンをリセット
        m_pressedButtonL = null;
        m_pressedButtonR = null;

        // Joy-Conが接続されていない場合は処理を終了
        if (m_joycons == null || m_joycons.Count <= 0) return;

        // すべてのボタンを確認して、押されているボタンを特定
        foreach (var button in m_buttons)
        {
            // 左のJoy-Conでボタンが押されたら、そのボタンを保存
            if (m_joyconL.GetButton(button))
            {
                m_pressedButtonL = button;
            }

            // 右のJoy-Conでボタンが押されたら、そのボタンを保存
            if (m_joyconR.GetButton(button))
            {
                m_pressedButtonR = button;
            }
        }


        // Zキーを押したら左のJoy-Conを振動させる
        if (Input.GetKeyDown(KeyCode.Z))
        {
            m_joyconL.SetRumble(160, 320, 0.6f, 200);// 振動の強さや時間を設定
        }

        // Xキーを押したら右のJoy-Conを振動させる
        if (Input.GetKeyDown(KeyCode.X))
        {
            m_joyconR.SetRumble(160, 320, 0.6f, 200);// 振動の強さや時間を設定
        }
    }

    // GUIに情報を表示
    private void OnGUI()
    {
        // GUIのラベルのスタイルを設定
        var style = GUI.skin.GetStyle("label");
        style.fontSize = 50;

        // Joy-Conが接続されていない場合のメッセージを表示
        if (m_joycons == null || m_joycons.Count <= 0)
        {
            GUILayout.Label("Joy-Con が接続されていません");
            return;
        }

        // 左のJoy-Conが接続されていない場合のメッセージを表示
        if (!m_joycons.Any(c => c.isLeft))
        {
            GUILayout.Label("Joy-Con (L) が接続されていません");
            return;
        }

        // 右のJoy-Conが接続されていない場合のメッセージを表示
        if (!m_joycons.Any(c => !c.isLeft))
        {
            GUILayout.Label("Joy-Con (R) が接続されていません");
            return;
        }
        // 横並びでJoy-Con情報を表示
        GUILayout.BeginHorizontal(GUILayout.Width(1080));

        // すべてのJoy-Conの情報を表示
        foreach (var joycon in m_joycons)
        {
            var isLeft = joycon.isLeft;
            var name = isLeft ? "Joy-Con (L)" : "Joy-Con (R)";        // 左右のJoy-Con名
            var key = isLeft ? "Z キー" : "X キー";                   // 振動用のキー名（ZかX）
            var button = isLeft ? m_pressedButtonL : m_pressedButtonR;// 押されているボタン
            var stick = joycon.GetStick();                            // スティックの位置
            var gyro = joycon.GetGyro();                              // ジャイロの値
            var accel = joycon.GetAccel();                            // 加速度
            var orientation = joycon.GetVector();                     // 傾きのベクトル

            GUILayout.BeginVertical(GUILayout.Width(1000));
            GUILayout.Label(name);                                    // Joy-Con名（LかR）
            GUILayout.Label(key + "：振動");                          // 振動するキー
            GUILayout.Label("押されているボタン：" + button);         // 押されたボタン
            GUILayout.Label(string.Format("スティック：({0}, {1})", stick[0], stick[1]));// スティックの位置
            GUILayout.Label("ジャイロ：" + gyro);                     // ジャイロ
            GUILayout.Label("加速度：" + accel);                      // 加速度
            GUILayout.Label("傾き：" + orientation);                  // 傾き
            GUILayout.EndVertical();
        }

        // 横並びの終了
        GUILayout.EndHorizontal();
    }
}
