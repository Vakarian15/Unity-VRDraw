using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingManager : MonoBehaviour
{
    public List<GameObject> paintings { get; private set; } = new List<GameObject>();

    [SerializeField]
    private VRHandDraw leftHandDraw;
    [SerializeField]
    private VRHandDraw rightHandDraw;

    public void Awake()
    {
        AddNewPainting();
        SetCurPainting();
    }

    public void AddNewPainting()
    {
        GameObject newPainting = GameObject.Find($"/Paintings/Painting_{paintings.Count}");
        if (newPainting == null)
        {
            newPainting = new GameObject($"Painting_{paintings.Count}");
            newPainting.transform.parent = transform;
            paintings.Add(newPainting);
        }
    }

    public void SetCurPainting()
    {
        GameObject newPainting = GameObject.Find($"/Paintings/Painting_{paintings.Count-1}");
        leftHandDraw.currentPainting = newPainting;
        rightHandDraw.currentPainting = newPainting;
        leftHandDraw.AddNewLineRenderer();
        rightHandDraw.AddNewLineRenderer();
        AddNewPainting();

    }

    public void ClearPaintings()
    {

        if (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
            SetCurPainting();
        }
    }
}
