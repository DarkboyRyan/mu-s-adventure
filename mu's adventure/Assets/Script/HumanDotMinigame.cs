using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HumanDotMinigame : MonoBehaviour
{
    [Header("Rotate")]
    public RectTransform humanPivot;          // HumanPivot（UI）
    public float speedDegPerSec = 360f;

    [Header("Targets (5 points under HumanPivot)")]
    public List<RectTransform> targetPoints;  // Point1~Point5（UI，都是HumanPivot子物体）

    [Header("Dot Anchor (must be under HumanPivot)")]
    public RectTransform dotAnchor;           // DotAnchor（UI，建议放HumanPivot下）

    [Header("Dot Prefab (UI Image)")]
    public RectTransform dotPrefab;           // BlackDot prefab（UI Image）
    public RectTransform dotParent;           // 一般填 humanPivot（让点跟着转）

    [Header("Hit Settings")]
    public float hitRadius = 25f;             // UI像素距离，先用25~40

    [Header("UI")]
    public TMP_Text resultText;

    [Header("Exit on Success")]
    public MinigameCameraSwitcher switcher;

    bool cleared = false;

    void Start()
    {
        if (resultText) resultText.text = "SPACE: place dot";
    }

    void Update()
    {
        if (cleared) return;
        if (humanPivot == null) return;

        // 1) 旋转人体
        humanPivot.Rotate(0f, 0f, -speedDegPerSec * Time.deltaTime);

        // 2) 空格放点
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlaceDotAndCheck();
        }
    }

    void PlaceDotAndCheck()
    {
        if (dotPrefab == null || dotAnchor == null || humanPivot == null) return;

    // 1) 先生成（随便先生成在Canvas下都行）
        RectTransform dot = Instantiate(dotPrefab, dotAnchor.parent);
        dot.gameObject.SetActive(true);

    // 2) 把点放到准星的世界位置
        dot.position = dotAnchor.position;

    // 3) 关键：把点“焊”到人体上（变成人体子物体），并保持世界位置不变
        dot.SetParent(humanPivot, true);

    // ---- 判定（用世界坐标距离）----
        Vector3 anchorWorld = dotAnchor.position;
        bool hit = false;

        foreach (var t in targetPoints)
        {
            if (t == null) continue;
            float d = Vector3.Distance(anchorWorld, t.position);
            if (d <= hitRadius) { hit = true; break; }
        }

        if (hit)
        {
            cleared = true;
            if (resultText) resultText.text = "Success!";
            if (switcher != null) switcher.ExitMinigameOnSuccess();
        }
        else
        {
            if (resultText) resultText.text = "Miss!";
        }
    }
}