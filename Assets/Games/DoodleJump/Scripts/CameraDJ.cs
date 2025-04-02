using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraDJ : MonoBehaviour
{
    public Transform playerPos;
    public float topMargin = -4f; // Отступ сверху
    public float botMargin = 4f; // Отступ снизу
    public float cameraMoveSpeed;
    
    private Camera cam;

    private CharacterDJ player;

    void Start()
    {
        player = playerPos.gameObject.GetComponent<CharacterDJ>();
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (player.isDead)
        {
            return;
        }
        // top
        if (playerPos.position.y > cam.transform.position.y + cam.orthographicSize + topMargin)
        {
            Vector3 newPosition = cam.transform.position;
            newPosition.y = Mathf.Lerp(cam.transform.position.y, playerPos.position.y - topMargin - cam.orthographicSize, cameraMoveSpeed * Time.deltaTime);
            cam.transform.position = newPosition;
            
        }
        
        if (playerPos.position.y < cam.transform.position.y - cam.orthographicSize - botMargin)
        {
            Vector3 newPosition = cam.transform.position;
            newPosition.y = Mathf.Lerp(cam.transform.position.y, playerPos.position.y + botMargin + cam.orthographicSize, cameraMoveSpeed * Time.deltaTime);
            cam.transform.position = newPosition;
        }
    }
}
