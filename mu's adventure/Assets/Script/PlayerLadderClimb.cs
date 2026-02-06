using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerLadderClimb : MonoBehaviour
{
    public float climbSpeed = 4f;

    [Header("Top Exit")]
    public Transform topStandPoint;   // ✅ 到顶后站的位置（你拖一个空物体进来）
    public float snapDuration = 0.05f; // 小缓冲，避免抖动

    [Header("Refs")]
    public PlayerMovement2D movement;

    private Rigidbody2D rb;
    private float defaultGravity;
    private bool inLadderZone = false;
    private bool isClimbing = false;
    private bool snapping = false;

    private InputAction climbAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;

        climbAction = new InputAction("Climb", InputActionType.Value);
        climbAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/s")
            .With("Down", "<Keyboard>/downArrow");
    }

    private void OnEnable() => climbAction.Enable();
    private void OnDisable() => climbAction.Disable();

    private void Update()
    {
        if (snapping) return;

        float v = climbAction.ReadValue<Vector2>().y;

        if (inLadderZone && Mathf.Abs(v) > 0.01f)
            isClimbing = true;

        if (!inLadderZone)
            isClimbing = false;

        if (isClimbing)
        {
            rb.gravityScale = 0f;

            if (movement != null)
            {
                movement.canMove = false;
                movement.canJump = false;
            }

            rb.linearVelocity = new Vector2(0f, v * climbSpeed);
        }
        else
        {
            rb.gravityScale = defaultGravity;

            if (movement != null)
            {
                movement.canMove = true;
                movement.canJump = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
            inLadderZone = true;

        // ✅ 到顶出口：把玩家放到二楼
        if (other.CompareTag("LadderTop"))
        {
            if (topStandPoint != null)
                StartCoroutine(SnapToTop());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
            inLadderZone = false;
    }

    private System.Collections.IEnumerator SnapToTop()
    {
        snapping = true;

        // 停止爬梯子状态
        isClimbing = false;
        inLadderZone = false;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        // 暂时禁用移动，防止被速度覆盖
        if (movement != null)
        {
            movement.canMove = false;
            movement.canJump = false;
        }

        // 直接“放”到二楼站点
        transform.position = topStandPoint.position;

        // 等一小会儿再恢复重力和移动
        yield return new WaitForSeconds(snapDuration);

        rb.gravityScale = defaultGravity;

        if (movement != null)
        {
            movement.canMove = true;
            movement.canJump = true;
        }

        snapping = false;
    }
}
