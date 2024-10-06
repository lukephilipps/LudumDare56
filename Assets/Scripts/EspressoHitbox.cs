using UnityEngine;

public class EspressoHitbox : MonoBehaviour
{
    public Item item;

    private void OnTriggerStay(Collider other)
    {
        if (!item)
        {
            Item item = other.gameObject.GetComponentInParent<Item>();

            if (!item.held)
            {
                TakeItem(item);
            }
        }
    }

    public void TakeItem(Item item)
    {
        this.item = item;
        item.transform.parent = transform;
        item.transform.localPosition = GetComponent<BoxCollider>().center;
        item.transform.localRotation = Quaternion.identity;
        item.onGrab += ReleaseItem;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.freezeRotation = true;

        if (item.id == 50)
        {
            item.GetComponent<EspressoCupAnimation>().PlaceCup();
            
        }
    }

    public void ReleaseItem()
    {
        item.transform.parent = null;
        item.onGrab -= ReleaseItem;
        item = null;
    }
}
