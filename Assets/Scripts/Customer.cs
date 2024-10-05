using UnityEngine;
public enum Satisfaction
{
    HAPPY,
    SATISFIED,
    UNHAPPY,
    ANGY
}

public class Customer : MonoBehaviour
{
    public Satisfaction currentSatisfaction;

    public Transform emotionTransform;
    public Transform orderTransform;

    void Start()
    {
        currentSatisfaction = Satisfaction.HAPPY;
    }
    
    public void PlaceOrder()
    {
        Debug.Log("Placing Order...");
        
        
    }
}
