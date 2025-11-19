//------------------------------------------
// スライサーギミック [ Slicer.cs ]
// Author:Souma Ueno
//------------------------------------------
using DG.Tweening;
using UnityEngine;

public class Slicer : GimmickBase
{
    [SerializeField] float rollX;

    // 移動速度 (インスペクターで調整)
    float speed;

    // Z軸の往復距離 (初期位置から片側への最大距離)
    [SerializeField] float maxDistance = 1f;

    private float startX,startZ;

    void Start()
    {
        // ギミックの基準となるZ座標を記録
        startZ = transform.localPosition.z;
        startX = transform.localPosition.x; // X座標を固定

        speed = Random.Range(1.5f, 5);
    }

    void Update()
    {
        // オフライン時
        if (RoomModel.Instance == null)
        {
            // Time.time * speed で、時間に速度を掛けた分だけ値が変化
            float pingPong = Mathf.PingPong(Time.time * speed, maxDistance * 2f);

            // 範囲を -maxDistance から +maxDistance にシフト
            float offsetZ = pingPong - maxDistance;

            // 新しいZ座標を計算し、位置を更新
            transform.localPosition = new Vector3(
                startX,
                transform.localPosition.y,
                startZ + offsetZ
            );

            gameObject.transform.Rotate(new Vector3(-rollX, 0, 0) * Time.deltaTime);
        }
        else
        {// オンライン時
            if (RoomModel.Instance.IsMaster)
            {
                // Time.time * speed で、時間に速度を掛けた分だけ値が変化
                float pingPong = Mathf.PingPong(Time.time * speed, maxDistance * 2f);

                // 範囲を -maxDistance から +maxDistance にシフト
                float offsetZ = pingPong - maxDistance;

                // 新しいZ座標を計算し、位置を更新
                transform.localPosition = new Vector3(
                    startX,
                    transform.localPosition.y,
                    startZ + offsetZ
                );

                gameObject.transform.Rotate(new Vector3(-rollX, 0, 0) * Time.deltaTime);
            }
        }
    }
}
