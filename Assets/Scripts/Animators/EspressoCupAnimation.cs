using UnityEngine;

public class EspressoCupAnimation : MonoBehaviour
{
    public bool done;
    public bool fillingUp;
    public Transform cookie;
    public Transform plate;
    public Transform espresso;
    public ParticleSystem particles;
    public EspressoHitbox hitboxRef;

    private void Update()
    {
        if (fillingUp)
        {
            espresso.transform.localPosition = new Vector3(0.0f, MathHelpers.Damp(espresso.transform.localPosition.y, 0.0f, 0.55f, Time.deltaTime), 0.0f);
            if (espresso.transform.localPosition.y >= -0.005f)
            {
                fillingUp = false;
                done = true;
                GetComponent<Item>().id = 5;
                particles.Play(false);
                hitboxRef.StopCoffeePour();
            }
        }
    }

    public void PlaceCup()
    {
        plate.gameObject.SetActive(true);
        plate.localPosition = Vector3.zero;
        plate.localRotation = Quaternion.identity;
        plate.localScale = Vector3.one;
    }

    public void StartFilling()
    {
        espresso.gameObject.SetActive(true);
        fillingUp = true;
    }

    public void TopOff()
    {
        cookie.gameObject.SetActive(true);
        fillingUp = false;
    }
}
