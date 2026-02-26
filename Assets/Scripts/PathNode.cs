#nullable enable
using UnityEngine;

public class PathNode : MonoBehaviour
{
    [SerializeField, Tooltip("Leave empty for the final node in the path.")]
    private PathNode? _nextNode;

    public PathNode? GetNextNode()
    {
        return _nextNode;
    }
}
