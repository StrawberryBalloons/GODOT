using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawHeightMap : MonoBehaviour
{
    public enum Draw
    {
        Heightmap, VoronoiMap
    };
    public int mapSize;
    public Draw draw;
    public MeshFilter filter;
    public MeshRenderer render;
    public Renderer textureRender;
    public void DrawTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void decideTexture()
    {
        float[] mapS = FindObjectOfType<HeightMapGenerator>().GenerateHeightMap(mapSize);
        float[,] map = floatTofloat(mapSize, mapS);

        if (draw == Draw.Heightmap)
        {
            DrawTexture(textureFromHeight(map));
        } else if (draw == Draw.VoronoiMap)
        {
            DrawTexture(FindObjectOfType<VoronoiMapGenerator>().GenerateVoronoiMap(mapSize));
        }
    }
    float[,] floatTofloat(int mapSize, float[] map)
    {
        float[,] maps = new float[mapSize, mapSize];
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                maps[x, y] = map[y * mapSize + x];
            }
        }
        return maps;
    }

    public static Texture2D textureFromHeight(float[,] hMap)
    { //decides the colours from the height map
        int width = hMap.GetLength(0);
        int height = hMap.GetLength(1);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, hMap[x, y]);
            }
        }
        return textureColourMap(colourMap, width, height);
    }
    public static Texture2D textureColourMap(Color[] cMap, int width, int height)
    {//applies the colours to the terrain
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(cMap);
        texture.Apply();
        return texture;
    }
}
