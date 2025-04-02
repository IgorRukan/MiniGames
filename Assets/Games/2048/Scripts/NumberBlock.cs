using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class NumberBlock : MonoBehaviour
{
    public int number;
    public TextMeshProUGUI numberText;

    public int x;
    public int y;

    private float duration;

    private Vector2 endMousePos;

    private Vector2 targetPosition = new(0, 0);
    private NumberBlock targetBlock = null; // block v kotoriy zaletaet tekushiy

    private Field field;

    // main tot, kogo dernuli(nado dlya spavna next bloka posle hoda)
    private bool isMain = false;

    public float Duration
    {
        get => duration;
        set => duration = value;
    }

    public void SetXAndY(int nx, int ny, bool clearPos)
    {
        field.field[nx, ny] = field.field[x, y];

        if (clearPos)
        {
            field.field[x, y] = null;
        }
        
        x = nx;
        y = ny;
    }

    private void Start()
    {
        number = StartNumber();
        field = Field.Instance;
        RefreshText();
    }

    // tolko 2 ili 4
    private int StartNumber()
    {
        var chance = Random.Range(0, 100);
        int num = chance >= 50 ? 4 : 2;
        return num;
    }

    // po pravilam igri vsegda x2
    public void ChangeNumber()
    {
        number *= 2;
        
        // mogno i peremennuy dlya beskonechnoy igri
        if (number == 2048)
        {
            field.WinGame();
        }
    }

    private void OnMouseUp()
    {
        endMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var direction = (endMousePos - (Vector2)transform.position).normalized;

        //metim mainom tot block, kotoriy dernuli
        isMain = true;

        //Move(direction);
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Движение по горизонтали
            // right
            if (direction.x > 0)
            {
                direction = Vector2.right;
                MoveAll(field.width - 1, -1, -1, 0, field.height, 1, direction);
            }
            //left
            else
            {
                direction = Vector2.left;
                MoveAll(0, field.width, 1, 0, field.height, 1, direction);
            }
        }
        else
        {
            // Движение по вертикали
            //up
            if (direction.y > 0)
            {
                direction = Vector2.up;
                MoveAll(0, field.width, 1, field.height - 1, -1, -1, direction);
            }
            //daun
            else
            {
                direction = Vector2.down;
                MoveAll(0, field.width, 1, 0, field.height, 1, direction);
            }
        }
    }

    // zakonchit peredvijenie blockov
    private void MoveAll(int startX, int endX, int stepX, int startY, int endY, int stepY, Vector2 dir)
    {
        var gameField = field.field;

        for (int i = startX; i != endX; i += stepX)
        {
            for (int j = startY; j != endY; j += stepY)
            {
                if (gameField[i, j] != null)
                {
                    gameField[i, j].Move(dir);
                }
            }
        }
    }

    private void Move(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Движение по горизонтали
            // right
            if (direction.x > 0)
            {
                FindRightAndLeft(true);
            }
            //left
            else
            {
                FindRightAndLeft(false);
            }
        }
        else
        {
            // Движение по вертикали
            //up
            if (direction.y > 0)
            {
                FindUpAndDown(true);
            }
            //daun
            else
            {
                FindUpAndDown(false);
            }
        }

        StartCoroutine(MoveTo());
    }

    private void FindUpAndDown(bool isUp)
    {
        int pos = y;
        // up and down granitsi
        var edge = 0;

        // pri up
        if (isUp)
        {
            edge = field.height - 1;

            for (int j = 1; j <= field.height - 1 - y; j++)
            {
                pos = y + j;

                if (field.field[x, pos] != null) // start: x = 3, y = 1; end: x = 3, y = 3
                {
                    targetBlock = field.field[x, pos];
                    if (field.field[x, pos].number.Equals(number))
                    {
                        targetPosition = field.fieldPos[targetBlock.x, targetBlock.y].position;
                        targetBlock.ChangeNumber();
                        // nujen tolko clear
                        SetXAndY(x, y, true);
                    }
                    else
                    {
                        //esli pred block ne tekushaya pos, to pos blocka -1
                        if (!field.fieldPos[targetBlock.x, targetBlock.y - 1].position.Equals(field.fieldPos[x,y].position))
                        {
                            targetPosition = field.fieldPos[targetBlock.x, targetBlock.y - 1].position;
                            SetXAndY(targetBlock.x, targetBlock.y - 1, true);
                        }
                        else
                        {
                            targetPosition = transform.position;
                        }

                        targetBlock = null;
                    }

                    break;
                }
            }
        }
        //pri dawne
        else
        {
            for (int j = y - 1; j >= 0; j--)
            {
                pos = j;
                if (field.field[x, pos] != null)
                {
                    targetBlock = field.field[x, pos];
                    if (field.field[x, pos].number.Equals(number))
                    {
                        targetPosition = field.fieldPos[targetBlock.x, targetBlock.y].position;
                        targetBlock.ChangeNumber();
                        SetXAndY(x, y,true);
                    }
                    else
                    {
                        //esli pred block ne tekushaya pos, to pos blocka -1
                        if (!field.fieldPos[targetBlock.x, targetBlock.y + 1].position.Equals(field.fieldPos[x,y].position))
                        {
                            targetPosition = field.fieldPos[targetBlock.x, targetBlock.y + 1].position;
                            SetXAndY(targetBlock.x, targetBlock.y + 1,true);
                        }
                        else
                        {
                            targetPosition = transform.position;
                        }

                        targetBlock = null;
                    }

                    break;
                }
            }
        }

        // esli bloka net, to na max dir dvigaetsya
        if (targetPosition.Equals(new Vector2(0, 0)))
        {
            //esli dvigaesh, kogda na krau, else na max dlinu
            if ((y == 0 && pos == 0) || (y + 1 == field.height && pos == field.height - 1))
            {
                targetPosition = transform.position;
            }
            else
            {
                targetPosition = field.fieldPos[x, edge].position;
                SetXAndY(x, edge,true);
            }
        }
    }

    private void FindRightAndLeft(bool isRight)
    {
        int pos = x;
        // right and left granitsi
        var edge = 0;

        // pri right
        if (isRight)
        {
            edge = field.width - 1;
            for (int j = 1; j <= field.width - 1 - x; j++)
            {
                pos = x + j;

                if (field.field[pos, y] != null)
                {
                    targetBlock = field.field[pos, y];
                    if (field.field[pos, y].number.Equals(number))
                    {
                        targetPosition = field.fieldPos[targetBlock.x, targetBlock.y].position;
                        targetBlock.ChangeNumber();
                        SetXAndY(x, y,true);
                    }
                    else
                    {
                        //esli ne tekushaya pos, to pos blocka -1
                        if (!field.fieldPos[targetBlock.x - 1, targetBlock.y].position.Equals(field.fieldPos[x,y].position))
                        {
                            targetPosition = field.fieldPos[targetBlock.x - 1, targetBlock.y].position;
                            SetXAndY(targetBlock.x - 1, targetBlock.y,true);
                        }
                        else
                        {
                            targetPosition = transform.position;
                        }

                        targetBlock = null;
                    }

                    break;
                }
            }
        }
        //pri left
        else
        {
            for (int j = x - 1; j >= 0; j--)
            {
                pos = j;
                //esli ne pustoe mesto
                if (field.field[pos, y] != null)
                {
                    targetBlock = field.field[pos, y];
                    // esli nomer odinakoviy
                    if (field.field[pos, y].number.Equals(number))
                    {
                        targetPosition = field.fieldPos[targetBlock.x, targetBlock.y].position;
                        targetBlock.ChangeNumber();
                        SetXAndY(x, y,true);
                    }
                    else
                    {
                        //esli ne tekushaya pos, to pos blocka -1
                        if (!field.fieldPos[targetBlock.x + 1, targetBlock.y].position.Equals(field.fieldPos[x,y].position))
                        {
                            targetPosition = field.fieldPos[targetBlock.x + 1, targetBlock.y].position;
                            SetXAndY(targetBlock.x + 1, targetBlock.y,true);
                        }
                        else
                        {
                            targetPosition = transform.position;
                        }

                        targetBlock = null;
                    }

                    break;
                }
            }
        }

        // esli bloka net, to na max dir dvigaetsya
        if (targetPosition.Equals(new Vector2(0, 0)))
        {
            //esli dvigaesh, kogda na krau
            if ((x == 0 && pos == 0) || (x + 1 == field.width && pos == field.width - 1))
            {
                targetPosition = transform.position;
            }
            else
            {
                targetPosition = field.fieldPos[edge, y].position;
                SetXAndY(edge, y, true);
            }
        }
    }
    
// sdelat nishuu sigrovku
    private IEnumerator MoveTo()
    {
        float t = 0f;
        Vector2 startPosition = transform.position;

        //esli block na puti bil, to delit stariy i merge v tom bloke
        while (t < 1f)
        {
            t += Time.deltaTime * duration; // Увеличиваем t
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null; // Ждём следующий кадр
        }

        if (isMain)
        {
            field.SpawnNumberBlock();
            isMain = false;
        }

        transform.position = targetPosition;

        if (targetBlock != null)
        {
            targetBlock.RefreshText();
            
            // ubirau block iz spiska
            Destroy(gameObject);

            targetBlock = null;
        }

        targetPosition = new Vector2(0, 0);
    }


    private void RefreshText()
    {
        numberText.text = number.ToString();
    }
}