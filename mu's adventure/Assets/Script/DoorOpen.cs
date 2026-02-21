using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public string interactKey = "e";
    public float interactRange = 1.5f;

    bool opened = false;
    Transform player;
    KeyCode interactKeyCode = KeyCode.E;

    void Start()
    {
        var pgo = GameObject.FindWithTag("Player");
        if (pgo != null) player = pgo.transform;
        if (!string.IsNullOrEmpty(interactKey))
        {
            System.Enum.TryParse(interactKey, true, out interactKeyCode);
        }
    }

    void Update()
    {
        if (opened) return;
        if (player == null) return;

        if (Vector3.Distance(player.position, transform.position) <= interactRange)
        {
            if (Input.GetKeyDown(interactKeyCode))
            {
                OpenDoor();
            }
        }
    }

    void OpenDoor()
    {
        var c2 = GetComponent<Collider2D>();
        if (c2 != null)
        {
            c2.isTrigger = true;
        }

        var c3 = GetComponent<Collider>();
        if (c3 != null)
        {
            c3.isTrigger = true;
        }

        opened = true;
    }
}


