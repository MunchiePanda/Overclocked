using UnityEngine;

/// <summary>
/// Handles switching between different cameras in the scene.
/// </summary>
public class CameraSwitcher : MonoBehaviour
{
    [Tooltip("The main game camera")]
    public Camera mainCamera;

    [Tooltip("The camera to use when the terminal is active")]
    public Camera terminalCamera;

    [Tooltip("Should the camera switcher be active?")]
    public bool isActive = false;

    /// <summary>
    /// Toggle between the main camera and terminal camera
    /// </summary>
    public void ToggleCamera()
    {
        isActive = !isActive;

        if (isActive)
        {
            SwitchToTerminalCamera();
        }
        else
        {
            SwitchToMainCamera();
        }
    }

    /// <summary>
    /// Switch to the terminal camera view
    /// </summary>
    public void SwitchToTerminalCamera()
    {
        if (mainCamera == null || terminalCamera == null)
        {
            Debug.LogError("Camera references not set in CameraSwitcher!");
            return;
        }

        // Store the main camera's settings
        bool mainCameraWasEnabled = mainCamera.enabled;
        int mainCameraDepth = (int)mainCamera.depth;
        string mainCameraTag = mainCamera.tag;

        // Make terminal camera the main camera by setting its tag
        terminalCamera.tag = "MainCamera";
        terminalCamera.enabled = true;

        // Set up terminal camera for UI rendering
        terminalCamera.clearFlags = CameraClearFlags.SolidColor;
        terminalCamera.backgroundColor = Color.black;

        // If we had a previous main camera, disable it
        if (mainCamera != terminalCamera)
        {
            mainCamera.tag = "Untagged"; // Remove MainCamera tag
            mainCamera.enabled = false;
        }

        Debug.Log("Switched to terminal camera: " + terminalCamera.name);
    }

    /// <summary>
    /// Switch back to the main camera view
    /// </summary>
    public void SwitchToMainCamera()
    {
        if (mainCamera == null || terminalCamera == null)
        {
            Debug.LogError("Camera references not set in CameraSwitcher!");
            return;
        }

        // Restore main camera
        mainCamera.tag = "MainCamera";
        mainCamera.enabled = true;

        // Deactivate terminal camera
        terminalCamera.tag = "Untagged";
        terminalCamera.enabled = false;

        Debug.Log("Switched back to main camera: " + mainCamera.name);
    }

    /// <summary>
    /// Set the main camera reference
    /// </summary>
    public void SetMainCamera(Camera camera)
    {
        mainCamera = camera;
    }

    /// <summary>
    /// Set the terminal camera reference
    /// </summary>
    public void SetTerminalCamera(Camera camera)
    {
        terminalCamera = camera;
    }

    /// <summary>
    /// Check if the terminal camera is currently active
    /// </summary>
    public bool IsTerminalCameraActive()
    {
        return isActive;
    }
}
