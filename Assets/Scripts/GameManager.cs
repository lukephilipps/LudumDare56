using System;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public enum AnimState
{
    IDLE,
    WALK,
    SIT,
    ANGY,
    HAPPY,
    DANCING
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

    [Header("UI References")]
    public Image[] emojis;

    private Random random;
    private float randXMin, randXMax, randYMin, randYMax;
    private int dangerCount;
    
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

    public Vector3 RandomOverflowLocation()
    {
        float x = (float)(randXMin + random.NextDouble() * Math.Abs(randXMax - randXMin));
        float y = (float)(randYMin + random.NextDouble() * Math.Abs(randYMax - randYMin));
        return new Vector3(x, -1.0f, y);
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
            case AnimState.DANCING:
                return animations[5];
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
                prev.customer.currentState = OrderState.WALKING_UP_LINE;
                prev.customer.ChangeAnimation(AnimState.WALK);
                prev.customer.agent.SetDestination(curr.transform.position);
                
                curr.taken = true;
                curr.customer = prev.customer;
                
                prev.taken = false;
                prev.customer = null;
            }
        }
    }

    // Returns a Tuple with the position and if it is at a table
    public Tuple<Vector3, bool> GetWaitingPosition(Customer customer)
    {
        for (int i = 0; i < table.Length; i++)
        {
            if (!table[i].taken) // returns first empty chair
            {
                table[i].customer = customer;
                table[i].taken = true;
                return new Tuple<Vector3, bool>(table[i].transform.position + table[i].transform.forward * 0.065f, true);
            }
        }

        return new Tuple<Vector3, bool>(RandomOverflowLocation(), false); // goes to volume square
    }

    public void FreeWaitingPosition(Customer customer)
    {
        for (int i = 0; i < table.Length; i++)
        {
            if (table[i].customer == customer)
            {
                table[i].taken = false;
                table[i].customer = null;
                return;
            }
        }
    }

    public Vector3 ExitLocation()
    {
        return exit.position;
    }

    public void AddSatisfaction(Satisfaction level)
    {
        Sprite sprite;

        switch (level)
        {
            case Satisfaction.HAPPY:
                sprite = emotions[0];
                break;
            case Satisfaction.NEUTRAL:
                sprite = emotions[1];
                break;
            case Satisfaction.UNHAPPY:
                sprite = emotions[2];
                ++dangerCount;
                break;
            default:
                sprite = emotions[3];
                ++dangerCount;
                break;
        }

        if (emojis[4].color == Color.black)
        {
            emojis[4].sprite = sprite;
            emojis[4].color = Color.white;
        }
        else if (emojis[3].color == Color.black)
        {
            emojis[3].sprite = sprite;
            emojis[3].color = Color.white;
        }
        else if (emojis[2].color == Color.black)
        {
            emojis[2].sprite = sprite;
            emojis[2].color = Color.white;
        }
        else if (emojis[1].color == Color.black)
        {
            emojis[1].sprite = sprite;
            emojis[1].color = Color.white;
        }
        else if (emojis[0].color == Color.black)
        {
            emojis[0].sprite = sprite;
            emojis[0].color = Color.white;
        }
        else
        {
            if (emojis[4].sprite == emotions[2] || emojis[4].sprite == emotions[3]) --dangerCount;

            emojis[4].sprite = emojis[3].sprite;
            emojis[3].sprite = emojis[2].sprite;
            emojis[2].sprite = emojis[1].sprite;
            emojis[1].sprite = emojis[0].sprite;
            emojis[0].sprite = sprite;
        }

        MusicManager.Singleton.ActivateTrack(dangerCount);
    }
}

[System.Serializable]
public class Destination
{
    public Transform transform;
    public Customer customer;
    public bool taken = false;
}
