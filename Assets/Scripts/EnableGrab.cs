using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableGrab : MonoBehaviour
{
    [SerializeField]
    private ControlHand controlHand = ControlHand.NoSet;

    OVRGrabber grab;
    VRDraw draw;

    enum ControlHand
    {
        NoSet,
        Left,
        Right
    }
    // Start is called before the first frame update
    void Start()
    {
        grab = GetComponent<OVRGrabber>();
        grab.enabled = true;
        draw = GetComponent<VRDraw>();
    }

    // Update is called once per frame
    void Update()
    {

        if (controlHand == ControlHand.Left && OVRInput.Get(OVRInput.RawButton.Y))
        {
            grab.enabled = true;
            draw.enabled = false;
        }
        else if (controlHand == ControlHand.Left && OVRInput.GetUp(OVRInput.RawButton.Y))
        {
            grab.enabled = false;
            draw.enabled = true;
        }

        if (OVRInput.Get(OVRInput.RawButton.B))
        {
            Debug.LogWarning("BBBBBBBBBBBBBBBB");
        }
        if (controlHand == ControlHand.Right && OVRInput.Get(OVRInput.RawButton.B))
        {
            Debug.LogWarning("BBBBB");
            grab.enabled = true;
            draw.enabled = false;
        }
        else if (controlHand == ControlHand.Right && OVRInput.GetUp(OVRInput.RawButton.B))
        {
            grab.enabled = false;
            draw.enabled = true;
        }
    }
}
