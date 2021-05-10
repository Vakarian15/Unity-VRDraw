using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Gesture
{
    public string name;
    public List<Vector3> fingerDatas;
    public UnityEvent onRecognized;
}

public class GestureDetector : MonoBehaviour
{
    public float recognizeThreshold = 0.05f;
    public OVRSkeleton skeleton;
    public List<Gesture> gestures;
    public bool debugMode;
    public UnityEvent notRecognized;

    private List<OVRBone> fingerBones;
    private bool isInitialized = false;
    private bool isNewGesRecognized = false;
    private bool isGestureHandled = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        while (!skeleton.IsInitialized)
        {
            yield return null;
        }
        fingerBones = new List<OVRBone>(skeleton.Bones);
        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (debugMode && Input.GetKeyDown(KeyCode.Space))
        {
            RecordGesture();
        }

        if (isInitialized)
        {
            Gesture currentGesture = Recognize();
            isNewGesRecognized = !currentGesture.Equals(new Gesture());
            if (isNewGesRecognized)
            {
                currentGesture.onRecognized?.Invoke();
                isGestureHandled = true;
            }
            else
            {
                if (isGestureHandled)
                {
                    isGestureHandled = false;
                    notRecognized?.Invoke();
                }
            }
        }
    }

    void RecordGesture()
    {
        Gesture g = new Gesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();
        foreach (var bone in fingerBones)
        {
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }

        g.fingerDatas = data;
        gestures.Add(g);
    }

    Gesture Recognize()
    {
        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach (var gesture in gestures)
        {
            float distanceSum = 0f;
            bool isDiscarded = false;
            for (int i = 0; i < fingerBones.Count; i++)
            {
                Vector3 currentData = skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);
                float distance = Vector3.Distance(gesture.fingerDatas[i], currentData);
                if (distance>recognizeThreshold)
                {
                    isDiscarded = true;
                    break;
                }
                distanceSum += distance;
            }

            if (!isDiscarded && distanceSum<currentMin)
            {
                currentMin = distanceSum;
                currentGesture = gesture;
            }
        }

        return currentGesture;
    }
}
