using System.Collections;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public string interactKey = "e";
    public float interactRange = 1.5f;
    public float animDuration = 0.35f;
    public bool isLocked = false;

    bool opened = false;
    Transform player;
    KeyCode interactKeyCode = KeyCode.E;

    void Start()
    {
        var pgo = GameObject.FindWithTag("Player");
        if (pgo != null) player = pgo.transform;
        if (!string.IsNullOrEmpty(interactKey))
            System.Enum.TryParse(interactKey, true, out interactKeyCode);
    }

    void Update()
    {
        if (opened) return;
        if (player == null) return;
        if (!isLocked && Vector3.Distance(player.position, transform.position) <= interactRange)
        {
            if (Input.GetKeyDown(interactKeyCode))
                StartCoroutine(OpenAnimation());
        }
    }

    IEnumerator OpenAnimation()
    {
        opened = true;

        // Disable collider immediately so player can pass through while animating
        var c2 = GetComponentInChildren<Collider2D>();
        if (c2 != null) c2.enabled = false;

        Vector3 startScale = transform.localScale;
        Vector3 endScale   = new Vector3(0f, startScale.y, startScale.z);
        float elapsed = 0f;

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / animDuration);
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
