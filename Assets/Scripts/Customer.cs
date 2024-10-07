using System;
using System.Collections;
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
    WALKING_UP_LINE,
    WAITING_IN_LINE,
    ORDERING,
    WALKING_TO_TABLE,
    WAITING_FOR_FOOD,
    LEAVING
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

    [Header("Animations")] public RuntimeAnimatorController idleAnim;

    [Header("Sound Effects")] 
    public AudioClip happySound;
    public AudioClip angySound;
    private AudioSource audioSource;

    private float emotionTimer;
    private bool standingInsteadOfSitting;

    private float waitingPosition;
    private bool stormingOut;
    
    void Start()
    {
        currentSatisfaction = Satisfaction.HAPPY;
        currentState = OrderState.WALKING_UP_LINE;

        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(GameManager.Singleton.WaitInLine(this));
        
        animator = GetComponent<Animator>();
        ChangeAnimation(AnimState.WALK);

        audioSource = GetComponent<AudioSource>();

        emotionTimer = 0f;
    }

    void Update()
    {
        DrainSatisfaction();

        if (agent.remainingDistance < 0.001f)
        {
            HandleEndOfPath();
        }
    }
    
    // Used to place orders
    public void Interact()
    {
        if (currentState == OrderState.WAITING_IN_LINE &&
            GameManager.Singleton.counter[0].customer &&
            GameManager.Singleton.counter[0].customer == this &&
            !stormingOut)
        {
            TakeOrder();
        }
    }

    private void TakeOrder()
    {
        // Order a random item
        orderID = UnityEngine.Random.Range(0, GameManager.Singleton.ItemsLen());

        // Set image settings
        orderImage.sprite = GameManager.Singleton.GetItemSprite(orderID);
        orderImage.color = new Color(255, 255, 255, 255);
        emotionImage.rectTransform.localPosition = new Vector3(0.33f, 0.22f, 0);

        // Add a bonus to the timer based on how early you took order
        emotionTimer -= 5.0f - (int)currentSatisfaction;

        currentState = OrderState.ORDERING;

        StartCoroutine(OrderAndMoveAnimation());
    }

    private IEnumerator OrderAndMoveAnimation()
    {
        Tuple<Vector3, bool> destination = GameManager.Singleton.GetWaitingPosition(this);
        standingInsteadOfSitting = !destination.Item2;

        if (destination.Item2)
        {
            PlaySoundEffect(happySound);
            ChangeAnimation(AnimState.HAPPY);
        }
        else
        {
            PlaySoundEffect(angySound);
            ChangeAnimation(AnimState.ANGY);
            emotionTimer += 10.0f;
        }

        // Wait for audio clip to play
        yield return new WaitForSeconds(3.2f);

        ChangeAnimation(AnimState.WALK);
        currentState = OrderState.WALKING_TO_TABLE;
        agent.SetDestination(destination.Item1);

        // Give time for customer to go down stairs before advancing line
        yield return new WaitForSeconds(1.8f);

        GameManager.Singleton.MoveLine();
    }

    public void ReceiveOrder()
    {
        if (!stormingOut)
        {
            GameManager.Singleton.AddSatisfaction(currentSatisfaction);
            StartCoroutine(TakeOrderAndLeave());
        }
    }

    public IEnumerator TakeOrderAndLeave()
    {
        orderImage.color = new Color(255, 255, 255, 0);
        emotionImage.rectTransform.localPosition = new Vector3(0.0f, 0.22f, 0.0f);

        // Celebrate or pout
        switch (currentSatisfaction)
        {
            case Satisfaction.HAPPY:
                ChangeAnimation(AnimState.DANCING);
                PlaySoundEffect(happySound);
                yield return new WaitForSeconds(3.2f);
                break;
            case Satisfaction.NEUTRAL:
                ChangeAnimation(AnimState.HAPPY);
                PlaySoundEffect(happySound);
                yield return new WaitForSeconds(2.1f);
                break;
            default:
                PlaySoundEffect(angySound);
                yield return new WaitForSeconds(1.8f);
                break;
        }

        // Leave
        if (!standingInsteadOfSitting) GameManager.Singleton.FreeWaitingPosition(this);
        ChangeAnimation(AnimState.WALK);
        currentState = OrderState.LEAVING;
        agent.SetDestination(GameManager.Singleton.ExitLocation());
    }

    private IEnumerator StormOut()
    {
        PlaySoundEffect(angySound);
        ChangeAnimation(AnimState.ANGY);
        yield return new WaitForSeconds(3.2f);

        ChangeAnimation(AnimState.WALK);
        agent.SetDestination(GameManager.Singleton.ExitLocation());

        // Advance line if in the front of the line
        if (GameManager.Singleton.counter[0].customer &&
            GameManager.Singleton.counter[0].customer == this)
        {
            yield return new WaitForSeconds(1.2f);
            GameManager.Singleton.MoveLine();
        }

        if (!standingInsteadOfSitting) GameManager.Singleton.FreeWaitingPosition(this);
        currentState = OrderState.LEAVING;
    }

    public void ChangeAnimation(AnimState state)
    {
        if (state == AnimState.IDLE)
        {
            animator.runtimeAnimatorController = idleAnim;
        }
        else
        {
            animator.runtimeAnimatorController = GameManager.Singleton.GetAnimation(state);
        }
        animState = state;
    }

    private void PlaySoundEffect(AudioClip clip)
    {
        if (clip)
        {
            audioSource.pitch = UnityEngine.Random.Range(0.96f, 1.04f);
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private void HandleEndOfPath()
    {
        switch (currentState)
        {
            case OrderState.WALKING_UP_LINE:
                if (GameManager.Singleton.counter[0].customer && GameManager.Singleton.counter[0].customer == this) transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                ChangeAnimation(AnimState.IDLE);
                currentState = OrderState.WAITING_IN_LINE;
                break;
            case OrderState.WAITING_IN_LINE:

                break;
            case OrderState.ORDERING:

                break;
            case OrderState.WALKING_TO_TABLE:
                if (!standingInsteadOfSitting)
                {
                    waitingPosition = transform.position.y;
                    if (transform.position.z > -4.0f)
                    {
                        transform.LookAt(new Vector3(-4.92912483f, transform.position.y, -2.46898365f));
                    }
                    else
                    {
                        transform.LookAt(new Vector3(-5.26622009f, -0.978960156f, -6.1971302f));
                    }
                    ChangeAnimation(AnimState.SIT);
                }
                else ChangeAnimation(AnimState.IDLE);
                currentState = OrderState.WAITING_FOR_FOOD;
                break;
            case OrderState.WAITING_FOR_FOOD:
                if (!standingInsteadOfSitting)
                {
                    transform.position = new Vector3(transform.position.x, waitingPosition + 0.0922936f, transform.position.z);
                }
                break;
            case OrderState.LEAVING:
                Destroy(gameObject);
                break;
        }
    }

    private void DrainSatisfaction()
    {
        float emotionMultiplier = 1.0f;

        switch (currentState)
        {
            case OrderState.WALKING_UP_LINE:
            case OrderState.ORDERING:
            case OrderState.WALKING_TO_TABLE:
            case OrderState.LEAVING:
                emotionMultiplier = 0.0f;
                break;
        }

        emotionTimer += Time.deltaTime * emotionMultiplier;

        if (emotionTimer < 15f)
        {
            currentSatisfaction = Satisfaction.HAPPY;
            emotionImage.sprite = GameManager.Singleton.GetEmotionSprite(Satisfaction.HAPPY);
        }
        else if (emotionTimer < 30f)
        {
            currentSatisfaction = Satisfaction.NEUTRAL;
            emotionImage.sprite = GameManager.Singleton.GetEmotionSprite(Satisfaction.NEUTRAL);
        }
        else if (emotionTimer < 45f)
        {
            currentSatisfaction = Satisfaction.UNHAPPY;
            emotionImage.sprite = GameManager.Singleton.GetEmotionSprite(Satisfaction.UNHAPPY);
        }
        else if (currentSatisfaction == Satisfaction.UNHAPPY)
        {
            currentSatisfaction = Satisfaction.ANGY;
            emotionImage.sprite = GameManager.Singleton.GetEmotionSprite(Satisfaction.ANGY);

            orderImage.color = new Color(255, 255, 255, 0);
            emotionImage.rectTransform.localPosition = new Vector3(0.0f, 0.22f, 0.0f);

            GameManager.Singleton.AddSatisfaction(currentSatisfaction);
            stormingOut = true;
            StopAllCoroutines();
            StartCoroutine(StormOut());
        }
    }
}
