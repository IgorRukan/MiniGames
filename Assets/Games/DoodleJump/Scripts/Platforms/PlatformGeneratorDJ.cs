using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlatformGeneratorDJ : MonoBehaviour
{
    public Transform platformPrefab;
    public float leftScreenSide;
    public float rightLeftSide;

    public float distance;
    public List<PlatformDJ> platformsPrefabs = new List<PlatformDJ>();
    public List<PlatformDJ> platforms = new List<PlatformDJ>();

    public float generateDistance; // how high did the platforms spawn

    private void Start()
    {
        GeneratePlatforms();
    }

    public enum PlatformTypes
    {
        Default,
        Broken
    }

    public void ResetPlatforms()
    {
        generateDistance = 0;
    }

    public void GeneratePlatforms()
    {
        float totalDistance = platformPrefab.transform.position.y;
        do
        {
            totalDistance += distance;

            int numOfPlatforms = Random.Range(1, 3);
            for (int i = 0; i < numOfPlatforms; i++)
            {
                var platform = Instantiate(GetRandomPlatform(), transform, true);
                platform.transform.position = GetRandomPosition(totalDistance);

                platforms.Add(platform);
                
            }
            
        } while (generateDistance > totalDistance);
    }

    private PlatformDJ GetRandomPlatform()
    {
        int index = Random.Range(0, platformsPrefabs.Count);
        return platformsPrefabs[index];
    }

    private Vector2 GetRandomPosition(float totalDistance)
    {
        float randomX = Random.Range(leftScreenSide, rightLeftSide);
        float y = totalDistance;
        Vector2 randomPosition = new Vector2(randomX, y);
        return randomPosition;
    }
}