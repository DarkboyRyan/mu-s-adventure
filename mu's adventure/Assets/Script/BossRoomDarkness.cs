using UnityEngine;

// Attach to a trigger zone covering the boss room entrance.
// Enables the darkness overlay when the player enters, disables it when they leave.
public class BossRoomDarkness : MonoBehaviour
{
    public GameObject darknessOverlay;
    public string playerTag = "Player";

    void Start()
    {
        if (darknessOverlay != null)
            darknessOverlay.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (darknessOverlay != null)
            darknessOverlay.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (darknessOverlay != null)
            darknessOverlay.SetActive(false);
    }
}
