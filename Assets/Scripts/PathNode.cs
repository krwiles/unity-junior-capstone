#nullable enable
using UnityEngine;

public class PathNode : MonoBehaviour
{
    [SerializeField, Tooltip("Leave empty for the final node in the path.")]
    private PathNode? _nextNode;

    [SerializeField]
    private float _distanceToGoal = 0f;
    public float DistanceToGoal
    {
        get { return _distanceToGoal; }
    }

    public float CalculateDistanceToGoal()
    {
        if (_nextNode == null)
        {
            _distanceToGoal = 0f;
            return _distanceToGoal;
        }
        else
        {
            float nextDistance = _nextNode.CalculateDistanceToGoal();
            float distanceToNext = (_nextNode.transform.position - transform.position).magnitude;
            _distanceToGoal = nextDistance + distanceToNext;
            return _distanceToGoal;
        }
    }

    public PathNode? GetNextNode()
    {
        return _nextNode;
    }
}
