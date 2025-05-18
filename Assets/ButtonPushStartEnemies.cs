using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.DedicatedServer;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonPushStartEnemies : MonoBehaviour
{
    public EnemySpawner enemySpawner;
    public GameObject buttonStand;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<XRSimpleInteractable>().selectEntered.AddListener(x => ToggleEnemySpawn());
        
    }

    public void ToggleEnemySpawn()
    {
        enemySpawner.enabled = true;
        enemySpawner.ChangeState(SpawnerState.WAVE);
        buttonStand.SetActive(false);
    }

    // Update is called once per frame
    
}
