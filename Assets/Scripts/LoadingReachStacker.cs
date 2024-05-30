using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class LoadingReachStacker : Car
{  
    public GameObject deactivatedContainerPrefab;

    public override void FindDestination()
    {
        if (!insideRow && attachmentPosition.childCount == 1 && !cooldown)
        {
            Transform[] waypointObjects = waypointsParent.GetComponentsInChildren<Transform>();
            //at each waypoint it calls the FindTargetInRow method to find an available target
            foreach (Transform waypointObject in waypointObjects)
            {
                Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
                if (waypoint != null)
                {
                    waypoint.FindTargetInRow();
                }
            }

            /*Stops searching for a new target if the new target is the previous target.
          * This is done in order not to change an already designated route
          * to the same destination.*/
            if (currentRoute.Count > 0 && goalSet)
            {
                foreach (Transform waypointObject in waypointObjects)
                {
                    Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
                    if (waypoint != null && waypoint.targetSpace == previousGoal)
                    {
                        return;
                    }
                }
            }

            /*One by one, it looks for the destination. If it finds a destination,
             * it calls the DetermineRoute method to determinate route to the destination.*/
            foreach (Transform waypointObject in waypointObjects)
            {
                Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
                if (waypoint != null && waypoint.targetSpace != null)
                {
                    returnToStartWaypoint = false;
                    destinationRow = waypoint.row;
                    destinationIndex = waypoint.freeSpaceIndex;
                    DetermineRoute(waypoint.targetSpace, waypointObject.GetSiblingIndex());
                    previousGoal = waypoint.targetSpace;
                    return;
                }
            }
            /*If the destination is not found, it calculates the route to the beginning
         * waypoint by calling the DetermineRoute  method*/
            if (!returnToStartWaypoint)
            {
                returnToStartWaypoint = true;
                destinationRow = null;
                destinationIndex = -1;
                DetermineRoute(0);
                previousGoal = null;
            }
        }
    }
    public override void ReachedDestination()
    {
        Container containerObject = transform.GetComponentInChildren<Container>();
        currentWaypointIndex--;
        containerObject.SetContainerActive(true);
        containerObject.transform.SetParent(currentRoute[currentWaypointIndex]);//Set new parent for container of this Reached Stacker
        currentRoute[currentWaypointIndex].GetChild(0).transform.localPosition = Vector3.zero;
        
        base.ReachedDestination();
    }
    public override void ReachedStart()
    {
        previousWaypointIndex = -1;
        if (attachmentPosition.childCount < 1)
        {
            GameObject container = Instantiate(deactivatedContainerPrefab, attachmentPosition);
            container.transform.localPosition = Vector3.zero;
            StartCoroutine(CoolDown());
        }
    }
}
