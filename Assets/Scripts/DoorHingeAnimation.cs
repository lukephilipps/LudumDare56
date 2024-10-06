using UnityEngine;

public class DoorHingeAnimation : MonoBehaviour
{
    public Transform doorTransform;

    public Vector3 open;
    public Vector3 closed;

    public bool isOpen;
    public bool slerping;

    private float elapsedTime;
    private float closeSlerpTime;
    private float openSlerpTime;
    
    void Start()
    {
        closeSlerpTime = 1f;
        openSlerpTime = 0.2f;
        elapsedTime = 0f;
        
        isOpen = false;
        slerping = false;
    }

    void Update()
    {
        if (slerping)
        {
            if (isOpen) CloseDoor();
            else OpenDoor();
        }
        
    }

    public void StartDoorAnim()
    {
        slerping = true;
    }

    private void OpenDoor()
    {
        if (elapsedTime < closeSlerpTime)
        {
            doorTransform.rotation = Quaternion.Slerp(Quaternion.Euler(closed), Quaternion.Euler(open), elapsedTime / openSlerpTime);
            elapsedTime += Time.deltaTime;
        }
        else
        {
            isOpen = true;
            slerping = false;
            elapsedTime = 0f;
            
            StartDoorAnim(); // start closing animation
        }
    }

    private void CloseDoor()
    {
        if (elapsedTime < closeSlerpTime)
        {
            doorTransform.rotation = Quaternion.Slerp(Quaternion.Euler(open), Quaternion.Euler(closed), elapsedTime / closeSlerpTime);
            elapsedTime += Time.deltaTime;
        }
        else
        {
            isOpen = false;
            slerping = false;
            elapsedTime = 0f;
        }

    }
}
