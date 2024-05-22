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
    public string deactivateContainer = "Deactivate Container";

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
    }
    /// <summary>
    /// updates quest progress
    /// </summary>
    /// <param name="sector">The sector in which the change occured</param>
    public void UpdateProgress(Transform sector)
    {
        //stored all types of containers that are in a given sector
        List<ContainerType> sectorContainerType = new List<ContainerType>();
        GameObject container;

        //counts and adds all container types present in the sector to sectorContainerType
        foreach (Transform row in sector.Find("All Places"))
        {
            if (row.name.Contains("Row"))
            {
                foreach (Transform place in row)
                {
                    if (place.childCount != 0)
                    {
                        container = place.GetComponentInChildren<Container>().gameObject;

                        if (container.gameObject.layer != Instance.deactivateContainerLayer)
                        {
                            sectorContainerType.Add(place.GetComponentInChildren<Container>().type);
                        }
                    }
                }
            }
        }
        /*sorts all container types by keys and values where keys are
         * the container types and value is the number of containers of a given type*/
        Dictionary<ContainerType, int> segregatedContainerTypes =
            sectorContainerType.GroupBy(x=>x).ToDictionary(y=>y.Key,y=>y.Count());
        
        foreach (ContainerType requiredContainerType in questStructure.PlayerProgress.Keys.ToList())
        {
            for (int i = 0; i < questStructure.PlayerProgress.Count(); i++)
            {
                if (segregatedContainerTypes.Keys.Contains(requiredContainerType) )
                {
                    UpdateSubtaskValue(requiredContainerType, segregatedContainerTypes[requiredContainerType]);
                    break;
                }
                else
                {
                    UpdateSubtaskValue(requiredContainerType, 0);
                }
            }
        }
    }

    /// <param name="containerTypeCount">current count of a given container type</param>
    private void UpdateSubtaskValue(ContainerType requiredContainerType, int containerTypeCount)
    {
        questStructure.PlayerProgress[requiredContainerType] = containerTypeCount;

        /*finds a subtask (slider) with the name of the required
         * container type and sets the progress of this subtask to
         * the number of containers of a given type in the sector */
        transform.Find(requiredContainerType.ToString())
            .GetComponent<Slider>().value = containerTypeCount;
    }

    public void CheckQuestComplete()
    {
        int allNotMatching = 0;
        if (questStructure.PlayerProgress.SequenceEqual(questStructure.QuestRequirements))
        {
            questStructure.isCompleted = true;
            Debug.Log("Wszystko siê zgadza :D!");
        }
        else
        //The feature is still being refined
        {
            allNotMatching = 0;
            foreach (var item in questStructure.PlayerProgress)
            {
                allNotMatching += questStructure.QuestRequirements.Where(x => x.Key == item.Key)
                    .Sum(x => Math.Abs(x.Value - item.Value));
            }
        }
    }
}