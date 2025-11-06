using UnityEngine;

public class BoltRepeaterProjectile : MonoBehaviour
{
    [field: SerializeField] public Rigidbody2D RigidBody { get; private set; }
    [field: SerializeField] public Transform CTransform { get; private set; }
}
