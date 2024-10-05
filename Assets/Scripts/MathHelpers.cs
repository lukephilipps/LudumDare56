using UnityEngine;

public class MathHelpers
{
    public static float Damp(float a, float b, float lambda, float dt)
    {
        return Mathf.Lerp(a, b, 1.0f - Mathf.Exp(-lambda * dt));
    }
}
