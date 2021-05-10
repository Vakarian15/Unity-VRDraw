using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VRGrabbable : OVRGrabbable
{
    private void Awake()
    {
        Collider collider = this.GetComponent<Collider>();
        if (collider == null)
        {
            throw new ArgumentException("Grabbables cannot have zero grab points and no collider -- please add a grab point or collider.");
        }

        // Create a default grab point
        m_grabPoints = new Collider[1] { collider };
    }
}
