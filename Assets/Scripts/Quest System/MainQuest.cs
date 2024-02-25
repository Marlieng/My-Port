using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static QuestManager;

public class MainQuest : MonoBehaviour
{
    public List<Quest> questList = new List<Quest>();


    private void Start()
    {
        transform.GetComponentInChildren<Button>().onClick.AddListener(CheckComplete);
    }
    public void CheckComplete()
    {
        if (questList.All(x=>x.questStructure.isCompleted))
        {
            Debug.Log("Ship quest is completly completed!");
        }
        else
        {
            Debug.Log("Oh no! Please try again");
            foreach (var quest in questList)
            {
                quest.CheckComplete();
            }
        }
    }
}
