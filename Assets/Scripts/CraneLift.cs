using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameManager;

public class CraneLift : MonoBehaviour
{
    [NonSerialized]
    public bool containerAttached;

    Transform goalSector;
    Transform startingSector;
    Collider2D container;
    Collider2D actualSector;

    [SerializeField]
    private InputActionReference attach;

    private void Start()
    {
        Instance.craneLift = gameObject.GetComponent<CraneLift>();
    }
    private void Update()
    {
        ContainerTransporting();
    }
    /// <summary>
    /// Is responsible for the container transport. Checks wheather the container
    /// can be attached or detached
    /// </summary>
    private void ContainerTransporting()
    {
        container = Physics2D.OverlapPoint(transform.position, Instance.activeContainerLayer);
        if (container!=null)
        {
            if (attach.action.triggered && ShipManager.Instance.shipParked)
            {
                actualSector = Physics2D.OverlapPoint(transform.position, Instance.sectorLayer);

                if (!containerAttached)
                {
                    UpdateSectorAssignment();
                    AttachContainer();
                    UpdateQuestProgressBySector();
                }
                else if (containerAttached)
                {
                    DetachContainer();
                    UpdateQuestProgressBySector();
                }
            }
            //warns Player that the ship has not parked yet, so it cannot attach container.
            else if (attach.action.triggered && !ShipManager.Instance.shipParked
                && container != null)
            {
                Message.Instance.WarningMessage("You cannot attach a container because the ship isn't parked.");
            }
        }
    }
    /// <summary>
    ///Sets the sector in which the crane lift is currently located to goalSector and another to startingSector
    /// </summary>
    public void UpdateSectorAssignment()
    {
        if (actualSector!=null)
        {
            if (actualSector.transform.Equals(Instance.portLoadSector) ||
                actualSector.transform.Equals(Instance.shipUnloadSector))
            {
                goalSector = Instance.shipUnloadSector;
                startingSector = Instance.portLoadSector;
            }
            else if (actualSector.transform.Equals(Instance.portUnloadSector) ||
                actualSector.transform.Equals(Instance.shipLoadSector))
            {
                goalSector = Instance.portUnloadSector;
                startingSector = Instance.shipLoadSector;
            }
        }
    }

    private void AttachContainer()
    {
        container.transform.SetParent(transform);
        container.transform.position = transform.position;

        container.GetComponent<SpriteRenderer>().sortingOrder = 1;

        containerAttached = true;
        CallUpReachStacker();
    }
    private void DetachContainer()
    {
        Collider2D freePlace = Physics2D.OverlapPoint(transform.position, Instance.freeSpaceLayer);
        if (freePlace != null)
        {
            if (CheckPlaceAvailability(Instance.sectorReachStackerToUnload, freePlace.transform) && (actualSector.transform == Instance.portLoadSector) || (actualSector.transform == Instance.shipUnloadSector) ||
                CheckPlaceAvailability(Instance.sectorReachStackerToLoad, freePlace.transform) && (actualSector.transform == Instance.portUnloadSector) || (actualSector.transform == Instance.shipLoadSector))
            {
                if (actualSector != null && freePlace.transform.childCount == 0)
                {
                    if (actualSector.transform.Equals(goalSector) || actualSector.transform.Equals(startingSector))
                    {
                        container.transform.SetParent(freePlace.transform);
                        container.transform.position = freePlace.transform.position;

                        container.GetComponent<SpriteRenderer>().sortingOrder = 0;

                        container = null;
                        containerAttached = false;

                        CallUpReachStacker();
                    }
                }
            }
        }
    }
    /// <summary>
    /// checks whether the place where the Player wants to detach the container
    /// is not in the row into which the reach stacker entered or whether 
    /// the car's target is further away than this place
    /// </summary>
    /// <param name="reachedStacker">Reach stacker of a given sector</param>
    /// <param name="freePlace">The Place where the Player want to detach a container</param>
    /// <returns>return true if the place where the Player wants to detach the container
    /// is in a differerent row than where reach stacker entered or is further
    /// from the destination </returns>
    bool CheckPlaceAvailability(Car reachedStacker, Transform freePlace)
    {
        //the row in which there is a free place where Player want to detach a container  
        Transform currentRow = freePlace.transform.parent;

        return currentRow != reachedStacker.destinationRow ||
            (currentRow == reachedStacker.destinationRow &&
           reachedStacker.destinationIndex < freePlace.transform.GetSiblingIndex()
            && reachedStacker.insideRow) ||
            (currentRow == reachedStacker.destinationRow) &&
            !reachedStacker.insideRow;
    }
    /// <summary>
    /// tells the UpdateQuestProgressByType method to update the quest progress with
    /// the appropriate quest type depending on what sector the crane lift is currently in
    /// </summary>
    private void UpdateQuestProgressBySector()
    {
        if (actualSector!=null)
        {
            if (actualSector.transform.Equals(Instance.shipLoadSector))
            {
                QuestManager.Instance.UpdateQuestProgressByType(QuestType.LoadOnShip, Instance.shipLoadSector);
            }
            else if (actualSector.transform.Equals(Instance.portLoadSector))
            {
                QuestManager.Instance.UpdateQuestProgressByType(QuestType.LoadOnPort, Instance.portLoadSector);
            }
        }
    }
    /// <summary>
    /// Call up the reach stacker to the sector where the crane lift is currently located
    /// </summary>
    public void CallUpReachStacker()
    {
        if (actualSector.transform.Equals(Instance.portLoadSector))
        {
            Instance.sectorReachStackerToUnload.GetComponent<UnloadReachStacker>().FindDestination();
        }
        else if (actualSector.transform.Equals(Instance.portUnloadSector))
        {
            Instance.sectorReachStackerToLoad.GetComponent<LoadingReachStacker>().FindDestination();
        }
    }
}