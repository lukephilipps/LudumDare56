using System;
using System.Collections.Generic;
using UnityEngine;

public enum AnimState
{
    IDLE,
    WALK
}

public enum Destination
{
    COUNTER,
    TABLE
}

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    
    [SerializeField] private Sprite[] items;
    [SerializeField] private Sprite[] emotions;
    [SerializeField] private Transform[] destinations;
    [SerializeField] private RuntimeAnimatorController[] animations;

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
                break;
            case Satisfaction.NEUTRAL:
                return emotions[1];
                break;
            case Satisfaction.UNHAPPY:
                return emotions[2];
                break;
            case Satisfaction.ANGY:
                return emotions[3];
                break;
        }

        return null;
    }

    public RuntimeAnimatorController GetAnimation(AnimState animState)
    {
        switch (animState)
        {
            case AnimState.IDLE:
                return animations[0];
                break;
            case AnimState.WALK:
                return animations[1];
                break;
        }

        return null;
    }

    public Vector3 GetDestination(Destination destination)
    {
        switch (destination)
        {
            case Destination.COUNTER:
                return destinations[0].position;
                break;
            case Destination.TABLE:
                return destinations[1].position;
                break;
        }
        
        return Vector3.zero;
    }
}
