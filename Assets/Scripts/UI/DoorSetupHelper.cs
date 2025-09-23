using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Comprehensive setup helper for door interaction system
/// </summary>
public class DoorSetupHelper : MonoBehaviour
{
    [Header("🚪 Door Setup Helper")]
    [Tooltip("Auto-setup door interaction on start")]
    public bool autoSetupOnStart = true;
    
    [Tooltip("Create UI if not found")]
    public bool createUIIfMissing = true;
    
    [Tooltip("Show setup progress logs")]
    public bool showSetupLogs = true;
    
    [Header("🎯 Target Door")]
    [Tooltip("Door to setup (leave empty to find automatically)")]
    public Door targetDoor;
    
    [Header("🖥️ UI Settings")]
    [Tooltip("Canvas for door UI (leave empty to create)")]
    public Canvas doorUICanvas;
    
    [Tooltip("Interaction prompt position offset")]
    public Vector3 promptOffset = new Vector3(0, 2, 0);
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupDoorInteraction();
        }
    }
    
    [ContextMenu("🚀 Setup Complete Door Interaction")]
    public void SetupDoorInteraction()
    {
        if (showSetupLogs)
            Debug.Log("🚪 Setting up door interaction system...");
        
        // Step 1: Find or validate door
        SetupDoorComponent();
        
        // Step 2: Setup door interaction helper
        SetupDoorInteractionHelper();
        
        // Step 3: Setup or create UI
        SetupDoorUI();
        
        // Step 4: Setup interaction prompt
        SetupInteractionPrompt();
        
        // Step 5: Configure settings
        ConfigureDoorSettings();
        
        if (showSetupLogs)
        {
            Debug.Log("✅ Door interaction system setup complete!");
            Debug.Log("🎮 Walk up to door and press E to interact");
            Debug.Log("🖥️ UI will appear with cursor management");
            Debug.Log("🔑 Enter correct code to unlock door");
        }
    }
    
    void SetupDoorComponent()
    {
        // Find door if not assigned
        if (targetDoor == null)
        {
            targetDoor = FindFirstObjectByType<Door>();
            
            if (targetDoor == null)
            {
                // Look for GameObject named "Door"
                GameObject doorObject = GameObject.Find("Door");
                if (doorObject != null)
                {
                    targetDoor = doorObject.GetComponent<Door>();
                    
                    if (targetDoor == null)
                    {
                        // Add Door component
                        targetDoor = doorObject.AddComponent<Door>();
                        if (showSetupLogs)
                            Debug.Log("✅ Added Door component to Door GameObject");
                    }
                }
            }
        }
        
        if (targetDoor == null)
        {
            Debug.LogError("❌ No Door found! Please assign a door or create a GameObject named 'Door'");
            return;
        }
        
        // Ensure door is on Interactable layer
        if (targetDoor.gameObject.layer != LayerMask.NameToLayer("Interactable"))
        {
            targetDoor.gameObject.layer = LayerMask.NameToLayer("Interactable");
            if (showSetupLogs)
                Debug.Log("✅ Set door to Interactable layer");
        }
        
        // Add collider if missing
        Collider doorCollider = targetDoor.GetComponent<Collider>();
        if (doorCollider == null)
        {
            BoxCollider boxCollider = targetDoor.gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = false; // Physical collision for the door
            if (showSetupLogs)
                Debug.Log("✅ Added BoxCollider to door");
        }
        
        if (showSetupLogs)
            Debug.Log($"✅ Door component validated: {targetDoor.name}");
    }
    
    void SetupDoorInteractionHelper()
    {
        if (targetDoor == null) return;
        
        // Check if interaction helper already exists
        DoorInteractionHelper interactionHelper = targetDoor.GetComponent<DoorInteractionHelper>();
        
        if (interactionHelper == null)
        {
            interactionHelper = targetDoor.gameObject.AddComponent<DoorInteractionHelper>();
            if (showSetupLogs)
                Debug.Log("✅ Added DoorInteractionHelper component");
        }
        
        // Configure interaction helper
        interactionHelper.interactionDistance = 3f;
        interactionHelper.enableDebugLogs = showSetupLogs;
        interactionHelper.showInteractionGizmo = true;
        
        if (showSetupLogs)
            Debug.Log("✅ Door interaction helper configured");
    }
    
    void SetupDoorUI()
    {
        if (targetDoor == null) return;
        
        // Find existing door UI
        if (doorUICanvas == null)
        {
            doorUICanvas = GameObject.Find("DoorUICanvas")?.GetComponent<Canvas>();
            
            if (doorUICanvas == null)
            {
                // Look in door children
                doorUICanvas = targetDoor.GetComponentInChildren<Canvas>();
            }
        }
        
        // Create UI if missing and requested
        if (doorUICanvas == null && createUIIfMissing)
        {
            CreateDoorUI();
        }
        
        // Assign UI to interaction helper
        DoorInteractionHelper interactionHelper = targetDoor.GetComponent<DoorInteractionHelper>();
        if (interactionHelper != null && doorUICanvas != null)
        {
            interactionHelper.doorUICanvas = doorUICanvas;
            
            // Auto-assign UI components
            interactionHelper.codeInputField = doorUICanvas.GetComponentInChildren<TMP_InputField>();
            
            Button[] buttons = doorUICanvas.GetComponentsInChildren<Button>();
            foreach (var btn in buttons)
            {
                if (btn.name.ToLower().Contains("submit"))
                    interactionHelper.submitButton = btn;
                else if (btn.name.ToLower().Contains("close") || btn.name.ToLower().Contains("cancel"))
                    interactionHelper.closeButton = btn;
            }
            
            interactionHelper.feedbackText = doorUICanvas.GetComponentInChildren<TMP_Text>();
            
            if (showSetupLogs)
                Debug.Log("✅ Door UI components auto-assigned");
        }
    }
    
    void CreateDoorUI()
    {
        if (showSetupLogs)
            Debug.Log("🏗️ Creating door UI from scratch...");
        
        // Create canvas
        GameObject canvasObject = new GameObject("DoorUICanvas");
        doorUICanvas = canvasObject.AddComponent<Canvas>();
        doorUICanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        doorUICanvas.sortingOrder = 10;
        
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
        
        // Create main panel
        GameObject panelObject = new GameObject("DoorPanel");
        panelObject.transform.SetParent(canvasObject.transform, false);
        
        Image panelImage = panelObject.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);
        
        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.3f, 0.3f);
        panelRect.anchorMax = new Vector2(0.7f, 0.7f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Create title
        GameObject titleObject = new GameObject("Title");
        titleObject.transform.SetParent(panelObject.transform, false);
        
        TMP_Text titleText = titleObject.AddComponent<TextMeshProUGUI>();
        titleText.text = "Enter Door Code";
        titleText.fontSize = 24;
        titleText.alignment = TextAlignmentOptions.Center;
        
        RectTransform titleRect = titleObject.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.7f);
        titleRect.anchorMax = new Vector2(1, 0.9f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        // Create input field
        GameObject inputObject = new GameObject("CodeInput");
        inputObject.transform.SetParent(panelObject.transform, false);
        
        Image inputImage = inputObject.AddComponent<Image>();
        inputImage.color = Color.white;
        
        TMP_InputField inputField = inputObject.AddComponent<TMP_InputField>();
        inputField.placeholder = CreatePlaceholder(inputObject.transform, "Enter code...");
        inputField.textComponent = CreateInputText(inputObject.transform);
        
        RectTransform inputRect = inputObject.GetComponent<RectTransform>();
        inputRect.anchorMin = new Vector2(0.1f, 0.5f);
        inputRect.anchorMax = new Vector2(0.9f, 0.65f);
        inputRect.offsetMin = Vector2.zero;
        inputRect.offsetMax = Vector2.zero;
        
        // Create feedback text
        GameObject feedbackObject = new GameObject("Feedback");
        feedbackObject.transform.SetParent(panelObject.transform, false);
        
        TMP_Text feedbackText = feedbackObject.AddComponent<TextMeshProUGUI>();
        feedbackText.text = "Enter door code:";
        feedbackText.fontSize = 18;
        feedbackText.alignment = TextAlignmentOptions.Center;
        feedbackText.color = Color.white;
        
        RectTransform feedbackRect = feedbackObject.GetComponent<RectTransform>();
        feedbackRect.anchorMin = new Vector2(0, 0.35f);
        feedbackRect.anchorMax = new Vector2(1, 0.45f);
        feedbackRect.offsetMin = Vector2.zero;
        feedbackRect.offsetMax = Vector2.zero;
        
        // Create submit button
        CreateUIButton(panelObject.transform, "SubmitButton", "Submit", new Vector2(0.1f, 0.1f), new Vector2(0.45f, 0.25f));
        
        // Create close button
        CreateUIButton(panelObject.transform, "CloseButton", "Close", new Vector2(0.55f, 0.1f), new Vector2(0.9f, 0.25f));
        
        // Initially hide the UI
        canvasObject.SetActive(false);
        
        if (showSetupLogs)
            Debug.Log("✅ Door UI created successfully");
    }
    
    TMP_Text CreatePlaceholder(Transform parent, string text)
    {
        GameObject placeholderObject = new GameObject("Placeholder");
        placeholderObject.transform.SetParent(parent, false);
        
        TMP_Text placeholder = placeholderObject.AddComponent<TextMeshProUGUI>();
        placeholder.text = text;
        placeholder.fontSize = 16;
        placeholder.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        placeholder.alignment = TextAlignmentOptions.MidlineLeft;
        
        RectTransform rect = placeholderObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(10, 0);
        rect.offsetMax = new Vector2(-10, 0);
        
        return placeholder;
    }
    
    TMP_Text CreateInputText(Transform parent)
    {
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(parent, false);
        
        TMP_Text text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = "";
        text.fontSize = 16;
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.MidlineLeft;
        
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(10, 0);
        rect.offsetMax = new Vector2(-10, 0);
        
        return text;
    }
    
    void CreateUIButton(Transform parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(parent, false);
        
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 1f, 1f);
        
        Button button = buttonObject.AddComponent<Button>();
        
        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = anchorMin;
        buttonRect.anchorMax = anchorMax;
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;
        
        // Create button text
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(buttonObject.transform, false);
        
        TMP_Text buttonText = textObject.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 16;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        button.targetGraphic = buttonImage;
    }
    
    void SetupInteractionPrompt()
    {
        if (targetDoor == null) return;
        
        // Look for existing interaction prompt
        GameObject existingPrompt = targetDoor.transform.Find("InteractionPrompt")?.gameObject;
        
        if (existingPrompt == null)
        {
            // Create interaction prompt
            GameObject promptObject = new GameObject("InteractionPrompt");
            promptObject.transform.SetParent(targetDoor.transform, false);
            promptObject.transform.localPosition = promptOffset;
            
            // Create canvas for world space UI
            Canvas promptCanvas = promptObject.AddComponent<Canvas>();
            promptCanvas.renderMode = RenderMode.WorldSpace;
            promptCanvas.worldCamera = Camera.main;
            
            RectTransform canvasRect = promptObject.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(2, 0.5f);
            
            // Create text
            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(promptObject.transform, false);
            
            TMP_Text promptText = textObject.AddComponent<TextMeshProUGUI>();
            promptText.text = "Press E to open door";
            promptText.fontSize = 36;
            promptText.alignment = TextAlignmentOptions.Center;
            promptText.color = Color.white;
            
            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            // Add background
            Image background = promptObject.AddComponent<Image>();
            background.color = new Color(0, 0, 0, 0.7f);
            
            // Initially hide
            promptObject.SetActive(false);
            
            // Assign to interaction helper
            DoorInteractionHelper interactionHelper = targetDoor.GetComponent<DoorInteractionHelper>();
            if (interactionHelper != null)
            {
                interactionHelper.interactionPrompt = promptObject;
                interactionHelper.interactionPromptText = promptText;
            }
            
            if (showSetupLogs)
                Debug.Log("✅ Created interaction prompt");
        }
        else
        {
            if (showSetupLogs)
                Debug.Log("✅ Found existing interaction prompt");
        }
    }
    
    void ConfigureDoorSettings()
    {
        if (targetDoor == null) return;
        
        // Configure door with default settings
        targetDoor.correctCode = "1234"; // You can change this
        
        if (showSetupLogs)
            Debug.Log($"✅ Door configured with code: {targetDoor.correctCode}");
    }
    
    [ContextMenu("🔍 Find Door Components")]
    public void FindDoorComponents()
    {
        targetDoor = FindFirstObjectByType<Door>();
        if (targetDoor != null)
        {
            Debug.Log($"✅ Found door: {targetDoor.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ No Door component found in scene");
        }
    }
    
    [ContextMenu("🏗️ Create Door UI Only")]
    public void CreateDoorUIOnly()
    {
        createUIIfMissing = true;
        SetupDoorUI();
    }
    
    [ContextMenu("🧪 Test Door Interaction")]
    public void TestDoorInteraction()
    {
        if (targetDoor != null)
        {
            DoorInteractionHelper helper = targetDoor.GetComponent<DoorInteractionHelper>();
            if (helper != null)
            {
                helper.TestShowDoorUI();
            }
        }
    }
}