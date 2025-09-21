using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShadowLogicPuzzle : BasePuzzle
{
    [Header("Light Sources")]
    [Tooltip("Adjustable light sources")]
    public Light[] adjustableLights;
    
    [Tooltip("Sliders to control light rotation")]
    public Slider[] lightRotationSliders;
    
    [Tooltip("Sliders to control light intensity")]
    public Slider[] lightIntensitySliders;
    
    [Tooltip("Buttons to toggle lights on/off")]
    public Button[] lightToggleButtons;

    [Header("Shadow Objects")]
    [Tooltip("Objects that cast shadows")]
    public Transform[] shadowObjects;
    
    [Tooltip("Target positions for shadow objects")]
    public Transform[] shadowObjectTargets;
    
    [Tooltip("Buttons to move shadow objects")]
    public Button[] moveObjectButtons;

    [Header("Detection & Display")]
    [Tooltip("Camera for shadow detection")]
    public Camera shadowCamera;
    
    [Tooltip("Text display for the revealed shadow code")]
    public TMP_Text shadowCodeDisplay;
    
    [Tooltip("Display showing alignment status")]
    public TMP_Text alignmentStatusDisplay;
    
    [Tooltip("Submit button when code is revealed")]
    public Button submitButton;

    [Header("Target Configuration")]
    [Tooltip("Target rotation angles for each light (Y-axis)")]
    public float[] targetRotations = {45f, 135f, 225f};
    
    [Tooltip("Target intensity for each light")]
    public float[] targetIntensities = {1.0f, 0.8f, 1.2f};
    
    [Tooltip("Tolerance for rotation alignment")]
    public float rotationTolerance = 15f;
    
    [Tooltip("Tolerance for intensity alignment")]
    public float intensityTolerance = 0.2f;

    private int randomNumber;
    private bool[] lightsAligned = new bool[3];
    private bool[] objectsPositioned = new bool[3];
    private bool isCodeRevealed = false;

    public override void Initialize()
    {
        base.Initialize();
        randomNumber = Random.Range(3000, 9999);
        
        SetupLightControls();
        SetupObjectControls();
        SetupSubmitButton();
        ResetToInitialState();
        UpdateDisplay();
    }

    private void SetupLightControls()
    {
        // Setup rotation sliders
        for (int i = 0; i < lightRotationSliders.Length && i < adjustableLights.Length; i++)
        {
            int index = i;
            if (lightRotationSliders[i] != null)
            {
                lightRotationSliders[i].minValue = 0f;
                lightRotationSliders[i].maxValue = 360f;
                lightRotationSliders[i].value = 0f;
                lightRotationSliders[i].onValueChanged.AddListener((value) => RotateLight(index, value));
            }
        }

        // Setup intensity sliders
        for (int i = 0; i < lightIntensitySliders.Length && i < adjustableLights.Length; i++)
        {
            int index = i;
            if (lightIntensitySliders[i] != null)
            {
                lightIntensitySliders[i].minValue = 0f;
                lightIntensitySliders[i].maxValue = 2f;
                lightIntensitySliders[i].value = 1f;
                lightIntensitySliders[i].onValueChanged.AddListener((value) => AdjustIntensity(index, value));
            }
        }

        // Setup toggle buttons
        for (int i = 0; i < lightToggleButtons.Length && i < adjustableLights.Length; i++)
        {
            int index = i;
            if (lightToggleButtons[i] != null)
                lightToggleButtons[i].onClick.AddListener(() => ToggleLight(index));
        }
    }

    private void SetupObjectControls()
    {
        for (int i = 0; i < moveObjectButtons.Length && i < shadowObjects.Length; i++)
        {
            int index = i;
            if (moveObjectButtons[i] != null)
                moveObjectButtons[i].onClick.AddListener(() => MoveObject(index));
        }
    }

    private void SetupSubmitButton()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(SubmitSolution);
            submitButton.interactable = false; // Initially disabled
        }
    }

    private void ResetToInitialState()
    {
        // Reset light states
        for (int i = 0; i < adjustableLights.Length; i++)
        {
            if (adjustableLights[i] != null)
            {
                adjustableLights[i].transform.rotation = Quaternion.identity;
                adjustableLights[i].intensity = 1f;
                adjustableLights[i].enabled = true;
            }
        }

        // Reset object positions
        for (int i = 0; i < shadowObjects.Length; i++)
        {
            if (shadowObjects[i] != null)
            {
                // Move to a default position (not the target)
                shadowObjects[i].position = shadowObjects[i].position + Vector3.forward * 2f;
            }
        }

        isCodeRevealed = false;
        System.Array.Fill(lightsAligned, false);
        System.Array.Fill(objectsPositioned, false);
    }

    private void RotateLight(int lightIndex, float rotationValue)
    {
        if (lightIndex >= adjustableLights.Length || adjustableLights[lightIndex] == null)
            return;

        adjustableLights[lightIndex].transform.rotation = Quaternion.Euler(0, rotationValue, 0);
        CheckLightAlignment(lightIndex);
        CheckShadowAlignment();
    }

    private void AdjustIntensity(int lightIndex, float intensity)
    {
        if (lightIndex >= adjustableLights.Length || adjustableLights[lightIndex] == null)
            return;

        adjustableLights[lightIndex].intensity = intensity;
        CheckLightAlignment(lightIndex);
        CheckShadowAlignment();
    }

    private void ToggleLight(int lightIndex)
    {
        if (lightIndex >= adjustableLights.Length || adjustableLights[lightIndex] == null)
            return;

        adjustableLights[lightIndex].enabled = !adjustableLights[lightIndex].enabled;
        CheckLightAlignment(lightIndex);
        CheckShadowAlignment();
    }

    private void MoveObject(int objectIndex)
    {
        if (objectIndex >= shadowObjects.Length || shadowObjects[objectIndex] == null)
            return;

        if (objectIndex < shadowObjectTargets.Length && shadowObjectTargets[objectIndex] != null)
        {
            // Toggle between target position and offset position
            float distanceToTarget = Vector3.Distance(shadowObjects[objectIndex].position, 
                                                     shadowObjectTargets[objectIndex].position);
            
            if (distanceToTarget > 0.5f)
            {
                // Move to target
                shadowObjects[objectIndex].position = shadowObjectTargets[objectIndex].position;
                objectsPositioned[objectIndex] = true;
            }
            else
            {
                // Move away from target
                shadowObjects[objectIndex].position = shadowObjectTargets[objectIndex].position + Vector3.right * 2f;
                objectsPositioned[objectIndex] = false;
            }
        }

        CheckShadowAlignment();
    }

    private void CheckLightAlignment(int lightIndex)
    {
        if (lightIndex >= adjustableLights.Length || lightIndex >= targetRotations.Length)
            return;

        Light light = adjustableLights[lightIndex];
        if (light == null) return;

        // Check rotation alignment
        float currentRotation = light.transform.eulerAngles.y;
        float targetRotation = targetRotations[lightIndex];
        bool rotationAligned = Mathf.Abs(Mathf.DeltaAngle(currentRotation, targetRotation)) <= rotationTolerance;

        // Check intensity alignment
        float currentIntensity = light.intensity;
        float targetIntensity = targetIntensities[lightIndex];
        bool intensityAligned = Mathf.Abs(currentIntensity - targetIntensity) <= intensityTolerance;

        // Check if light is enabled
        bool lightEnabled = light.enabled;

        lightsAligned[lightIndex] = rotationAligned && intensityAligned && lightEnabled;
    }

    private void CheckShadowAlignment()
    {
        // Check all lights
        for (int i = 0; i < adjustableLights.Length; i++)
        {
            CheckLightAlignment(i);
        }

        // Count aligned elements
        int alignedLights = 0;
        int alignedObjects = 0;

        foreach (bool aligned in lightsAligned)
            if (aligned) alignedLights++;

        foreach (bool positioned in objectsPositioned)
            if (positioned) alignedObjects++;

        // Update status display
        UpdateAlignmentStatus(alignedLights, alignedObjects);

        // Check if puzzle is solved
        bool allLightsAligned = alignedLights == adjustableLights.Length;
        bool allObjectsPositioned = alignedObjects == shadowObjects.Length;

        if (allLightsAligned && allObjectsPositioned && !isCodeRevealed)
        {
            RevealShadowCode();
        }
        else if ((!allLightsAligned || !allObjectsPositioned) && isCodeRevealed)
        {
            HideShadowCode();
        }
    }

    private void UpdateAlignmentStatus(int alignedLights, int alignedObjects)
    {
        if (alignmentStatusDisplay != null)
        {
            string status = $"Shadow Alignment Status:\n";
            status += $"Lights Aligned: {alignedLights}/{adjustableLights.Length}\n";
            status += $"Objects Positioned: {alignedObjects}/{shadowObjects.Length}";
            alignmentStatusDisplay.text = status;
        }
    }

    private void RevealShadowCode()
    {
        isCodeRevealed = true;
        
        if (shadowCodeDisplay != null)
        {
            shadowCodeDisplay.text = randomNumber.ToString();
            shadowCodeDisplay.color = Color.green;
        }

        if (submitButton != null)
            submitButton.interactable = true;

        GiveFeedback("The shadows reveal the hidden number! Submit when ready.");
    }

    private void HideShadowCode()
    {
        isCodeRevealed = false;
        
        if (shadowCodeDisplay != null)
        {
            shadowCodeDisplay.text = "----";
            shadowCodeDisplay.color = Color.gray;
        }

        if (submitButton != null)
            submitButton.interactable = false;

        GiveFeedback("Shadow alignment lost. Realign the light sources and objects.");
    }

    private void SubmitSolution()
    {
        if (!isCodeRevealed) return;

        GiveFeedback($"Shadow code captured! Your escape number is: {randomNumber}");
        CompletePuzzle();

        // Notify the escape code manager
        EscapeCodeManager escapeManager = FindFirstObjectByType<EscapeCodeManager>();
        if (escapeManager != null)
            escapeManager.OnPuzzleCompleted(randomNumber);
    }

    private void UpdateDisplay()
    {
        UpdateAlignmentStatus(0, 0);
        
        if (shadowCodeDisplay != null)
        {
            shadowCodeDisplay.text = "----";
            shadowCodeDisplay.color = Color.gray;
        }
    }

    public override void ResetPuzzle()
    {
        base.ResetPuzzle();
        ResetToInitialState();
        UpdateDisplay();
    }

    public override void ShowPuzzle()
    {
        base.ShowPuzzle();
        GiveFeedback("Manipulate lights and objects to cast the correct shadow pattern.");
    }
}