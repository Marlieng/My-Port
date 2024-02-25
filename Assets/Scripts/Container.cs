using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static GameManager;

public class Container : MonoBehaviour
{
    Sprite[] typeOfContainers;
    public ContainerType containerType;

     void Start()
    {
        typeOfContainers = instance.typeOfContainers;
        int random = UnityEngine.Random.Range(0, typeOfContainers.Length);
        GetComponent<SpriteRenderer>().sprite = typeOfContainers[random];


        string spriteName = typeOfContainers[random].name.ToLower();
        if (spriteName.Contains("red"))
        {
            containerType = GameManager.ContainerType.RedContainer;
        }
        else if (spriteName.Contains("blue"))
        {
            containerType = GameManager.ContainerType.BlueContainer;
        }
        else if (spriteName.Contains("green"))
        {
            containerType = GameManager.ContainerType.GreenContainer;
        }
        else if (spriteName.Contains("white"))
        {
            containerType = GameManager.ContainerType.WhiteContainer;
        }
        else if (spriteName.Contains("yellow"))
        {
            containerType = GameManager.ContainerType.YellowContainer;
        }
        else if (spriteName.Contains("orange"))
        {
            containerType = GameManager.ContainerType.OrangeContainer;
        }
    }
}
