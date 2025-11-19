//------------------------------------------
// 回転トゲギミック [ Thorn.cs ]
// Author:Souma Ueno
//------------------------------------------
using UnityEngine;

public class Thorn : GimmickBase
{
    [SerializeField] float rollY = 200f;

    // Update is called once per frame
    void Update()
    {
        // オフライン時
        if (RoomModel.Instance == null)
        {
            gameObject.transform.Rotate(new Vector3(0, -rollY, 0) * Time.deltaTime);
        }
        else
        {// オンライン時
            if (RoomModel.Instance.IsMaster)
            {
                gameObject.transform.Rotate(new Vector3(0, -rollY, 0) * Time.deltaTime);
            }
        }
    }
}
