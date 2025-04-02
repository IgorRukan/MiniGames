using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZoneDJ : MonoBehaviour
{
    public Transform cameraTransform;
    public float offset = -2f;
    private float highestY;

    private void Update()
    {
        float newY = cameraTransform.position.y - Camera.main.orthographicSize - offset;

        // Только поднимаем триггер, но не опускаем
        if (newY > highestY)
        {
            highestY = newY;
            transform.position = new Vector3(cameraTransform.position.x, highestY, transform.position.z);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<CharacterDJ>())
        {
            other.gameObject.GetComponent<CharacterDJ>().Die();
        }
    }
}
