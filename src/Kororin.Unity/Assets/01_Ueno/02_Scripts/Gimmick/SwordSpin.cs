//------------------------------------------
// ìÅâÒì]ÉMÉ~ÉbÉN [ SwordSpin.cs ]
// Author:Souma Ueno
//------------------------------------------
using UnityEngine;

public class SwordSpin : GimmickBase
{
    [SerializeField] float rollY = 15f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rollY = Random.Range(220, 380);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(new Vector3(0, rollY, 0) * Time.deltaTime);
    }
}
