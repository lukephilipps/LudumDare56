using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public enum AnimState
{
    IDLE,
    WALK
}

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    
    [SerializeField] private Sprite[] items;
    [SerializeField] private Sprite[] emotions;
    [SerializeField] private RuntimeAnimatorController[] animations;
    
    public Destination[] counter;
    public Destination[] table;
    [SerializeField] private Transform exit;
    
    public DoorHingeAnimation door;

    private void Awake()
    {
        Singleton = this;
    }

    public Sprite GetItemSprite(int id)
    {
        return items[id];
    }

    public int ItemsLen()
    {
        return items.Length;
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
        counter[0].taken = false; // front of line is false
        MoveLine();
        
        for (int i = 0; i < table.Length; i++)
        {
            if (!table[i].taken)
            {
                table[i].taken = true;
                return table[i].transform.position;
            }
        }
        
        return Vector3.zero;
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
