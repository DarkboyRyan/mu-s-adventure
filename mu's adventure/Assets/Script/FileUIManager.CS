using TMPro;
using UnityEngine;

public class FileUIManager : MonoBehaviour
{
    public static FileUIManager Instance;

    [Header("UI References")]
    public GameObject fileUIPanel;
    public TMP_Text titleText;
    public TMP_Text contentText;
    public TMP_Text pageNumberText;
    public TMP_Text hintText;

    [Header("Player Control")]
    public MonoBehaviour playerController;  

    private FileDocument currentDocument;
    private int currentPage = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        fileUIPanel.SetActive(false);
    }

    private void Update()
    {
        if (!fileUIPanel.activeSelf || currentDocument == null)
            return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            PreviousPage();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            NextPage();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseFile();
        }
    }

    public void OpenFile(FileDocument document)
    {
        if (document == null || document.pages == null || document.pages.Length == 0)
            return;

        currentDocument = document;
        currentPage = 0;

        fileUIPanel.SetActive(true);

        if (hintText != null)
        {
            hintText.text = "A - Previous\nD - Next\nEsc - Close";
        }

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        UpdatePage();
    }

    public void CloseFile()
    {
        fileUIPanel.SetActive(false);
        currentDocument = null;

        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }

    private void NextPage()
    {
        if (currentDocument == null)
            return;

        if (currentPage < currentDocument.pages.Length - 1)
        {
            currentPage++;
            UpdatePage();
        }
    }

    private void PreviousPage()
    {
        if (currentDocument == null)
            return;

        if (currentPage > 0)
        {
            currentPage--;
            UpdatePage();
        }
    }

    private void UpdatePage()
    {
        if (currentDocument == null)
            return;

        titleText.text = currentDocument.fileTitle;
        contentText.text = currentDocument.pages[currentPage];
        pageNumberText.text = $"{currentPage + 1} / {currentDocument.pages.Length}";
    }
}