using System.Collections;
using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    public Customer[] customers; // customer prefabs

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnCreature();
        }
    }

    private void SpawnCreature()
    {
        int rnd = Random.Range(0, customers.Length);

        Instantiate(customers[rnd], transform.position, transform.rotation);
    }

}
