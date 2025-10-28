using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerBase : MonoBehaviour
{
    protected virtual void Awake()
    {
        if (!LoadingManager.Instance) SceneManager.LoadScene("01_UI_Loading", LoadSceneMode.Additive);
    }
}
