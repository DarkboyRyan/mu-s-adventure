using UnityEngine;

public class DoorHintSimple : MonoBehaviour
{
    [SerializeField] private GameObject hint; // Door/Hint
    [SerializeField] private string playerTag = "Player";

    private void Awake()
    {
        if (hint != null) hint.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (hint != null) hint.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (hint != null) hint.SetActive(false);
    }
}