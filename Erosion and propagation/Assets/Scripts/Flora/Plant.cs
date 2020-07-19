using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour {

    public enum tagType {Organic, Flora}
    public enum PlantType { Daffodil, BleedingHeart, Foxglove, LilyOfTheValley }
    public PlantType plantType;


    public string plantName;
    public MeshRenderer stem;
    public Seed seeds;

    public float growTime = 2;
    public float flowerGrowTime = 1;

    [Header ("Baked Data")]
    public Transform[] flowers;
    public float[] flowerStartGrowTime;

    [Header ("Info")]

    Material stemMat;
    float growthPercent;
    float seedGrowthPercent;

    bool growing;

    TerrainGenerator terrain;

    void Start () {
        stemMat = stem.material;

        terrain = FindObjectOfType<TerrainGenerator>();

        growing = true;

        stemMat.SetFloat ("_GrowthPercent", 0);
        transform.localScale = Vector3.one * Random.Range (.9f, 1.1f);

        for (int i = 0; i < flowers.Length; i++) {
            flowers[i].localScale = Vector3.zero;
        }
    }

    void Update () {
        if (growing) {
            growthPercent += Time.deltaTime / growTime;
            stemMat.SetFloat ("_GrowthPercent", growthPercent);

            // stop growing once growthPercent exceeds 1 (unless flowers are still growing -- handled in loop)
            growing = growthPercent < 1;
            for (int i = 0; i < flowers.Length; i++) {
                if (growthPercent > flowerStartGrowTime[i]) {
                    flowers[i].localScale = Vector3.MoveTowards (flowers[i].localScale, Vector3.one, Time.deltaTime / flowerGrowTime);
                    if (flowers[i].localScale != Vector3.one) {
                        growing = true;
                    }
                }
            }



            // Finished growing
            if (!growing) {
                
                FindObjectOfType<Garden> ().AddPlant (this);
            }
        } else if (!growing){
            seedGrowthPercent += Time.deltaTime / (growTime * (Random.Range(4f, 5f)));
            if (seedGrowthPercent > 0.99)
            {
                Vector3 spawnPos = transform.position;
                spawnPos.y += 0.5f;
                
                float terrainHeight = terrain.GetHeight(new Vector2(spawnPos.x, spawnPos.z));
                //Debug.Log("plant transform " + transform.position);
                //Debug.Log("seed transform " + spawnPos);
                if (spawnPos.y > terrainHeight)
                {
                    var seed = Instantiate(seeds, spawnPos, Random.rotation);
                    seed.Throw(-1);
                }
                seedGrowthPercent = 0;
            }
        }
    }

    [ContextMenu ("Bake Flower Calculation")]
    public void PrecalculateFlowerGrowTimes () {
        // Get flower/leaf objects that should be 'grown' (scaled) as stem appears
        flowers = new Transform[stem.transform.childCount];
        for (int i = 0; i < flowers.Length; i++) {
            flowers[i] = stem.transform.GetChild (i);
            flowers[i].localScale = Vector3.one;
        }

        // Figure out where along stem the leaves/flowers appear
        flowerStartGrowTime = new float[flowers.Length];
        Mesh stemMesh = stem.gameObject.GetComponent<MeshFilter> ().sharedMesh;
        for (int flowerIndex = 0; flowerIndex < flowers.Length; flowerIndex++) {
            Vector3 flowerPos = flowers[flowerIndex].position;
            float closestDst = float.MaxValue;
            int closestVertIndex = 0;
            for (int vertIndex = 0; vertIndex < stemMesh.vertices.Length; vertIndex++) {
                Vector3 vertWorld = stem.transform.TransformPoint (stemMesh.vertices[vertIndex]);
                float sqrDst = (vertWorld - flowerPos).sqrMagnitude;
                if (sqrDst < closestDst) {
                    closestDst = sqrDst;
                    closestVertIndex = vertIndex;
                }
            }
            float startGrowingTime = stemMesh.uv[closestVertIndex].y;
            flowerStartGrowTime[flowerIndex] = startGrowingTime;
        }

    }

}