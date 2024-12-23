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
            Debug.Log($"Orientation changed from {lastOrientation} to {Screen.orientation}");
            OnOrientationChange(Screen.orientation);
            lastOrientation = Screen.orientation;
        }

        // Detect screen size changes (web/desktop)
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            Debug.Log($"Screen size changed: {Screen.width}x{Screen.height}");
            UpdateOrientation();
            lastWidth = Screen.width;
            lastHeight = Screen.height;
        }
        // if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || 
        //     Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        // {
        //     EnableLandscape();
        //     // Get all objects in the scene that inherit from the OrientationManager class
        //     OrientationManager[] orientationManagers = FindObjectsOfType<OrientationManager>();
        //     foreach (OrientationManager obstacle in orientationManagers)
        // {
        //     Debug.Log($"Found obstacle: {obstacle.name}");
        // }
        // }
        // else if (Input.deviceOrientation == DeviceOrientation.Portrait || 
        //          Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
        // {
        //     EnablePortrait();
        // }
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

    // public GameObject GetUIElement(string elementName)
    // {
    //     if (currentCanvas != null)
    //     {
    //         Transform element = currentCanvas.transform.Find(elementName);
    //         if (element != null)
    //             return element.gameObject;
    //     }
    //     return null;
    // }
    public string GetCurrentCanvasName() {
        return currentCanvas.name;
    }
}
