using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class Roll : GimmickBase
{
    [SerializeField] float rollY = 15f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (RoomModel.Instance.IsMaster)
        {
            gameObject.transform.Rotate(new Vector3(0, rollY, 0) * Time.deltaTime);
        }
    }
}
