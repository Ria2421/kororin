//------------------------------------------
// ‰ñ“]ƒgƒQƒMƒ~ƒbƒN [ Thorn.cs ]
// Author:Souma Ueno
//------------------------------------------
using UnityEngine;

public class Thorn : GimmickBase
{
    [SerializeField] float rollY = 200f;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(new Vector3(0, -rollY, 0) * Time.deltaTime);
    }
}
