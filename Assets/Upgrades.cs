using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



public class Upgrades : MonoBehaviour
{
    public GameObject selection1;
    public GameObject selection2;
    public GameObject selection3;
    
    public EnemySpawner enemies;
    public Transform head;
    public GameObject akPrefab;
    private Dictionary<string, UnityAction> upgradeActions;
    public float spawnDistance = 2;
    public float gunSpawnDistance = 0.5f;
    public float damageMultiplier = 1.0f;
    private KeyValuePair<string, UnityAction>[] selectedUpgrades = new KeyValuePair<string, UnityAction>[3];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        upgradeActions = new Dictionary<string, UnityAction>
        {
            { "Damage Upgrade", damageUpgrade },
            { "Speed Boost", speedUpgrade },
            { "AK-47 Weapon", () => newGun(akPrefab) }
            
        };
    }

    // Update is called once per frame
     public void Update()
    {
        
        
        selection1.transform.LookAt(new Vector3 (head.position.x, selection1.transform.position.y, head.position.z));
        selection1.transform.forward *= 1; 
        selection2.transform.LookAt(new Vector3 (head.position.x, selection2.transform.position.y, head.position.z));
        selection2.transform.forward *= 1;
        selection3.transform.LookAt(new Vector3 (head.position.x, selection3.transform.position.y, head.position.z));
        selection3.transform.forward *= 1;
        selection1.transform.Rotate(0, 180, 0);
        selection2.transform.Rotate(0, 180, 0);
        selection3.transform.Rotate(0, 180, 0);
    }
    
    public void UpgradeMenu()
    {
        Vector3 forward = new Vector3(head.forward.x,0,head.forward.z).normalized;
        Vector3 right = new Vector3(head.right.x,0,head.right.z).normalized;
        enemies.enabled = false;
        selection1.SetActive(!selection1.activeSelf);
        selection2.SetActive(!selection2.activeSelf);
        selection3.SetActive(!selection3.activeSelf);
        selection1.transform.position = head.position + forward * spawnDistance + right * -1.5f;
        selection2.transform.position = head.position + forward * spawnDistance;
        selection3.transform.position = head.position + forward * spawnDistance + right * 1.5f;
        
        
        var keys = new List<string>(upgradeActions.Keys);
        List<int> used = new();
        for (int i = 0; i < 3; i++)
        {
            int index;
            do {
                index = Random.Range(0, keys.Count);
            } while (used.Contains(index));
            used.Add(index);
            string upgradeName = keys[index];
            UnityAction action = upgradeActions[upgradeName];
            selectedUpgrades[i] = new KeyValuePair<string, UnityAction>(upgradeName, action);
        }

        AssignUpgrade(selection1, selectedUpgrades[0]);
        AssignUpgrade(selection2, selectedUpgrades[1]);
        AssignUpgrade(selection3, selectedUpgrades[2]);
        
    }
    void AssignUpgrade(GameObject selection, KeyValuePair<string, UnityAction> upgrade)
    {
        var button = selection.GetComponentInChildren<Button>();
        var text = button.GetComponentInChildren<Text>();
        text.text = upgrade.Key;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            upgrade.Value.Invoke();
            CloseUpgradeMenu();
        });
    }
    void CloseUpgradeMenu()
    {
        selection1.SetActive(!selection1.activeSelf);
        selection2.SetActive(!selection2.activeSelf);
        selection3.SetActive(!selection3.activeSelf);

        enemies.ChangeState(SpawnerState.PRE_WAVE);
    }
    
    void damageUpgrade()
    {
        damageMultiplier += 0.1f;
    }
    void speedUpgrade()
    {
        Debug.Log("Speed upgrade");
    }
    void newGun(GameObject gunPrefab)
    {
        Vector3 spawnPos = head.position + head.forward * gunSpawnDistance;
        Quaternion spawnRot = Quaternion.LookRotation(new Vector3(head.forward.x, 0, head.forward.z));
        GameObject gun = Instantiate(gunPrefab, spawnPos, spawnRot);

        var fireScript = gun.GetComponent<FireBulletOnActivate>();
        if (fireScript != null)
        {
            fireScript.upgrades = this;
        }
        Debug.Log($"Spawned gun: {gunPrefab.name}");
    }
}
