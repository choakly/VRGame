using UnityEngine;
using System.Collections;

public class ClipSpawner : MonoBehaviour
{
    public GameObject pistolClip;
    public Transform spawnArea;
    public int maxClips = 3;
    public float respawnDelay = 3f;
    public Vector3 spawnOffset = new Vector3(0.5f, 0, 0);
    private int currentClipCount = 0;
    private bool isRespawning = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnClip();
        SpawnClip();
        SpawnClip();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnClip()
    {
        if (currentClipCount < maxClips)
        {
            Vector3 spawnPosition = spawnArea.position + spawnOffset * currentClipCount;
            Instantiate(pistolClip, spawnPosition, Quaternion.identity);
            currentClipCount++;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Magazine"))
        {
            currentClipCount--;
            if (!isRespawning)
            {
                StartCoroutine(RespawnClip());
            }
        }
    }
    private IEnumerator RespawnClip()
    {
        isRespawning = true;
        yield return new WaitForSeconds(respawnDelay);
        SpawnClip();
        isRespawning = false;
    }
}
