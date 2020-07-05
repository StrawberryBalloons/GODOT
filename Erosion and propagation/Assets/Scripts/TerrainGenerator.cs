using System;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

    public bool printTimers;

    [Header ("Mesh Settings")]
    public int mapSize = 255;
    public float scale = 20;
    public float elevationScale = 10;
    public Material material;
    public GameObject instance;

    [Header ("Erosion Settings")]
    public ComputeShader erosion;
    public int numErosionIterations = 50000;
    public int erosionBrushRadius = 3;

    public int maxLifetime = 30;
    public float sedimentCapacityFactor = 3;
    public float minSedimentCapacity = .01f;
    public float depositSpeed = 0.3f;
    public float erodeSpeed = 0.3f;

    public float evaporateSpeed = .01f;
    public float gravity = 4;
    public float startSpeed = 1;
    public float startWater = 1;
    [Range (0, 1)]
    public float inertia = 0.3f;

    // Internal
    float[] map;
    Mesh mesh;
    int mapSizeWithBorder;
    

   [HideInInspector]
    float size = 40;
    float[,] maps;
    Vector3[,] normalsMap;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    [SerializeField, HideInInspector]
    GameObject meshHolder;

    public GameObject getInstance()
    {
        return instance;
    }
    public void GenerateHeightMap () {
        mapSizeWithBorder = mapSize + erosionBrushRadius * 2;
        map = FindObjectOfType<HeightMapGenerator>().GenerateHeightMap(mapSizeWithBorder);
    }
    private void Generate()
    {
        maps = new float[mapSize, mapSize];
        GenerateHeightMap();
        ContructMesh();
        //needsUpdate = false;
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                maps[x, y] = map[y * mapSize + x];
            }
        }

    }
 
    public void Erode () {
        int numThreads = numErosionIterations / 1024;

        // Create brush
        List<int> brushIndexOffsets = new List<int> ();
        List<float> brushWeights = new List<float> ();

        float weightSum = 0;
        for (int brushY = -erosionBrushRadius; brushY <= erosionBrushRadius; brushY++) {
            for (int brushX = -erosionBrushRadius; brushX <= erosionBrushRadius; brushX++) {
                float sqrDst = brushX * brushX + brushY * brushY;
                if (sqrDst < erosionBrushRadius * erosionBrushRadius) {
                    brushIndexOffsets.Add (brushY * mapSize + brushX);
                    float brushWeight = 1 - Mathf.Sqrt (sqrDst) / erosionBrushRadius;
                    weightSum += brushWeight;
                    brushWeights.Add (brushWeight);
                }
            }
        }
        for (int i = 0; i < brushWeights.Count; i++) {
            brushWeights[i] /= weightSum;
        }

        // Send brush data to compute shader
        ComputeBuffer brushIndexBuffer = new ComputeBuffer (brushIndexOffsets.Count, sizeof (int));
        ComputeBuffer brushWeightBuffer = new ComputeBuffer (brushWeights.Count, sizeof (int));
        brushIndexBuffer.SetData (brushIndexOffsets);
        brushWeightBuffer.SetData (brushWeights);
        erosion.SetBuffer (0, "brushIndices", brushIndexBuffer);
        erosion.SetBuffer (0, "brushWeights", brushWeightBuffer);

        // Generate random indices for droplet placement
        int[] randomIndices = new int[numErosionIterations];
        for (int i = 0; i < numErosionIterations; i++) {
            int randomX = UnityEngine.Random.Range (erosionBrushRadius, mapSize + erosionBrushRadius);
            int randomY = UnityEngine.Random.Range (erosionBrushRadius, mapSize + erosionBrushRadius);
            randomIndices[i] = randomY * mapSize + randomX;
        }

        // Send random indices to compute shader
        ComputeBuffer randomIndexBuffer = new ComputeBuffer (randomIndices.Length, sizeof (int));
        randomIndexBuffer.SetData (randomIndices);
        erosion.SetBuffer (0, "randomIndices", randomIndexBuffer);

        // Heightmap buffer
        ComputeBuffer mapBuffer = new ComputeBuffer (map.Length, sizeof (float));
        mapBuffer.SetData (map);
        erosion.SetBuffer (0, "map", mapBuffer);

        // Settings
        erosion.SetInt ("borderSize", erosionBrushRadius);
        erosion.SetInt ("mapSize", mapSizeWithBorder);
        erosion.SetInt ("brushLength", brushIndexOffsets.Count);
        erosion.SetInt ("maxLifetime", maxLifetime);
        erosion.SetFloat ("inertia", inertia);
        erosion.SetFloat ("sedimentCapacityFactor", sedimentCapacityFactor);
        erosion.SetFloat ("minSedimentCapacity", minSedimentCapacity);
        erosion.SetFloat ("depositSpeed", depositSpeed);
        erosion.SetFloat ("erodeSpeed", erodeSpeed);
        erosion.SetFloat ("evaporateSpeed", evaporateSpeed);
        erosion.SetFloat ("gravity", gravity);
        erosion.SetFloat ("startSpeed", startSpeed);
        erosion.SetFloat ("startWater", startWater);

        // Run compute shader
        erosion.Dispatch (0, numThreads, 1, 1);
        mapBuffer.GetData (map);

        // Release buffers
        mapBuffer.Release ();
        randomIndexBuffer.Release ();
        brushIndexBuffer.Release ();
        brushWeightBuffer.Release ();
    }

    public void ContructMesh () {
        Vector3[] verts = new Vector3[mapSize * mapSize];
        int[] triangles = new int[(mapSize - 1) * (mapSize - 1) * 6];
        int t = 0;

        for (int a = 0; a < mapSize * mapSize; a++) {
            int x = a % mapSize;
            int y = a / mapSize;
            int borderedMapIndex = (y + erosionBrushRadius) * mapSizeWithBorder + x + erosionBrushRadius;
            int meshMapIndex = y * mapSize + x;

            Vector2 percent = new Vector2 (x / (mapSize - 1f), y / (mapSize - 1f));
            Vector3 pos = new Vector3 (percent.x * 2 - 1, 0, percent.y * 2 - 1) * scale;

            float normalizedHeight = map[borderedMapIndex];
            pos += Vector3.up * normalizedHeight * elevationScale;
            verts[meshMapIndex] = pos;

            // Construct triangles
            if (x != mapSize - 1 && y != mapSize - 1) {
                t = (y * (mapSize - 1) + x) * 3 * 2;

                triangles[t + 0] = meshMapIndex + mapSize;
                triangles[t + 1] = meshMapIndex + mapSize + 1;
                triangles[t + 2] = meshMapIndex;

                triangles[t + 3] = meshMapIndex + mapSize + 1;
                triangles[t + 4] = meshMapIndex + 1;
                triangles[t + 5] = meshMapIndex;
                t += 6;
            }
        }

        if (mesh == null) {
            mesh = new Mesh ();
        } else {
            mesh.Clear ();
        }
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.RecalculateNormals ();

        Vector3[] normals = mesh.normals;
        normalsMap = new Vector3[mapSize, mapSize];
        int i = 0;
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                normalsMap[x, y] = normals[i];
                i++;
            }
        }
        AssignMeshComponents ();
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = material;

        material.SetFloat ("_MaxHeight", elevationScale);
    }

    void AssignMeshComponents () {
        // Find/creator mesh holder object in children
        string meshHolderName = "Mesh Holder";
        Transform meshHolder = transform.Find (meshHolderName);
        if (meshHolder == null) {
            meshHolder = new GameObject (meshHolderName).transform;
            meshHolder.transform.parent = transform;
            meshHolder.transform.localPosition = Vector3.zero;
            meshHolder.transform.localRotation = Quaternion.identity;
        }
        if (meshHolder.gameObject.GetComponent<MeshCollider>())
        {
            DestroyImmediate(meshHolder.GetComponent<MeshCollider>());
        }
        // Ensure mesh renderer and filter components are assigned
        if (!meshHolder.gameObject.GetComponent<MeshFilter> ()) {
            meshHolder.gameObject.AddComponent<MeshFilter> ();
        }
        if (!meshHolder.GetComponent<MeshRenderer> ()) {
            meshHolder.gameObject.AddComponent<MeshRenderer> ();
        }
        if (!meshHolder.gameObject.GetComponent<MeshCollider>())
        {
            meshHolder.gameObject.AddComponent<MeshCollider>();
        }
        meshRenderer = meshHolder.GetComponent<MeshRenderer> ();
        meshFilter = meshHolder.GetComponent<MeshFilter> ();
        //meshCollider = meshHolder.GetComponent<MeshCollider>();
        meshHolder.GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;
    }
    public Coord CoordFromPoint(Vector2 point)//world to local
    {//not sure if they want local between 0 and 40 or 1500 e.g inputting -19.99 gets 0.375
        //float x = (point.x / size + 0.5f) * (mapSize - 1f);
        //float y = (point.y / size + 0.5f) * (mapSize - 1f);
        float x = (mapSize / size) * (point.x + size / 2);
        float y = (mapSize / size) * (point.y + size / 2);
        return new Coord((int)(x), (int)(y));
    }
    /* point + (size/2) gets a value between 0 and size
     * mapsize / size = 37.5 -> 37.5 * 40 = 1500
     * 37.5 * (point + size/2) will give a value between 0 and 1500
     * This means the full equation is:
     * (mapSize/size) * (point + size/2)
    */
    public Vector2 PosFromCoord(Coord coord)
    {// so, this should be between map[x,y] (0-1500) to the world coordinates, and this should work inputting 0.375 gives -19.99
        //float x = (coord.x / (mapSize - 1f) - 0.5f) * size;
        //float y = (coord.y / (mapSize - 1f) - 0.5f) * size;
        float x = ((size * 1) / mapSize * coord.x) - (size/2);
        float y = ((size * 1) / mapSize * coord.y) - (size/2);
        return new Vector2(x, y);
    }
    /*
* The distance between corners is 40
* the map size is X, currently set at 1500
* that means there are 1500 local points over the 40 world points
* (size * scale / mapSize * coord.?) - (size/2)
* the size to mapsize gets modifier, multiply to coord to get the value of coordinate to world position
* minus it by half the size because it sets on the middle of 0,0
*/
    public float GetHeight(Vector2 point)
    {

        /*Coord coordNW = CoordFromPoint(point);
        coordNW.x = Mathf.Clamp(coordNW.x, 0, mapSize - 2);
        coordNW.y = Mathf.Clamp(coordNW.y, 0, mapSize - 2);

        Coord coordNE = coordNW + new Coord(1, 0);
        Coord coordSW = coordNW + new Coord(0, 1);
        Coord coordSE = coordSW + new Coord(1, 0);

        Vector2 posNW = PosFromCoord(coordNW);
        Vector2 posNE = PosFromCoord(coordNE);
        Vector2 posSW = PosFromCoord(coordSW);
        Vector2 posSE = PosFromCoord(coordSE);
        float heightNW = maps[coordNW.x, coordNW.y] * 10;
        float heightNE = maps[coordNE.x, coordNE.y] * 10;
        float heightSW = maps[coordSW.x, coordSW.y] * 10;
        float heightSE = maps[coordSE.x, coordSE.y] * 10;

        // Calculate offset inside the cell (0,0) = at NW node, (1,1) = at SE node
        float x = Mathf.InverseLerp(posNW.x, posNE.x, point.x);
        float y = Mathf.InverseLerp(posNW.y, posSW.y, point.y);
        float height = heightNW * (1 - x) * (1 - y) + heightNE * x * (1 - y) + heightSW * (1 - x) * y + heightSE * x * y;*/
        float height = heightCast(point);
        Debug.Log(height + " The Height");
        return height;
    }

    private float heightCast(Vector2 point)
    {
        int layerMask = 1 << 8;
        RaycastHit hit;
        Vector3 abovePoint = new Vector3(point.x, 10, point.y);
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(abovePoint, Vector3.down, out hit, 10, layerMask))
        {
            //Debug.DrawRay(abovePoint, Vector3.down * hit.distance, Color.green);
            return hit.point.y;
        }
        else
        {
            //Debug.DrawRay(abovePoint, Vector3.down * 10, Color.red);
            return 0f;
        }
    }

    public TerrainPointInfo Raycast(Vector2 point)
    {

        Coord coordNW = CoordFromPoint(point);
        coordNW.x = Mathf.Clamp(coordNW.x, 0, mapSize - 2);
        coordNW.y = Mathf.Clamp(coordNW.y, 0, mapSize - 2);

        Coord coordNE = coordNW + new Coord(1, 0);
        Coord coordSW = coordNW + new Coord(0, 1);
        Coord coordSE = coordSW + new Coord(1, 0);

        Vector2 posNW = PosFromCoord(coordNW);
        Vector2 posNE = PosFromCoord(coordNE);
        Vector2 posSW = PosFromCoord(coordSW);
        Vector2 posSE = PosFromCoord(coordSE);

        //float heightNW = map[coordNW.x * coordNW.y];
        float heightNW = heightCast(new Vector2(coordNW.x, coordNW.y));
        float heightNE = heightCast(new Vector2(coordNE.x, coordNE.y));
        float heightSW = heightCast(new Vector2(coordSW.x, coordSW.y));
        float heightSE = heightCast(new Vector2(coordSE.x, coordSE.y));

        Debug.DrawRay (new Vector3 (posNW.x, heightNW, posNW.y), Vector3.up, Color.red);
        Debug.DrawRay (new Vector3 (posNE.x, heightNE, posNE.y), Vector3.up);
        Debug.DrawRay (new Vector3 (posSW.x, heightSW, posSW.y), Vector3.up);
        Debug.DrawRay (new Vector3 (posSE.x, heightSE, posSE.y), Vector3.up);


        /*Vector3 normalNW = normalsMap[coordNW.x, coordNW.y];
        Vector3 normalNE = normalsMap[coordNE.x, coordNE.y];
        Vector3 normalSW = normalsMap[coordSW.x, coordSW.y];
        Vector3 normalSE = normalsMap[coordSE.x, coordSE.y];*/

        // Calculate offset inside the cell (0,0) = at NW node, (1,1) = at SE node
        float x = Mathf.InverseLerp(posNW.x, posNE.x, point.x);
        float y = Mathf.InverseLerp(posNW.y, posSW.y, point.y);
        float height = heightNW * (1 - x) * (1 - y) + heightNE * x * (1 - y) + heightSW * (1 - x) * y + heightSE * x * y;
        //Vector3 normal = normalNW * (1 - x) * (1 - y) + normalNE * x * (1 - y) + normalSW * (1 - x) * y + normalSE * x * y;
        return new TerrainPointInfo(height, new Vector3(1,1,1));
    }



    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Coord operator +(Coord a, Coord b)
        {
            return new Coord(a.x + b.x, a.y + b.y);
        }

        public static Coord operator -(Coord a, Coord b)
        {
            return new Coord(a.x - b.x, a.y - b.y);
        }

    }
    public struct TerrainPointInfo
    {
        public readonly float height;
        public readonly Vector3 normal;

        public TerrainPointInfo(float height, Vector3 normal)
        {
            this.height = height;
            this.normal = normal;
        }
    }
}