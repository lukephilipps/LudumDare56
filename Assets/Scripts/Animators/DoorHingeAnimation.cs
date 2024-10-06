using System;
using System.Collections;
using UnityEngine;

public class DoorHingeAnimation : MonoBehaviour
{
    public Transform doorTransform;

    public Vector3 open;
    public Vector3 closed;

    [Header("How long the door stay open time:")]
    [Range(0.0f, 10.0f)]
    public float doorTime = 5f;

    private bool isOpen;
    private bool slerping;

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
            
            StartCoroutine(WaitToClose());
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

    IEnumerator WaitToClose()
    {
        yield return new WaitForSeconds(doorTime);
        StartDoorAnim();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("entering trigger");
        StartDoorAnim();
    }
}
