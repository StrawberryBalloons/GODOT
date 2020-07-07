using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garden : MonoBehaviour {
    public float boundsSize = 10;
    public int numDivisions = 10;
    List<Plant>[, ] cells;
    public int testX;
    public int testY;

    public Transform test;

    public bool gizmosOnlyWhenSelected;

    GameObject terrainObj;
    TerrainGenerator terrain;

    void Start () {
        cells = new List<Plant>[numDivisions, numDivisions];
        for (int i = 0; i < numDivisions; i++) {
            for (int j = 0; j < numDivisions; j++) {
                cells[i, j] = new List<Plant> ();
            }
        }
        terrainObj = FindObjectOfType<TerrainGenerator>().getInstance();
        terrain = terrainObj.GetComponent<TerrainGenerator>();
    }

    public void AddPlant (Plant plant) {//doesn't add plant to scene, adds to a data holder
        float posX = plant.transform.position.x;
        float posY = plant.transform.position.z;

        float cellSize = boundsSize / numDivisions;
        int cellX = Mathf.Clamp ((int) ((posX + boundsSize / 2) / cellSize), 0, numDivisions - 1);
        int cellY = Mathf.Clamp ((int) ((posY + boundsSize / 2) / cellSize), 0, numDivisions - 1);

        cells[cellX, cellY].Add (plant);
    }

    public Plant GetNearbyPlant (Vector3 pos) {
        float posX = pos.x;
        float posY = pos.z;
        float cellSize = boundsSize / numDivisions;
        int cellX = Mathf.Clamp ((int) ((posX + boundsSize / 2) / cellSize), 0, numDivisions - 1);
        int cellY = Mathf.Clamp ((int) ((posY + boundsSize / 2) / cellSize), 0, numDivisions - 1);
        var plants = cells[cellX, cellY];

        int numPlants = plants.Count;
        if (numPlants == 0) {
            return null;
        }
        return plants[Random.Range (0, numPlants)];
    }

    void DrawGizmos () {
        if (terrain == null)
        {
            terrainObj = FindObjectOfType<TerrainGenerator>().getInstance();
            terrain = terrainObj.GetComponent<TerrainGenerator>();
        }
        Gizmos.color = Color.yellow;
        Vector3 topLeft = Vector3.left * boundsSize / 2 + Vector3.forward * -boundsSize / 2 + Vector3.up * transform.position.y;
        int numSteps = 30;
        for (int x = 1; x < numDivisions; x++) {
            for (int step = 0; step < numSteps; step++) {
                float p1 = step / (float) numSteps;
                float p2 = (step + 1) / (float) numSteps;
                Vector3 startX = topLeft + Vector3.forward * x / (float) numDivisions * boundsSize + Vector3.right * boundsSize * p1;
                Vector3 endX = topLeft + Vector3.forward * x / (float) numDivisions * boundsSize + Vector3.right * boundsSize * p2;
                DrawProjectedLineGizmo (startX, endX);
                Vector3 startY = topLeft + Vector3.right * x / (float) numDivisions * boundsSize + Vector3.forward * boundsSize * p1;
                Vector3 endY = topLeft + Vector3.right * x / (float) numDivisions * boundsSize + Vector3.forward * boundsSize * p2;
                DrawProjectedLineGizmo (startY, endY);
            }
        }
    }

    void DrawProjectedLineGizmo (Vector3 a, Vector3 b) {
        float height = 0.1f;
        float h1 = terrain.GetHeight (new Vector2 (a.x, a.z)) + height;
        float h2 = terrain.GetHeight (new Vector2 (b.x, b.z)) + height;
        Gizmos.DrawLine (new Vector3 (a.x, h1, a.z), new Vector3 (b.x, h2, b.z));
    }
    // SHOULD STORE HEIGHT IN SCRIPT TO MAKE THEM EASIER TO ACCESS, RAYCAST ONCE AND THEN CALL LATER ON, UPDATE ON CHANGE
    void OnDrawGizmosSelected () {
        if (gizmosOnlyWhenSelected) {
            DrawGizmos ();
        }
    }

    void OnDrawGizmos () {
        if (!gizmosOnlyWhenSelected) {
            DrawGizmos ();
        }
    }
}