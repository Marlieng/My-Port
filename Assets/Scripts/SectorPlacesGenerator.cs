using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SectorPlacesGenerator : MonoBehaviour
{
    public GameObject placePrefab;
    public GameObject emptyObjectPrefab;
    public GameObject allPlacesObject;

    [Header("Sector Sizes")]
    public int rows; //number of rows
    public int placesInRow; //number of places in row
    public float dashLineSpacing = 0.2f;
    public float placeSpacing = 0.05f;

    BoxCollider2D placeCollider;

    public void GenerateSpaces()
    {
        DestroyOldPlaces(transform.Find("All Places"));

        placeCollider = placePrefab.GetComponent<BoxCollider2D>();
        float placeSizeX = placeCollider.size.x;
        float placeSizeY = placeCollider.size.y;

        Vector3 currentRowPosition = transform.position;
        Vector3 currentPlacePosition;

        //creates rows and places for them
        for (int i = 0; i < rows; i++)
        {
            currentPlacePosition = currentRowPosition;
            Transform currentRow = Instantiate(emptyObjectPrefab, currentRowPosition, Quaternion.identity,allPlacesObject.transform).transform;
            currentRow.name = "Row " + i;
            for (int j = 0; j < placesInRow; j++)
            {
                Vector3 currentPosition = new Vector3(currentPlacePosition.x, currentPlacePosition.y + placeSizeY + placeSpacing*0.5f, 0);
                Instantiate(placePrefab, currentPlacePosition, Quaternion.identity, currentRow);
                currentPlacePosition = currentPosition;
            }
            currentRowPosition = new Vector3(currentRowPosition.x + placeSizeX + placeSpacing * 0.5f, currentRowPosition.y, 0);
        }

        //Sets the boundaries for the sprite and Box Collider sectors
        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        Transform[] children = allPlacesObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            BoxCollider2D childCollider = child.GetComponent<BoxCollider2D>();
            if (childCollider != null)
            {
                //calculates the farthest point in the lower left corner
                Vector3 childMin = (Vector2)child.position - childCollider.size / 2;
                //calculates the farthest point in the upper right corner
                Vector3 childMax = (Vector2)child.position + childCollider.size / 2;

                //Extends bounds to the autermost rows and places
                bounds.Encapsulate(childMin);
                bounds.Encapsulate(childMax);
            }
        }
        //sets allPlacesObject in the centre of its children
        allPlacesObject.transform.localPosition -= bounds.size * 0.5f - new Vector3(placeSizeX * 0.5f, placeSizeY * 0.5f);
        
        bounds.size += new Vector3(dashLineSpacing, dashLineSpacing);

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = bounds.size;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.size = bounds.size;
    }

    private void DestroyOldPlaces(Transform allPlaces)
    {
        GameObject[] children = new GameObject[allPlaces.childCount];
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = allPlaces.GetChild(i).gameObject;
        }
        foreach (GameObject child in children)
        {
            DestroyImmediate(child);
        }
    }
}
