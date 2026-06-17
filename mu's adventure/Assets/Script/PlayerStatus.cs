using UnityEngine;

// Attach to the Player GameObject. Other scripts read and write PlayerStatus.Instance.current.
public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus Instance { get; private set; }

    public enum Status
    {
        BareHand  = 0,   // no clothes, no body
        HasClothes = 1,  // wearing npc clothes
        HasBody    = 2   // carrying the dead body
    }

    public Status current = Status.BareHand;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
