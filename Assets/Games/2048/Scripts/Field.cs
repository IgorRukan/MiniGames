using System.Linq;
using UnityEngine;

public class Field : Singleton<Field>
{
    public NumberBlock numberBlockPrefab;
    public GameObject filedBlockPrefab;
    public NumberBlock[,] field;
    public Transform[,] fieldPos;

    public Transform blocksParent;
    public Transform numberBlocksParent;

    public GameObject losePanel;
    public GameObject winPanel;
    
    public int width;
    public int height;

    public float blockSize;
    public float fieldBlockCoef;
    public float padding;

    public float slidingBlockTime;

    private void Start()
    {
        field = new NumberBlock[width, height];
        fieldPos = new Transform[width, height];
        NewGame();
    }

    private void NewGame()
    {
        //dlya testov poley
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SpawnFieldBlock(x,y);
            }
        }
        
        SpawnNumberBlock();
    }

    public void ResetField()
    {
        // obnulyau pole
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                field[x, y] = null;
            }
        }

        //udalyau vse bloki 
        foreach (Transform child in numberBlocksParent.transform)
        {
            Destroy(child.gameObject);
        }
        
        NewGame();
    }

    public void SpawnNumberBlock()
    {
        var randX = 0;
        var randY = 0;
        // po roflu + potom nado budet oblegchit pamyat
        var count = 0;

        do
        {
            //esli net pustih mest
            if (!CheckEmptySpace())
            {
                LoseGame();
                break;
            }
            
            count++;
            randX = Random.Range(0, width);
            randY = Random.Range(0, height);

            if (field[randX, randY] == null)
            {
                break;
            }
        } while (true);

        field[randX, randY] = SpawnBlock(randX, randY);
    }

    private void SpawnFieldBlock(int x,int y)
    {
        var block = Instantiate(filedBlockPrefab,blocksParent);

        float posX = x * (blockSize + padding) + CentreX() + blockSize / 2 + padding;
        float posY = y * (blockSize + padding) + CentreY() + blockSize / 2 + padding;
        block.transform.position = new Vector3(posX, posY, 0);

        block.transform.localScale = new Vector3(blockSize+fieldBlockCoef, blockSize+fieldBlockCoef, 1);

        block.transform.SetParent(blocksParent);

        fieldPos[x, y] = block.transform;
    }

    private NumberBlock SpawnBlock(int x,int y)
    {
        var block = Instantiate(numberBlockPrefab,numberBlocksParent);

        block.transform.localScale = new Vector3(blockSize, blockSize, 1);
        
        block.transform.position = fieldPos[x, y].position;

        block.Duration = slidingBlockTime;

        block.x = x;
        block.y = y;
        block.name = $"Block {x} {y}";

        return block;
    }

    private int CentreX()
    {
        return (int)(GetComponent<Transform>().position.x -
                     (width * (blockSize + padding) + padding)) / 2;
    }

    private int CentreY()
    {
        return (int)(GetComponent<Transform>().position.y -
                     (height * (blockSize + padding) + padding)) / 2;
    }

    private bool CheckEmptySpace()
    {
        return field.Cast<NumberBlock>().Any(x => x == null);
    }

    public void WinGame()
    {
        winPanel.SetActive(true);
    }
    private void LoseGame()
    {
        losePanel.SetActive(true);
    }
}
