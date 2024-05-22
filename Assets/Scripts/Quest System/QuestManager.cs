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
    public static QuestManager Instance { get { return instance; } }
    public Transform allQuestsList;
    public GameObject mainQuestPrefab;
    public GameObject questPrefab;
    public GameObject finishQuestPrefab;
    /*I used a list here because I foresee the possibility
   * of having more active main quests in the future*/
    [NonSerialized]
    public MainQuest activeMainQuest;
    //Stores all quests that the player can receive during the game
    List<QuestStructure> allExistQuests = new List<QuestStructure>();
    private static QuestManager instance;

    private void Awake()
    {
        if (instance!=null && instance!=this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        ShipQuests();
        PortQuests();
    }

    private void PortQuests()
    {
        QuestConstruction(QuestType.LoadOnPort, "Load the port", "Load 2 orange, 2 red and 2 yellow containers to the port",
            new Dictionary<ContainerType, int>{
                {ContainerType.RedContainer,2},
                {ContainerType.OrangeContainer, 2},
                {ContainerType.YellowContainer, 2},
            });
        QuestConstruction(QuestType.LoadOnPort, "Load the port", "Load 1 green, 2 yellow and 1 red containers to the port",
            new Dictionary<ContainerType, int>{
                {ContainerType.GreenContainer, 1},
                {ContainerType.YellowContainer, 2},
                {ContainerType.RedContainer, 1 }
            });
        QuestConstruction(QuestType.LoadOnPort, "Load the port", "Load 2 yellow and 4 blue containers to the port",
           new Dictionary<ContainerType, int>{
                {ContainerType.YellowContainer, 2},
                {ContainerType.BlueContainer, 2}
           });
    }
    public void ShipQuests()
    {
        QuestConstruction(QuestType.LoadOnShip,"Load the ship", "Load 1 red, 1 orange, 1 yellow, 1 green and" +
            " 1 blue container on board the ship", new Dictionary<ContainerType, int>{
                {ContainerType.RedContainer,1},
                {ContainerType.OrangeContainer,1 },
                {ContainerType.YellowContainer,1 },
                {ContainerType.GreenContainer,1 },
                {ContainerType.BlueContainer,1 }
            });
        QuestConstruction(QuestType.LoadOnShip, "Load the ship", "Load 1 green and 3 white containers on board the ship",
            new Dictionary<ContainerType, int>
            {
                {ContainerType.GreenContainer, 1 },
                {ContainerType.WhiteContainer, 3 }
            });
        QuestConstruction(QuestType.LoadOnShip, "Load the ship", "Load 2 blue, 2 white and 2 red on board the ship", new Dictionary<ContainerType, int>{
                {ContainerType.BlueContainer,2},
                {ContainerType.WhiteContainer,2 },
                {ContainerType.RedContainer,2 },
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

        allExistQuests.Add(quest);
    }

    public void RandomizeQuest()
    {
        GameObject mainQuest = Instantiate(mainQuestPrefab, allQuestsList.transform, allQuestsList);
        mainQuest.transform.localScale = new Vector3(1,1,1);
        activeMainQuest = mainQuest.GetComponent<MainQuest>();

        CreatingQuest(mainQuest, QuestType.LoadOnPort);
        CreatingQuest(mainQuest, QuestType.LoadOnShip);

        GameObject finishQuestButton = Instantiate(finishQuestPrefab, mainQuest.transform);
        finishQuestButton.transform.localScale = new Vector3(1, 1, 1);
    }

    private void CreatingQuest(GameObject mainQuest, QuestType questType)
    {
        GameObject quest = Instantiate(questPrefab, mainQuest.transform);

        List<QuestStructure> questsCurrentType = allExistQuests.Where(x => x.Type == questType).ToList();
        quest.GetComponent<Quest>().questStructure = questsCurrentType[UnityEngine.Random.Range(0, questsCurrentType.Count)];
        quest.transform.localScale = new Vector3(1, 1, 1);
        mainQuest.GetComponent<MainQuest>().questList.Add(quest.GetComponent<Quest>());
    }

    /// <param name="type">The type of quest in which progress is to be updated</param>
    /// <param name="sector">The sector in which the change occured</param>
    public void UpdateQuestProgressByType(QuestType type, Transform sector)
    {
        foreach (Quest quest in activeMainQuest.questList)
        {
            if (quest.questStructure.Type == type)
            {
                quest.UpdateProgress(sector);
            }
        }
    }
    /// <summary>
    /// Ends a given main quest
    /// </summary>
    /// <param name="ID">Main quest ID</param>
    public void FinishMainQuest(int ID)
    {
        if (activeMainQuest.GetInstanceID() == ID)
        {
            Destroy(activeMainQuest.gameObject);
        }
    }
}