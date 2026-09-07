using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 6f;

    [Header("Jump")]
    public float jumpForce = 12f;
    [SerializeField, Min(0.01f)]
    [Tooltip("两次成功跳跃之间的最短间隔（秒）。")]
    private float jumpInterval = 0.2f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("State")]
    public bool canMove = true; // ✅ 新增：是否允许左右移动
    public bool canJump = true; // ✅ 新增：是否允许跳跃

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private const int MaxJumps = 2;
    private int jumpsUsed;
    private float nextJumpTime;
    private bool jumpRequested;
    private readonly System.Collections.Generic.List<ContactPoint2D> groundContacts =
        new System.Collections.Generic.List<ContactPoint2D>(8);

    // New Input System
    private InputAction moveAction;
    private InputAction jumpAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;

        // Create actions in code (no InputActions asset needed)
        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("2DVector")
            .With("Left", "<Keyboard>/a")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/d")
            .With("Right", "<Keyboard>/rightArrow");

        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        jumpAction.performed += OnJump;
    }

    private void OnDisable()
    {
        jumpRequested = false;
        jumpAction.performed -= OnJump;
        moveAction.Disable();
        jumpAction.Disable();
    }

    private void Update()
    {
        // Read movement
        Vector2 move = moveAction.ReadValue<Vector2>();
        float x = move.x;
        
        // Flip（可选：不允许移动时不flip）
        if (sr != null && canMove)
        {
            if (x > 0.01f) sr.flipX = false;
            else if (x < -0.01f) sr.flipX = true;
        }
    }

    private void FixedUpdate()
    {
        float x = canMove ? moveAction.ReadValue<Vector2>().x : 0f;
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        // 起跳后可能还残留上一物理帧的地面接触，上升时不能重置次数。
        if (rb.linearVelocity.y <= 0.01f && IsGrounded())
            jumpsUsed = 0;

        if (!jumpRequested) return;
        jumpRequested = false;

        if (!canJump || jumpsUsed >= MaxJumps || Time.time < nextJumpTime) return;

        jumpsUsed++;
        nextJumpTime = Time.time + Mathf.Max(0.01f, jumpInterval);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        // 冷却期间的按键直接忽略，不在冷却结束后自动补跳。
        if (!canJump || Time.timeScale <= 0f || Time.time < nextJumpTime) return;
        jumpRequested = true;
    }

    private bool IsGrounded()
    {
        // 保留已有检测点配置；未绑定时直接使用刚体接触，兼容当前场景。
        if (groundCheck != null &&
            Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) == null)
            return false;

        var filter = new ContactFilter2D();
        filter.SetLayerMask(groundLayer);
        filter.useTriggers = false;
        int contactCount = rb.GetContacts(filter, groundContacts);
        for (int i = 0; i < contactCount; i++)
        {
            // 只有脚下的实体支撑才算落地，墙壁、天花板和触发器不能补充跳跃。
            if (groundContacts[i].normal.y > 0.5f)
                return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
