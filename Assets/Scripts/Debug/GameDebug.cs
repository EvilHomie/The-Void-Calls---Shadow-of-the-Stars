using UnityEngine;
using UnityEngine.InputSystem;

public class GameDebug : MonoBehaviour
{
    [SerializeField] ShipStatsDebug shipStatsDebug;
    [SerializeField] FrameTimeProfiler  frameTimeProfiler;


    private void Update()
    {
        if (Keyboard.current.f11Key.wasPressedThisFrame)
        {
            shipStatsDebug.gameObject.SetActive(!shipStatsDebug.gameObject.activeSelf);
        }

        if (Keyboard.current.f12Key.wasPressedThisFrame)
        {
            frameTimeProfiler.gameObject.SetActive(!frameTimeProfiler.gameObject.activeSelf);
        }
    }
}
