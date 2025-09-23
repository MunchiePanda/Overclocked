using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Complete door setup helper with mesh creation, UI, and escape logic integration
/// Creates a fully functional door with keypad interface and escape room integration
/// </summary>
public class CompleteDoorSetupHelper : MonoBehaviour
{
    [Header("🚪 Complete Door Setup Helper")]
    [Tooltip("Auto-setup everything on start")]
    public bool autoSetupOnStart = true;
    
    [Tooltip("Show detailed setup logs")]
    public bool showDetailedLogs = true;
    
    [Header("🎯 Door Configuration")]
    [Tooltip("Position for the door")]
    public Vector3 doorPosition = new Vector3(15f, 0f, -46f);
    
    [Tooltip("Door mesh scale")]
    public Vector3 doorScale = new Vector3(2f, 3f, 0.2f);
    
    [Tooltip("Door material color when locked")]
    public Color lockedColor = new Color(0.8f, 0.2f, 0.2f);
    
    [Tooltip("Door material color when unlocked")]
    public Color unlockedColor = new Color(0.2f, 0.8f, 0.2f);
    
    [Header("🔢 Keypad Configuration")]
    [Tooltip("Position offset for the keypad from door")]
    public Vector3 keypadOffset = new Vector3(0.8f, 0f, 0.1f);
    
    [Tooltip("Keypad button size")]
    public float keypadButtonSize = 0.3f;
    
    [Header("🖥️ UI Configuration")]
    [Tooltip("UI panel position (screen space)")]
    public Vector2 uiPanelPosition = new Vector2(0.5f, 0.5f);
    
    [Tooltip("UI panel size")]
    public Vector2 uiPanelSize = new Vector2(400f, 600f);
    
    [Header("🎮 Integration Settings")]
    [Tooltip("Required number of puzzles for escape code")]
    public int requiredPuzzleCount = 3;
    
    [Tooltip("Interaction distance")]
    public float interactionDistance = 3f;
    
    [Header("🎨 Visual Effects")]
    [Tooltip("Door opening animation speed")]
    public float doorOpenSpeed = 2f;
    
    [Tooltip("Keypad button glow color")]
    public Color keypadGlowColor = new Color(0.3f, 0.7f, 1f);
    
    // Internal references
    private GameObject doorObject;
    private GameObject keypadObject;
    private Canvas doorUICanvas;
    private EscapeCodeManager escapeManager;
    private Door doorComponent;
    private DoorCodeInput codeInputComponent;

    void Start()
    {
        if (autoSetupOnStart)
        {
            StartCoroutine(SetupCompleteEscapeDoorSystem());
        }
    }

    [ContextMenu("🚀 Create Complete Escape Door System")]
    public void CreateCompleteEscapeDoorSystem()
    {
        StartCoroutine(SetupCompleteEscapeDoorSystem());
    }

    private IEnumerator SetupCompleteEscapeDoorSystem()
    {
        if (showDetailedLogs)
            Debug.Log("🚪 Creating complete escape door system...");

        // Step 1: Create or find escape code manager
        yield return StartCoroutine(SetupEscapeCodeManager());
        
        // Step 2: Create door with mesh
        yield return StartCoroutine(CreateDoorWithMesh());
        
        // Step 3: Create keypad
        yield return StartCoroutine(CreateKeypad());
        
        // Step 4: Create UI system
        yield return StartCoroutine(CreateAdvancedUI());
        
        // Step 5: Setup interaction system
        yield return StartCoroutine(SetupInteractionSystem());
        
        // Step 6: Connect to win screen
        yield return StartCoroutine(ConnectToWinScreen());
        
        // Step 7: Final configuration
        yield return StartCoroutine(FinalConfiguration());

        if (showDetailedLogs)
        {
            Debug.Log("✅ Complete escape door system created!");
            Debug.Log("🎮 How to use:");
            Debug.Log("   • Complete puzzles to collect numbers");
            Debug.Log("   • Walk up to door and press E to interact");
            Debug.Log("   • Enter the escape code using keypad or input field");
            Debug.Log("   • Door unlocks and win screen appears on success!");
        }
    }

    private IEnumerator SetupEscapeCodeManager()
    {
        escapeManager = FindFirstObjectByType<EscapeCodeManager>();
        
        if (escapeManager == null)
        {
            GameObject managerObject = GameObject.Find("EscapeCodeManager");
            if (managerObject == null)
            {
                managerObject = new GameObject("EscapeCodeManager");
            }
            
            escapeManager = managerObject.GetComponent<EscapeCodeManager>();
            if (escapeManager == null)
            {
                escapeManager = managerObject.AddComponent<EscapeCodeManager>();
            }
            
            escapeManager.requiredPuzzleCount = requiredPuzzleCount;
            
            if (showDetailedLogs)
                Debug.Log("✅ Created EscapeCodeManager");
        }
        else
        {
            if (showDetailedLogs)
                Debug.Log("✅ Found existing EscapeCodeManager");
        }
        
        yield return null;
    }

    private IEnumerator CreateDoorWithMesh()
    {
        // Find existing door or create new one
        doorObject = GameObject.Find("EscapeDoor");
        if (doorObject == null)
        {
            doorObject = new GameObject("EscapeDoor");
        }
        
        doorObject.transform.position = doorPosition;
        doorObject.layer = LayerMask.NameToLayer("Interactable");
        
        // Create door mesh
        if (doorObject.GetComponent<MeshRenderer>() == null)
        {
            MeshRenderer meshRenderer = doorObject.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = doorObject.AddComponent<MeshFilter>();
            
            // Create cube mesh
            meshFilter.mesh = CreateCubeMesh();
            
            // Create material
            Material doorMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            doorMaterial.color = lockedColor;
            meshRenderer.material = doorMaterial;
            
            if (showDetailedLogs)
                Debug.Log("✅ Created door mesh and material");
        }
        
        doorObject.transform.localScale = doorScale;
        
        // Add collider
        if (doorObject.GetComponent<Collider>() == null)
        {
            BoxCollider doorCollider = doorObject.AddComponent<BoxCollider>();
            doorCollider.isTrigger = false;
        }
        
        // Add Door component
        doorComponent = doorObject.GetComponent<Door>();
        if (doorComponent == null)
        {
            doorComponent = doorObject.AddComponent<Door>();
        }
        
        // Configure door properties
        doorComponent.correctCode = "0000"; // Will be set by escape manager
        doorComponent.lockedColor = lockedColor;
        doorComponent.unlockedColor = unlockedColor;
        
        yield return null;
    }

    private IEnumerator CreateKeypad()
    {
        // Create keypad object
        keypadObject = new GameObject("DoorKeypad");
        keypadObject.transform.SetParent(doorObject.transform);
        keypadObject.transform.localPosition = keypadOffset;
        
        // Create keypad background
        GameObject keypadBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        keypadBackground.name = "KeypadBackground";
        keypadBackground.transform.SetParent(keypadObject.transform, false);
        keypadBackground.transform.localScale = new Vector3(0.8f, 1.2f, 0.1f);
        
        // Set keypad material
        MeshRenderer keypadRenderer = keypadBackground.GetComponent<MeshRenderer>();
        Material keypadMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        keypadMaterial.color = new Color(0.1f, 0.1f, 0.1f);
        keypadRenderer.material = keypadMaterial;
        
        // Remove collider from background
        Destroy(keypadBackground.GetComponent<Collider>());
        
        // Create keypad buttons
        CreateKeypadButtons();
        
        if (showDetailedLogs)
            Debug.Log("✅ Created physical keypad");
        
        yield return null;
    }

    private void CreateKeypadButtons()
    {
        // Create number buttons in 3x4 grid
        int buttonIndex = 1;
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (row == 3 && col != 1) continue; // Only center button on bottom row (0)
                
                int number = (row == 3) ? 0 : buttonIndex;
                Vector3 buttonPos = new Vector3(
                    (col - 1) * keypadButtonSize * 1.2f,
                    (1.5f - row) * keypadButtonSize * 1.2f,
                    0.05f
                );
                
                CreateKeypadButton(number, buttonPos);
                
                if (row != 3) buttonIndex++;
            }
        }
        
        // Create submit and clear buttons
        CreateKeypadButton(-1, new Vector3(-keypadButtonSize * 1.2f, -1.8f * keypadButtonSize * 1.2f, 0.05f)); // Clear
        CreateKeypadButton(-2, new Vector3(keypadButtonSize * 1.2f, -1.8f * keypadButtonSize * 1.2f, 0.05f)); // Submit
    }

    private void CreateKeypadButton(int number, Vector3 position)
    {
        GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);
        button.name = number >= 0 ? $"Button_{number}" : (number == -1 ? "ClearButton" : "SubmitButton");
        button.transform.SetParent(keypadObject.transform, false);
        button.transform.localPosition = position;
        button.transform.localScale = Vector3.one * keypadButtonSize;
        
        // Set button material
        MeshRenderer buttonRenderer = button.GetComponent<MeshRenderer>();
        Material buttonMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        buttonMaterial.color = number >= 0 ? Color.white : keypadGlowColor;
        buttonRenderer.material = buttonMaterial;
        
        // Add text to button
        CreateButtonText(button, number);
        
        // Add interaction
        button.layer = LayerMask.NameToLayer("Interactable");
        
        // Store reference for later use
        button.AddComponent<KeypadButton>().Initialize(number, this);
    }

    private void CreateButtonText(GameObject button, int number)
    {
        GameObject textObject = new GameObject("ButtonText");
        textObject.transform.SetParent(button.transform, false);
        textObject.transform.localPosition = new Vector3(0, 0, 0.51f);
        
        // Create canvas for text
        Canvas textCanvas = textObject.AddComponent<Canvas>();
        textCanvas.renderMode = RenderMode.WorldSpace;
        textCanvas.worldCamera = Camera.main;
        
        RectTransform canvasRect = textObject.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(0.8f, 0.8f);
        
        // Create text
        GameObject textChild = new GameObject("Text");
        textChild.transform.SetParent(textObject.transform, false);
        
        TMP_Text text = textChild.AddComponent<TextMeshProUGUI>();
        text.text = number >= 0 ? number.ToString() : (number == -1 ? "C" : "✓");
        text.fontSize = 48;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.black;
        
        RectTransform textRect = textChild.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }

    private IEnumerator CreateAdvancedUI()
    {
        // Create UI Canvas
        GameObject canvasObject = new GameObject("DoorUICanvas");
        doorUICanvas = canvasObject.AddComponent<Canvas>();
        doorUICanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        doorUICanvas.sortingOrder = 100;
        
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
        
        // Create main panel
        GameObject panelObject = CreateUIPanel();
        
        // Create UI elements
        CreateUITitle(panelObject);
        CreateUIInputField(panelObject);
        CreateUIKeypad(panelObject);
        CreateUIFeedback(panelObject);
        CreateUIButtons(panelObject);
        CreateUIStatusDisplay(panelObject);
        
        // Add DoorCodeInput component
        codeInputComponent = canvasObject.AddComponent<DoorCodeInput>();
        ConfigureDoorCodeInput();
        
        // Initially hide the UI
        canvasObject.SetActive(false);
        
        if (showDetailedLogs)
            Debug.Log("✅ Created advanced door UI");
        
        yield return null;
    }

    private GameObject CreateUIPanel()
    {
        GameObject panelObject = new GameObject("DoorPanel");
        panelObject.transform.SetParent(doorUICanvas.transform, false);
        
        Image panelImage = panelObject.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.9f);
        
        RectTransform panelRect = panelObject.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = uiPanelSize;
        panelRect.anchoredPosition = Vector2.zero;
        
        return panelObject;
    }

    private void CreateUITitle(GameObject parent)
    {
        GameObject titleObject = new GameObject("Title");
        titleObject.transform.SetParent(parent.transform, false);
        
        TMP_Text titleText = titleObject.AddComponent<TextMeshProUGUI>();
        titleText.text = "🚪 ESCAPE DOOR ACCESS";
        titleText.fontSize = 28;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        
        RectTransform titleRect = titleObject.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.85f);
        titleRect.anchorMax = new Vector2(1, 0.95f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
    }

    private void CreateUIInputField(GameObject parent)
    {
        GameObject inputObject = new GameObject("CodeInput");
        inputObject.transform.SetParent(parent.transform, false);
        
        Image inputImage = inputObject.AddComponent<Image>();
        inputImage.color = Color.white;
        
        TMP_InputField inputField = inputObject.AddComponent<TMP_InputField>();
        inputField.placeholder = CreatePlaceholder(inputObject.transform, "Enter access code...");
        inputField.textComponent = CreateInputText(inputObject.transform);
        inputField.characterLimit = 8;
        
        RectTransform inputRect = inputObject.GetComponent<RectTransform>();
        inputRect.anchorMin = new Vector2(0.1f, 0.7f);
        inputRect.anchorMax = new Vector2(0.9f, 0.8f);
        inputRect.offsetMin = Vector2.zero;
        inputRect.offsetMax = Vector2.zero;
    }

    private void CreateUIKeypad(GameObject parent)
    {
        GameObject keypadContainer = new GameObject("UIKeypad");
        keypadContainer.transform.SetParent(parent.transform, false);
        
        RectTransform keypadRect = keypadContainer.GetComponent<RectTransform>();
        keypadRect.anchorMin = new Vector2(0.1f, 0.25f);
        keypadRect.anchorMax = new Vector2(0.9f, 0.65f);
        keypadRect.offsetMin = Vector2.zero;
        keypadRect.offsetMax = Vector2.zero;
        
        // Create grid layout
        GridLayoutGroup grid = keypadContainer.AddComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 3;
        grid.spacing = new Vector2(10, 10);
        grid.cellSize = new Vector2(80, 60);
        
        // Create number buttons
        for (int i = 1; i <= 9; i++)
        {
            CreateUIKeypadButton(keypadContainer, i.ToString(), i);
        }
        
        // Create bottom row
        CreateUIKeypadButton(keypadContainer, "CLEAR", -1);
        CreateUIKeypadButton(keypadContainer, "0", 0);
        CreateUIKeypadButton(keypadContainer, "ENTER", -2);
    }

    private void CreateUIKeypadButton(GameObject parent, string text, int value)
    {
        GameObject buttonObject = new GameObject($"KeypadButton_{text}");
        buttonObject.transform.SetParent(parent.transform, false);
        
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = value >= 0 ? new Color(0.3f, 0.3f, 0.3f) : keypadGlowColor;
        
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        
        // Create button text
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(buttonObject.transform, false);
        
        TMP_Text buttonText = textObject.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 18;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Store button reference
        KeypadUIButton uiButton = buttonObject.AddComponent<KeypadUIButton>();
        uiButton.Initialize(value, this);
    }

    private void CreateUIFeedback(GameObject parent)
    {
        GameObject feedbackObject = new GameObject("FeedbackText");
        feedbackObject.transform.SetParent(parent.transform, false);
        
        TMP_Text feedbackText = feedbackObject.AddComponent<TextMeshProUGUI>();
        feedbackText.text = "Complete all puzzles to access door";
        feedbackText.fontSize = 16;
        feedbackText.alignment = TextAlignmentOptions.Center;
        feedbackText.color = Color.yellow;
        
        RectTransform feedbackRect = feedbackObject.GetComponent<RectTransform>();
        feedbackRect.anchorMin = new Vector2(0.05f, 0.15f);
        feedbackRect.anchorMax = new Vector2(0.95f, 0.22f);
        feedbackRect.offsetMin = Vector2.zero;
        feedbackRect.offsetMax = Vector2.zero;
    }

    private void CreateUIButtons(GameObject parent)
    {
        // Submit button
        CreateUIButton(parent, "SubmitButton", "🔓 UNLOCK DOOR", new Vector2(0.1f, 0.05f), new Vector2(0.45f, 0.12f), keypadGlowColor);
        
        // Close button
        CreateUIButton(parent, "CloseButton", "❌ CLOSE", new Vector2(0.55f, 0.05f), new Vector2(0.9f, 0.12f), new Color(0.8f, 0.2f, 0.2f));
    }

    private void CreateUIButton(GameObject parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(parent.transform, false);
        
        Image buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = color;
        
        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        
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
        buttonText.fontSize = 14;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }

    private void CreateUIStatusDisplay(GameObject parent)
    {
        GameObject statusObject = new GameObject("StatusDisplay");
        statusObject.transform.SetParent(parent.transform, false);
        
        TMP_Text statusText = statusObject.AddComponent<TextMeshProUGUI>();
        statusText.text = "Puzzles Completed: 0/3";
        statusText.fontSize = 14;
        statusText.alignment = TextAlignmentOptions.Center;
        statusText.color = Color.cyan;
        
        RectTransform statusRect = statusObject.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0.1f, 0.82f);
        statusRect.anchorMax = new Vector2(0.9f, 0.85f);
        statusRect.offsetMin = Vector2.zero;
        statusRect.offsetMax = Vector2.zero;
    }

    private TMP_Text CreatePlaceholder(Transform parent, string text)
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

    private TMP_Text CreateInputText(Transform parent)
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

    private void ConfigureDoorCodeInput()
    {
        // Auto-assign references
        codeInputComponent.targetDoor = doorComponent;
        codeInputComponent.escapeCodeManager = escapeManager;
        codeInputComponent.doorInputPanel = doorUICanvas.transform.Find("DoorPanel").gameObject;
        codeInputComponent.codeInputField = doorUICanvas.GetComponentInChildren<TMP_InputField>();
        codeInputComponent.feedbackText = doorUICanvas.transform.Find("DoorPanel/FeedbackText").GetComponent<TMP_Text>();
        codeInputComponent.submitButton = doorUICanvas.transform.Find("DoorPanel/SubmitButton").GetComponent<Button>();
        codeInputComponent.clearButton = doorUICanvas.transform.Find("DoorPanel/CloseButton").GetComponent<Button>();
    }

    private IEnumerator SetupInteractionSystem()
    {
        // Add interaction helper
        DoorInteractionHelper interactionHelper = doorObject.GetComponent<DoorInteractionHelper>();
        if (interactionHelper == null)
        {
            interactionHelper = doorObject.AddComponent<DoorInteractionHelper>();
        }
        
        // Configure interaction
        interactionHelper.interactionDistance = interactionDistance;
        interactionHelper.doorUICanvas = doorUICanvas;
        interactionHelper.enableDebugLogs = showDetailedLogs;
        interactionHelper.showInteractionGizmo = true;
        
        // Create interaction prompt
        CreateInteractionPrompt();
        
        if (showDetailedLogs)
            Debug.Log("✅ Interaction system configured");
        
        yield return null;
    }

    private void CreateInteractionPrompt()
    {
        GameObject promptObject = new GameObject("InteractionPrompt");
        promptObject.transform.SetParent(doorObject.transform, false);
        promptObject.transform.localPosition = new Vector3(0, 2, 0);
        
        Canvas promptCanvas = promptObject.AddComponent<Canvas>();
        promptCanvas.renderMode = RenderMode.WorldSpace;
        promptCanvas.worldCamera = Camera.main;
        
        RectTransform canvasRect = promptObject.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(3, 0.8f);
        
        // Create background
        Image background = promptObject.AddComponent<Image>();
        background.color = new Color(0, 0, 0, 0.8f);
        
        // Create text
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(promptObject.transform, false);
        
        TMP_Text promptText = textObject.AddComponent<TextMeshProUGUI>();
        promptText.text = "Press E to access door";
        promptText.fontSize = 24;
        promptText.alignment = TextAlignmentOptions.Center;
        promptText.color = Color.white;
        
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Assign to interaction helper
        DoorInteractionHelper interactionHelper = doorObject.GetComponent<DoorInteractionHelper>();
        if (interactionHelper != null)
        {
            interactionHelper.interactionPrompt = promptObject;
            interactionHelper.interactionPromptText = promptText;
        }
        
        promptObject.SetActive(false);
    }

    private IEnumerator ConnectToWinScreen()
    {
        // Find or assign win screen
        GameObject winScreenObject = GameObject.Find("WinScreen");
        if (winScreenObject != null)
        {
            escapeManager.winScreen = winScreenObject.GetComponent<WinScreen>();
            
            // Ensure win screen has proper component
            WinScreen winScreenComponent = winScreenObject.GetComponent<WinScreen>();
            if (winScreenComponent == null)
            {
                winScreenComponent = winScreenObject.AddComponent<WinScreen>();
            }
            
            if (showDetailedLogs)
                Debug.Log("✅ Connected to existing win screen");
        }
        else
        {
            if (showDetailedLogs)
                Debug.LogWarning("⚠️ No win screen found - escape will work but no win screen will appear");
        }
        
        yield return null;
    }

    private IEnumerator FinalConfiguration()
    {
        // Connect escape manager to door
        escapeManager.escapeDoor = doorComponent;
        
        // Set door reference in escape manager
        doorComponent.correctCode = "0000"; // Placeholder until escape code is generated
        
        if (showDetailedLogs)
        {
            Debug.Log("✅ Final configuration complete");
            Debug.Log($"🚪 Door created at position: {doorPosition}");
            Debug.Log($"🔢 Required puzzles: {requiredPuzzleCount}");
            Debug.Log($"🎮 Interaction distance: {interactionDistance}m");
        }
        
        yield return null;
    }

    // Helper method to create a cube mesh
    private Mesh CreateCubeMesh()
    {
        Mesh mesh = new Mesh();
        
        Vector3[] vertices = {
            new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, -0.5f), new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f), new Vector3(-0.5f, -0.5f, 0.5f),
        };
        
        int[] triangles = {
            0, 2, 1, 0, 3, 2, 2, 3, 4, 2, 4, 5, 1, 2, 5, 1, 5, 6,
            0, 7, 4, 0, 4, 3, 5, 4, 7, 5, 7, 6, 0, 6, 7, 0, 1, 6
        };
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        return mesh;
    }

    // Public methods for keypad interaction
    public void OnKeypadButtonPressed(int number)
    {
        if (codeInputComponent != null)
        {
            if (number >= 0)
            {
                codeInputComponent.InputNumber(number);
            }
            else if (number == -1)
            {
                codeInputComponent.ClearInput();
            }
            else if (number == -2)
            {
                codeInputComponent.SubmitCode();
            }
        }
    }

    [ContextMenu("🧪 Test Complete System")]
    public void TestCompleteSystem()
    {
        // Add test numbers to escape manager
        if (escapeManager != null)
        {
            escapeManager.OnPuzzleCompleted(1234);
            escapeManager.OnPuzzleCompleted(5678);
            escapeManager.OnPuzzleCompleted(9012);
            
            Debug.Log("🧪 Added test puzzle numbers - escape code should now be available!");
        }
    }

    [ContextMenu("🎮 Show Door UI")]
    public void TestShowDoorUI()
    {
        if (doorUICanvas != null)
        {
            doorUICanvas.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Debug.Log("🎮 Door UI shown for testing");
        }
    }
}

// Helper component for physical keypad buttons
public class KeypadButton : MonoBehaviour
{
    private int buttonValue;
    private CompleteDoorSetupHelper doorHelper;
    
    public void Initialize(int value, CompleteDoorSetupHelper helper)
    {
        buttonValue = value;
        doorHelper = helper;
    }
    
    void OnMouseDown()
    {
        doorHelper?.OnKeypadButtonPressed(buttonValue);
        
        // Visual feedback
        StartCoroutine(ButtonPressEffect());
    }
    
    private System.Collections.IEnumerator ButtonPressEffect()
    {
        Vector3 originalScale = transform.localScale;
        transform.localScale = originalScale * 0.9f;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = originalScale;
    }
}

// Helper component for UI keypad buttons
public class KeypadUIButton : MonoBehaviour
{
    private int buttonValue;
    private CompleteDoorSetupHelper doorHelper;
    
    public void Initialize(int value, CompleteDoorSetupHelper helper)
    {
        buttonValue = value;
        doorHelper = helper;
        
        // Add button listener
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => doorHelper?.OnKeypadButtonPressed(buttonValue));
        }
    }
}