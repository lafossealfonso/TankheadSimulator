using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

public class LegAimGrounding : MonoBehaviour
{
    public LayerMask legsWalkOnLayer;
    [SerializeField] private Transform raycastOrigin;
    void Start()
    {
        raycastOrigin = transform.parent.gameObject.transform;
    }

    
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.position, -transform.up, out hit, Mathf.Infinity, legsWalkOnLayer))
        {
            transform.position = hit.point;
        }
    }
}
