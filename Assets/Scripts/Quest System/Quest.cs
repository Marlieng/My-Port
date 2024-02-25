using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static GameManager;

public class Quest : MonoBehaviour
{
    [NonSerialized]
    public QuestStructure questStructure;

    public TextMeshProUGUI questName;
    public TextMeshProUGUI questDescription;

    public Slider questStagePrefab;

    Dictionary<ContainerType, int> segregatedContainerTypes;

    [NonSerialized]
    int allNotMatching = 0;

    private void Start()
    {
        questName.text = questStructure.Name;
        questDescription.text = questStructure.Description;

        foreach (var item in questStructure.QuestRequirements)
        {
            Slider partQuest = Instantiate(questStagePrefab, transform);
            partQuest.maxValue = item.Value;
            partQuest.name = item.Key.ToString();

            //assign a Description to the partQuest text
            partQuest.GetComponentInChildren<TextMeshProUGUI>().text = 
                $"{questStructure.Type} {item.Value} {item.Key}";
        }

        //UnityAction finishButtonAction = new UnityAction(CheckComplete);
        //Button finishQuest = Instantiate(finishQuestPrefab, transform);
        //finishQuest.onClick.AddListener(finishButtonAction);
    }

    public void ProgressUpdate(Transform sector)
    {
        List<ContainerType> placeContainerType = new List<ContainerType>();
        foreach (Transform space in sector)
        {
            if (space.GetComponentInChildren<Container>()!=null)
            {
                placeContainerType.Add(space.GetComponentInChildren<Container>().containerType);
            }
        }
        segregatedContainerTypes = placeContainerType.GroupBy(x=>x).ToDictionary(y=>y.Key,y=>y.Count());

        foreach (ContainerType requiredContainerType in questStructure.PlayerProgress.Keys.ToList())
        {
            for (int i = 0; i < questStructure.PlayerProgress.Count(); i++)
            {
                if (segregatedContainerTypes.Keys.Contains(requiredContainerType) )
                {
                    questStructure.PlayerProgress[requiredContainerType] = segregatedContainerTypes[requiredContainerType];

                    transform.Find(requiredContainerType.ToString())
                        .GetComponent<Slider>().value = segregatedContainerTypes[requiredContainerType];
                    break;
                }
                else
                {
                    questStructure.PlayerProgress[requiredContainerType] = 0;

                    transform.Find(requiredContainerType.ToString())
                        .GetComponent<Slider>().value = 0;
                }
            }
        }
    }
    public void CheckComplete()
    {
        if (questStructure.PlayerProgress.SequenceEqual(questStructure.QuestRequirements))
        {
            questStructure.isCompleted = true;
            Debug.Log("That's good!");
        }
        else
        {
            allNotMatching = 0;
            foreach (var item in questStructure.PlayerProgress)
            {
                allNotMatching += questStructure.QuestRequirements.Where(x => x.Key == item.Key)
                    .Sum(x => Math.Abs(x.Value - item.Value));
            }
            //foreach (var questRequirement in questStructure.QuestRequirements)
            //{
            //    foreach (var playerProgress in questStructure.PlayerProgress)
            //    {
            //        int notMatching = 0;
            //        if (questRequirement.Key==playerProgress.Key)
            //        {
            //            notMatching = questRequirement.Value - playerProgress.Value;
            //            allNotMatching += allNotMatching;
            //        }
            //        Debug.Log(notMatching);
            //    }
            //    //notMatching += questStructure.PlayerProgress.Where(
            //    //    x => x.Key == questRequirement.Key 
            //    //    && notMatchingx.Value == item.Value).Count();
            //}
            Debug.Log(allNotMatching);
        }
    }
}