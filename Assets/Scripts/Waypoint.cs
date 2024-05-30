using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Transform row;//The row to which the waypoint leads
    //[NonSerialized]
    public Transform targetContainer;
    //[NonSerialized]
    public Transform targetSpace;

    public int containerIndex=-1;
    public int freeSpaceIndex=-1;

    ///<summary>
    ///Searches the given waypoint's row for an available target
    ///</summary>
    public void FindTargetInRow()
    {
        for (int i = 0; i < row.childCount; i++)
        {
            Transform place = row.GetChild(i);

            //Exuted when the first space in the waypoint row contains a container
            if (place.GetComponentInChildren<Container>() != null)
            {
                /*resets the values in variables so that if the waypoint belogs
                 * to a sector to be loaded, i.e where the target of the reach stacker
                 * is a free place, these values are not saved from previous searches*/
                if (i == 0)
                {
                    targetSpace = null;
                    freeSpaceIndex = -1;
                }
                GameObject container = place.GetComponentInChildren<Container>().gameObject;

                if (container.layer == GameManager.Instance.deactivateContainerLayer)
                {
                    targetContainer = container.transform;
                    containerIndex = place.GetSiblingIndex();
                }
                else
                {
                    targetContainer = null;
                    containerIndex = -1;
                }
                break;
            }
            //Exuted when the first space in the waypoint row is empty
            else
            {
                targetSpace = place;
                freeSpaceIndex = place.GetSiblingIndex();
            }
        }
    }
}
