using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameField : Singleton<GameField>
{
    public int width;
    public int height;
    public Blocks[] gemsPrefabs;
    public Transform gemParent;
    public float matchDelay = 0.5f;
    public float spawnDelay = 0.5f;
    public Blocks[,] gems;
    public Vector2[,] gemsPos;
    public List<RectTransform> otherElements;

    public float gemSize = 90f;
    public float padding = 10f;

    private bool isNeedToCheck = true;

    public int score = 0;
    public TextMeshProUGUI scoreTxt;


    private void Start()
    {
        gems = new Blocks[width, height];

        gemsPos = new Vector2[width, height];

        InitializeBoard();
    }

    private void ResetBoard()
    {
        Array.Clear(gems, 0, gems.Length);

        InitializeBoard();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowMiniField();
        }
    }

    void InitializeBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gems[x, y] = CreateGem(x, y);
                gemsPos[x, y] = gems[x, y].transform.position;
            }
        }

        CheckForMatches();
    }

    private Blocks CreateGem(int x, int y)
    {
        var gem = Instantiate(RandomGem(), gemParent);

        RectTransform rectTransform = gem.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(gemSize, gemSize);
        rectTransform.anchoredPosition =
            new Vector2(x * (gemSize + padding) + CentreX() + gemSize / 2 + padding,
                y * (gemSize + padding) + CentreY() + gemSize / 2 + padding);

        gem.x = x;
        gem.y = y;
        gem.name = $"Gem {x} {y}";

        // Сохраняем ссылку на гем в массиве
        return gem;
    }

    private Blocks RandomGem()
    {
        return gemsPrefabs[Random.Range(0, gemsPrefabs.Length)];
    }

    public Blocks[] GetLine(int x, int endX, int y)
    {
        var line = gems.Cast<Blocks>().Where(l => l.y == y && l.x >= x && l.x <= endX)
            .OrderByDescending(l => l.x).ToArray();
        return line;
    }

    public Blocks[] GetRow(int y, int endY, int x)
    {
        var line = gems.Cast<Blocks>().Where(l => l.x == x && l.y >= y && l.y <= endY)
            .OrderByDescending(l => l.y).ToArray();
        return line;
    }

    public int CentreX()
    {
        return (int)(gemParent.GetComponent<RectTransform>().sizeDelta.x -
                     (width * (gemSize + padding) + padding)) / 2;
    }

    public int CentreY()
    {
        return (int)(gemParent.GetComponent<RectTransform>().sizeDelta.y -
                     (height * (gemSize + padding) + padding)) / 2;
    }

    private void CheckForMatches()
    {
        if (isNeedToCheck)
        {
            isNeedToCheck = false;

            CheckOnLines();
        }
    }

    public void CheckOnLines()
    {
        int number = 3; //default match

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (RowCheck(x, y, x + 1) && RowCheck(x + 1, y, x + 2))
                {
                    // check na 4
                    if (RowCheck(x + 2, y, x + 3))
                    {
                        number++;
                        //check na 5
                        if (RowCheck(x + 3, y, x + 4))
                        {
                            number++;
                        }
                    }

                    Debug.Log("Matched :" + "x - " + x + " y - " + y);
                    StartCoroutine(DestroyGems(x, y, true, number));

                    number = 3;
                    isNeedToCheck = true;
                    break;
                }

                if (ColumnCheck(x, y, y + 1) && ColumnCheck(x, y + 1, y + 2))
                {
                    Debug.Log("true");

                    // check na 4
                    if (ColumnCheck(x, y + 2, y + 3))
                    {
                        number++;
                        //check na 5
                        if (ColumnCheck(x, y + 3, y + 4))
                        {
                            number++;
                        }
                    }

                    StartCoroutine(DestroyGems(x, y, false, number));

                    number = 3;
                    isNeedToCheck = true;
                    break;
                }
            }

            if (isNeedToCheck)
            {
                break;
            }
        }
    }

    private bool ColumnCheck(int x, int y, int nextY)
    {
        if (nextY >= height || nextY < 0 || y >= height || y < 0)
        {
            return false;
        }
        
        //сравнивается текущий и соседний(выше или ниже стоящий)
        return gems[x, y].currentFigure.Equals(gems[x, nextY].currentFigure);
       
    }

    private bool RowCheck(int x, int y, int nextX)
    {
        if (nextX >= width || nextX < 0 || x >= height || x < 0)
        {
            return false;
        }

       
        //сравнивается текущий и соседний(справа или слева стоящий)
        return gems[x, y].currentFigure.Equals(gems[nextX, y].currentFigure);
    }


    private IEnumerator DestroyGems(int x, int y, bool isRow, int number)
    {
        int count = 1;
        int defX = x; //default x
        int defY = y; //default y

        ScoreUpdate(gems[x, y].points * number);

        for (int i = 0; i < number; i++)
        {
            //gems[x, y].gameObject.SetActive(false);

            Destroy(gems[x, y].gameObject);

            gems[x, y] = null;

            // v stroku
            if (isRow)
            {
                x += count;
            }
            // v stolbec
            else
            {
                y += count;
            }

            yield return new WaitForSeconds(matchDelay);
        }

        // Fill Empty spaces

        FillEmptySpaces(isRow, defX, defY, number);
    }

    private void FillEmptySpaces(bool isWidth, int defX, int defY, int number)
    {
        // v stroku
        if (isWidth)
        {
            StartCoroutine(RowFall(defX, defY, number));
        }
        // v stolbec
        else
        {
            StartCoroutine(ColumnFall(defX, defY, number));
        }
    }

    private IEnumerator RowFall(int defX, int defY, int number)
    {
        var x = defX;
        var size = height - 1 - defY; //skolkim ryadam nado upast

        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < number; i++)
            {
                gems[defX, defY + 1].transform.position = gemsPos[defX, defY];
                gems[defX, defY] = gems[defX, defY + 1];
                gems[defX, defY].SetXY(defX, defY);

                gems[defX, defY + 1] = null;
                defX++;

                yield return new WaitForSeconds(matchDelay);
            }

            defX = x;
            defY++;
        }

        StartCoroutine(SpawnNewGems());
    }

    private IEnumerator ColumnFall(int defX, int defY, int number)
    {
        var count = height - number - defY;

        for (int i = 0; i < count; i++)
        {
            gems[defX, defY + number].transform.position = gemsPos[defX, defY];
            gems[defX, defY] = gems[defX, defY + number];
            gems[defX, defY].SetXY(defX, defY);

            gems[defX, defY + number] = null;

            defY++;

            yield return new WaitForSeconds(matchDelay);
        }

        StartCoroutine(SpawnNewGems());
    }

    private IEnumerator SpawnNewGems()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (gems[x, y] == null)
                {
                    yield return new WaitForSeconds(spawnDelay);
                    gems[x, y] = CreateGem(x, y);
                }
            }
        }

        CheckForMatches();
    }

    public bool ValidTurn(Blocks gem)
    {
        //Check in all sides

        // down
        if (ColumnCheck(gem.x, gem.y, gem.y - 1) && ColumnCheck(gem.x, gem.y - 1, gem.y - 2))
        {
            isNeedToCheck = true;
            CheckForMatches();
            return true;
        }

        // up
        if (ColumnCheck(gem.x, gem.y, gem.y + 1) && ColumnCheck(gem.x, gem.y + 1, gem.y + 2))
        {
            isNeedToCheck = true;
            CheckForMatches();
            return true;
        }

        // left
        if (RowCheck(gem.x, gem.y, gem.x - 1) && RowCheck(gem.x - 1, gem.y, gem.x - 2))
        {
            isNeedToCheck = true;
            CheckForMatches();
            return true;
        }

        // right
        if (RowCheck(gem.x, gem.y, gem.x + 1) && RowCheck(gem.x + 1, gem.y, gem.x + 2))
        {
            isNeedToCheck = true;
            CheckForMatches();
            return true;
        }

        // center Column
        if (ColumnCheck(gem.x, gem.y, gem.y + 1) && ColumnCheck(gem.x, gem.y + 1, gem.y - 1))
        {
            isNeedToCheck = true;
            CheckForMatches();
            return true;
        }

        // center Row
        if (RowCheck(gem.x, gem.y, gem.x + 1) && RowCheck(gem.x + 1, gem.y, gem.x - 1))
        {
            isNeedToCheck = true;
            CheckForMatches();
            return true;
        }

        return false;
    }

    public void SetPosForGems()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (gems[x, y].x != x || gems[x, y].y != y)
                {
                    (gems[gems[x, y].x, gems[x, y].y], gems[x, y]) = (gems[x, y], gems[gems[x, y].x, gems[x, y].y]);
                    x = 0;
                    y = 0;
                }
            }
        }
    }

    public void ShowMiniField()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Debug.Log("{x:}" + "{y}" + " - X: " + x + "Y: " + y + "Figure " + gems[x, y].currentFigure + " " +
                          gems[x, y].x + " " + gems[x, y].y);
            }
        }
    }

    public void ScoreUpdate(int value)
    {
        score += value;
        scoreTxt.text = score.ToString();
    }
}