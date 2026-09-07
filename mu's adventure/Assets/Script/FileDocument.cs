using UnityEngine;

[CreateAssetMenu(fileName = "NewFileDocument", menuName = "Game/File Document")]
public class FileDocument : ScriptableObject
{
    public string fileTitle;

    [TextArea(5, 15)]
    public string[] pages;

    public bool HasContent => pages != null &&
        System.Array.Exists(pages, page => !string.IsNullOrWhiteSpace(page));
}
