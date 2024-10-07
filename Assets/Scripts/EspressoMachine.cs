using UnityEngine;

public class EspressoMachine : MonoBehaviour, IInteractable
{
    public GameObject physicsShape;
    public GameObject cupHolderHitbox;
    public Transform coffeeEffect;

    private EspressoHitbox hitbox;

    private void Start()
    {
        Instantiate(physicsShape, transform.position, transform.rotation);
        hitbox = Instantiate(cupHolderHitbox, transform.position, transform.rotation).GetComponent<EspressoHitbox>();
        hitbox.machine = this;
    }

    public void Interact()
    {
        if (hitbox.item && hitbox.item.id == 50)
        {
            hitbox.item.GetComponent<EspressoCupAnimation>().StartFilling();
            coffeeEffect.gameObject.SetActive(true);
        }
    }
}
