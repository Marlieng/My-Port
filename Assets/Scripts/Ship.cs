using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameManager;

public class Ship : MonoBehaviour
{
    public GameObject containerPrefab;
    /// <summary>
    /// generates containers on a given ship
    /// </summary>
    public void GenerateContainers()
    {
        Transform row;
        Transform space;
        //iterates through each active quest requirement of type LoadOnPort in activeMainQuest
        foreach (var requirement in QuestManager.Instance.activeMainQuest.questList
            .FirstOrDefault(x => x.questStructure.Type == QuestType.LoadOnPort)
            .questStructure.QuestRequirements)
        {
            /*depending on the number of required containers, it creates a certain number
             * of containers in a random free place on ship*/
            for (int i = 0; i < requirement.Value; i++)
            {
                //randomizes the row in which the container is to be created
                row = Instance.shipUnloadSector.GetChild(0).GetChild(Random.Range(0, Instance.shipUnloadSector.GetChild(0).childCount));
                //randomizes the place in the row in which the container is to be created
                space = row.GetChild(Random.Range(0, row.childCount));

                if (space.childCount == 0)
                {
                    Transform generatedContainer = Instantiate(containerPrefab, Vector2.zero, Quaternion.identity, space).transform;
                    generatedContainer.localPosition = Vector2.zero;
                    generatedContainer.localRotation = Quaternion.identity;
                }
                else
                {
                    i--;
                }
            }
        }
    }
}
