using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ColorController : MonoBehaviour
{
    [SerializeField]
    private VRHandDraw leftHandDraw = null;

    [SerializeField]
    private VRHandDraw rightHandDraw=null;

    [SerializeField]
    private PaintingManager paintingManager = null;

    [SerializeField]
    private GameObject board;

    private bool isHandled = false;
    private bool isCleared = false;

    private void Awake()
    {
        Assert.IsNotNull(rightHandDraw);
        Assert.IsNotNull(leftHandDraw);
        Assert.IsNotNull(paintingManager);
    }

    public void ColorButtonPressed(string HTMLColor)
    {
        Color color;
        if(ColorUtility.TryParseHtmlString(HTMLColor, out color))
        {
            leftHandDraw.UpdateLineColor(color);
            rightHandDraw.UpdateLineColor(color);
        }      
    }

    public void MaterialButtonPressed(string mat)
    {
        leftHandDraw.UpdateLineMaterial(mat);
        rightHandDraw.UpdateLineMaterial(mat);
    }

    public void ClearButtonPressed()
    {
        if(!isCleared)
        {
            paintingManager.ClearPaintings();
        }
        isCleared = true;

    }

    public void OnColorPanelStateChanged()
    {
        if (!isHandled)
        {
            if (board.activeInHierarchy)
            {
                board.SetActive(false);
            }
            else
            {
                board.SetActive(true);
            }
            isHandled = true;
        }
    }

    public void OnNotRecognized()
    {
        isHandled = false;
        isCleared = false;
    }
}
