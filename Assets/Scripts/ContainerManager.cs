using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerManager : MonoBehaviour
{
    private static ContainerManager instance;
    public static ContainerManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance!=null&& instance!=this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    /// <summary>
    /// deactivate all containers in the sector to be loaded
    /// so that the reach stacker can them pick them up
    /// </summary>
    public void DeactivateContainers()
    {
        GameObject container;
        foreach (Transform row in GameManager.Instance.portLoadSector.Find("All Places"))
        {
            foreach (Transform place in row)
            {
                if (place.childCount != 0)
                {
                    container = place.GetComponentInChildren<Container>().gameObject;
                    if (container.GetComponent<Container>() && container.layer != GameManager.Instance.deactivateContainerLayer)
                    {
                        container.layer = GameManager.Instance.deactivateContainerLayer;
                        container.GetComponent<SpriteRenderer>().color = GameManager.Instance.deactivateContainerColor;
                    }
                }
            }
        }
        GameManager.Instance.sectorReachStackerToUnload.GetComponent<UnloadReachStacker>().FindDestination();
    }
}
