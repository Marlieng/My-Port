using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class QuestStructure : MonoBehaviour
{
    public QuestType Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Dictionary<ContainerType, int> QuestRequirements { get; set; }
    public Dictionary<ContainerType, int> PlayerProgress = new Dictionary<ContainerType, int>();

    public bool isCompleted;
}
