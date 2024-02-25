using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ContainersSpacing : MonoBehaviour
{
    public GameObject container;
    Transform[] containerSpace;
    private void Start()
    {
        containerSpace = new Transform[transform.childCount];

        //Add conteiners to "containers" variable
        for (int i = 0; i < transform.childCount; i++)
        {
            containerSpace[i] = transform.GetChild(i);
        }

        //Number of unnecessary containers
        int numberContainers = Random.Range(0,transform.childCount);

        List<int> createdContainers = new List<int>();

        int j = 0;
        do
        {
            //Number of container for romeve
            int createContainer = Random.Range(0, transform.childCount);

            if (createdContainers.Contains(createContainer))
            {
                j--;
            }
            else
            {
                createdContainers.Add(createContainer);
                Instantiate(container, containerSpace[j].position, containerSpace[j].rotation, containerSpace[j]);
            }
            j++;
        } while (j < numberContainers);
    }
}
