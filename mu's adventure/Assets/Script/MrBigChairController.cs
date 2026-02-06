using UnityEngine;

/// <summary>
/// Mr Big's Chair - platform that can move up or down.
/// Call GoUp() / GoDown() from the button script.
/// </summary>
public class MrBigChairController : MonoBehaviour
{
    [Header("Levels (Y position)")]
    public float downLevel = 0f;
    public float upLevel = 5f;

    [Header("Movement")]
    public float moveDuration = 1.5f;
    public bool smoothMove = true;

    private Vector3 _startPos;
    private Vector3 _targetPos;
    private float _moveTimer;
    private bool _isMoving;

    void Start()
    {
        _isMoving = false;
    }

    void Update()
    {
        if (!_isMoving) return;

        _moveTimer -= Time.deltaTime;
        float t = 1f - Mathf.Clamp01(_moveTimer / moveDuration);
        if (smoothMove) t = Mathf.SmoothStep(0f, 1f, t);
        transform.position = Vector3.Lerp(_startPos, _targetPos, t);

        if (_moveTimer <= 0f)
        {
            transform.position = _targetPos;
            _isMoving = false;
        }
    }

    /// <summary>
    /// Move the chair platform up to upLevel.
    /// </summary>
    public void GoUp()
    {
        _startPos = transform.position;
        _targetPos = new Vector3(transform.position.x, upLevel, transform.position.z);
        _moveTimer = moveDuration;
        _isMoving = true;
    }

    /// <summary>
    /// Move the chair platform down to downLevel.
    /// </summary>
    public void GoDown()
    {
        _startPos = transform.position;
        _targetPos = new Vector3(transform.position.x, downLevel, transform.position.z);
        _moveTimer = moveDuration;
        _isMoving = true;
    }
}
