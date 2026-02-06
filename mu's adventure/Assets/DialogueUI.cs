using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    private string[] lines;
    private int index;
    private bool isOpen;

    // ✅ 新增：跟随目标
    private Transform followTarget;
    private Vector3 offset;

    public bool IsOpen => isOpen;

    void Awake()
    {
        Close();
    }

    void Update()
    {
        // 如果正在对话，让对话框跟着 NPC
        if (isOpen && followTarget != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(followTarget.position);
            dialoguePanel.transform.position = screenPos + offset;
        }
    }

    // ✅ 打开对话（带目标）
    public void Open(string[] newLines, Transform target, Vector3 screenOffset)
    {
        lines = newLines;
        index = 0;
        isOpen = true;

        followTarget = target;
        offset = screenOffset;

        dialoguePanel.SetActive(true);
        dialogueText.text = lines[index];
    }

    public void Next()
    {
        if (!isOpen) return;

        index++;
        if (index >= lines.Length)
        {
            Close();
            return;
        }

        dialogueText.text = lines[index];
    }

    public void Close()
    {
        isOpen = false;
        lines = null;
        index = 0;
        followTarget = null;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }
}
