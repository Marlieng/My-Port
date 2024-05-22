using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ShipManager : MonoBehaviour
{
    private static ShipManager instance;
    public static ShipManager Instance { get { return instance; } }

    public Transform spawner;
    public GameObject shipPrefab;

    [NonSerialized]
    public bool shipParked;
    GameObject actualShip;
    Animator actualShipAnimation;
    bool shipSpawned;
    CraneLift craneLift;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Update()
    {
        craneLift = GameManager.Instance.craneLift;
        if (!shipSpawned)
        {
            shipSpawned = true;
            Invoke("SpawnShip", 1.5f);
        }
        else
        {
            if (actualShipAnimation != null)
            {
                if (actualShipAnimation.GetCurrentAnimatorStateInfo(0).IsName("Ship Parked"))
                {
                    shipParked = true;
                    QuestManager.Instance.activeMainQuest.gameObject.SetActive(true);
                }
            }
        }
    }
    public void SpawnShip()
    {
        actualShip = Instantiate(shipPrefab, spawner.position, spawner.rotation);
        actualShipAnimation = actualShip.GetComponent<Animator>();
        QuestManager.Instance.RandomizeQuest();
        QuestManager.Instance.activeMainQuest.gameObject.SetActive(false);
        GameManager.Instance.shipLoadSector = actualShip.transform.Find("Sector To Load");
        GameManager.Instance.shipUnloadSector = actualShip.transform.Find("Sector To Unload");
        actualShip.GetComponent<Ship>().GenerateContainers();
        craneLift.UpdateSectorAssignment();
    }

    public void ShipDeparture()
    {
        if (shipParked && !craneLift.containerAttached)
        {
            ContainerManager.Instance.DeactivateContainers();
            actualShipAnimation.Play("Ship Departure");
            StartCoroutine(ShipDepartureCoroutine());
        }
        else
        {
            Message.Instance.WarningMessage("You cannot check in a ship while you are holding a container.");
        }
    }

    IEnumerator ShipDepartureCoroutine()
    {
        yield return new WaitForEndOfFrame();
        shipParked = false;

        yield return new WaitForSeconds(actualShipAnimation.GetCurrentAnimatorStateInfo(0).length);

        Destroy(actualShip);
        shipSpawned = false;
    }
}
