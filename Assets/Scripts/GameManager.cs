using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; set; }

    public enum ContainerType
    {
        RedContainer,
        BlueContainer,
        GreenContainer,
        WhiteContainer,
        YellowContainer,
        OrangeContainer
    }
    public enum QuestType
    {
        LoadOntoShip,
        LoadOntoPort
    }
    public Sprite[] typeOfContainers;

    private void Awake()
    {
        instance = this;
    }
}