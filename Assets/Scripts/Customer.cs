using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum Satisfaction
{
    HAPPY,
    NEUTRAL,
    UNHAPPY,
    ANGY
}

public enum OrderState
{
    WAITING,
    ORDERED,
    DONE
}

public class Customer : MonoBehaviour
{
    public Satisfaction currentSatisfaction;
    public OrderState currentState;

    public Image emotionImage;
    public Image orderImage;

    private NavMeshAgent agent;

    public Transform[] destination;

    void Start()
    {
        currentSatisfaction = Satisfaction.HAPPY;
        currentState = OrderState.WAITING;

        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(destination[0].position);
    }
    
    public void PlaceOrder()
    {
        if (currentState == OrderState.WAITING)
        {
            Debug.Log("Placing Order...");

            int order = Random.Range(0, GameManager.Singleton.items.Length);
            
            orderImage.sprite = GameManager.Singleton.GetItemSprite(order);
            orderImage.color = new Color(255, 255, 255, 255);

            emotionImage.sprite = GameManager.Singleton.GetEmotionSprite(Satisfaction.HAPPY);
            emotionImage.color = new Color(255, 255, 255, 255);

            currentState = OrderState.ORDERED;
            agent.SetDestination(destination[1].position);
        }
    }
}
