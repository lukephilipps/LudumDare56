using System.Collections;
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

    [Header("Sound Effects")] 
    public AudioClip happySound;
    public AudioClip angySound;
    private AudioSource audioSource;

    private float emotionTimer;
    
    void Start()
    {
        currentSatisfaction = Satisfaction.HAPPY;
        currentState = OrderState.WALKING;

        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(GameManager.Singleton.WaitInLine(this));
        
        animator = GetComponent<Animator>();
        ChangeAnimation(AnimState.WALK);

        audioSource = GetComponent<AudioSource>();

        emotionTimer = 0f;
    }

    void Update()
    {
        if (currentState == OrderState.WAIT_FOOD)
        {
            emotionTimer += Time.deltaTime;

            if (emotionTimer < 10f)
            {
                currentSatisfaction = Satisfaction.HAPPY;
                emotionImage.sprite = GameManager.Singleton.GetEmotionSprite(Satisfaction.HAPPY);
            }
            else if ( (emotionTimer >= 10f && emotionTimer < 20f) && currentSatisfaction == Satisfaction.HAPPY)
            {
                currentSatisfaction = Satisfaction.NEUTRAL;
                emotionImage.sprite = GameManager.Singleton.GetEmotionSprite(Satisfaction.NEUTRAL);
            }
            else if ( (emotionTimer >= 20f && emotionTimer < 30f) && currentSatisfaction == Satisfaction.NEUTRAL)
            {
                currentSatisfaction = Satisfaction.UNHAPPY;
                emotionImage.sprite = GameManager.Singleton.GetEmotionSprite(Satisfaction.UNHAPPY);
            }
            else if (emotionTimer >= 30f && currentSatisfaction == Satisfaction.UNHAPPY)
            {
                currentSatisfaction = Satisfaction.ANGY;
                emotionImage.sprite = GameManager.Singleton.GetEmotionSprite(Satisfaction.ANGY);

                StartCoroutine(GetOrder());
            }
        }
        
        if (currentState == OrderState.WALKING && agent.remainingDistance < 0.001f)
        {
            ChangeAnimation(AnimState.IDLE);
            currentState = OrderState.WAIT_QUEUE;
        }

        else if (currentState == OrderState.ORDERED && agent.remainingDistance < 0.001f)
        {
            if(currentSatisfaction == Satisfaction.HAPPY) ChangeAnimation(AnimState.SIT);
            else if(currentSatisfaction == Satisfaction.NEUTRAL) ChangeAnimation(AnimState.IDLE);
            currentState = OrderState.WAIT_FOOD;
        }
        
        else if (currentState == OrderState.DONE && agent.remainingDistance < 0.001f)
        {
            Destroy(gameObject);
        }
        
    }
    
    public IEnumerator PlaceOrder()
    {
        if (currentState == OrderState.WAIT_QUEUE)
        {
            orderID = Random.Range(0, GameManager.Singleton.ItemsLen());

            orderImage.sprite = GameManager.Singleton.GetItemSprite(orderID);
            orderImage.color = new Color(255, 255, 255, 255);
            
            Vector3 movePos = GameManager.Singleton.SitAtTable();

            if (movePos == Vector3.zero) // no more seats
            {
                ChangeAnimation(AnimState.IDLE);
                currentSatisfaction = Satisfaction.NEUTRAL;
                
                Vector2 randLocation = GameManager.Singleton.randomOverflowLocation();

                movePos = new Vector3(randLocation.x, transform.position.y, randLocation.y);
                Debug.Log(movePos);

                emotionImage.sprite = GameManager.Singleton.GetEmotionSprite(Satisfaction.NEUTRAL);
                emotionImage.color = new Color(255, 255, 255, 255);
                
                if (angySound != null)
                {
                    audioSource.clip = angySound;
                    audioSource.Play();
                    yield return new WaitForSeconds(angySound.length);
                }
            }
            else // seats available
            {
                ChangeAnimation(AnimState.HAPPY);
                currentSatisfaction = Satisfaction.HAPPY;

                emotionImage.sprite = GameManager.Singleton.GetEmotionSprite(Satisfaction.HAPPY);
                emotionImage.color = new Color(255, 255, 255, 255);
            
                if (happySound != null)
                {
                    audioSource.clip = happySound;
                    audioSource.Play();
                    yield return new WaitForSeconds(happySound.length);
                }
            }

            GameManager.Singleton.MoveLine();
            agent.SetDestination(movePos);
            ChangeAnimation(AnimState.WALK);
            currentState = OrderState.ORDERED;
        }
    }

    public IEnumerator GetOrder()
    {
        if (currentState == OrderState.WAIT_FOOD)
        {
            orderImage.color = new Color(255, 255, 255, 0);
            emotionImage.color = new Color(255, 255, 255, 0);

            agent.SetDestination(GameManager.Singleton.ExitLocation());
            ChangeAnimation(AnimState.WALK);

            if (currentSatisfaction == Satisfaction.ANGY)
            {
                if (angySound != null)
                {
                    audioSource.clip = angySound;
                    audioSource.Play();
                    yield return new WaitForSeconds(angySound.length);
                }
            }
            else
            {
                if (happySound != null)
                {
                    audioSource.clip = happySound;
                    audioSource.Play();
                    yield return new WaitForSeconds(happySound.length);
                }
            }

            currentState = OrderState.DONE;
        }
    }

    public void ChangeAnimation(AnimState state)
    {
        animator.runtimeAnimatorController = GameManager.Singleton.GetAnimation(state);
        animState = state;
    }

    IEnumerator PlaySoundEffect(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
        yield return new WaitForSeconds(clip.length);
    }
}
