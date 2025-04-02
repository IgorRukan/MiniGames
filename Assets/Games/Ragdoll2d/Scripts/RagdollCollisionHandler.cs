using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollCollisionHandler : MonoBehaviour
{
    public GameObject rootObject;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rootObject != null)
        {
            rootObject.SendMessage("OnRagdollCollision", collision, SendMessageOptions.DontRequireReceiver);
        }
    }
}
