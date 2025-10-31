using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{
    Transform camera;

    private void Start()
    {
        camera = Camera.main.transform;
    }

    void Update()
    {
        transform.LookAt(camera);
    }
}
