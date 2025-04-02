using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideScreen : MonoBehaviour
{
    public Transform teleportPoint;
    public float offset;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<CharacterDJ>())
        {
            other.GetComponent<CharacterDJ>().transform.position = new Vector2(teleportPoint.position.x-offset,transform.position.y);
        }
    }
}
