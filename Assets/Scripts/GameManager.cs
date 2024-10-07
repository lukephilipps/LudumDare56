using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Timeline;
using Random = System.Random;

public enum AnimState
{
    IDLE,
    WALK,
    SIT,
    ANGY,
    HAPPY
}

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    
    [SerializeField] private Sprite[] items;
    [SerializeField] private Sprite[] emotions;
    [SerializeField] private RuntimeAnimatorController[] animations;
    
    [Header("Creature Destinations")]
    public Destination[] counter;
    public Destination[] table;
    [SerializeField] private Transform exit;
    public Transform overflowArea;

    private Random random;
    private float randXMin, randXMax, randYMin, randYMax;
    
    public DoorHingeAnimation door;
    private void Awake()
    {
        Singleton = this;

        random = new Random();
        
        Vector3 boundaries = overflowArea.GetComponent<BoxCollider>().size * 0.5f;
        Vector3 center = overflowArea.position;
        randXMin = center.x - boundaries.x;
        randXMax = center.x + boundaries.x;
        randYMax = center.z + boundaries.z;
        randYMin = center.z - boundaries.z;
    }

    public Vector2 randomOverflowLocation()
    {
        float x = (float)(randXMin + random.NextDouble() * Math.Abs(randXMax - randXMin));
        float y = (float)(randYMin + random.NextDouble() * Math.Abs(randYMax - randYMin));
        return new Vector2(x, y);
    }

    public Sprite GetItemSprite(int id)
    {
        return items[id];
    }

    public int ItemsLen()
    {
        return items.Length;
    }

    public bool LastSpotTaken()
    {
        return counter[counter.Length - 1].taken;
    }

    public Sprite GetEmotionSprite(Satisfaction emotion)
    {
        switch (emotion)
        {
            case Satisfaction.HAPPY:
                return emotions[0];
            case Satisfaction.NEUTRAL:
                return emotions[1];
            case Satisfaction.UNHAPPY:
                return emotions[2];
            case Satisfaction.ANGY:
                return emotions[3];
        }

        return null;
    }

    public RuntimeAnimatorController GetAnimation(AnimState animState)
    {
        switch (animState)
        {
            case AnimState.IDLE:
                return animations[0];
            case AnimState.WALK:
                return animations[1];
            case AnimState.SIT:
                return animations[2];
            case AnimState.ANGY:
                return animations[3];
            case AnimState.HAPPY:
                return animations[4];
        }

        return null;
    }
    
    public Vector3 WaitInLine(Customer c)
    {
        for (int i = 0; i < counter.Length; i++)
        {
            if (!counter[i].taken)
            {
                counter[i].taken = true;
                counter[i].customer = c;
                return counter[i].transform.position;
            }
        }
        
        return Vector3.zero;
    }

    public void MoveLine()
    {
        counter[0].taken = false; // front of line is false

        for (int i = 0; i < counter.Length - 1; i++)
        {
            Destination curr = counter[i];
            Destination prev = counter[i + 1];

            if (!curr.taken && prev.taken)
            {
                prev.customer.currentState = OrderState.WALKING;
                prev.customer.ChangeAnimation(AnimState.WALK);
                prev.customer.agent.SetDestination(curr.transform.position);
                
                curr.taken = true;
                curr.customer = prev.customer;
                
                prev.taken = false;
                prev.customer = null;
            }
        }
    }

    public Vector3 SitAtTable()
    {
        if (!table[table.Length - 1].taken) // there are empty chairs
        {
            for (int i = 0; i < table.Length; i++)
            {
                if (!table[i].taken) // returns first empty chair
                {
                    table[i].taken = true;
                    return table[i].transform.position;
                }
            }
        }

        return Vector3.zero; // goes to volume square
    }

    public Vector3 ExitLocation()
    {
        return exit.position;
    }
}

[System.Serializable]
public class Destination
{
    public Transform transform;
    public Customer customer;
    public bool taken = false;
}
