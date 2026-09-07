using System.Collections.Generic;
using UnityEngine;

// Shared by loose documents, blackboards and other readable objects.
[RequireComponent(typeof(Collider2D))]
public class FileInteractable : MonoBehaviour
{
    public FileDocument document;
    public string interactionLabel = "E - Read document";

    private static readonly List<FileInteractable> activeReaders = new List<FileInteractable>();
    private readonly HashSet<Collider2D> playersInRange = new HashSet<Collider2D>();

    private void OnEnable() => activeReaders.Add(this);

    private void OnDisable()
    {
        activeReaders.Remove(this);
        playersInRange.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") ||
            (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player")))
            playersInRange.Add(other);
    }

    private void OnTriggerExit2D(Collider2D other) => playersInRange.Remove(other);

    // Choose once per frame, so overlapping readers cannot consume the same E press.
    public static FileInteractable GetNearestAvailable()
    {
        FileInteractable nearest = null;
        float nearestDistance = float.PositiveInfinity;
        foreach (var reader in activeReaders)
        {
            if (reader == null || !reader.isActiveAndEnabled ||
                reader.document == null || !reader.document.HasContent) continue;

            reader.playersInRange.RemoveWhere(player =>
                player == null || !player.enabled || !player.gameObject.activeInHierarchy);
            foreach (var player in reader.playersInRange)
            {
                float distance = (player.transform.position - reader.transform.position).sqrMagnitude;
                if (distance < nearestDistance ||
                    (distance == nearestDistance && nearest != null &&
                     reader.GetInstanceID() < nearest.GetInstanceID()))
                {
                    nearest = reader;
                    nearestDistance = distance;
                }
            }
        }
        return nearest;
    }
}
