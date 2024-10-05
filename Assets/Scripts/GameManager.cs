using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    
    public Sprite[] items;
    public Sprite[] emotions;

    private void Start()
    {
        Singleton = this;
    }

    public Sprite GetItemSprite(int id)
    {
        return items[id];
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
}
