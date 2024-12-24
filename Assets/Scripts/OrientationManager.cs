using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OrientationManager : MonoBehaviour
{
    public GameObject landscapeCanvas; // Assign the landscape Canvas in the Inspector
    public GameObject portraitCanvas;  // Assign the portrait Canvas in the Inspector
    public GameObject currentCanvas;

    private ScreenOrientation lastOrientation;
    private int lastWidth;
    private int lastHeight;

    private void Awake() 
    {
        UpdateOrientation();
    }

    private void Start()
    {
        // Store the initial orientation and screen size
        lastOrientation = Screen.orientation;
        lastWidth = Screen.width;
        lastHeight = Screen.height;
        UpdateOrientation();
    }

    private void Update()
    {
        // Detect orientation changes (mobile)
        if (Screen.orientation != lastOrientation)
        {
            OnOrientationChange(Screen.orientation);
            lastOrientation = Screen.orientation;
        }

        // Detect screen size changes (web/desktop)
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            UpdateOrientation();
            lastWidth = Screen.width;
            lastHeight = Screen.height;
        }
    }

    private void OnOrientationChange(ScreenOrientation newOrientation)
    {
        if (newOrientation == ScreenOrientation.Portrait || newOrientation == ScreenOrientation.PortraitUpsideDown)
        {
            EnablePortrait();
        }
        else if (newOrientation == ScreenOrientation.LandscapeLeft || newOrientation == ScreenOrientation.LandscapeRight)
        {
            EnableLandscape();
        }
    }

    private void EnableLandscape()
    {
        landscapeCanvas.SetActive(true);
        portraitCanvas.SetActive(false);
        currentCanvas = landscapeCanvas;
        UserInterfaceManager[] uiManagers = FindObjectsOfType<UserInterfaceManager>();
        foreach (UserInterfaceManager uiManger in uiManagers) {
            uiManger.SetToLandscapeElements();
        }
    }

    private void EnablePortrait()
    {
        portraitCanvas.SetActive(true);
        landscapeCanvas.SetActive(false);
        currentCanvas = portraitCanvas;
        UserInterfaceManager[] uiManagers = FindObjectsOfType<UserInterfaceManager>();
        foreach (UserInterfaceManager uiManger in uiManagers) {
            uiManger.SetToPortraitElements();
        }
    }

    private void UpdateOrientation()
    {
        if (Screen.width > Screen.height)
            EnableLandscape();
        else
            EnablePortrait();
    }

    public string GetCurrentCanvasName() {
        return currentCanvas.name;
    }
}
