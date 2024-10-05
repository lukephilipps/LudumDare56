using UnityEngine;

public class ObjectHolding : MonoBehaviour
{
    public float grabRange = 0.5f;
    public float heldObjectFloatSpeed = 1.0f;
    public float maxHeldObjectFloatSpeed = 2.0f;

    private Transform heldObject;
    private Vector3 holdPoint;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void LateUpdate()
    {
        Transform cameraTransform = Camera.main.transform;
        holdPoint = cameraTransform.position + cameraTransform.forward;

        if (Input.GetKeyDown(KeyCode.E))
        {
            // Drop object if holding one, otherwise try to grab a new object
            if (heldObject)
            {
                Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                rb.useGravity = true;
                heldObject = null;
                rb.freezeRotation = false;
            }
            else
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    Transform objectHit = hit.transform;

                    if (objectHit.CompareTag("Grabbable"))
                    {
                        heldObject = objectHit;
                        Rigidbody rb = objectHit.GetComponent<Rigidbody>();
                        rb.useGravity = false;
                        rb.freezeRotation = true;
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        // Move the held object to the hold point
        if (heldObject)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.ClampMagnitude((holdPoint - heldObject.position) * heldObjectFloatSpeed, maxHeldObjectFloatSpeed);
        }
    }

    private void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(holdPoint, 0.1f);
    }
}
