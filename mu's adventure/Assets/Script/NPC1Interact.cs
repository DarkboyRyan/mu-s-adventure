using UnityEngine;

// Attach to npc_1. Needs a BoxCollider2D (Is Trigger = true) and a SpriteRenderer.
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class NPC1Interact : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite clothedSprite;     // normal corpse appearance
    public Sprite unclothedSprite;   // after clothes are taken

    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    private SpriteRenderer _sr;
    private bool _clothesTaken = false;
    private bool _playerInRange = false;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        if (clothedSprite != null) _sr.sprite = clothedSprite;
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
        if (!_playerInRange) return;
        if (!Input.GetKeyDown(interactKey)) return;
        if (PlayerStatus.Instance == null) return;

        var s = PlayerStatus.Instance.current;

        if (!_clothesTaken && s == PlayerStatus.Status.BareHand)
        {
            // First interaction: take clothes
            _clothesTaken = true;
            PlayerStatus.Instance.current = PlayerStatus.Status.HasClothes;
            if (unclothedSprite != null) _sr.sprite = unclothedSprite;
        }
        else if (_clothesTaken && s == PlayerStatus.Status.HasClothes)
        {
            // Second interaction: pick up body
            PlayerStatus.Instance.current = PlayerStatus.Status.HasBody;
            gameObject.SetActive(false);
        }
    }
}
