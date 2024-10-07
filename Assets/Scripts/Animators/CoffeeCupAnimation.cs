using UnityEngine;

public class CoffeeCupAnimation : MonoBehaviour
{
    public bool done;
    public Transform coffee;
    public ParticleSystem particles;

    public bool Fill()
    {
        coffee.transform.localPosition = new Vector3(0.0f, MathHelpers.Damp(coffee.transform.localPosition.y, 0.0f, 2.15f, Time.deltaTime), 0.0f);
        if (coffee.transform.localPosition.y >= -0.005f)
        {
            done = true;
            particles.Play(false);

            return true;
        }

        return false;
    }
}
