using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragUIObject : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Vector2 prevPos;

    public GameField field;

    private Blocks gem;

    private Blocks[,] defaultField;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        gem = GetComponent<Blocks>();

        field = GameField.Instance;
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        prevPos = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        defaultField = field.gems;

    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        
        var swappedGem = eventData.pointerEnter.GetComponent<Blocks>();
        
        if (swappedGem != null)
        {
            ShiftGems(swappedGem);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        if (field.ValidTurn(gem))
        {
            Debug.Log("Est sovpadnie");
            var swappedGem = eventData.pointerEnter.GetComponent<Blocks>();

            Debug.Log(swappedGem);

            field.CheckOnLines();
        }
        else
        {
            SetDefaultField();
            //transform.position = prevPos;
        }
    }

    private void ShiftGems(Blocks pointedGem)
    {
        var deltaX = gem.x - pointedGem.x;
        var deltaY = gem.y - pointedGem.y;

        // massiv vseh gemov mejdu 1 i 2
        
        //liniyu
        if (deltaY == 0 && deltaX != 0)
        {
            var gems = deltaX > 0
                ? field.GetLine(pointedGem.x, gem.x, gem.y)  // sprava nalevo 
                : field.GetLine(gem.x, pointedGem.x, gem.y);  // sleva napravo

            ShiftGem(gems, Mathf.Abs(deltaX), true);
        }
        
        //stolbec
        else if (deltaX == 0 && deltaY != 0)
        {
            var gems = deltaY > 0
                ? field.GetRow(pointedGem.y, gem.y, gem.x) // snizu vveph
                : field.GetRow(gem.y, pointedGem.y, gem.x); // sverhu vniz
 
            ShiftGem(gems, Mathf.Abs(deltaY), false);
        }
    }
// razobratsya s sigrovkami (vrode ok, no mogno potstit na krayah)
    private void ShiftGem(Blocks[] gems, int steps, bool isHorizontal)
    {
        for (int i = 0; i < steps; i++)
        {
            if (isHorizontal)
            {
                // v liniu
                //pos
                gems[i].transform.position =  field.gemsPos[gems[i+1].x, gems[i].y];
                gems[i+1].transform.position =  field.gemsPos[gems[i].x, gems[i].y];
                
                (gems[i],gems[i+1]) = (gems[i+1],gems[i]);
                (gems[i].x, gems[i + 1].x) = (gems[i + 1].x, gems[i].x);
                
            }
            else
            {
                // v stolbec
                //pos
                gems[i].transform.position =  field.gemsPos[gems[i].x, gems[i+1].y];
                gems[i+1].transform.position =  field.gemsPos[gems[i].x, gems[i].y];
                
                (gems[i],gems[i+1]) = (gems[i+1],gems[i]);
                (gems[i].y, gems[i + 1].y) = (gems[i + 1].y, gems[i].y);
            }
        }

        field.SetPosForGems();
        
    }

    //ne sdvigautsya pri ne valid move
    private void SetDefaultField()
    {
        field.gems = defaultField;
        for (int i = 0; i < field.width; i++)
        {
            for (int j = 0; j < field.height; j++)
            {
                field.gems[i, j].transform.position = field.gemsPos[i, j];
                field.gems[i, j].x = i;
                field.gems[i, j].y = j;
            }
        }
        field.SetPosForGems();
    }
}