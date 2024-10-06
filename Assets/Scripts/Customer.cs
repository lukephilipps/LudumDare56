using Unity.VisualScripting;
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
    WALKING,
    WAITING,
    ORDERED,
    DONE
}

public class Customer : MonoBehaviour
{
    public Satisfaction currentSatisfaction;
    public OrderState currentState;
    public AnimState animState;

    public Image emotionImage;
    public Image orderImage;

    private NavMeshAgent agent;
    private Animator animator;
    
    void Start()
    {
        currentSatisfaction = Satisfaction.HAPPY;
        currentState = OrderState.WALKING;

        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(GameManager.Singleton.GetDestination(Destination.COUNTER));
        
        GameManager.Singleton.door.StartDoorAnim();

        animator = GetComponent<Animator>();
        ChangeAnimation(AnimState.WALK);
    }

    void Update()
    {
        if (currentState == OrderState.WALKING && agent.remainingDistance < 0.001f)
        {
            ChangeAnimation(AnimState.IDLE);
            currentState = OrderState.WAITING;
        }
    }
    
    public void PlaceOrder()
    {
        if (currentState == OrderState.WAITING)
        {
            ChangeAnimation(AnimState.WALK);

            int order = Random.Range(0, GameManager.Singleton.ItemsLen());
            
            orderImage.sprite = GameManager.Singleton.GetItemSprite(order);
            orderImage.color = new Color(255, 255, 255, 255);

            emotionImage.sprite = GameManager.Singleton.GetEmotionSprite(Satisfaction.HAPPY);
            emotionImage.color = new Color(255, 255, 255, 255);

            currentState = OrderState.ORDERED;
            agent.SetDestination(GameManager.Singleton.GetDestination(Destination.TABLE));
        }
    }

    private void ChangeAnimation(AnimState state)
    {
        animator.runtimeAnimatorController = GameManager.Singleton.GetAnimation(state);
        animState = state;
    }
}
