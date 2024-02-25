using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoDisplay : MonoBehaviour
{
    private Vector2 cursor;
    public Vector2 infoDistance = new Vector2(0.73f, 0.15f);
    public GameObject Info;
    private void Update()
    {
        //Get cursor position
        cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Assign "cursor" position to "Info"
        Info.transform.position = cursor + infoDistance;
    }
    private void OnMouseEnter()
    {
        Info.SetActive(true);
    }
    private void OnMouseExit()
    {
        Info.SetActive(false);
    }
}
