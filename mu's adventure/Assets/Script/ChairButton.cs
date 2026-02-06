using UnityEngine;
using System.Collections;

/// <summary>
/// Button for Mr Big's Chair: press E when in range, wait 2 seconds, then chair goes up.
/// Put this on the button GameObject. Add a Collider2D (Is Trigger) so the player can enter range.
/// </summary>
public class ChairButton : MonoBehaviour
{
    [Header("Reference")]
    public MrBigChairController chair;

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;
    public float waitSeconds = 2f;
    public string playerTag = "Player";

    private bool _playerInRange;
    private bool _alreadyTriggered;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            _playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
            _playerInRange = false;
    }

    void Update()
    {
        if (_alreadyTriggered || !_playerInRange || chair == null) return;
        if (!Input.GetKeyDown(interactKey)) return;

        _alreadyTriggered = true;
        StartCoroutine(WaitThenRaiseChair());
    }

    IEnumerator WaitThenRaiseChair()
    {
        yield return new WaitForSeconds(waitSeconds);
        chair.GoUp();
    }
}
