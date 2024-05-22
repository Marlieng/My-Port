using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameManager;

public class CraneMovement : MonoBehaviour
{
    [Header("Crane movement")]
    public Transform minRange;
    public Transform maxRange;
    public float speed = 3;
    float horizontal;
    [SerializeField]
    private InputActionReference movement;

    private void Update()
    {
        horizontal = movement.action.ReadValue<Vector2>().x;
        Movement();
    }

    private void Movement()
    {
        float positionX = Mathf.Clamp(transform.position.x + horizontal * speed * Time.deltaTime, minRange.position.x, maxRange.position.x);
        transform.position = new Vector2(positionX, transform.position.y);
    }
}
