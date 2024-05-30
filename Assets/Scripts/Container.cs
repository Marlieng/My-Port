using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static GameManager;
using Unity.Mathematics;

public class Container : MonoBehaviour
{
    public ContainerType type;
     void Start()
    {
        int random = UnityEngine.Random.Range(0, Instance.typeOfContainers.Length);
        SetContainerType(random);
    }
    /// <summary>
    /// Sets the container type
    /// </summary>
    /// <param name="num">The type will be set based on this number</param>
    public void SetContainerType(int num)
    {
        string typeName = Instance.typeOfContainers[num].name.ToLower();
        //Decides what type the container will take
        switch (typeName)
        {
            case string name when name.Contains("red"):
                type = ContainerType.RedContainer;
                break;
            case string name when name.Contains("blue"):
                type = ContainerType.BlueContainer;
                break;
            case string name when name.Contains("green"):
                type = ContainerType.GreenContainer;
                break;
            case string name when name.Contains("white"):
                type = ContainerType.WhiteContainer;
                break;
            case string name when name.Contains("yellow"):
                type = ContainerType.YellowContainer;
                break;
            case string name when name.Contains("orange"):
                type = ContainerType.OrangeContainer;
                break;
        }
        //decides what sprite type the container will take
        GetComponent<SpriteRenderer>().sprite = Instance.typeOfContainers[num];
    }
    /// <summary>
    /// Specifies whether the container should be active or deactive
    /// </summary>
    /// <param name="isActive">decides whether the container is active or not. When the value is true the container is active and when false it is deactive</param>
    public void SetContainerActive(bool isActive)
    {
        if (isActive)
        {
            gameObject.layer = 6;
            GetComponent<SpriteRenderer>().color = Instance.activeContainerColor;
        }
    }
}
