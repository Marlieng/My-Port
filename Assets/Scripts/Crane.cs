using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Crane : MonoBehaviour
{
    //Move direction
    float horizontal;
    //Move speed
    public float speedCrane = 3;

    [SerializeField]
    private InputActionReference movement;

    private void Update()
    {
        horizontal = movement.action.ReadValue<Vector2>().x;
    }
    private void FixedUpdate()
    {
        CraneMove();
    }
    private void CraneMove()
    {

        gameObject.transform.Translate(new Vector2(horizontal * Time.deltaTime * speedCrane, 0));

    }
}
