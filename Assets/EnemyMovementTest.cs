using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovementTest : MonoBehaviour
{
    [SerializeField] private float moveForce = 20f;
    [SerializeField] private float turnTorque = 10f;
    [SerializeField] private float maxSpeed = 10f;

    private Rigidbody2D _rb;

    private float _timer;

    private enum State
    {
        Forward,
        Backward,
        StrafeRight,
        StrafeLeft,
        CircleRight,
        CircleLeft
    }

    private State _state;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Debug.LogError(_state);
        _timer += Time.fixedDeltaTime;

        // Каждые 15 секунд меняем режим
        if (_timer >= 15f)
        {
            _timer = 0f;
            _state = (State)(((int)_state + 1) % 6);
        }

        switch (_state)
        {
            case State.Forward:
                Move(Vector2.up);
                break;

            case State.Backward:
                Move(Vector2.down);
                break;

            case State.StrafeRight:
                Move(Vector2.right);
                break;

            case State.StrafeLeft:
                Move(Vector2.left);
                break;

            case State.CircleRight:
                Circle(1f);
                break;

            case State.CircleLeft:
                Circle(-1f);
                break;
        }

        ClampSpeed();
    }

    private void Move(Vector2 localDirection)
    {
        Vector2 worldDirection =
            transform.TransformDirection(localDirection);

        _rb.AddForce(worldDirection * moveForce);
    }

    private void Circle(float turnDirection)
    {
        // Тяга вперед
        _rb.AddForce(transform.up * moveForce);

        // Поворот корпуса
        _rb.AddTorque(-turnDirection * turnTorque);
    }

    private void ClampSpeed()
    {
        float speed = _rb.linearVelocity.magnitude;

        if (speed > maxSpeed)
        {
            _rb.linearVelocity =
                _rb.linearVelocity.normalized * maxSpeed;
        }
    }
}