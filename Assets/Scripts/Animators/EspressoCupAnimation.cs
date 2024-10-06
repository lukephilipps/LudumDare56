using UnityEngine;

public class EspressoCupAnimation : MonoBehaviour
{
    public bool done;
    public bool fillingUp;
    public Transform cookie;
    public Transform plate;
    public Transform espresso;

    private void Update()
    {
        if (done)
        {
            done = false;
            PlaceCup();
        }

        if (fillingUp)
        {
            espresso.transform.localPosition = new Vector3(0.0f, MathHelpers.Damp(espresso.transform.localPosition.y, 0.0f, 0.35f, Time.deltaTime), 0.0f);
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
}
