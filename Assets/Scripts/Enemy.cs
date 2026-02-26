#nullable enable
using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private PathNode? _target;
    [SerializeField] private float _speed;
    [SerializeField] private float _reachRadius;


    public void Update()
    {
        Move();
    }

    private void Move()
    {
        if (_target == null) return;
        
        Vector3 differenceToTarget = _target.gameObject.transform.position - transform.position;
        
        // check if target is reached
        if (differenceToTarget.sqrMagnitude <= _reachRadius * _reachRadius)
        {
            _target = _target.GetNextNode(); // set next node
            Move();
        }
        else
        {
            transform.Translate(differenceToTarget.normalized * _speed * Time.deltaTime);
        }
    }
}
