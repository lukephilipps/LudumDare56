using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    
    public Sprite[] items;

    public Sprite GetSprite(int id)
    {
        return items[id];
    }
}
