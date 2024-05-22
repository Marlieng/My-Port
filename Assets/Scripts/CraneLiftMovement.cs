using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CraneLiftMovement : MonoBehaviour
{
    [Header("Crane lift movement")]
    public Transform minRange;
    public Transform maxRange;
    public float speed = 3f;
    float vertical;

    [SerializeField]
    private InputActionReference movement;
    private void Update()
    {
        vertical = movement.action.ReadValue<Vector2>().y;
        LiftMovement();
    }
    private void LiftMovement()
    {
        float positionY = Mathf.Clamp(transform.position.y + vertical * speed * Time.deltaTime, minRange.position.y, maxRange.position.y);
        transform.position = new Vector2(transform.position.x, positionY);
    }
}
