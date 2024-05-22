using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public Sprite[] typeOfContainers;

    [Header("Containers")]
    public Color deactivateContainerColor;
    public Color activeContainerColor;

    [Header("Transporting Containers")]
    public CraneLift craneLift;
    public LoadingReachStacker sectorReachStackerToLoad;
    public UnloadReachStacker sectorReachStackerToUnload;

    [Header("Current Sectors")]
    public Transform portLoadSector;
    public Transform portUnloadSector;
    public Transform shipLoadSector;
    public Transform shipUnloadSector;

    [Header("Layers")]
    public LayerMask sectorLayer;
    public LayerMask freeSpaceLayer;
    public LayerMask activeContainerLayer;
    public int deactivateContainerLayer = 9;

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
        LoadOnShip,
        LoadOnPort
    }

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
    }
}