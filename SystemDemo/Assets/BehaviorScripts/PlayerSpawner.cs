using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerSpawner : MonoBehaviour {
    public SpawnData spawnData;
    public GameObject playerPrefab;
    public GameObject playerDummy;

    [Header("Runtime Data")]
    [SerializeField] private GameObject runtimePlayer;

    private void Awake()
    {
        runtimePlayer = InstantiatePlayer(spawnData.spawnLocation);
    }

    public GameObject GetRuntimePlayer() {
        return runtimePlayer;
    }

    [ContextMenu("Update Spawn Location")]
    public void UpdateSpawnLocationFromDummy() {
        UpdatePlayerLocationInSpawnData();
    }


    public Vector2 UpdatePlayerLocationInSpawnData() {
        if (playerDummy != null) {
            spawnData.spawnLocation = playerDummy.transform.position;
            return spawnData.spawnLocation;
        } else {
            Debug.LogError("PlayerDummy is not assigned.");
            return spawnData.spawnLocation; // Return the current spawn location in case of error
        }
    }

    private GameObject InstantiatePlayer(Vector2 spawnLocation)
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {  // Check if the player doesn't already exist
            GameObject player = Instantiate(playerPrefab, spawnLocation, Quaternion.identity);
            player.tag = "Player";
            return player;
        }
        else
        {
            GameObject.FindGameObjectWithTag("Player").transform.position = spawnLocation;
            return GameObject.FindGameObjectWithTag("Player");
        }
    }

    public void SaveScene() {
        ES3.Save("PlayerScene", SceneManager.GetActiveScene().name);
    }

    public void LoadScene()
    {
        LoadPlayerScene();
        LoadPlayerTransform();
    }

    public void ClearPlayerSceneSave() {
        if (ES3.KeyExists("PlayerScene")) {
            ES3.DeleteKey("PlayerScene");
        }
    }     

    private void LoadPlayerScene()
    {
        if (ES3.KeyExists("PlayerScene") && ES3.Load<string>("PlayerScene") == SceneManager.GetActiveScene().name)
        {  
            UpdateSpawnLocation();
        }
        else if (ES3.KeyExists("PlayerScene") && ES3.Load<string>("PlayerScene") != SceneManager.GetActiveScene().name)
        {
            UpdateSpawnLocation();
            SceneManager.LoadScene((string)ES3.Load("PlayerScene"));
        }
        else
        {
            Debug.LogError("No PlayerScene save to load!");
        }
    }

    private void UpdateSpawnLocation() {
        spawnData.spawnLocation = ES3.Load<Transform>("PlayerTransform").position;
    }

    private void LoadPlayerTransform() {
        if (ES3.KeyExists("PlayerTransform")) {
            InstantiatePlayer(ES3.Load<Transform>("PlayerTransform").position);
        } else {
            Debug.LogError("No PlayerTransform save to load!");
        }
    }
}

