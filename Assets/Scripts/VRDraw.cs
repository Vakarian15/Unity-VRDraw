using UnityEngine;
using System.Collections.Generic;


public class VRDraw : MonoBehaviour
{
    [SerializeField]
    private ControlHand controlHand = ControlHand.NoSet;

    [SerializeField]
    private GameObject objectToTrackMovement;

    private Vector3 prevPointDistance = Vector3.zero;

    [SerializeField, Range(0, 1.0f)]
    private float minDistanceBeforeNewPoint = 0.2f;

    [SerializeField, Range(0, 1.0f)]
    private float minDrawingPressure = 0.8f;

    [SerializeField, Range(0, 1.0f)]
    private float lineDefaultWidth = 0.010f;

    private int positionCount = 0; // 2 by default

    private List<LineRenderer> lines = new List<LineRenderer>();

    private LineRenderer currentLineRender;

    [SerializeField]
    private Color defaultColor = Color.white;

    [SerializeField]
    private GameObject editorObjectToTrackMovement;

    [SerializeField]
    private bool allowEditorControls = true;

    Material CreateMaterial(Color color, string name, string shaderName = "Standard")
    {
        Material material = new Material(Shader.Find(shaderName));
        material.name = name;
        material.color = color;
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", color);
        return material;
    }

    enum ControlHand
    {
        NoSet,
        Left,
        Right
    }

    void Awake()
    {
#if UNITY_EDITOR

        // if we allow editor controls use the editor object to track movement because oculus
        // blocks the movement of LeftControllerAnchor and RightControllerAnchor
        if (allowEditorControls)
        {
            objectToTrackMovement = editorObjectToTrackMovement != null ? editorObjectToTrackMovement : objectToTrackMovement;
        }

#endif

        AddNewLineRenderer();
    }

    void AddNewLineRenderer()
    {
        positionCount = 0;
        GameObject go = new GameObject($"LineRenderer_{controlHand.ToString()}_{lines.Count}");
        go.transform.parent = objectToTrackMovement.transform.parent;
        go.transform.position = objectToTrackMovement.transform.position;
        LineRenderer goLineRenderer = go.AddComponent<LineRenderer>();
        goLineRenderer.startWidth = lineDefaultWidth;
        goLineRenderer.endWidth = lineDefaultWidth;
        goLineRenderer.useWorldSpace = true;
        goLineRenderer.material = CreateMaterial(defaultColor, $"Material_{controlHand.ToString()}_{lines.Count}");
        goLineRenderer.positionCount = 1;
        goLineRenderer.numCapVertices = 90;
        goLineRenderer.SetPosition(0, objectToTrackMovement.transform.position);

        currentLineRender = goLineRenderer;
        lines.Add(goLineRenderer);
    }

    void Update()
    {

// #if !UNITY_EDITOR
        // primary left controller
        if(controlHand == ControlHand.Left && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > minDrawingPressure)
        {
            UpdateLine();
        }
        else if(controlHand == ControlHand.Left && OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
        {
            AddNewLineRenderer();
        }

        // secondary right controller
        if(controlHand == ControlHand.Right && OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > minDrawingPressure)
        {
            UpdateLine();
        }
        else if(controlHand == ControlHand.Right && OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
        {
            AddNewLineRenderer();
        }

// #endif
/*
#if UNITY_EDITOR
        if (!allowEditorControls) return;

        // left controller
        if (controlHand == ControlHand.Left && Input.GetKey(KeyCode.K))
        {
            UpdateLine();
        }
        else if (controlHand == ControlHand.Left && Input.GetKeyUp(KeyCode.K))
        {
            AddNewLineRenderer();
        }

        // right controller
        if (controlHand == ControlHand.Right && Input.GetKey(KeyCode.L))
        {
            UpdateLine();
        }
        else if (controlHand == ControlHand.Right && Input.GetKeyUp(KeyCode.L))
        {
            AddNewLineRenderer();
        }
#endif
*/
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
        // in case we haven't drawn anything
        if (currentLineRender.positionCount == 1)
        {
            currentLineRender.material.color = color;
            currentLineRender.material.EnableKeyword("_EMISSION");
            currentLineRender.material.SetColor("_EmissionColor", color);
        }
        defaultColor = color;
    }

    public void UpdateLineMinDistance(float newValue)
    {
        minDistanceBeforeNewPoint = newValue;
    }
}


