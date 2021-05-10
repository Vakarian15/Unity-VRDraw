using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

public class HandTrackGrabber : OVRGrabber
{
    private OVRHand hand;
    private float pintchThreshold = 0.1f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        hand = GetComponent<OVRHand>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        CheckMiddlePinch();
    }

    void CheckMiddlePinch()
    {
        bool isMiddlePintching = hand.GetFingerIsPinching(OVRHand.HandFinger.Middle);
        float middlePintchStrength = hand.GetFingerPinchStrength(OVRHand.HandFinger.Middle);
        if (!m_grabbedObj&&m_grabCandidates.Count>0&& isMiddlePintching&&middlePintchStrength>=pintchThreshold)
        {
            GrabBegin();
        }
        else if(m_grabbedObj&&(!isMiddlePintching||middlePintchStrength<pintchThreshold))
        {
            GrabEnd();
        }
    }
}
