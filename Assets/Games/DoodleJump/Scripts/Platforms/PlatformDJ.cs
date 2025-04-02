using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDJ : MonoBehaviour
{
    public bool isBreakable = false;

    private void Break()
    {
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        var player = other.gameObject.GetComponent<CharacterDJ>();
        
        if (player != null)
        {
            player.Jump();
        }
        
        if (isBreakable)
        {
            Break();
        }
    }
}
