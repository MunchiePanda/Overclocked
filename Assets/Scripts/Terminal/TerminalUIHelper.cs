using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper script for creating and customizing terminal UIs
/// </summary>
public class TerminalUIHelper : MonoBehaviour
{
    [Header("Terminal UI Creation")]
    [Tooltip("Click to create a puzzle terminal UI")]
    public bool createPuzzleTerminalUI = false;
    
    [Tooltip("Terminal to create UI for")]
    public PuzzleTerminal targetTerminal;

    [Header("UI Customization")]
    [Tooltip("Background color for terminal panels")]
    public Color panelBackgroundColor = new Color(0.1f, 0.1f, 0.2f, 0.9f);
    
    [Tooltip("Text color for terminal UI")]
    public Color textColor = Color.cyan;
    
    [Tooltip("Button normal color")]
    public Color buttonNormalColor = new Color(0.2f, 0.3f, 0.5f, 1f);
    
    [Tooltip("Button highlight color")]
    public Color buttonHighlightColor = new Color(0.3f, 0.5f, 0.8f, 1f);

    void OnValidate()
    {
        if (createPuzzleTerminalUI)
        {
            CreatePuzzleTerminalUI();
            createPuzzleTerminalUI = false;
        }
    }

    [ContextMenu("Create Puzzle Terminal UI")]
    public void CreatePuzzleTerminalUI()
    {
        if (targetTerminal == null)
        {
            Debug.LogError("No target terminal assigned!");
            return;
        }

        // Create Canvas if it doesn't exist
        Canvas canvas = targetTerminal.GetComponentInChildren<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject($"{targetTerminal.terminalName}_Canvas");
            canvasObj.transform.SetParent(targetTerminal.transform);
            canvasObj.transform.localPosition = Vector3.zero;
            
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay; // Full screen UI
            canvas.sortingOrder = 100; // High sorting order to appear on top
            
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create main panel (full screen overlay)
        GameObject panelObj = new GameObject("TerminalPanel");
        panelObj.transform.SetParent(canvas.transform, false);
        
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = panelBackgroundColor;
        
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Create title text
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panelObj.transform, false);
        
        TMP_Text titleText = titleObj.AddComponent<TMP_Text>();
        titleText.text = targetTerminal.terminalName;
        titleText.fontSize = 36;
        titleText.color = textColor;
        titleText.alignment = TextAlignmentOptions.Center;
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.85f);
        titleRect.anchorMax = new Vector2(1, 0.95f);
        titleRect.offsetMin = new Vector2(20, 0);
        titleRect.offsetMax = new Vector2(-20, 0);

        // Create instructions text
        GameObject instructionsObj = new GameObject("Instructions");
        instructionsObj.transform.SetParent(panelObj.transform, false);
        
        TMP_Text instructionsText = instructionsObj.AddComponent<TMP_Text>();
        instructionsText.text = "Use this terminal to control the puzzle.\nPress ESC or Close to exit.";
        instructionsText.fontSize = 20;
        instructionsText.color = textColor;
        instructionsText.alignment = TextAlignmentOptions.Top;
        
        RectTransform instructionsRect = instructionsObj.GetComponent<RectTransform>();
        instructionsRect.anchorMin = new Vector2(0, 0.75f);
        instructionsRect.anchorMax = new Vector2(1, 0.85f);
        instructionsRect.offsetMin = new Vector2(40, 0);
        instructionsRect.offsetMax = new Vector2(-40, 0);

        // Create puzzle content area (where the actual puzzle UI will go)
        GameObject contentAreaObj = new GameObject("PuzzleContentArea");
        contentAreaObj.transform.SetParent(panelObj.transform, false);
        
        RectTransform contentRect = contentAreaObj.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.1f, 0.2f);
        contentRect.anchorMax = new Vector2(0.9f, 0.75f);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;

        // Create close button
        GameObject closeButtonObj = new GameObject("CloseButton");
        closeButtonObj.transform.SetParent(panelObj.transform, false);
        
        Button closeButton = closeButtonObj.AddComponent<Button>();
        Image closeButtonImage = closeButtonObj.AddComponent<Image>();
        closeButtonImage.color = buttonNormalColor;
        
        // Set button colors
        ColorBlock colors = closeButton.colors;
        colors.normalColor = buttonNormalColor;
        colors.highlightedColor = buttonHighlightColor;
        colors.pressedColor = buttonHighlightColor * 0.8f;
        closeButton.colors = colors;
        
        // Add close functionality
        closeButton.onClick.AddListener(() => targetTerminal.OnInteract());
        
        RectTransform closeButtonRect = closeButtonObj.GetComponent<RectTransform>();
        closeButtonRect.anchorMin = new Vector2(0.4f, 0.05f);
        closeButtonRect.anchorMax = new Vector2(0.6f, 0.15f);
        closeButtonRect.offsetMin = Vector2.zero;
        closeButtonRect.offsetMax = Vector2.zero;

        // Close button text
        GameObject closeTextObj = new GameObject("Text");
        closeTextObj.transform.SetParent(closeButtonObj.transform, false);
        
        TMP_Text closeText = closeTextObj.AddComponent<TMP_Text>();
        closeText.text = "Close Terminal";
        closeText.fontSize = 18;
        closeText.color = textColor;
        closeText.alignment = TextAlignmentOptions.Center;
        
        RectTransform closeTextRect = closeTextObj.GetComponent<RectTransform>();
        closeTextRect.anchorMin = Vector2.zero;
        closeTextRect.anchorMax = Vector2.one;
        closeTextRect.offsetMin = Vector2.zero;
        closeTextRect.offsetMax = Vector2.zero;

        // Assign references to terminal
        targetTerminal.terminalCanvas = canvas;
        targetTerminal.terminalPanel = panelObj;
        targetTerminal.terminalTitle = titleText;
        targetTerminal.instructionsText = instructionsText;

        // Hide UI initially
        canvas.gameObject.SetActive(false);

        Debug.Log($"Created full-screen UI for puzzle terminal: {targetTerminal.terminalName}");
        Debug.Log("Now you need to move your existing puzzle UI into the PuzzleContentArea!");
    }

    [ContextMenu("Setup Existing Puzzle UIs")]
    public void SetupExistingPuzzleUIs()
    {
        // Find existing puzzle UIs and help integrate them with terminals
        
        // Cipher Wheel
        GameObject cipherCanvas = GameObject.Find("CipherWheelCanvas");
        if (cipherCanvas != null)
        {
            CreateTerminalForExistingPuzzle("Cipher Wheel Terminal", "CipherWheel", cipherCanvas);
        }
        
        // Shadow Logic
        GameObject shadowCanvas = GameObject.Find("ShadowLogicCanvas");
        if (shadowCanvas != null)
        {
            CreateTerminalForExistingPuzzle("Shadow Logic Terminal", "ShadowLogic", shadowCanvas);
        }
        
        Debug.Log("Setup complete! Check the created puzzle terminals.");
    }

    void CreateTerminalForExistingPuzzle(string terminalName, string puzzleType, GameObject existingCanvas)
    {
        // Create terminal GameObject
        GameObject terminalObj = new GameObject(terminalName.Replace(" ", ""));
        terminalObj.transform.position = Vector3.zero; // You'll need to position these manually
        terminalObj.layer = LayerMask.NameToLayer("Interactable");
        
        // Add visual components
        terminalObj.AddComponent<MeshRenderer>();
        terminalObj.AddComponent<MeshFilter>();
        terminalObj.AddComponent<BoxCollider>();
        
        // Add PuzzleTerminal component
        PuzzleTerminal puzzleTerminal = terminalObj.AddComponent<PuzzleTerminal>();
        puzzleTerminal.terminalName = terminalName;
        puzzleTerminal.puzzleType = puzzleType;
        puzzleTerminal.hoverText = $"Press E to access {terminalName}";
        
        // Link existing canvas to terminal
        if (existingCanvas != null)
        {
            Canvas canvas = existingCanvas.GetComponent<Canvas>();
            if (canvas != null)
            {
                puzzleTerminal.terminalCanvas = canvas;
                puzzleTerminal.terminalPanel = existingCanvas;
                
                // Initially hide the existing canvas
                existingCanvas.SetActive(false);
            }
        }
        
        Debug.Log($"Created {terminalName} at origin - move it to desired position!");
    }

    [ContextMenu("Clean Up Duplicate Components")]
    public void CleanUpDuplicateComponents()
    {
        // Find the main AI terminal and clean it up
        GameObject aiTerminal = GameObject.Find("AITerminal");
        if (aiTerminal != null)
        {
            // Remove the old component
            InteractableAITerminal oldComponent = aiTerminal.GetComponent<InteractableAITerminal>();
            if (oldComponent != null)
            {
                DestroyImmediate(oldComponent);
                Debug.Log("Removed duplicate InteractableAITerminal component from AITerminal");
            }
            
            // Make sure it's on the right layer
            aiTerminal.layer = LayerMask.NameToLayer("Interactable");
            Debug.Log("Set AITerminal to Interactable layer");
        }
    }

    [ContextMenu("Style All Terminal UIs")]
    public void StyleAllTerminalUIs()
    {
        PuzzleTerminal[] terminals = FindObjectsByType<PuzzleTerminal>(FindObjectsSortMode.None);
        
        foreach (PuzzleTerminal terminal in terminals)
        {
            StyleTerminalUI(terminal);
        }
        
        Debug.Log($"Styled {terminals.Length} terminal UIs");
    }

    void StyleTerminalUI(PuzzleTerminal terminal)
    {
        if (terminal.terminalCanvas == null) return;

        // Update panel colors
        Image[] images = terminal.terminalCanvas.GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            if (img.name.Contains("Panel"))
                img.color = panelBackgroundColor;
            else if (img.name.Contains("Button"))
                img.color = buttonNormalColor;
        }

        // Update text colors
        TMP_Text[] texts = terminal.terminalCanvas.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text text in texts)
        {
            text.color = textColor;
        }

        // Update button colors
        Button[] buttons = terminal.terminalCanvas.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = buttonNormalColor;
            colors.highlightedColor = buttonHighlightColor;
            colors.pressedColor = buttonHighlightColor * 0.8f;
            button.colors = colors;
        }
    }

    [ContextMenu("Remove Duplicate Components")]
    public void RemoveDuplicateComponents()
    {
        // Find main AI terminal and remove old component
        InteractableAITerminalNew mainTerminal = FindFirstObjectByType<InteractableAITerminalNew>();
        if (mainTerminal != null)
        {
            InteractableAITerminal oldComponent = mainTerminal.GetComponent<InteractableAITerminal>();
            if (oldComponent != null)
            {
                DestroyImmediate(oldComponent);
                Debug.Log("Removed duplicate InteractableAITerminal component");
            }
        }
    }
}