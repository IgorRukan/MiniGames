using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundChecker : MonoBehaviour
{
    public GameObject rootObject;
    public string leg;
    public bool onGround;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Ground"))
        {
            onGround = true;
            rootObject.SendMessage("IsOnGround", this, SendMessageOptions.DontRequireReceiver);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Ground"))
        {
            onGround = false;
            rootObject.SendMessage("IsOnGround",this, SendMessageOptions.DontRequireReceiver);
            //rootObject.SendMessage("StartFall", SendMessageOptions.DontRequireReceiver);
        }
    }
}
