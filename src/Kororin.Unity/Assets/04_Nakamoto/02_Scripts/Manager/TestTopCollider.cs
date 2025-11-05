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
            LoadingManager.Instance.StartSceneLoad("01_Stage");
        }
        else if(modeType == "Multi")
        {
            LoadingManager.Instance.StartSceneLoad("03_SampleMultiLobby");
        }
    }
}
