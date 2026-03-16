using UnityEngine;

public class PlacementProbe : MonoBehaviour
{
    [SerializeField] private LayerMask blockingLayers;
    private int _blockingContacts;

    public bool IsValid => _blockingContacts == 0;

    private void OnTriggerEnter(Collider other)
    {
        if (IsInMask(other.gameObject.layer, blockingLayers))
            _blockingContacts++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsInMask(other.gameObject.layer, blockingLayers))
            _blockingContacts = Mathf.Max(0, _blockingContacts - 1);
    }

    private static bool IsInMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}
