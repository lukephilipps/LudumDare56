using UnityEngine;

public class UIFacePlayer : MonoBehaviour
{
    Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        transform.LookAt(cameraTransform);
    }
}
