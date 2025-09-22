using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;

[System.Serializable]
public class SimpleRiddleSetup : MonoBehaviour
{
    [Header("🧩 SIMPLE RIDDLE SETUP")]
    [Space(10)]
    
    [Header("📋 ONE-CLICK SETUP:")]
    [Header("Click the button below to create a complete riddle puzzle system")]
    [Space(10)]
    
    [SerializeField, Tooltip("Status of the current setup")]
    private string setupStatus = "Ready to create riddle puzzle";
    
    void Start()
    {
        UpdateStatus();
    }
    
    [ContextMenu("Create Riddle Puzzle System")]
    public void CreateRiddlePuzzleSystem()
    {
        Debug.Log("🧩 Creating Simple Riddle Puzzle System...");
        
        try
        {
            // Clean up existing
            CleanupExisting();
            
            // Create using manual UI creation (Unity 6 compatible)
            CreateUIManually();
            
            // Create the puzzle controller
            CreatePuzzleController();
            
            // Connect everything
            ConnectComponents();
            
            setupStatus = "✅ Riddle puzzle created successfully!";
            Debug.Log("✅ Simple Riddle Puzzle System created successfully!");
        }
        catch (System.Exception e)
        {
            setupStatus = $"❌ Error: {e.Message}";
            Debug.LogError($"Failed to create riddle puzzle: {e.Message}");
        }
    }
    
    private void CleanupExisting()
    {
        // Remove existing riddle puzzle controller
        RiddlePuzzle[] existing = FindObjectsByType<RiddlePuzzle>(FindObjectsSortMode.None);
        foreach (RiddlePuzzle puzzle in existing)
        {
            DestroyImmediate(puzzle.gameObject);
        }
        
        // Remove existing canvas
        GameObject existingCanvas = GameObject.Find("RiddleCanvas");
        if (existingCanvas != null)
        {
            DestroyImmediate(existingCanvas);
        }
        
        Debug.Log("🗑️ Cleaned up existing riddle components");
    }
    
    private void CreateUIManually()
    {
        // Create Canvas manually (Unity 6 compatible)
        GameObject canvasGO = new GameObject("RiddleCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasGO.AddComponent<GraphicRaycaster>();
        canvasGO.SetActive(false);
        
        // Create Panel manually
        GameObject panel = CreatePanel(canvasGO.transform, "RiddlePanel", new Vector2(800, 600));
        
        // Create all UI elements manually
        CreateText(panel.transform, "TitleText", "🧩 RIDDLE CHALLENGE", new Vector2(600, 50), new Vector2(0, 250), 24, Color.cyan, FontStyle.Bold);
        CreateText(panel.transform, "RiddleDisplay", "Welcome to the Riddle Challenge! Use the navigation buttons to browse riddles.", new Vector2(700, 100), new Vector2(0, 150), 16);
        CreateText(panel.transform, "RiddleProgressDisplay", "Riddle 1 of 4", new Vector2(200, 30), new Vector2(0, 80), 14, Color.cyan, FontStyle.Bold);
        
        // Navigation buttons
        CreateButton(panel.transform, "PreviousRiddleButton", "← Previous", new Vector2(120, 40), new Vector2(-150, 40));
        CreateButton(panel.transform, "NextRiddleButton", "Next →", new Vector2(120, 40), new Vector2(150, 40));
        
        // Input field
        CreateInputField(panel.transform, "RiddleAnswerInput", "Enter your answer here...", new Vector2(400, 40), new Vector2(0, -10));
        
        // Action buttons
        CreateButton(panel.transform, "SubmitAnswerButton", "Submit Answer", new Vector2(140, 40), new Vector2(-100, -60));
        CreateButton(panel.transform, "ResetButton", "Clear", new Vector2(80, 40), new Vector2(50, -60));
        
        // Displays
        CreateText(panel.transform, "SolvedRiddlesDisplay", "Solved Riddles:\nNone yet", new Vector2(300, 120), new Vector2(-200, -140), 12, Color.white, FontStyle.Normal, TextAnchor.UpperLeft);
        CreateText(panel.transform, "FeedbackText", "💡 Tip: If you're stuck, try asking the AI terminal for 'help'!", new Vector2(600, 60), new Vector2(0, -200), 14, Color.cyan);
        
        // Close button
        CreateButton(panel.transform, "CloseButton", "✕ Close", new Vector2(100, 40), new Vector2(300, 250));
        
        Debug.Log("🎨 UI created manually for Unity 6 compatibility");
    }
    
    private GameObject CreatePanel(Transform parent, string name, Vector2 size)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;
        rect.anchorMin = Vector2.one * 0.5f;
        rect.anchorMax = Vector2.one * 0.5f;
        
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        
        return panel;
    }
    
    private GameObject CreateText(Transform parent, string name, string text, Vector2 size, Vector2 position, int fontSize = 16, Color? color = null, FontStyle fontStyle = FontStyle.Normal, TextAnchor alignment = TextAnchor.MiddleCenter)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = color ?? Color.white;
        textComponent.fontStyle = fontStyle;
        textComponent.alignment = alignment;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        return textObj;
    }
    
    private GameObject CreateButton(Transform parent, string name, string text, Vector2 size, Vector2 position)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.3f, 0.8f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        
        // Create text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.fontSize = 14;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        return buttonObj;
    }
    
    private GameObject CreateInputField(Transform parent, string name, string placeholder, Vector2 size, Vector2 position)
    {
        GameObject inputObj = new GameObject(name);
        inputObj.transform.SetParent(parent, false);
        
        RectTransform rect = inputObj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        
        Image image = inputObj.AddComponent<Image>();
        image.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        
        InputField inputField = inputObj.AddComponent<InputField>();
        
        // Create text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(inputObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        textRect.offsetMin = new Vector2(10, 2);
        textRect.offsetMax = new Vector2(-10, -2);
        
        Text text = textObj.AddComponent<Text>();
        text.text = "";
        text.fontSize = 14;
        text.color = Color.black;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        // Create placeholder
        GameObject placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(inputObj.transform, false);
        
        RectTransform placeholderRect = placeholderObj.AddComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.sizeDelta = Vector2.zero;
        placeholderRect.anchoredPosition = Vector2.zero;
        placeholderRect.offsetMin = new Vector2(10, 2);
        placeholderRect.offsetMax = new Vector2(-10, -2);
        
        Text placeholderText = placeholderObj.AddComponent<Text>();
        placeholderText.text = placeholder;
        placeholderText.fontSize = 14;
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        placeholderText.fontStyle = FontStyle.Italic;
        placeholderText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        // Configure InputField
        inputField.textComponent = text;
        inputField.placeholder = placeholderText;
        
        return inputObj;
    }
    
    private void CreatePuzzleController()
    {
        GameObject controllerObj = new GameObject("RiddlePuzzleController");
        RiddlePuzzle puzzle = controllerObj.AddComponent<RiddlePuzzle>();
        
        puzzle.puzzleName = "Riddle Challenge";
        puzzle.puzzleDescription = "Solve all riddles to unlock your escape code!";
        
        Debug.Log("🎮 Puzzle controller created");
    }
    
    private void ConnectComponents()
    {
        RiddlePuzzle puzzle = FindFirstObjectByType<RiddlePuzzle>();
        if (puzzle == null)
        {
            Debug.LogError("Could not find RiddlePuzzle component!");
            return;
        }
        
        // Connect UI references
        puzzle.puzzleUI = GameObject.Find("RiddleCanvas");
        
        // Connect text components - convert to TMP_Text
        ConvertTextToTMP("RiddleDisplay");
        ConvertTextToTMP("RiddleProgressDisplay");
        ConvertTextToTMP("SolvedRiddlesDisplay");
        ConvertTextToTMP("FeedbackText");
        
        // Connect TMP_Text components
        puzzle.riddleDisplay = GameObject.Find("RiddleDisplay")?.GetComponent<TMP_Text>();
        puzzle.riddleProgressDisplay = GameObject.Find("RiddleProgressDisplay")?.GetComponent<TMP_Text>();
        puzzle.solvedRiddlesDisplay = GameObject.Find("SolvedRiddlesDisplay")?.GetComponent<TMP_Text>();
        puzzle.feedbackText = GameObject.Find("FeedbackText")?.GetComponent<TMP_Text>();
        
        // Convert and connect input field
        ConvertInputFieldToTMP("RiddleAnswerInput");
        puzzle.riddleAnswerInput = GameObject.Find("RiddleAnswerInput")?.GetComponent<TMP_InputField>();
        
        // Connect buttons
        puzzle.submitAnswerButton = GameObject.Find("SubmitAnswerButton")?.GetComponent<Button>();
        puzzle.nextRiddleButton = GameObject.Find("NextRiddleButton")?.GetComponent<Button>();
        puzzle.previousRiddleButton = GameObject.Find("PreviousRiddleButton")?.GetComponent<Button>();
        puzzle.resetButton = GameObject.Find("ResetButton")?.GetComponent<Button>();
        puzzle.closeButton = GameObject.Find("CloseButton")?.GetComponent<Button>();
        
        // Try to connect AI Terminal
        TerminalControllerNew terminal = FindFirstObjectByType<TerminalControllerNew>();
        if (terminal != null)
        {
            puzzle.aiTerminal = terminal;
            Debug.Log("🔗 Connected to AI Terminal");
        }
        
        Debug.Log("🔗 Components connected successfully");
    }
    
    private void ConvertTextToTMP(string objectName)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj == null) return;
        
        Text oldText = obj.GetComponent<Text>();
        if (oldText == null) return;
        
        string text = oldText.text;
        int fontSize = oldText.fontSize;
        Color color = oldText.color;
        TextAnchor alignment = oldText.alignment;
        FontStyle fontStyle = oldText.fontStyle;
        
        // Remove old component
        DestroyImmediate(oldText);
        
        // Add TMP component
        TMP_Text tmpText = obj.AddComponent<TMP_Text>();
        tmpText.text = text;
        tmpText.fontSize = fontSize;
        tmpText.color = color;
        tmpText.fontStyle = (FontStyles)fontStyle;
        
        // Convert alignment
        switch (alignment)
        {
            case TextAnchor.UpperLeft:
                tmpText.alignment = TextAlignmentOptions.TopLeft;
                break;
            case TextAnchor.UpperCenter:
                tmpText.alignment = TextAlignmentOptions.Top;
                break;
            case TextAnchor.UpperRight:
                tmpText.alignment = TextAlignmentOptions.TopRight;
                break;
            case TextAnchor.MiddleLeft:
                tmpText.alignment = TextAlignmentOptions.Left;
                break;
            case TextAnchor.MiddleCenter:
                tmpText.alignment = TextAlignmentOptions.Center;
                break;
            case TextAnchor.MiddleRight:
                tmpText.alignment = TextAlignmentOptions.Right;
                break;
            case TextAnchor.LowerLeft:
                tmpText.alignment = TextAlignmentOptions.BottomLeft;
                break;
            case TextAnchor.LowerCenter:
                tmpText.alignment = TextAlignmentOptions.Bottom;
                break;
            case TextAnchor.LowerRight:
                tmpText.alignment = TextAlignmentOptions.BottomRight;
                break;
        }
        
        Debug.Log($"📝 Converted {objectName} to TMP_Text");
    }
    
    private void ConvertInputFieldToTMP(string objectName)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj == null) return;
        
        InputField oldInput = obj.GetComponent<InputField>();
        if (oldInput == null) return;
        
        // Store placeholder text
        string placeholderText = "";
        if (oldInput.placeholder != null)
        {
            Text placeholderTextComponent = oldInput.placeholder.GetComponent<Text>();
            if (placeholderTextComponent != null)
                placeholderText = placeholderTextComponent.text;
        }
        
        // Remove old components but keep the children for conversion
        DestroyImmediate(oldInput);
        
        // Add TMP InputField
        TMP_InputField tmpInput = obj.AddComponent<TMP_InputField>();
        
        // Find and convert existing text components
        Transform textTransform = obj.transform.Find("Text");
        Transform placeholderTransform = obj.transform.Find("Placeholder");
        
        if (textTransform != null)
        {
            Text oldTextComponent = textTransform.GetComponent<Text>();
            if (oldTextComponent != null)
            {
                DestroyImmediate(oldTextComponent);
                TMP_Text newText = textTransform.gameObject.AddComponent<TMP_Text>();
                newText.text = "";
                newText.fontSize = 14;
                newText.color = Color.black;
                tmpInput.textComponent = newText;
                tmpInput.textViewport = textTransform.GetComponent<RectTransform>();
            }
        }
        
        if (placeholderTransform != null)
        {
            Text oldPlaceholderComponent = placeholderTransform.GetComponent<Text>();
            if (oldPlaceholderComponent != null)
            {
                DestroyImmediate(oldPlaceholderComponent);
                TMP_Text newPlaceholder = placeholderTransform.gameObject.AddComponent<TMP_Text>();
                newPlaceholder.text = placeholderText;
                newPlaceholder.fontSize = 14;
                newPlaceholder.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                newPlaceholder.fontStyle = FontStyles.Italic;
                tmpInput.placeholder = newPlaceholder;
            }
        }
        
        Debug.Log($"📝 Converted {objectName} to TMP_InputField");
    }
    
    private void UpdateStatus()
    {
        RiddlePuzzle puzzle = FindFirstObjectByType<RiddlePuzzle>();
        GameObject canvas = GameObject.Find("RiddleCanvas");
        
        if (puzzle != null && canvas != null)
        {
            setupStatus = "✅ Riddle puzzle system is set up and ready";
        }
        else if (puzzle != null || canvas != null)
        {
            setupStatus = "⚠️ Partial setup detected - consider recreating";
        }
        else
        {
            setupStatus = "Ready to create riddle puzzle";
        }
    }
    
    [ContextMenu("Test Riddle Puzzle")]
    public void TestRiddlePuzzle()
    {
        RiddlePuzzle puzzle = FindFirstObjectByType<RiddlePuzzle>();
        if (puzzle != null)
        {
            puzzle.TestShowPuzzle();
            Debug.Log("🧪 Testing riddle puzzle");
        }
        else
        {
            Debug.LogWarning("⚠️ No riddle puzzle found to test");
        }
    }
    
    [ContextMenu("Remove Riddle Puzzle")]
    public void RemoveRiddlePuzzle()
    {
        CleanupExisting();
        UpdateStatus();
        Debug.Log("🗑️ Riddle puzzle removed");
    }
    
    [ContextMenu("Create Riddle Terminal")]
    public void CreateRiddleTerminal()
    {
        Debug.Log("🖥️ Creating Riddle Terminal...");
        
        // Check if riddle puzzle exists
        RiddlePuzzle riddlePuzzle = FindFirstObjectByType<RiddlePuzzle>();
        if (riddlePuzzle == null)
        {
            Debug.LogWarning("⚠️ No RiddlePuzzle found. Create the riddle puzzle system first!");
            return;
        }
        
        // Check if terminal already exists
        RiddleTerminal existingTerminal = FindFirstObjectByType<RiddleTerminal>();
        if (existingTerminal != null)
        {
            Debug.LogWarning("⚠️ Riddle terminal already exists!");
            return;
        }
        
        try
        {
            // Create terminal using the setup helper
            GameObject setupHelper = new GameObject("TerminalSetupHelper");
            setupHelper.transform.position = transform.position + Vector3.forward * 5f;
            
            RiddleTerminalSetup terminalSetup = setupHelper.AddComponent<RiddleTerminalSetup>();
            terminalSetup.terminalName = "Neural Interface Terminal";
            terminalSetup.terminalPosition = setupHelper.transform.position;
            terminalSetup.terminalMessage = "RIDDLE CHALLENGE TERMINAL\n\nPress E to access\nriddle challenges\n\nStatus: READY";
            
            // Create the terminal
            terminalSetup.CreateRiddleTerminal();
            
            // Clean up helper
            DestroyImmediate(setupHelper);
            
            Debug.Log("✅ Riddle terminal created successfully!");
            Debug.Log("💡 Walk up to the terminal and press E to test the interaction.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error creating riddle terminal: {e.Message}");
        }
    }
    
    [ContextMenu("Test Terminal Interaction")]
    public void TestTerminalInteraction()
    {
        RiddleTerminal terminal = FindFirstObjectByType<RiddleTerminal>();
        if (terminal != null)
        {
            Debug.Log("🧪 Testing terminal interaction...");
            terminal.OnHover();
            terminal.OnInteract();
            Debug.Log("✅ Terminal test completed. Check console for results.");
        }
        else
        {
            Debug.LogWarning("⚠️ No riddle terminal found. Create one first!");
        }
    }
}

[CustomEditor(typeof(SimpleRiddleSetup))]
public class SimpleRiddleSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        SimpleRiddleSetup setup = (SimpleRiddleSetup)target;
        
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("🎯 Quick Actions", EditorStyles.boldLabel);
        
        if (GUILayout.Button("🧩 Create Riddle Puzzle System", GUILayout.Height(40)))
        {
            setup.CreateRiddlePuzzleSystem();
        }
        
        if (GUILayout.Button("🖥️ Create Riddle Terminal", GUILayout.Height(30)))
        {
            setup.CreateRiddleTerminal();
        }
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🧪 Test Puzzle"))
        {
            setup.TestRiddlePuzzle();
        }
        
        if (GUILayout.Button("🎮 Test Terminal"))
        {
            setup.TestTerminalInteraction();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🗑️ Remove Puzzle"))
        {
            if (EditorUtility.DisplayDialog("Remove Riddle Puzzle", 
                "This will delete the riddle puzzle system. Continue?", "Yes", "Cancel"))
            {
                setup.RemoveRiddlePuzzle();
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("This helper creates UI manually for maximum Unity 6 compatibility.", MessageType.Info);
    }
}
#endif