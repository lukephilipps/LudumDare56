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
    WAIT_QUEUE,
    ORDERED,
    WAIT_FOOD,
    DONE
}

public class Customer : MonoBehaviour
{
    public Satisfaction currentSatisfaction;
    public OrderState currentState;
    public AnimState animState;

    public Image emotionImage;
    public Image orderImage;
    public int orderID;

    public NavMeshAgent agent;
    private Animator animator;
    
    void Start()
    {
        currentSatisfaction = Satisfaction.HAPPY;
        currentState = OrderState.WALKING;

        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(GameManager.Singleton.WaitInLine(this));
        
        animator = GetComponent<Animator>();
        ChangeAnimation(AnimState.WALK);
    }

    void Update()
    {
        if (currentState == OrderState.WALKING && agent.remainingDistance < 0.001f)
        {
            ChangeAnimation(AnimState.IDLE);
            currentState = OrderState.WAIT_QUEUE;
        }

        else if (currentState == OrderState.ORDERED && agent.remainingDistance < 0.001f)
        {
            ChangeAnimation(AnimState.IDLE);
            currentState = OrderState.WAIT_FOOD;
        }
        
        else if (currentState == OrderState.DONE && agent.remainingDistance < 0.001f)
        {
            Destroy(gameObject);
        }
        
    }
    
    public void PlaceOrder()
    {
        if (currentState == OrderState.WAIT_QUEUE)
        {
            agent.SetDestination(GameManager.Singleton.SitAtTable());
            Debug.Log(agent.destination);
            ChangeAnimation(AnimState.WALK);

            orderID = Random.Range(0, GameManager.Singleton.ItemsLen());
            
            orderImage.sprite = GameManager.Singleton.GetItemSprite(orderID);
            orderImage.color = new Color(255, 255, 255, 255);

            emotionImage.sprite = GameManager.Singleton.GetEmotionSprite(Satisfaction.HAPPY);
            emotionImage.color = new Color(255, 255, 255, 255);

            currentState = OrderState.ORDERED;
        }
    }

    public void GetOrder()
    {
        if (currentState == OrderState.WAIT_FOOD)
        {
            orderImage.color = new Color(255, 255, 255, 0);
            emotionImage.color = new Color(255, 255, 255, 0);

            agent.SetDestination(GameManager.Singleton.ExitLocation());
            ChangeAnimation(AnimState.WALK);

            currentState = OrderState.DONE;
        }
    }

    public void ChangeAnimation(AnimState state)
    {
        animator.runtimeAnimatorController = GameManager.Singleton.GetAnimation(state);
        animState = state;
    }
}
