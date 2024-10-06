using UnityEngine;

public class EspressoMachine : MonoBehaviour, IInteractable
{
    public GameObject physicsShape;
    public GameObject cupHolderHitbox;

    private void Start()
    {
        Instantiate(physicsShape, transform.position, transform.rotation);
        Instantiate(cupHolderHitbox, transform.position, transform.rotation);
    }

    public void Interact()
    {
        
    }
}
