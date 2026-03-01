using UnityEngine;

public class MinigameCameraSwitcher : MonoBehaviour
{
    [Header("Refs")]
    public CameraFollow cameraFollow;        // 你现有的相机跟随脚本
    public Transform playerTarget;           // 玩家Transform
    public Transform minigameFocusTarget;    // MinigameFocus
    public GameObject rouletteRoot;          // RouletteRoot（开关小游戏）

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    [Header("Optional: Lock Player Control")]
    public MonoBehaviour[] disableWhileMinigame; // 把玩家移动脚本拖进来

    private bool inRange = false;
    private bool inMinigame = false;

    [Header("Input")]
    public KeyCode exitKey = KeyCode.Escape;

    void Start()
    {
        if (rouletteRoot != null)
            rouletteRoot.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) inRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag)) inRange = false;
    }

    void Update()
    {
        // 1) 在小游戏里，ESC 退出（必须放在最前面，且不能被 return 挡住）
        if (inMinigame)
        {
            if (Input.GetKeyDown(exitKey))
            {
                ExitMinigame();
            }
            return; // 小游戏状态不需要再处理 E 进入
        }

    // 2) 不在小游戏时，必须靠近才能按 E 进入
        if (!inRange) return;

        if (Input.GetKeyDown(interactKey))
        {
            EnterMinigame();
        }
    }

    public void EnterMinigame()
    {
        inMinigame = true;

        if (rouletteRoot != null) rouletteRoot.SetActive(true);

        // 切相机跟随目标到小游戏中心
        if (cameraFollow != null && minigameFocusTarget != null)
            cameraFollow.SetTarget(minigameFocusTarget);

        // 禁用玩家控制
        if (disableWhileMinigame != null)
        {
            foreach (var mb in disableWhileMinigame)
                if (mb != null) mb.enabled = false;
        }
    }

    public void ExitMinigame()
    {
        inMinigame = false;

        if (rouletteRoot != null) rouletteRoot.SetActive(false);

        // 切回跟随玩家
        if (cameraFollow != null && playerTarget != null)
            cameraFollow.SetTarget(playerTarget);

        // 恢复玩家控制
        if (disableWhileMinigame != null)
        {
            foreach (var mb in disableWhileMinigame)
                if (mb != null) mb.enabled = true;
        }
    }

    // 给轮盘脚本调用：成功时退出
    public void ExitMinigameOnSuccess()
    {
        ExitMinigame();
        // 这里你也可以加：播放音效、开门、给奖励等
    }
}