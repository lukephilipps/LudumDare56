using UnityEngine;

public class Item : MonoBehaviour
{
    public int id;
    public bool held;

    public delegate void OnGrab();
    public OnGrab onGrab;

    public void Grabbed()
    {
        onGrab?.Invoke();
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("Guy"))
        {
            Customer customer = other.gameObject.GetComponent<Customer>();
            if (id == customer.orderID)
            {
                customer.GetOrder();
                Destroy(gameObject);
            }
        }
    }
}
