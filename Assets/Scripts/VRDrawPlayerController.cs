using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRDrawPlayerController : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    private float moveMultiplier = 4f;
    private float rotateMultiplier = 800f;
    private Vector3 prevPosR = Vector3.zero;
    private Vector3 currentPosR = Vector3.zero;
    private Vector3 handMoveR = Vector3.zero;
    private Vector3 prevPosL = Vector3.zero;
    private Vector3 currentPosL = Vector3.zero;
    private Vector3 handMoveL = Vector3.zero;

    private void Update()
    {
        currentPosR=rightHand.localPosition;
        if (prevPosR==Vector3.zero)
        {
        }
        else
        {
            handMoveR = currentPosR - prevPosR;
        }
        prevPosR = currentPosR;

        currentPosL = leftHand.localPosition;
        if (prevPosL == Vector3.zero)
        {
        }
        else
        {
            handMoveL = currentPosL - prevPosL;
        }
        prevPosL = currentPosL;
    }

    public void OnRightThumbUp()
    {

        transform.position += transform.TransformDirection(-handMoveR) * moveMultiplier;
    }

    public void OnLeftThumbUp()
    {
        float angley = handMoveL.z * rotateMultiplier;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + angley, transform.eulerAngles.z);
    }
}
