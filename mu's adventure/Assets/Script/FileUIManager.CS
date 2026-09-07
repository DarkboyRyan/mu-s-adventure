using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Read interactions run before other gameplay Update handlers (including E and Esc).
[DefaultExecutionOrder(-200)]
public class FileUIManager : MonoBehaviour
{
    public static FileUIManager Instance { get; private set; }
    public static bool IsInputBlocked => Instance != null &&
        (Instance.IsOpen || Instance.closedFrame == Time.frameCount);
    public bool IsOpen => currentDocument != null;

    [Header("UI References")]
    public GameObject fileUIPanel;
    public TMP_Text titleText;
    public TMP_Text contentText;
    public TMP_Text pageNumberText;
    public TMP_Text hintText;
    public TMP_Text interactionPrompt;

    [Header("Player Control")]
    public MonoBehaviour playerController;

    private readonly Dictionary<FileDocument, int> bookmarks = new Dictionary<FileDocument, int>();
    private FileDocument currentDocument;
    private int currentPage;
    private int closedFrame = -1;
    private float previousTimeScale;
    private bool controllerWasEnabled;
    private PlayerLadderClimb playerLadder;
    private bool ladderWasEnabled;
    private MonoBehaviour lockedController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (fileUIPanel != null) fileUIPanel.SetActive(false);
        SetPrompt(null);
    }

    private void Update()
    {
        if (Instance != this) return;
        if (IsOpen)
        {
            // Recover controls even if another script hides the panel.
            if (fileUIPanel == null || !fileUIPanel.activeInHierarchy)
            {
                CloseFile();
                return;
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseFile();
                return;
            }
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                PreviousPage();
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                NextPage();
            return;
        }

        bool canRead = !IsInputBlocked && Time.timeScale > 0f &&
            !MinigameCameraSwitcher.IsAnyMinigameActive &&
            (playerController == null || playerController.isActiveAndEnabled);
        var reader = canRead ? FileInteractable.GetNearestAvailable() : null;
        SetPrompt(reader != null ? reader.interactionLabel : null);
        if (reader != null && Input.GetKeyDown(KeyCode.E))
            OpenFile(reader.document);
    }

    public void OpenFile(FileDocument document)
    {
        if (!isActiveAndEnabled || Instance != this || IsInputBlocked ||
            MinigameCameraSwitcher.IsAnyMinigameActive || document == null || !document.HasContent)
            return;
        if (fileUIPanel == null || titleText == null || contentText == null || pageNumberText == null)
        {
            Debug.LogWarning("FileUIManager: assign the reading panel, title, content and page number.", this);
            return;
        }

        currentDocument = document;
        currentPage = bookmarks.TryGetValue(document, out int savedPage)
            ? Mathf.Clamp(savedPage, 0, document.pages.Length - 1) : 0;

        // Pause physics and timed gameplay, while Update continues to handle the reading UI.
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        lockedController = playerController;
        if (lockedController != null)
        {
            controllerWasEnabled = lockedController.enabled;
            playerLadder = lockedController.GetComponent<PlayerLadderClimb>();
            if (playerLadder != null)
            {
                ladderWasEnabled = playerLadder.enabled;
                playerLadder.enabled = false;
            }
            lockedController.enabled = false;
        }

        SetPrompt(null);
        fileUIPanel.SetActive(true);
        if (hintText != null) hintText.text = "A / Left - Previous    D / Right - Next    Esc - Close";
        UpdatePage();
    }

    public void CloseFile()
    {
        if (!IsOpen) return;
        bookmarks[currentDocument] = currentPage;
        currentDocument = null;
        closedFrame = Time.frameCount; // Do not pass this Esc/E press through to gameplay.
        if (fileUIPanel != null) fileUIPanel.SetActive(false);
        Time.timeScale = previousTimeScale;
        if (lockedController != null) lockedController.enabled = controllerWasEnabled;
        if (playerLadder != null) playerLadder.enabled = ladderWasEnabled;
        lockedController = null;
        playerLadder = null;
        SetPrompt(null);
    }

    private void OnDisable()
    {
        if (Instance != this) return;
        CloseFile();
        SetPrompt(null);
    }

    private void OnDestroy()
    {
        if (Instance != this) return;
        CloseFile();
        Instance = null;
    }

    public void NextPage()
    {
        if (!IsOpen || currentPage >= currentDocument.pages.Length - 1) return;
        currentPage++;
        UpdatePage();
    }

    public void PreviousPage()
    {
        if (!IsOpen || currentPage <= 0) return;
        currentPage--;
        UpdatePage();
    }

    private void UpdatePage()
    {
        titleText.text = currentDocument.fileTitle ?? string.Empty;
        contentText.text = currentDocument.pages[currentPage] ?? string.Empty;
        pageNumberText.text = $"{currentPage + 1} / {currentDocument.pages.Length}";
    }

    private void SetPrompt(string text)
    {
        if (interactionPrompt == null) return;
        interactionPrompt.text = text ?? string.Empty;
        interactionPrompt.gameObject.SetActive(!string.IsNullOrEmpty(text));
    }
}
