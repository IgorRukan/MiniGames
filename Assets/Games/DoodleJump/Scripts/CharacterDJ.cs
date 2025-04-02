using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDJ : MonoBehaviour
{
   public float jumpForce;
   
   public float moveSpeed;
   public float maxJumpSpeed;
   
   private Rigidbody2D rb2d;
   
   public bool isDead = false;

   private void Start()
   {
      rb2d = GetComponent<Rigidbody2D>();
   }

   public void Die()
   {
      isDead = true;
      rb2d.isKinematic = true;
   }

   public void Jump()
   {
      rb2d.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
   }

   private void FixedUpdate()
   {
      if (isDead == false)
      {
         float moveHorizontal = Input.GetAxis("Horizontal");

         rb2d.velocity = new Vector2(moveHorizontal * moveSpeed, rb2d.velocity.y);
      }
      
      if (rb2d.velocity.y > maxJumpSpeed)
      {
         rb2d.velocity = new Vector2(rb2d.velocity.x, maxJumpSpeed);
      }
   }
}
