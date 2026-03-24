using UnityEngine;

public class DoorHints : MonoBehaviour
{
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private string playerTag = "Player";

    private int insideCount = 0;

    private void Awake()
    {
        if (promptPanel != null) promptPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        insideCount++;
        if (promptPanel != null) promptPanel.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        insideCount = Mathf.Max(0, insideCount - 1);
        if (insideCount == 0 && promptPanel != null) promptPanel.SetActive(false);
    }
}