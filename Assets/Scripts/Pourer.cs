using UnityEngine;

public class Pourer : MonoBehaviour
{
    public enum Type
    { 
        TEA,
        COFFEE
    }

    public Type type;
    public ParticleSystem pourParticles;
}
