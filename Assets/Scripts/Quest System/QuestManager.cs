using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    public Transform allQuestsList;
    public GameObject mainQuestPrefab;
    public GameObject questPrefab;
    public GameObject finishQuestPrefab;

    List<QuestStructure> allShipLoadQuests = new List<QuestStructure>();
    List<QuestStructure> allPortLoadQuests = new List<QuestStructure>();

    List<Quest> activeQuests = new List<Quest>();

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        ExitingLoadQuests();
        ExitingUnloadQuests();
        RandomizeQuest();
    }

    private void ExitingUnloadQuests()
    {
        QuestConstruction(QuestType.LoadOntoPort, "Load Port", "Load 2 orange, 2 red and 2 yellow containers" +
            " on the port", new Dictionary<ContainerType, int>{
                {ContainerType.RedContainer,2},
                {ContainerType.OrangeContainer, 2},
                {ContainerType.YellowContainer, 2},
            });
    }

    public void ExitingLoadQuests()
    {
        QuestConstruction(QuestType.LoadOntoShip,"Rainbow", "Load 1 red, 1 orange, 1 yellow, 1 green and" +
            " 1 blue container on board the ship", new Dictionary<ContainerType, int>{
                {ContainerType.RedContainer,1},
                {ContainerType.OrangeContainer,1 },
                {ContainerType.YellowContainer,1 },
                {ContainerType.GreenContainer,1 },
                {ContainerType.BlueContainer,1 }
            });
    }
    public void QuestConstruction(QuestType type, string name, string description, Dictionary<ContainerType, int> questRequirements)
    {
        QuestStructure quest = new QuestStructure();
        quest.Type = type;
        quest.Name = name;
        quest.Description = description;
        quest.QuestRequirements = questRequirements;

        foreach (var item in quest.QuestRequirements.Keys)
        {
            quest.PlayerProgress.Add(item, 0);
        }

        switch (type)
        {
            case QuestType.LoadOntoShip:
                allShipLoadQuests.Add(quest);
                break;
            case QuestType.LoadOntoPort:
                allPortLoadQuests.Add(quest);
                break;
        }
    }

    public void RandomizeQuest()
    {
        int randomLoadQuest = UnityEngine.Random.Range(0, allShipLoadQuests.Count);
        int randomUnloadQuest = UnityEngine.Random.Range(0, allPortLoadQuests.Count);

        Vector3 questPosition = new Vector3(10,-10,0);
        Quaternion questRotation = new Quaternion(0,0,0,0);

        GameObject mainQuest = Instantiate(mainQuestPrefab, allQuestsList.transform, allQuestsList);
        mainQuest.transform.localScale = new Vector3(1,1,1);

        GameObject shipLoadQuest = Instantiate(questPrefab, mainQuest.transform);
        shipLoadQuest.GetComponent<Quest>().questStructure = allShipLoadQuests[randomLoadQuest];
        activeQuests.Add(shipLoadQuest.GetComponent<Quest>());
        shipLoadQuest.transform.localScale = new Vector3(1, 1, 1);

        GameObject portLoadQuest = Instantiate(questPrefab, mainQuest.transform);
        portLoadQuest.GetComponent<Quest>().questStructure = allPortLoadQuests[randomUnloadQuest];
        activeQuests.Add(portLoadQuest.GetComponent<Quest>());
        portLoadQuest.transform.localScale = new Vector3(1, 1, 1);

        mainQuest.GetComponent<MainQuest>().questList.Add(shipLoadQuest.GetComponent<Quest>());
        mainQuest.GetComponent<MainQuest>().questList.Add(portLoadQuest.GetComponent<Quest>());

        GameObject finishQuest = Instantiate(finishQuestPrefab, mainQuest.transform);
        finishQuest.transform.localScale = new Vector3(1, 1, 1);
    }

    public void QuestProgressUpdate(QuestType type, Transform place)
    {
        foreach (Quest item in activeQuests)
        {
            if (item.questStructure.Type==type)
            {
                item.ProgressUpdate(place);

                CheckIfQuestCompleted(item.questStructure);
            }
        }
    }

    private void CheckIfQuestCompleted(QuestStructure questStructure)
    {
        if (questStructure.isCompleted)
        {
            Debug.Log("Quest Is Completed, Congratulations!");
        }
    }
}