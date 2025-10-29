using UnityEngine;

public class TestTopCollider : MonoBehaviour
{
    [SerializeField] string modeType;

    private void Awake()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (modeType == "Single")
        {
            TestTopManager.Instance.TransitionSinglScene();
        }
        else if(modeType == "Multi")
        {
            TestTopManager.Instance.TransitionMultiScene();
        }
    }
}
