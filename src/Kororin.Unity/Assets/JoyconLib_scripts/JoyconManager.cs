using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System;
/// <summary>
/// Joy-Conの接続・管理を行うクラス
/// Unity上でJoy-Conを扱うための基本的なマネージャー
/// </summary>
public class JoyconManager: MonoBehaviour
{

    // Settings accessible via Unity
    public bool EnableIMU = true;     // ジャイロ・加速度（IMU）を有効にするか
    public bool EnableLocalize = true;// 空間位置追跡を有効にするか（IMU使用時のみ）

    // --- Joy-Conの識別ID ---
    // NintendoのJoy-ConのベンダーID（ハードウェア識別用）
    // Different operating systems either do or don't like the trailing zero
    private const ushort vendor_id = 0x57e;
	private const ushort vendor_id_ = 0x057e;

    // Joy-Con (L) と (R) のプロダクトID
    private const ushort product_l = 0x2006;
	private const ushort product_r = 0x2007;

    // --- 管理用変数 ---
    public List<Joycon> j;        // 接続中のJoy-Con一覧
    static JoyconManager instance;// シングルトン（どこからでもアクセスできる）


    /// <summary>
    /// JoyconManagerのインスタンスを取得
    /// </summary>
    public static JoyconManager Instance
    {
        get { return instance; }
    }
    // --- 起動時処理 ---
    void Awake()
    {
        // シングルトン化（重複生成防止）
        if (instance != null) Destroy(gameObject);
        instance = this;
		int i = 0;

        // Joy-Con管理用リスト初期化
        j = new List<Joycon>();
		bool isLeft = false;

        // HIDライブラリの初期化（Joy-ConはHIDデバイスとして扱う）
        HIDapi.hid_init();

        // Joy-Conデバイスを列挙（vendor_id で検索）
        IntPtr ptr = HIDapi.hid_enumerate(vendor_id, 0x0);
		IntPtr top_ptr = ptr;// ループ終了後に解放するため保持


        // Joy-Conが見つからなかった場合、別のIDでも試す
        if (ptr == IntPtr.Zero)
		{
			ptr = HIDapi.hid_enumerate(vendor_id_, 0x0);
			if (ptr == IntPtr.Zero)
			{ 
				HIDapi.hid_free_enumeration(ptr);
				Debug.Log ("No Joy-Cons found!");
			}
		}
		hid_device_info enumerate;

        // --- デバイス列挙処理 ---
        while (ptr != IntPtr.Zero) {
            // 構造体に変換（HID情報を取得）
            enumerate = Marshal.PtrToStructure<hid_device_info>(ptr);

            Debug.Log (enumerate.product_id);

            // Joy-Con (L) または (R) の場合のみ処理
            if (enumerate.product_id == product_l || enumerate.product_id == product_r) {
					if (enumerate.product_id == product_l) {
						isLeft = true;
						Debug.Log ("Left Joy-Con connected.");
					} else if (enumerate.product_id == product_r) {
						isLeft = false;
						Debug.Log ("Right Joy-Con connected.");
					} else {
						Debug.Log ("Non Joy-Con input device skipped.");
					}

                // デバイスを開く
                IntPtr handle = HIDapi.hid_open_path (enumerate.path);
                // 非ブロッキングモードに設定（待機せずにデータを受け取れるように）
                HIDapi.hid_set_nonblocking (handle, 1);
                // Joy-Conオブジェクトを生成してリストに追加
                j.Add (new Joycon (handle, EnableIMU, EnableLocalize & EnableIMU, 0.05f, isLeft));
					++i;
				}

            // 次のデバイスへ
            ptr = enumerate.next;
			}
        // HID列挙結果の解放
        HIDapi.hid_free_enumeration (top_ptr);
    }
    // --- 起動完了後（Joy-Con初期化） ---
    void Start()
    {
		for (int i = 0; i < j.Count; ++i)
		{
			Debug.Log (i);
			Joycon jc = j [i];
			byte LEDs = 0x0;

            // インデックスに応じてLEDを点灯（どのJoy-Conか識別用）
            LEDs |= (byte)(0x1 << i);
			jc.Attach (leds_: LEDs);// LED設定
            jc.Begin ();// 通信開始
        }
    }
    // --- 毎フレーム更新 ---
    void Update()
    {
        // すべてのJoy-Conを更新
        for (int i = 0; i < j.Count; ++i)
		{
			j[i].Update();
		}
    }
    // --- 終了処理 ---
    void OnApplicationQuit()
    {
        // Joy-Conの接続を解除
        for (int i = 0; i < j.Count; ++i)
		{
			j[i].Detach ();
		}
    }
}
