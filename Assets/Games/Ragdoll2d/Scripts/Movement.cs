using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject leftLeg;
    public GameObject rightLeg;
    Rigidbody2D leftLegRB;
    Rigidbody2D rightLegRB;

    Animator anim;
    [SerializeField] float speed = 2f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float legWait = 0.5f;

    public bool onGroundLeft;
    public bool onGroundRight;

    void Start()
    {
        leftLegRB = leftLeg.GetComponent<Rigidbody2D>();
        rightLegRB = rightLeg.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                anim.Play("WalkLeft");
                StartCoroutine(MoveRight(legWait));
            }
            else
            {
                anim.Play("WalkRight");
                StartCoroutine(MoveLeft(legWait));
            }
        }
        else
        {
            anim.Play("idle");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("jump");
            leftLegRB.AddForce(Vector2.up * (jumpHeight * 1000));
            rightLegRB.AddForce(Vector2.up * (jumpHeight * 1000));
        }
    }


    IEnumerator MoveRight(float seconds)
    {
        leftLegRB.AddForce(Vector2.right * (speed * 1000) * Time.deltaTime);
        yield return new WaitForSeconds(seconds);
        rightLegRB.AddForce(Vector2.right * (speed * 1000) * Time.deltaTime);
    }

    IEnumerator MoveLeft(float seconds)
    {
        rightLegRB.AddForce(Vector2.left * (speed * 1000) * Time.deltaTime);
        yield return new WaitForSeconds(seconds);
        leftLegRB.AddForce(Vector2.left * (speed * 1000) * Time.deltaTime);
    }

    private void IsOnGround(OnGroundChecker leg)
    {
        switch (leg.leg)
        {
            case "left":
            {
                onGroundLeft = leg.onGround;
                break;
            }
            case "right":
            {
                onGroundRight = leg.onGround;
                break;
            }
        }

        if (!onGroundLeft && !onGroundRight)
        {
            StartFall();
        }
    }

    public void StartFall()
    {
        StartCoroutine(Fall());
    }

    IEnumerator Fall()
    {
        yield return new WaitForSeconds(1f);

        if (!onGroundLeft && !onGroundRight)
        {
            FreeFall(false);
        }
    }

    IEnumerator UpAfterFall()
    {
        yield return new WaitForSeconds(1.5f);
        if (onGroundLeft || onGroundRight)
        {
            FreeFall(true);
        }
    }

    private void FreeFall(bool isFall)
    {
        var balances = GetComponentsInChildren<Balance>();

        //disable balance
        
        foreach (var balance in balances)
        {
            balance.enabled = isFall;
        }
    }

    //if touch ground, up after few seconds
    private void OnRagdollCollision(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(UpAfterFall());
        }
    }

    //dodelat damag, heal, map
}