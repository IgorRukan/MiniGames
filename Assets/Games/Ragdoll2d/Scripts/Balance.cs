using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balance : MonoBehaviour
{
    public float targetRotation;
    private Rigidbody2D rb;
    public float force;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rb.MoveRotation(Mathf.LerpAngle(rb.rotation,targetRotation, force*Time.deltaTime));
    }
}
