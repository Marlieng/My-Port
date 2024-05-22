using System.Collections;
using UnityEngine;

public class UnloadReachStacker : Car
{
    private bool containerAttached;
    protected override void Update()
    {
        base.Update();
        if (currentWaypointIndex <= -1)
        {
            currentWaypointIndex = 0;
            if (containerAttached == true)
            {
                containerAttached = false;
            }
        }
    }
    public override void FindDestination()
    {
        if (!insideRow && attachmentPosition.childCount == 0 && !cooldown)
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
                    if (waypoint != null && waypoint.targetContainer == previousGoal)
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
                if (waypoint != null && waypoint.targetContainer != null)
                {
                    returnToStartWaypoint = false;
                    destinationRow = waypoint.row;
                    destinationIndex = waypoint.containerIndex;
                    DetermineRoute(waypoint.targetContainer, waypointObject.GetSiblingIndex());
                    previousGoal = waypoint.targetContainer;
                    return;
                }
            }
            /*If the destination is not found, it calculates the route to the beginning
             * waypoint by calling the DetermineRoute  method*/
            returnToStartWaypoint = true;
            destinationRow = null;
            destinationIndex = -1;
            DetermineRoute(0);
            previousGoal = null;
        }
    }
    public override void ReachedDestination()
    {
        currentWaypointIndex--;
        currentRoute[currentWaypointIndex].SetParent(attachmentPosition);
        attachmentPosition.GetChild(0).transform.localPosition = Vector3.zero;
        returnToStartWaypoint = true;
        goalSet = false;

        currentWaypointIndex = currentRoute[currentWaypointIndex - 1].GetSiblingIndex();
        currentRoute.Clear();
        for (int i = 0; currentWaypointIndex >= i; i++)
        {
            currentRoute.Add(waypointsParent.GetChild(i));
        }
    }
    public override void ReachedStart()
    {
        if (attachmentPosition.childCount > 0)
        {
            Destroy(attachmentPosition.GetChild(0).gameObject);
            StartCoroutine(CoolDown());
        }
    }
}
