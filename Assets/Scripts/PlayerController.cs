using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.0f;
    public bool crouching;
    public bool sprinting;
    [Range(0.0f, 1.0f)] public float crouchMultiplier = 0.35f;
    [Range(1.0f, 2.0f)]public float sprintMultiplier = 1.45f;
    public float crouchHeight = 1.0f;

    private bool cameraLerping;
    private float standingHeight;
    private float cameraRotation;
    private Transform cameraTransform;
    private CharacterController characterController;

    [Header("Grabbing Objects")]
    public float grabRange = 2.0f;
    public float holdRange = 1.0f;
    public float heldObjectFloatSpeed = 15.0f;
    public float maxHeldObjectFloatSpeed = 20.0f;

    private Transform heldObject;
    private Vector3 holdPoint;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<Camera>().transform;

        standingHeight = cameraTransform.localPosition.y;
    }

    private void Update()
    {
        HandleCrouching();
        Movement();
        CameraMovement();

        if (cameraLerping)
        {
            LerpCamera();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        if (Input.GetKeyDown("r"))
        {
            if (Application.targetFrameRate == 30) Application.targetFrameRate = -1;
            else Application.targetFrameRate = 30;
        }
    }

    private void Movement()
    {
        float xMovement = Input.GetAxis("Horizontal");
        float yMovement = Input.GetAxis("Vertical");

        Vector3 frameVelocity = Vector3.ClampMagnitude(transform.right * xMovement + transform.forward * yMovement, 1.0f) * moveSpeed;

        if (crouching)
        {
            frameVelocity *= crouchMultiplier;
        }
        else if (sprinting = Input.GetKey(KeyCode.LeftShift))
        {
            frameVelocity *= sprintMultiplier;
        }

        // Move player
        characterController.Move(frameVelocity * Time.deltaTime);
    }

    private void CameraMovement()
    {
        float xMouse = Input.GetAxis("Mouse X");
        float yMouse = Input.GetAxis("Mouse Y");

        transform.Rotate(0.0f, xMouse, 0.0f);
        cameraRotation = Mathf.Clamp(cameraRotation + yMouse, -90.0f, 90.0f);

        cameraTransform.localRotation = Quaternion.Euler(-cameraRotation, 0, 0.0f);
    }

    private void Interact()
    {
        // Drop object if holding one, otherwise interact as normal
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
            if (Physics.Raycast(ray, out hit, grabRange))
            {
                Transform objectHit = hit.transform;

                if (objectHit.CompareTag("Grabbable"))
                {
                    heldObject = objectHit;
                    Rigidbody rb = objectHit.GetComponent<Rigidbody>();
                    rb.useGravity = false;
                    rb.freezeRotation = true;
                }
                else if (objectHit.CompareTag("Guy"))
                {
                    objectHit.GetComponent<Customer>().PlaceOrder();
                }
            }
        }
    }

    private void HandleCrouching()
    {
        if (true /*check if player has hold or toggle crouch, rn just hold crouch*/)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                crouching = true;
                cameraLerping = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                crouching = false;
                cameraLerping = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                crouching = !crouching;
                cameraLerping = true;
            }
        }
    }

    private void FixedUpdate()
    {
        // Adjust the point to lerp held objects to
        Transform cameraTransform = Camera.main.transform;
        holdPoint = cameraTransform.position + cameraTransform.forward * holdRange;

        // Move the held object to the hold point
        if (heldObject)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.ClampMagnitude((holdPoint - heldObject.position) * heldObjectFloatSpeed, maxHeldObjectFloatSpeed);
        }
    }

    private void OnDrawGizmos()
    {
        Camera mainCam = Camera.main;
        Ray ray;
        ray = mainCam.ScreenPointToRay(Input.mousePosition);
        holdPoint = mainCam.transform.position + mainCam.transform.forward * holdRange;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(holdPoint, (holdPoint - mainCam.transform.position).normalized * (grabRange - holdRange));
        Gizmos.color = Color.red;
        Gizmos.DrawRay(mainCam.transform.position, holdPoint - mainCam.transform.position);
        Gizmos.DrawSphere(holdPoint, 0.05f);

    }

    private void LerpCamera()
    {
        float targetHeight = crouching ? crouchHeight - 1.0f : standingHeight;
        float newHeight = MathHelpers.Damp(cameraTransform.localPosition.y, targetHeight, 10.0f, Time.deltaTime);
        
        if (Mathf.Abs(targetHeight - newHeight) < 0.01f)
        {
            cameraLerping = false;
            newHeight = targetHeight;
        }

        cameraTransform.localPosition = new Vector3(0.0f, newHeight, 0.0f);
    }
}
