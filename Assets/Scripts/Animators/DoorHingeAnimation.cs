using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class DoorHingeAnimation : MonoBehaviour
{
    public Transform doorTransform;

    public Vector3 open;
    public Vector3 closed;

    [Header("How long the door stay open time:")]
    [Range(0.0f, 10.0f)]
    public float doorTime = 5f;

    bool moving;
    bool canClose;
    private bool isOpen;
    private bool slerping;

    private float elapsedTime;
    private float elapsedTimeClose;
    private float closeSlerpTime;
    private float openSlerpTime;
    
    void Start()
    {
        closeSlerpTime = 1f;
        openSlerpTime = 0.2f;
        elapsedTime = 0;
        
        isOpen = false;
        slerping = false;
    }

    void Update()
    {
        if (moving) SimDoor();
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
        elapsedTimeClose = 0.0f;
        canClose = true;
    }

    void SimDoor()
    {
        if (!canClose)
        {
            elapsedTime += Time.deltaTime;
            doorTransform.rotation = Quaternion.Slerp(Quaternion.Euler(closed), Quaternion.Euler(open), elapsedTime / openSlerpTime);
        }
        else
        {
            elapsedTimeClose += Time.deltaTime;
            doorTransform.rotation = Quaternion.Slerp(Quaternion.Euler(open), Quaternion.Euler(closed), elapsedTimeClose / closeSlerpTime);

            if (elapsedTimeClose > closeSlerpTime)
            {
                doorTransform.rotation = Quaternion.Euler(closed);
                moving = false;
                elapsedTime = 0.0f;
                elapsedTimeClose = 0.0f;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        moving = true;
        canClose = false;
        StopAllCoroutines();
        StartCoroutine(WaitToClose());
    }
}
