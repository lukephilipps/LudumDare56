using System.Collections;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    public Customer[] customers; // customer prefabs

    public float elapsedGameTime;
    
    [Range(1f, 15f)]
    public float spawnFrequency;

    void Start()
    {
        SpawnCreature();
    }

    void Update()
    {
        elapsedGameTime += Time.deltaTime;

        if (elapsedGameTime >= spawnFrequency && !GameManager.Singleton.LastSpotTaken())
        {
            SpawnCreature();
            elapsedGameTime = 0f;
            if (spawnFrequency > 3f) spawnFrequency -= 0.05f;
        }
    }

    private void SpawnCreature()
    {
        
        int rnd = Random.Range(0, customers.Length);

        Instantiate(customers[rnd], transform.position, transform.rotation);
    }

}
