using Unity.VisualScripting;
using UnityEngine;

public class PlacementProbe : MonoBehaviour
{
    [SerializeField] private LayerMask blockingLayers;
    [SerializeField] private Material _validMaterial;
    [SerializeField] private Material _invalidMaterial;

    private int _blockingContacts;

    public bool IsValid => _blockingContacts == 0;

    private void OnTriggerEnter(Collider other)
    {
        if (IsInMask(other.gameObject.layer, blockingLayers))
        {
            _blockingContacts++;
            OverrideAllMaterials(_invalidMaterial);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsInMask(other.gameObject.layer, blockingLayers))
        {
            _blockingContacts = Mathf.Max(0, _blockingContacts - 1);
        }
        
        if (_blockingContacts == 0)
        {
            OverrideAllMaterials(_validMaterial);
        }
    }

    private static bool IsInMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    public void OverrideAllMaterials(Material replacement)
    {
        if (replacement == null) return;

        var renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            var mats = r.materials; // instance materials (safe for runtime override)
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = replacement;
            }
            r.materials = mats;
        }
    }
}
