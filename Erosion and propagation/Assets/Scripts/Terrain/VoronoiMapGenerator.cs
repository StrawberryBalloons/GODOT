using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class VoronoiMapGenerator : MonoBehaviour
{
    public int numRegions;
    public Texture2D GenerateVoronoiMap(int mapSize)
    {
        return GenerateVoronoiMapCPU(mapSize);
    }

    private Texture2D GenerateVoronoiMapCPU(int mapSize)
    {
        Vector2Int[] centroids = new Vector2Int[numRegions];
        Color[] regions = new Color[numRegions];
        randPoints(mapSize, centroids, regions);
        Color[] pixelColors = new Color[mapSize * mapSize];
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                int index = x * mapSize + y;
                pixelColors[index] = regions[GetClosestCentroidIndex(new Vector2Int(x, y), centroids)];
            }
        }
        return GetImageFromColorArray(pixelColors, mapSize);
    }
    private void randPoints(int mapSize, Vector2Int[] centroids, Color[] regions)
    {
        for (int i = 0; i < numRegions; i++)
        {
            centroids[i] = new Vector2Int(Random.Range(0, mapSize), Random.Range(0, mapSize));
            regions[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }
        debugCentroidColours(regions);
    }

    private void debugCentroidColours(Color[] regions)
    {
        for (int i = 0; i < regions.Length; i++)
        {
            for (int j = 0; j< regions.Length; j++)
            {
                if (regions[i] == regions[j] && i != j)
                {
                    Debug.Log("Colour Collision at region: " + i + " with: " + j);
                }
            }
        }
    }

    int GetClosestCentroidIndex(Vector2Int pixelPos, Vector2Int[] centroids)
    {
        float smallestDst = float.MaxValue;
        int index = 0;
        for (int i = 0; i < centroids.Length; i++)
        {
            if (Vector2.Distance(pixelPos, centroids[i]) < smallestDst)
            {
                smallestDst = Vector2.Distance(pixelPos, centroids[i]);
                index = i;
            }
        }
        return index;
    }
    Texture2D GetImageFromColorArray(Color[] pixelColors, int mapSize)
    {
        Texture2D tex = new Texture2D(mapSize, mapSize);
        tex.filterMode = FilterMode.Point;
        tex.SetPixels(pixelColors);
        tex.Apply();
        return tex;
    }
}
