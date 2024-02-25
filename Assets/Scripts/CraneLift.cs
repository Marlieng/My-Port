using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;
using static GameManager;

public class CraneLifr : MonoBehaviour
{
    [Header("Crance lift moving")]
    Rigidbody2D liftRB;

    public Transform minRange;
    public Transform maxRange;

    public float speedLift = 1000;

    float vertical;

    [Header("Containers Transporting")]
    public Transform portLoadSector;
    public Transform portUnloadSector;
    public Transform shipLoadSector;
    public Transform shipUnloadSector;

    public LayerMask containerLayer;
    public LayerMask sectorLayer;
    public LayerMask freeSpaceLayer;

    Transform goalPlace;
    Transform startingPlace;

    Collider2D container;

    Collider2D actualPlace;
    Collider2D freeSpace;

    bool attachedContainer;

    [SerializeField]
    private InputActionReference movement;
    [SerializeField]
    private InputActionReference attach;

    private void Start()
    {
        liftRB = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        vertical = movement.action.ReadValue<Vector2>().y;
        ContainerTransporting();
        LiftMove();
    }
    public void ContainerTransporting()
    {
        if (attach.action.triggered)
        {
            container = Physics2D.OverlapPoint(transform.position, containerLayer);
            actualPlace = Physics2D.OverlapPoint(transform.position, sectorLayer);

            if (!attachedContainer)
            {
                SetTargerLocation();
                AttachContainer();
                QuestProgressUpdate();
            }
            else if (attachedContainer)
            {
                DetachContainer();
                QuestProgressUpdate();
            }
        }
    }

    private void SetTargerLocation()
    {
        if (!attachedContainer&&actualPlace!=null)
        {
            if (actualPlace.transform.Equals(portLoadSector) ||
                actualPlace.transform.Equals(shipUnloadSector))//Miejsce do za³adowania na port
            {
                goalPlace = shipUnloadSector;
                startingPlace = portLoadSector;
            }
            else if (actualPlace.transform.Equals(portUnloadSector) ||
                actualPlace.transform.Equals(shipLoadSector))//Miejsce do roz³adowania na port
            {
                goalPlace = portUnloadSector;
                startingPlace = shipLoadSector;
            }
        }
    }

    private void AttachContainer()
    {
        if (container != null)
        {
            container.transform.SetParent(transform);
            container.transform.position = transform.position;

            container.GetComponent<SpriteRenderer>().sortingOrder = 1;

            attachedContainer = true;

        }
    }
    private void DetachContainer()
    {
        freeSpace = Physics2D.OverlapPoint(transform.position, freeSpaceLayer);
        if (actualPlace!=null&&freeSpace!=null&&freeSpace.transform.childCount==0)
        {
            if (actualPlace.transform.Equals(goalPlace) || actualPlace.transform.Equals(startingPlace))
            {

                container.transform.SetParent(freeSpace.transform);
                container.transform.position = freeSpace.transform.position;

                container.GetComponent<SpriteRenderer>().sortingOrder = 0;

                container = null;
                attachedContainer = false;
            }
        }
    }

    private void QuestProgressUpdate()
    {
        if (actualPlace.transform.Equals(shipUnloadSector))
        {
            QuestManager.instance.QuestProgressUpdate(QuestType.LoadOntoShip, shipUnloadSector);
        }
        else if (actualPlace.transform.Equals(portUnloadSector))
        {
            QuestManager.instance.QuestProgressUpdate(QuestType.LoadOntoPort, portUnloadSector);
        }
    }

    private void LiftMove()
    {
        float Range = Mathf.Clamp(transform.position.y, minRange.position.y, maxRange.position.y);
        if (Range!=transform.position.y)
        {
            liftRB.velocity = Vector2.zero;
        }
        else
        {
            liftRB.velocity = new Vector2(liftRB.velocity.x, vertical * speedLift);
        }
        transform.position = new Vector2(transform.position.x, Range);
    }
}