using System.Collections;
using UnityEngine;

// Attach to hiddenbox. Needs a BoxCollider2D (Is Trigger = true).
[RequireComponent(typeof(Collider2D))]
public class HiddenBoxInteract : MonoBehaviour
{
    [Header("Door Unlock (assign in Inspector)")]
    public DoorOpen doorToUnlock;        // DoorOpen script on the level2 exit door
    public GameObject doorHintToEnable;  // the hint/prompt child of that door — starts inactive

    [Header("Locker Animation")]
    public float openScaleX = 0.15f;    // how squished the sprite looks when open
    public float animDuration = 0.25f;

    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    private bool _playerInRange = false;
    private bool _isOpen = false;
    private bool _bodyInside = false;
    private bool _animating = false;
    private Vector3 _closedScale;

    void Start()
    {
        _closedScale = transform.localScale;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) _playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) _playerInRange = false;
    }

    void Update()
    {
        if (!_playerInRange || _animating || _bodyInside) return;
        if (!Input.GetKeyDown(interactKey)) return;
        if (PlayerStatus.Instance == null) return;

        if (PlayerStatus.Instance.current == PlayerStatus.Status.HasBody)
            StartCoroutine(HideBody());
        else
            StartCoroutine(_isOpen ? CloseLocker() : OpenLocker());
    }

    IEnumerator OpenLocker()
    {
        _animating = true;
        yield return AnimateTo(new Vector3(openScaleX, _closedScale.y, _closedScale.z));
        _isOpen = true;
        _animating = false;
    }

    IEnumerator CloseLocker()
    {
        _animating = true;
        yield return AnimateTo(_closedScale);
        _isOpen = false;
        _animating = false;
    }

    IEnumerator HideBody()
    {
        _animating = true;
        _bodyInside = true;

        // Close the locker around the body
        yield return AnimateTo(_closedScale);
        _isOpen = false;

        // Body is now hidden — player is left with just clothes
        PlayerStatus.Instance.current = PlayerStatus.Status.HasClothes;

        // Unlock the exit door
        if (doorToUnlock != null)   doorToUnlock.isLocked = false;
        if (doorHintToEnable != null) doorHintToEnable.SetActive(true);

        _animating = false;
    }

    IEnumerator AnimateTo(Vector3 target)
    {
        Vector3 start = transform.localScale;
        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / animDuration);
            transform.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }
        transform.localScale = target;
    }
}
