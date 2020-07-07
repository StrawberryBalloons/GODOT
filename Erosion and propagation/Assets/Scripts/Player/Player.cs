using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [Header ("Plant Settings")]
    public bool infiniteSeedMode;
    public bool debug_autoPlantSeeds;

    public Seed[] seeds;
    public GameObject plantPrefab;
    public Transform plantHandPos;


    TerrainGenerator terrain;
    FPSController controller;

    Transform cam;

    int[] numSeedsByType;

    int activePlantIndex = 0;

    KeyCode[] numberKeys = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9 };
    public event System.Action<string, int> onPlantTypeSwitched;

    void Start () {
        terrain = FindObjectOfType<TerrainGenerator> ();
        cam = Camera.main.transform;
        controller = FindObjectOfType<FPSController> ();

        if (!Application.isEditor) {
            //infiniteSeedMode = false;
        }

        numSeedsByType = new int[seeds.Length];

        // TEST:
        if (debug_autoPlantSeeds) {
            int n = 0;
            for (int i = 0; i < 100; i++) {
                if (n > 30) {
                    break;
                }
                Vector3 spawnPos = Random.insideUnitSphere * 20;
                float terrainHeight = terrain.GetHeight (new Vector2 (spawnPos.x, spawnPos.z));
                if (spawnPos.y > terrainHeight) {
                    n++;
                    Instantiate (seeds[0], spawnPos, plantHandPos.rotation);
                }
            }
        }
    }

    void Update () {

        HandlePlantInput ();

    }

    void HandlePlantInput () {
        int plantIndexOld = activePlantIndex;
        // Switch plant type:
        for (int i = 0; i < seeds.Length; i++) {
            if (Input.GetKeyDown (numberKeys[i])) {
                if (numSeedsByType[i] > 0 || infiniteSeedMode) {
                    activePlantIndex = (activePlantIndex == i) ? -1 : i; // if already in this slot exit plant mode, otherwise switch
                    if (activePlantIndex != -1 && onPlantTypeSwitched != null) {
                        onPlantTypeSwitched (seeds[i].plantPrefab.plantName, numSeedsByType[i]);
                    }
                }
                break;
            }
        }

        bool plantModeBecameActiveThisFrame = activePlantIndex != -1 && plantIndexOld == -1;
        bool plantModeBecameInactiveThisFrame = activePlantIndex == -1 && plantIndexOld != -1;

        if (activePlantIndex != -1 && Input.GetMouseButtonDown (0)) {
            Vector3 spawnPos = plantHandPos.position;
            float terrainHeight = terrain.GetHeight (new Vector2 (spawnPos.x, spawnPos.z));
            if (spawnPos.y > terrainHeight) {
                var seed = Instantiate (seeds[activePlantIndex], spawnPos, plantHandPos.rotation);
                seed.Throw (controller.velocity.magnitude);
                if (!infiniteSeedMode) {
                    numSeedsByType[activePlantIndex]--;
                    if (numSeedsByType[activePlantIndex] == 0) {
                        activePlantIndex = -1;
                    }
                }
            }
        }
    }

}