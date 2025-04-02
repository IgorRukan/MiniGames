using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Blocks : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int points;

    public Figures currentFigure;

    private RectTransform rectTransform;
    public int x;
    public int y;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public enum Figures
    {
        Blue,
        Yellow,
        Green,
        Red,
        Black,
        Purple,
        Orange
    }

    public void SetXY(int newX, int newY)
    {
        x = newX;
        y = newY;
    }

    public void Break()
    {
    }

    public void SetPosition(Vector2 newPosition)
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = newPosition;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }
}