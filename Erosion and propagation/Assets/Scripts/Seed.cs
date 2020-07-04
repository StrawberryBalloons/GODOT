using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour {

    public Vector2 throwForceMinMax = new Vector2 (4.5f, 5.5f);
    public const float gravity = 10;

    public Plant plantPrefab;
    Vector3 velocity;

    TerrainGenerator terrain;

    void Start () {
        terrain = FindObjectOfType<TerrainGenerator> ();

    }

    public void Throw (float inheritedForce) {
        velocity = transform.forward * (Random.Range (throwForceMinMax.x, throwForceMinMax.y) + inheritedForce);
    }

    void Update () {
        velocity -= Vector3.up * gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        float terrainHeight = terrain.GetHeight (new Vector2 (transform.position.x, transform.position.z));
        if (transform.position.y < terrainHeight) {
            Vector3 terrainUp = terrain.Raycast (new Vector2 (transform.position.x, transform.position.z)).normal;
            float angle = Random.value * Mathf.PI * 2;
            var plantRot = Quaternion.LookRotation (new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos (angle)), terrainUp);
            Instantiate (plantPrefab, new Vector3 (transform.position.x, terrainHeight, transform.position.z), plantRot);
            Destroy (gameObject);
        }
    }
}