using UnityEngine;

public class Thorn : MonoBehaviour
{
    [SerializeField] float rollY = 200f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(new Vector3(0, -rollY, 0) * Time.deltaTime);
    }
}
