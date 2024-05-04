using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour {
    public SpawnData spawnData;
    public GameObject playerPrefab;

    void Start() {
        if (GameObject.FindGameObjectWithTag("Player") == null) {  // Check if the player doesn't already exist
            GameObject player = Instantiate(playerPrefab, spawnData.spawnLocation, Quaternion.identity);
            player.tag = "Player";
        } else {
            GameObject.FindGameObjectWithTag("Player").transform.position = spawnData.spawnLocation;
        }
    }
}

