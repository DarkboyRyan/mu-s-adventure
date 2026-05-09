using UnityEngine;

// Attach to a trigger zone at the boss room entrance.
// When the player enters, the camera locks to fixedTarget (room center).
// When the player exits, the camera returns to following the player.
public class CameraZoneTrigger : MonoBehaviour
{
    [Header("References")]
    public CameraFollow cameraFollow;
    public Transform fixedTarget;    // empty GO placed at boss room center
    public Transform playerTarget;   // player Transform
    public string playerTag = "Player";

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (cameraFollow != null && fixedTarget != null)
            cameraFollow.SetTarget(fixedTarget);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (cameraFollow != null && playerTarget != null)
            cameraFollow.SetTarget(playerTarget);
    }
}
