using UnityEngine;

public class LookAheadCursor : MonoBehaviour
{
    public Transform Transform;

    private void Awake()
    {
        Transform = transform;
    }
}
