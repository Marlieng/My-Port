using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public GameObject reacherStacker;

    public float waypointSphereRadius;

    private void OnDrawGizmos()
    {
        foreach (Transform item in transform.GetComponentsInChildren<Transform>(true))
        {
            Gizmos.DrawWireSphere(item.position,waypointSphereRadius);
        }
    }
}
