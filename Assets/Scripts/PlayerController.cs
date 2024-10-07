using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.0f;
    public bool crouching;
    public bool sprinting;
    [Range(0.0f, 1.0f)] public float crouchMultiplier = 0.35f;
    [Range(1.0f, 2.0f)]public float sprintMultiplier = 1.45f;
    public float crouchHeight = 0.65f;

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
    public LayerMask hitLayers;
    public bool holdingPourer;

    private Pourer.Type pourerType;
    private Transform heldObject;
    private Vector3 holdPoint;
    private Quaternion holdRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<Camera>().transform;

        standingHeight = cameraTransform.localPosition.y;
    }

    private void Update()
    {
        if (!PauseMenu.paused)
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

            // Check if pouring
            if (heldObject && holdingPourer)
            {
                bool playParticles = false;
                Quaternion endRotation = transform.rotation * cameraTransform.localRotation *
                                         Quaternion.Euler(0.0f, 90.0f, 0.0f);
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, grabRange, hitLayers))
                {
                    Transform objectHit = hit.transform;

                    if (objectHit.CompareTag("Grabbable"))
                    {
                        Item item = objectHit.GetComponent<Item>();
                        if (item)
                        {
                            if (item.id == 51 && pourerType == Pourer.Type.COFFEE) // Coffee
                            {
                                endRotation *= Quaternion.Euler(0.0f, 0.0f, 25.0f);
                                playParticles = true;
                                if (item.GetComponent<CoffeeCupAnimation>().Fill())
                                {
                                    item.id = 0;
                                }
                            }
                            else if (item.id == 52 && pourerType == Pourer.Type.TEA) // Tea
                            {
                                endRotation *= Quaternion.Euler(0.0f, 0.0f, 25.0f);
                                playParticles = true;
                                if (item.GetComponent<CoffeeCupAnimation>().Fill())
                                {
                                    item.id = 13;
                                }
                            }
                        }
                    }
                }

                heldObject.GetComponent<Rigidbody>().MoveRotation(endRotation);

                ParticleSystem particleSystem = heldObject.GetComponent<Pourer>().pourParticles;
                if (playParticles && !particleSystem.isPlaying)
                {
                    particleSystem.Play();
                }
                else if (!playParticles)
                {
                    particleSystem.Stop();
                }
            }
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
            Item item = heldObject.GetComponent<Item>();
            if (item)
            {
                item.held = false;
            }

            Pourer pourer = heldObject.GetComponentInParent<Pourer>();
            if (pourer)
            {
                holdRange = holdRange / 0.6f;
                heldObjectFloatSpeed = heldObjectFloatSpeed / 1.5f;
                holdingPourer = false;
                heldObject.GetComponent<Pourer>().pourParticles.Stop();
            }

            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.freezeRotation = false;
            heldObject = null;
        }
        else
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, grabRange, hitLayers))
            {
                Transform objectHit = hit.transform;

                if (objectHit.CompareTag("Grabbable"))
                {
                    Item item = objectHit.GetComponent<Item>();
                    if (item)
                    {
                        item.held = true;
                        item.Grabbed();
                    }

                    Pourer pourer = objectHit.GetComponentInParent<Pourer>();
                    if (pourer)
                    {
                        holdRange = holdRange * 0.6f;
                        heldObjectFloatSpeed = heldObjectFloatSpeed * 1.5f;
                        holdingPourer = true;
                        pourerType = pourer.type;
                    }

                    heldObject = objectHit;
                    Rigidbody rb = hit.rigidbody;
                    rb.useGravity = false;
                    rb.constraints = RigidbodyConstraints.None;
                    rb.freezeRotation = true;
                    holdRotation = Quaternion.Inverse(transform.rotation) * objectHit.rotation;
                }
                else if (objectHit.CompareTag("Guy"))
                {
                    objectHit.GetComponent<Customer>().Interact();
                }
                else if (objectHit.CompareTag("FoodMachine"))
                {
                    objectHit.GetComponent<IInteractable>().Interact();
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
        if (holdingPourer)
        {
            holdPoint += cameraTransform.up * -0.5f + cameraTransform.right * 0.15f;
        }

        // Move the held object to the hold point
        if (heldObject)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.ClampMagnitude((holdPoint - heldObject.position) * heldObjectFloatSpeed, maxHeldObjectFloatSpeed);
            if (!holdingPourer)
            {
                rb.MoveRotation(transform.rotation * holdRotation);
            }
        }
    }

    private void LerpCamera()
    {
        float targetHeight = crouching ? crouchHeight - 1.0f : standingHeight;
        float newHeight = MathHelpers.Damp(cameraTransform.localPosition.y, targetHeight, 10.0f, Time.deltaTime);
        
        if (Mathf.Abs(targetHeight - newHeight) < 0.0025f)
        {
            cameraLerping = false;
            newHeight = targetHeight;
        }

        cameraTransform.localPosition = new Vector3(0.0f, newHeight, 0.0f);
    }

    private void OnDrawGizmos()
    {
        Camera mainCam = Camera.main;
        holdPoint = mainCam.transform.position + mainCam.transform.forward * holdRange;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(holdPoint, (holdPoint - mainCam.transform.position).normalized * (grabRange - holdRange));
        Gizmos.color = Color.red;
        Gizmos.DrawRay(mainCam.transform.position, holdPoint - mainCam.transform.position);
        Gizmos.DrawSphere(holdPoint, 0.05f);

    }
}
