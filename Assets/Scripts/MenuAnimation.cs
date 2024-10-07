using UnityEngine;

public class MenuAnimation : MonoBehaviour
{
    private float gameTime;
    public float animTime;
    
    public Vector3 firstPos;
    public Vector3 secondPos;
    void Update()
    {
        gameTime += Time.deltaTime;

        if (gameTime >= animTime)
        {
            if (transform.rotation == Quaternion.Euler(firstPos)) transform.rotation = Quaternion.Euler(secondPos);
            else if (transform.rotation == Quaternion.Euler(secondPos)) transform.rotation = Quaternion.Euler(firstPos);
            else transform.rotation = Quaternion.Euler(secondPos);

            gameTime = 0;
        }
    }
}
