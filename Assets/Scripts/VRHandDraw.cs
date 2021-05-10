using System.Linq;
using UnityEngine;
using System.Collections.Generic;


public enum HandToTrack
{
    Left,
    Right
}

public class VRHandDraw : MonoBehaviour
{
    [SerializeField]
    private HandToTrack handToTrack = HandToTrack.Left;
    [SerializeField]
    private OVRSkeleton skeleton;

    [SerializeField]
    private GameObject allPaintings;

    [SerializeField]
    private GameObject objectToTrackMovement;

    [SerializeField, Range(0, 0.1f)]
    private float minDistanceBeforeNewPoint = 0.01f;

    public GameObject currentPainting { set;private get; }

    private Vector3 prevPointDistance = Vector3.zero;

//    [SerializeField]
//    private float minFingerPinchDownStrength = 0.5f;

    [SerializeField, Range(0, 0.1f)]
    private float lineDefaultWidth = 0.01f;

    private int positionCount = 0;

    private PaintingManager paintingManager;

    private List<LineRenderer> lines = new List<LineRenderer>();
    
    private LineRenderer currentLineRender;

    [SerializeField]
    private Color defaultColor = Color.white;

    private Material defaultLineMaterial;

    [SerializeField]
    private Material plainLineMaterial;

    [SerializeField]
    private Material woodMaterial;

    [SerializeField]
    private Material metalMaterial;

    private bool IsPinchingReleased = false;

    #region Oculus Types

    private OVRHand ovrHand;

    private OVRSkeleton ovrSkeleton;

    private OVRBone boneToTrack;
    #endregion

    void Awake()
    {
        ovrHand = objectToTrackMovement.GetComponent<OVRHand>();
        ovrSkeleton = objectToTrackMovement.GetComponent<OVRSkeleton>();
        defaultLineMaterial = plainLineMaterial;

        // get initial bone to track
        boneToTrack = ovrSkeleton.Bones
                .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Index1)
                .SingleOrDefault();

        paintingManager = allPaintings.GetComponent<PaintingManager>();

        // add initial line rerderer
//        AddNewLineRenderer();
    }

    public void AddNewLineRenderer()
    {
        positionCount = 0;

        GameObject go = new GameObject($"LineRenderer_{handToTrack.ToString()}_{lines.Count}");
        go.transform.parent = currentPainting.transform;
        go.transform.position = objectToTrackMovement.transform.position;

        LineRenderer goLineRenderer = go.AddComponent<LineRenderer>();
        goLineRenderer.startWidth = lineDefaultWidth;
        goLineRenderer.endWidth = lineDefaultWidth;
        goLineRenderer.useWorldSpace = true;
        goLineRenderer.material = new Material(defaultLineMaterial);
        goLineRenderer.material.color = defaultColor;
        if (goLineRenderer.material.IsKeywordEnabled("_EmissionColor"))
        {
            goLineRenderer.material.SetColor("_EmissionColor", defaultColor);
        }
        goLineRenderer.positionCount = 1;
        goLineRenderer.numCapVertices = 5;

        currentLineRender = goLineRenderer;

        lines.Add(goLineRenderer);
    }

    void Update()
    {
        if (skeleton.IsInitialized)
        {
            if (boneToTrack == null)
            {
                boneToTrack = ovrSkeleton.Bones
                    .Where(b => b.Id == OVRSkeleton.BoneId.Hand_Index3)
                    .SingleOrDefault();

                objectToTrackMovement = boneToTrack.Transform.gameObject;
            }

            CheckPinchState();
        }

    }

    private void CheckPinchState()
    {
        bool isIndexFingerPinching = ovrHand.GetFingerIsPinching(OVRHand.HandFinger.Index);

        float indexFingerPinchStrength = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index);

        lineDefaultWidth = indexFingerPinchStrength / 20f;

        if (ovrHand.GetFingerConfidence(OVRHand.HandFinger.Index) != OVRHand.TrackingConfidence.High)
            return;

        // finger pinch down
        if (isIndexFingerPinching)
        {
            UpdateLine();
            IsPinchingReleased = true;
            return;
        }

        // finger pinch up
        if (IsPinchingReleased)
        {
            AddNewLineRenderer();
            IsPinchingReleased = false;
        }
    }

    void UpdateLine()
    {
        if (prevPointDistance == null)
        {
            prevPointDistance = objectToTrackMovement.transform.position;
        }

        if (prevPointDistance != null && Mathf.Abs(Vector3.Distance(prevPointDistance, objectToTrackMovement.transform.position)) >= minDistanceBeforeNewPoint)
        {
            prevPointDistance = objectToTrackMovement.transform.position;
            AddPoint(prevPointDistance);
        }
    }

    void AddPoint(Vector3 position)
    {
        currentLineRender.SetPosition(positionCount, position);
        positionCount++;
        currentLineRender.positionCount = positionCount + 1;
        currentLineRender.SetPosition(positionCount, position);
    }

    public void UpdateLineWidth(float newValue)
    {
        currentLineRender.startWidth = newValue;
        currentLineRender.endWidth = newValue;
        lineDefaultWidth = newValue;
    }

    public void UpdateLineColor(Color color)
    {

        currentLineRender.material.color = color;

        if (currentLineRender.material.IsKeywordEnabled("_EmissionColor"))
        {
            currentLineRender.material.SetColor("_EmissionColor", color);
        }  
        defaultColor = color;

    }

    public void UpdateLineMaterial(string mat)
    {
        Material newMaterial=defaultLineMaterial;
        switch (mat)
        {
            case "plain":
                newMaterial = new Material(plainLineMaterial);
//                newMaterial.EnableKeyword("_EMISSION");
//                newMaterial.SetColor("_EmissionColor", defaultColor);
                break;
            case "wood":
                newMaterial = new Material(woodMaterial);
                break;
            case "metal":
                newMaterial = new Material(metalMaterial);
                break;
            default:
                break;
        }
        currentLineRender.material = newMaterial;
        defaultColor = Color.white;
        defaultLineMaterial = newMaterial;

    }

    public void UpdateLineMinDistance(float newValue)
    {
        minDistanceBeforeNewPoint = newValue;
    }

}
