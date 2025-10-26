using DI;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitDI()
    {
        var installer = FindFirstObjectByType<Installer>(FindObjectsInactive.Include);
        if (installer != null)
        {
            installer.Init();
        }
        else
        {
            Debug.Log("No installers founded");
        }
    }
}
