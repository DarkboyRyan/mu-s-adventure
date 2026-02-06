using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [TextArea(2, 4)]
    public string[] lines;

    public Transform talkPoint;        // 👈 拖 NPC 的 TalkPoint
    public Vector3 screenOffset = new Vector3(0, 40f, 0); // UI 偏移（像素）

    private bool playerInRange = false;
    private DialogueUI dialogueUI;

    void Start()
    {
        dialogueUI = FindObjectOfType<DialogueUI>();
    }

    void Update()
    {
        if (!playerInRange || dialogueUI == null) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!dialogueUI.IsOpen)
                dialogueUI.Open(lines, talkPoint, screenOffset);
            else
                dialogueUI.Next();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (dialogueUI.IsOpen)
                dialogueUI.Close();
        }
    }
}
