using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpGuy : MonoBehaviour
{
    public float baseJumpForce;
    private float jumpForce;
    public float holdTimeThreshold = 2f;

    public KeyCode key = KeyCode.Space;
    public KeyCode leftSide = KeyCode.A;
    public KeyCode rightSide = KeyCode.D;

    private float holdTime = 0f; // Время зажатия
    public bool isHolding = false;

    private int direction = 0; // 0 - вверх, 1 - вправо, -1 - влево

    private Rigidbody2D rb2d;

    public float botLim;
    public float topLim;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            isHolding = true;
            holdTime = 0f;
            direction = 0;
            Debug.Log("Pressed Jump Key");
        }

        if (Input.GetKey(leftSide)) direction = -1;
        if (Input.GetKey(rightSide)) direction = 1;

        if (isHolding)
        {
            holdTime += Time.deltaTime;

            if (holdTime >= holdTimeThreshold) // Достигли максимального удержания — прыгаем
            {
                Jump();
                isHolding = false; // Сбрасываем состояние удержания
            }
        }

        if (Input.GetKeyUp(key) && isHolding) // Если отпустили раньше максимума — все равно прыгаем
        {
            Jump();
            isHolding = false;
        }
    }

    private void Jump()
    {
        Debug.Log("Released Jump Key");

        SetForce(holdTime, direction);
        isHolding = false;
    }

    private void SetForce(float time, int side) // side: -1 left, 1 right, 0 center
    {
        float angle = 0;
        var timePercentage = time / holdTimeThreshold;
        if (timePercentage < botLim)
        {
            angle = topLim;
        }
        else if (timePercentage > topLim)
        {
            angle = botLim;
        }
        else
        {
            angle = 1 - timePercentage;
        }
        Debug.Log("Jumping");
        jumpForce = baseJumpForce * time; // Ограничение максимальной силы

        Vector2 dir = new Vector2(side * angle, 1).normalized; // Нормализация, чтобы сила была равномерной
        rb2d.velocity = Vector2.zero; // Обнуляем скорость перед прыжком
        rb2d.AddForce(dir * jumpForce, ForceMode2D.Impulse);
    }
}