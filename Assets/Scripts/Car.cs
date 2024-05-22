using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public abstract class Car : MonoBehaviour
{
    public Transform waypointsParent;
    public float speed;

    [NonSerialized]
    public Transform destinationRow;//Row where is located the target waypoint
    [NonSerialized]
    public int destinationIndex = 0;
    [NonSerialized]
    public bool insideRow;

    protected double minDistanceWaypoint = 0.05f;//Min car distance from the target waypoint
    CarStates state; //State of the car, i.e wheather the car returns to start, drives to the goal or rests

    private protected Transform attachmentPosition;//Container attachment position
    private protected Transform current;
    private protected Transform previousGoal;
    private protected List<Transform> currentRoute = new List<Transform>();
    private protected int currentWaypointIndex;
    private protected int previousWaypointIndex = -1;
    private protected bool returnToStartWaypoint;
    private protected bool cooldown;
    private protected bool goalSet;

    enum CarStates
    {
        ReturnToBeginning,
        GoingToGoalFromBeginning,
        GoingToGoalFromCurrentWaypoint,
        Rest
    }
    private void Start()
    {
        current = transform;
        attachmentPosition = transform.GetChild(0);
        FindDestination();
    }
    protected virtual void Update()
    {
        if (currentRoute.Count >= 1)
        {
            if (!returnToStartWaypoint)
            {
                DriveToDestination();
            }
            else
            {
                ReturnToBeginning();
            }
        }
    }

    ///<summary>
    ///Follows the designated path in currentRoute to the goal
    ///</summary>
    public void DriveToDestination()
    {
        if (currentWaypointIndex < currentRoute.Count)
        {
            transform.position = Vector2.MoveTowards(transform.position, currentRoute[currentWaypointIndex].position, speed * Time.deltaTime);

            if (Vector2.Distance(current.position, currentRoute[currentWaypointIndex].position) < 0.05)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex == currentRoute.Count - 1)
                {
                    /*Sets the current position to attachmentPosition so that when
                   * calculating distance between target place, the container
                   * is positioned int its center*/
                    current = attachmentPosition;
                }
                else
                {
                    /*Sets the current position to the reached stacker position so
                    * that the distance between the next waypoint is calculated from it*/
                    current = transform;
                }
                if (currentWaypointIndex >= currentRoute.Count - 1)
                {
                    insideRow = true;
                    if (currentWaypointIndex == currentRoute.Count)
                    {
                        ReachedDestination();
                    }
                }
            }
        }
    }
    ///<summary>
    ///Follows the designated path in currentRoute to the start waypoint
    ///</summary>
    public void ReturnToBeginning()
    {
        if (currentWaypointIndex > -1)
        {
            transform.position = Vector2.MoveTowards(transform.position, currentRoute[currentWaypointIndex].position, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, currentRoute[currentWaypointIndex].position) < minDistanceWaypoint)
            {
                currentWaypointIndex--;

                if (currentWaypointIndex <= currentRoute.Count - 1)
                {
                    insideRow = false;
                }
                if (currentWaypointIndex == -1)
                {
                    currentRoute.Clear();
                    returnToStartWaypoint = false;
                    destinationRow = null;
                    destinationIndex = -1;
                    currentWaypointIndex++;
                    previousWaypointIndex = 0;

                    ReachedStart();
                }
            }
        }
    }
    ///<summary>
    ///Determines path to the start waypoint
    ///</summary>
    ///<param name="waypointIndex">The waypoint Index which leads to the row where the goal is located</param>
    public void DetermineRoute(int waypointIndex)
    {
        if (returnToStartWaypoint)
        {
            if (waypointIndex <= currentWaypointIndex && currentRoute.Count > 0)
            {
                //Sets to the currentWaypointIndex the sibling index of the waypoint which the reached stacker goes
                currentWaypointIndex = currentRoute[currentWaypointIndex].GetSiblingIndex();

                currentRoute.Clear();
                //sets a new route
                for (int i = waypointIndex; i <= waypointsParent.GetChild(currentWaypointIndex).GetSiblingIndex() + 1; i++)
                {
                    if (waypointsParent.childCount > i)
                    {
                        currentRoute.Add(waypointsParent.GetChild(i));
                    }
                }
                /*determines wheather currentWaypointIndex should be reduced or not.
                 * This is needed so that car turns immediately when a new destination is set*/
                if ((currentWaypointIndex >= previousWaypointIndex && state == CarStates.GoingToGoalFromBeginning) || (currentWaypointIndex > previousWaypointIndex))
                {
                    previousWaypointIndex = currentWaypointIndex;
                    currentWaypointIndex--;
                }
                else
                {
                    previousWaypointIndex = currentWaypointIndex;
                }
                goalSet = false;
                state = CarStates.ReturnToBeginning;
            }
        }
    }
    ///<summary>
    ///Determines path to the goal
    ///</summary>
    ///<param name="goal">The goal to which the reached stacker should go</param>
    ///<param name="waypointIndex">The waypoint Index which leads to the row where the goal is located</param>
    public void DetermineRoute(Transform goal, int waypointIndex)
    {
        if (!returnToStartWaypoint)
        {
            /*Sets to the currentWaypointIndex the sibling index of the waypoint which the 
             *reached stacker goes if currentRoute is greather than 0*/
            if (currentRoute.Count > 0)
            {
                currentWaypointIndex = currentRoute[currentWaypointIndex].GetSiblingIndex();
            }

            //excuted if the reached stacker is already goes to a waypoint which leads to the destination
            if (currentWaypointIndex == waypointIndex)
            {
                currentRoute.Clear();
                currentRoute.Add(waypointsParent.GetChild(waypointIndex));
                currentRoute.Add(goal);
                previousWaypointIndex = currentWaypointIndex;
                currentWaypointIndex = 0;
            }
            /*Excuted if the reached stacker goes directly to the destination, e.g goes
             * from the start waypoint straight to the destination */
            else if (currentWaypointIndex < waypointIndex)
            {
                currentRoute.Clear();
                /*Calculates the route to the destination, starting from start waypoint
                 * and ending with the waypoint that containes row with the destination*/
                for (int i = 0; i < waypointIndex + 1; i++)
                {
                    currentRoute.Add(waypointsParent.GetChild(i));
                }
                //At the end of the route adds a destination
                currentRoute.Add(goal);

                /*Determines wheather currentWaypointIndex should be incremented or not.
                 * This is needed so that car turns immediately when a new destination is set*/
                if (((currentWaypointIndex <= previousWaypointIndex && state == CarStates.ReturnToBeginning) ||
                    (currentWaypointIndex < previousWaypointIndex)) && previousWaypointIndex != -1)
                {
                    previousWaypointIndex = currentWaypointIndex;
                    currentWaypointIndex++;
                }
                else
                {
                    previousWaypointIndex = currentWaypointIndex;
                }
                state = CarStates.GoingToGoalFromBeginning;
            }
            /*Excuted if the reached stacker goes to the direction by returning to the start waypoint,
             * e.g goes from final waypoint and arrives at the destination*/
            else if (currentWaypointIndex > waypointIndex)
            {
                currentRoute.Clear();
                /*Calculates the new route to the destination, starting from the current waypoint
                 * and ending with the waypoint that containes row with the destination*/
                for (int i = currentWaypointIndex; i >= waypointIndex; i--)
                {
                    currentRoute.Add(waypointsParent.GetChild(i));
                }
                //At the end of the route adds a destination
                currentRoute.Add(goal);

                /*Determines whether currentWaypointIndex should be set to 1 or 0.
                 This is needed so that car turns immediately when a new destination is set*/
                if (currentWaypointIndex > previousWaypointIndex)
                {
                    previousWaypointIndex = currentWaypointIndex;
                    currentWaypointIndex = 1;
                }
                else
                {
                    previousWaypointIndex = currentWaypointIndex;
                    currentWaypointIndex = 0;
                }
                state = CarStates.GoingToGoalFromCurrentWaypoint;
            }
            goalSet = true;
        }
    }
    ///<summary>
    ///Searches all rows for an avaible target
    ///</summary>
    public abstract void FindDestination();
    /// <summary>
    /// call when reach stacker has reached the target
    /// </summary>
    public abstract void ReachedDestination();
    /// <summary>
    /// call when reach stacker has reached the start waypoint
    /// </summary>
    public abstract void ReachedStart();
    private protected IEnumerator CoolDown()
    {
        cooldown = true;
        yield return new WaitForSeconds(2.5f);
        cooldown = false;
        FindDestination();
    }
}
