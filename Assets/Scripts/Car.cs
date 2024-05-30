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

    private protected Transform attachmentPosition;//Container attachment position
    private protected Transform current;
    private protected Transform previousGoal;
    private protected List<Transform> currentRoute = new List<Transform>();
    private protected int currentWaypointIndex;
    private protected int previousWaypointIndex = -1;

    private protected bool returnToStartWaypoint;
    private protected bool cooldown;
    private protected bool goalSet;

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
            current.position = Vector2.MoveTowards(current.position, currentRoute[currentWaypointIndex].position, speed * Time.deltaTime);

            if (Vector2.Distance(current.position, currentRoute[currentWaypointIndex].position) < 0.05)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= currentRoute.Count - 1)
                {
                    if (currentWaypointIndex == currentRoute.Count - 1)
                    {
                        Swap(transform, attachmentPosition);
                        /*Sets the current position to attachmentPosition so that when
                       * calculating distance between target place, the container
                       * is positioned int its center*/
                        current = attachmentPosition;
                    }
                    insideRow = true;
                    if (currentWaypointIndex == currentRoute.Count)
                    {
                        Swap(attachmentPosition, transform);
                        /*Sets the current position to the reached stacker position so
                        * that the distance between the next waypoint is calculated from it*/
                        current = transform;

                        ReachedDestination();
                    }
                }
            }
        }
    }
    /// <summary>
    /// Swaps the places of parent and its child, i.e child becomes the parent of its parent and parent becomes its child
    /// </summary>
    /// <param name="targetChild">Object which is to become a child of its child</param>
    /// <param name="targetParent">Object which is to become a parent of its parent</param>
    private void Swap(Transform targetChild, Transform targetParent)
    {
        //Sets the current parent and its child at the same hierarchy level
        targetParent.SetParent(targetChild.parent);
        //Sets parent as the child of its child
        targetChild.SetParent(targetParent);
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
            currentRoute.Clear();
            //determines a new route
            currentRoute.Add(waypointsParent.GetChild(0));
            currentRoute.Add(waypointsParent.GetChild(1));

            //decides whether the reach stacker should go to the start waypoint
            if (currentWaypointIndex <= 1)
            {
                previousWaypointIndex = currentWaypointIndex;
                currentWaypointIndex = 0;
            }
            //or go to the waypoint that is after the start waypoint
            else if (currentWaypointIndex > 1)
            {
                previousWaypointIndex = currentWaypointIndex;
                currentWaypointIndex = 1;
            }
            goalSet = false;
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
            currentRoute.Clear();
            //determines a new route
            currentRoute.Add(waypointsParent.GetChild(0));
            currentRoute.Add(waypointsParent.GetChild(1));
            currentRoute.Add(waypointsParent.GetChild(waypointIndex));
            currentRoute.Add(goal);

            //decides whether the reach stacker should go to its destination
            if (currentWaypointIndex>1 || (currentWaypointIndex == 1 && previousWaypointIndex==2))
            {
                previousWaypointIndex = currentWaypointIndex;
                currentWaypointIndex = 2;
            }
            //or go to the waypoint that is after the start waypoint
            else if (currentWaypointIndex <= 1)
            {
                previousWaypointIndex = currentWaypointIndex;
                currentWaypointIndex = 1;
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
    public virtual void ReachedDestination()
    {
        returnToStartWaypoint = true;
        goalSet = false;
        currentWaypointIndex = currentRoute[currentWaypointIndex - 1].GetSiblingIndex();
        currentRoute.Clear();
        for (int i = 0; currentWaypointIndex >= i; i++)
        {
            currentRoute.Add(waypointsParent.GetChild(i));
        }
    }
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
