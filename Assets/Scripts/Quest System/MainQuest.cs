using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static QuestManager;

public class MainQuest : MonoBehaviour
{
    [NonSerialized]
    public int ID;
    [NonSerialized]
    public List<Quest> questList = new List<Quest>();
    CraneLift craneLiftScript;

    private void Start()
    {
        transform.GetComponentInChildren<Button>().onClick.AddListener(CheckQuestsComplete);
        transform.GetComponentInChildren<Button>().onClick.AddListener(ShipManager.Instance.ShipDeparture);

        craneLiftScript = GameManager.Instance.craneLift.GetComponent<CraneLift>();
    }
    public void CheckQuestsComplete()
    {
        if (ShipManager.Instance.shipParked && !craneLiftScript.containerAttached)
        {
            foreach (Quest quest in questList)
            {
                quest.CheckQuestComplete();
            }

            if (questList.All(x => x.questStructure.isCompleted))
            {
                Debug.Log("Ship quest is completly completed!");
            }
            else
            {
                Debug.Log("Oh no! Please try again");
            }
            
            Instance.FinishMainQuest(GetInstanceID());
        }
    }
}
