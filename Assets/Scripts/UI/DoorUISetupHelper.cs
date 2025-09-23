using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper to setup door code input UI automatically
/// </summary>
public class DoorUISetupHelper : MonoBehaviour
{
    [Header("Setup Options")]
    [Tooltip("Create full keypad with number buttons")]
    public bool createKeypadButtons = true;
    
    [Tooltip("Create input field for direct typing")]
    public bool createInputField = true;
    
    [Tooltip("Use existing canvas or create new one")]
    public bool useExistingCanvas = true;
    
    [Tooltip("Target canvas for UI creation")]
    public Canvas targetCanvas;
    
    [Header("Visual Style")]
    [Tooltip("Button size for keypad")]
    public Vector2 buttonSize = new Vector2(80, 80);
    
    [Tooltip("Button spacing")]
    public float buttonSpacing = 10f;
    
    [Tooltip("Main panel size")]
    public Vector2 panelSize = new Vector2(400, 500);
    
    [Tooltip("Button color scheme")]
    public ColorBlock buttonColors;
    
    [Header("References")]
    [Tooltip("Door component to connect")]
    public Door targetDoor;
    
    [Tooltip("Escape code manager")]
    public EscapeCodeManager escapeCodeManager;
    
    void Start()
    {
        // Initialize color scheme if not set
        if (buttonColors.normalColor == Color.white)
        {
            SetupDefaultColors();
        }
    }
    
    private void SetupDefaultColors()
    {
        buttonColors = new ColorBlock
        {
            normalColor = new Color(0.2f, 0.2f, 0.2f, 1f),
            highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1f),
            pressedColor = new Color(0.1f, 0.1f, 0.1f, 1f),
            selectedColor = new Color(0.2f, 0.3f, 0.5f, 1f),
            disabledColor = new Color(0.1f, 0.1f, 0.1f, 0.5f),
            colorMultiplier = 1f,
            fadeDuration = 0.1f
        };
    }
    
    [ContextMenu("🏗️ Setup Complete Door UI")]
    public void SetupCompleteDoorUI()
    {
        FindReferences();
        
        GameObject doorPanel = CreateMainDoorPanel();
        
        if (doorPanel != null)
        {
            CreateInputField(doorPanel);
            
            if (createKeypadButtons)
            {
                CreateKeypadButtons(doorPanel);
            }
            
            CreateControlButtons(doorPanel);
            CreateFeedbackElements(doorPanel);
            SetupDoorCodeInput(doorPanel);
            
            Debug.Log("✅ Complete door UI setup finished!");
        }
    }
    
    [ContextMenu("🔍 Find Door References")]
    public void FindReferences()
    {
        if (targetDoor == null)
        {
            targetDoor = FindFirstObjectByType<Door>();
            if (targetDoor != null)
                Debug.Log($"✅ Found Door: {targetDoor.name}");
        }
        
        if (escapeCodeManager == null)
        {
            escapeCodeManager = FindFirstObjectByType<EscapeCodeManager>();
            if (escapeCodeManager != null)
                Debug.Log($"✅ Found EscapeCodeManager: {escapeCodeManager.name}");
        }
        
        if (targetCanvas == null && useExistingCanvas)
        {
            // Look for door canvas first
            if (targetDoor != null)
            {
                targetCanvas = targetDoor.GetComponentInChildren<Canvas>();
            }
            
            // Fallback to any canvas
            if (targetCanvas == null)
            {
                targetCanvas = FindFirstObjectByType<Canvas>();
            }
            
            if (targetCanvas != null)
                Debug.Log($"✅ Found Canvas: {targetCanvas.name}");
        }
    }
    
    private GameObject CreateMainDoorPanel()
    {
        if (targetCanvas == null)
        {
            Debug.LogError("❌ No target canvas found!");
            return null;
        }
        
        // Check if door input panel already exists
        Transform existingPanel = targetCanvas.transform.Find("DoorInputPanel");
        if (existingPanel != null)
        {
            Debug.Log("🔄 Using existing DoorInputPanel");
            return existingPanel.gameObject;
        }
        
        // Create main panel
        GameObject panel = new GameObject("DoorInputPanel");
        panel.transform.SetParent(targetCanvas.transform, false);
        
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = panelSize;
        panelRect.anchoredPosition = Vector2.zero;
        
        // Add background image
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.8f);
        panelImage.raycastTarget = true;
        
        // Add title
        GameObject title = new GameObject("Title");
        title.transform.SetParent(panel.transform, false);
        
        RectTransform titleRect = title.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.sizeDelta = new Vector2(0f, 60f);
        titleRect.anchoredPosition = new Vector2(0f, -30f);
        
        TMP_Text titleText = title.AddComponent<TextMeshProUGUI>();
        titleText.text = "ESCAPE DOOR";
        titleText.fontSize = 24;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;
        
        Debug.Log("✅ Created main door panel");
        return panel;
    }
    
    private void CreateInputField(GameObject parent)
    {
        if (!createInputField) return;
        
        GameObject inputContainer = new GameObject("InputContainer");
        inputContainer.transform.SetParent(parent.transform, false);
        
        RectTransform containerRect = inputContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.1f, 0.75f);
        containerRect.anchorMax = new Vector2(0.9f, 0.85f);
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;
        
        // Input field background
        Image inputBg = inputContainer.AddComponent<Image>();
        inputBg.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        
        // Input field
        GameObject inputField = new GameObject("CodeInputField");
        inputField.transform.SetParent(inputContainer.transform, false);
        
        RectTransform inputRect = inputField.AddComponent<RectTransform>();
        inputRect.anchorMin = Vector2.zero;
        inputRect.anchorMax = Vector2.one;
        inputRect.offsetMin = new Vector2(10, 0);
        inputRect.offsetMax = new Vector2(-10, 0);
        
        TMP_InputField input = inputField.AddComponent<TMP_InputField>();
        input.textComponent = inputField.AddComponent<TextMeshProUGUI>();
        input.textComponent.fontSize = 28;
        input.textComponent.color = Color.white;
        input.textComponent.alignment = TextAlignmentOptions.Center;
        
        input.placeholder = CreatePlaceholder(inputField);
        input.characterLimit = 8;
        input.contentType = TMP_InputField.ContentType.IntegerNumber;
        
        Debug.Log("✅ Created input field");
    }
    
    private TMP_Text CreatePlaceholder(GameObject parent)
    {
        GameObject placeholder = new GameObject("Placeholder");
        placeholder.transform.SetParent(parent.transform, false);
        
        RectTransform placeholderRect = placeholder.AddComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = Vector2.zero;
        placeholderRect.offsetMax = Vector2.zero;
        
        TMP_Text placeholderText = placeholder.AddComponent<TextMeshProUGUI>();
        placeholderText.text = "Enter Code...";
        placeholderText.fontSize = 28;
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        placeholderText.alignment = TextAlignmentOptions.Center;
        
        return placeholderText;
    }
    
    private void CreateKeypadButtons(GameObject parent)
    {
        GameObject keypadContainer = new GameObject("KeypadContainer");
        keypadContainer.transform.SetParent(parent.transform, false);
        
        RectTransform keypadRect = keypadContainer.AddComponent<RectTransform>();
        keypadRect.anchorMin = new Vector2(0.1f, 0.25f);
        keypadRect.anchorMax = new Vector2(0.9f, 0.65f);
        keypadRect.offsetMin = Vector2.zero;
        keypadRect.offsetMax = Vector2.zero;
        
        // Create a 3x4 grid (1-9, 0 on bottom)
        int[,] keypadLayout = {
            {1, 2, 3},
            {4, 5, 6},
            {7, 8, 9},
            {-1, 0, -1} // -1 for empty spots
        };
        
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                int number = keypadLayout[row, col];
                if (number == -1) continue; // Skip empty spots
                
                CreateNumberButton(keypadContainer, number, row, col);
            }
        }
        
        Debug.Log("✅ Created keypad buttons");
    }
    
    private void CreateNumberButton(GameObject parent, int number, int row, int col)
    {
        GameObject button = new GameObject($"Button_{number}");
        button.transform.SetParent(parent.transform, false);
        
        RectTransform buttonRect = button.AddComponent<RectTransform>();
        
        // Calculate position in grid
        float totalWidth = parent.GetComponent<RectTransform>().rect.width;
        float totalHeight = parent.GetComponent<RectTransform>().rect.height;
        
        float buttonWidth = (totalWidth - (buttonSpacing * 2)) / 3f;
        float buttonHeight = (totalHeight - (buttonSpacing * 3)) / 4f;
        
        float xPos = col * (buttonWidth + buttonSpacing) + buttonWidth * 0.5f - totalWidth * 0.5f;
        float yPos = totalHeight * 0.5f - row * (buttonHeight + buttonSpacing) - buttonHeight * 0.5f;
        
        buttonRect.anchoredPosition = new Vector2(xPos, yPos);
        buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);
        
        // Add button component
        Button btn = button.AddComponent<Button>();
        btn.colors = buttonColors;
        
        // Add button image
        Image buttonImage = button.AddComponent<Image>();
        buttonImage.color = buttonColors.normalColor;
        
        // Add text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(button.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TMP_Text buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = number.ToString();
        buttonText.fontSize = 24;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.fontStyle = FontStyles.Bold;
        
        btn.targetGraphic = buttonImage;
    }
    
    private void CreateControlButtons(GameObject parent)
    {
        // Submit button
        CreateControlButton(parent, "SubmitButton", "SUBMIT", new Vector2(0.1f, 0.05f), new Vector2(0.45f, 0.15f), Color.green);
        
        // Clear button
        CreateControlButton(parent, "ClearButton", "CLEAR", new Vector2(0.55f, 0.05f), new Vector2(0.9f, 0.15f), Color.red);
    }
    
    private void CreateControlButton(GameObject parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax, Color baseColor)
    {
        GameObject button = new GameObject(name);
        button.transform.SetParent(parent.transform, false);
        
        RectTransform buttonRect = button.AddComponent<RectTransform>();
        buttonRect.anchorMin = anchorMin;
        buttonRect.anchorMax = anchorMax;
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;
        
        Button btn = button.AddComponent<Button>();
        
        ColorBlock colors = buttonColors;
        colors.normalColor = baseColor * 0.8f;
        colors.highlightedColor = baseColor;
        colors.pressedColor = baseColor * 0.6f;
        btn.colors = colors;
        
        Image buttonImage = button.AddComponent<Image>();
        buttonImage.color = colors.normalColor;
        
        // Button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(button.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TMP_Text buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 18;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.fontStyle = FontStyles.Bold;
        
        btn.targetGraphic = buttonImage;
    }
    
    private void CreateFeedbackElements(GameObject parent)
    {
        // Instructions text
        GameObject instructions = new GameObject("InstructionsText");
        instructions.transform.SetParent(parent.transform, false);
        
        RectTransform instructionsRect = instructions.AddComponent<RectTransform>();
        instructionsRect.anchorMin = new Vector2(0.1f, 0.65f);
        instructionsRect.anchorMax = new Vector2(0.9f, 0.75f);
        instructionsRect.offsetMin = Vector2.zero;
        instructionsRect.offsetMax = Vector2.zero;
        
        TMP_Text instructionsText = instructions.AddComponent<TextMeshProUGUI>();
        instructionsText.text = "Complete all puzzles to reveal escape code";
        instructionsText.fontSize = 16;
        instructionsText.color = Color.yellow;
        instructionsText.alignment = TextAlignmentOptions.Center;
        instructionsText.fontStyle = FontStyles.Italic;
        
        // Feedback text
        GameObject feedback = new GameObject("FeedbackText");
        feedback.transform.SetParent(parent.transform, false);
        
        RectTransform feedbackRect = feedback.AddComponent<RectTransform>();
        feedbackRect.anchorMin = new Vector2(0.1f, 0.15f);
        feedbackRect.anchorMax = new Vector2(0.9f, 0.25f);
        feedbackRect.offsetMin = Vector2.zero;
        feedbackRect.offsetMax = Vector2.zero;
        
        TMP_Text feedbackText = feedback.AddComponent<TextMeshProUGUI>();
        feedbackText.text = "";
        feedbackText.fontSize = 18;
        feedbackText.color = Color.white;
        feedbackText.alignment = TextAlignmentOptions.Center;
        feedbackText.fontStyle = FontStyles.Bold;
        
        Debug.Log("✅ Created feedback elements");
    }
    
    private void SetupDoorCodeInput(GameObject doorPanel)
    {
        // Add DoorCodeInput component to the panel
        DoorCodeInput doorCodeInput = doorPanel.GetComponent<DoorCodeInput>();
        if (doorCodeInput == null)
        {
            doorCodeInput = doorPanel.AddComponent<DoorCodeInput>();
        }
        
        // Wire up references
        doorCodeInput.doorInputPanel = doorPanel;
        doorCodeInput.targetDoor = targetDoor;
        doorCodeInput.escapeCodeManager = escapeCodeManager;
        
        // Find and assign UI elements
        doorCodeInput.codeInputField = doorPanel.GetComponentInChildren<TMP_InputField>();
        doorCodeInput.instructionsText = doorPanel.transform.Find("InstructionsText")?.GetComponent<TMP_Text>();
        doorCodeInput.feedbackText = doorPanel.transform.Find("FeedbackText")?.GetComponent<TMP_Text>();
        doorCodeInput.submitButton = doorPanel.transform.Find("SubmitButton")?.GetComponent<Button>();
        doorCodeInput.clearButton = doorPanel.transform.Find("ClearButton")?.GetComponent<Button>();
        
        // Find number buttons
        Transform keypadContainer = doorPanel.transform.Find("KeypadContainer");
        if (keypadContainer != null)
        {
            doorCodeInput.numberButtons = new Button[10];
            for (int i = 0; i <= 9; i++)
            {
                Transform buttonTransform = keypadContainer.Find($"Button_{i}");
                if (buttonTransform != null)
                {
                    doorCodeInput.numberButtons[i] = buttonTransform.GetComponent<Button>();
                }
            }
        }
        
        Debug.Log("✅ Setup DoorCodeInput component");
    }
    
    [ContextMenu("🧹 Clean Up Existing UI")]
    public void CleanUpExistingUI()
    {
        if (targetCanvas != null)
        {
            Transform existingPanel = targetCanvas.transform.Find("DoorInputPanel");
            if (existingPanel != null)
            {
                DestroyImmediate(existingPanel.gameObject);
                Debug.Log("🧹 Removed existing DoorInputPanel");
            }
        }
    }
    
    [ContextMenu("🔄 Rebuild Door UI")]
    public void RebuildDoorUI()
    {
        CleanUpExistingUI();
        SetupCompleteDoorUI();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DoorUISetupHelper))]
public class DoorUISetupHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Door UI Setup Tools", EditorStyles.boldLabel);
        
        DoorUISetupHelper helper = (DoorUISetupHelper)target;
        
        if (GUILayout.Button("🔍 Find Door References", GUILayout.Height(35)))
        {
            helper.FindReferences();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("🏗️ Setup Complete Door UI", GUILayout.Height(40)))
        {
            helper.SetupCompleteDoorUI();
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🧹 Clean Up Existing"))
        {
            helper.CleanUpExistingUI();
        }
        
        if (GUILayout.Button("🔄 Rebuild UI"))
        {
            helper.RebuildDoorUI();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This helper creates a complete door code input interface with keypad, input field, and feedback elements.", MessageType.Info);
    }
}
#endif