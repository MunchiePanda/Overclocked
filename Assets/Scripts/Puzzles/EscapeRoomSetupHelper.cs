using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;

public class EscapeRoomSetupHelper : MonoBehaviour
{
    [Header("🎯 NEW TERMINAL SYSTEM - Unity 6 Compatible")]
    [Space(10)]
    [Header("📋 STEP-BY-STEP SETUP GUIDE:")]
    [Space(5)]
    [Header("1️⃣ If you don't have puzzles yet, create them using Legacy options below")]
    [Header("2️⃣ Clean up AI Terminal → 3️⃣ Setup Terminal System → 4️⃣ Create Puzzle Terminals")]
    [Header("5️⃣ Style All Terminals → 6️⃣ Test Terminal System")]
    [Space(10)]
    
    [Header("🚀 QUICK SETUP")]
    [Space(5)]
    [Tooltip("🔥 ONE-CLICK SETUP: Does steps 1-4 automatically!")]
    public bool doCompleteSetup = false;
    
    [Space(10)]
    
    [Tooltip("🚀 STEP 1: Clean up and fix existing AI Terminal")]
    public bool cleanupAITerminal = false;
    
    [Tooltip("🎮 STEP 2: Setup complete terminal system")]
    public bool setupTerminalSystem = false;
    
    [Tooltip("🔧 STEP 3: Create individual puzzle terminals")]
    public bool createPuzzleTerminals = false;
    
    [Tooltip("🎨 STEP 4: Style all terminals consistently")]
    public bool styleAllTerminals = false;
    
    [Tooltip("🧪 STEP 5: Test all terminal interactions")]
    public bool testTerminalSystem = false;

    [Header("🔧 OLD SYSTEM (Legacy)")]
    [Space(5)]
    [Tooltip("Click to create the EscapeCodeManager GameObject")]
    public bool createEscapeCodeManager = false;
    
    [Tooltip("Click to create basic AI Terminal")]
    public bool createAITerminal = false;
    
    [Tooltip("Click to create Cipher Wheel Puzzle UI")]
    public bool createCipherWheelUI = false;
    
    [Tooltip("Click to create Frequency Puzzle UI")]
    public bool createFrequencyUI = false;
    
    [Tooltip("Click to create Shadow Logic Puzzle UI")]
    public bool createShadowLogicUI = false;
    
    [Tooltip("Click to create Win Screen")]
    public bool createWinScreen = false;

    [Header("References")]
    public Canvas mainCanvas;
    public Camera mainCamera;

    void OnValidate()
    {
        // QUICK ONE-CLICK SETUP
        if (doCompleteSetup)
        {
            DoCompleteSetup();
            doCompleteSetup = false;
        }

        // NEW TERMINAL SYSTEM SETUP
        if (cleanupAITerminal)
        {
            CleanupAITerminal();
            cleanupAITerminal = false;
        }
        
        if (setupTerminalSystem)
        {
            SetupTerminalSystem();
            setupTerminalSystem = false;
        }
        
        if (createPuzzleTerminals)
        {
            CreatePuzzleTerminals();
            createPuzzleTerminals = false;
        }
        
        if (styleAllTerminals)
        {
            StyleAllTerminals();
            styleAllTerminals = false;
        }
        
        if (testTerminalSystem)
        {
            TestTerminalSystem();
            testTerminalSystem = false;
        }

        // LEGACY SYSTEM
        if (createEscapeCodeManager)
        {
            CreateEscapeCodeManager();
            createEscapeCodeManager = false;
        }
        
        if (createAITerminal)
        {
            CreateAITerminal();
            createAITerminal = false;
        }
        
        if (createCipherWheelUI)
        {
            CreateCipherWheelUI();
            createCipherWheelUI = false;
        }
        
        if (createFrequencyUI)
        {
            CreateFrequencyUI();
            createFrequencyUI = false;
        }
        
        if (createShadowLogicUI)
        {
            CreateShadowLogicUI();
            createShadowLogicUI = false;
        }
        
        if (createWinScreen)
        {
            CreateWinScreen();
            createWinScreen = false;
        }
    }

    // ============================================================================
    // 🎯 NEW TERMINAL SYSTEM SETUP METHODS
    // ============================================================================

    [ContextMenu("🔥 Complete Terminal Setup (One-Click)")]
    private void DoCompleteSetup()
    {
        Debug.Log("🔥 Starting complete terminal system setup...");
        
        // Check if we have an AI Terminal
        if (GameObject.Find("AITerminal") == null)
        {
            Debug.LogError("❌ No AITerminal found! Please create it first using 'Create AI Terminal' in the Legacy section.");
            return;
        }

        // Run all setup steps
        CleanupAITerminal();
        SetupTerminalSystem();
        CreatePuzzleTerminals();
        StyleAllTerminals();

        Debug.Log("🎯 🎉 COMPLETE SETUP FINISHED! Your terminal system is ready!");
        Debug.Log("📝 Next steps:");
        Debug.Log("   • Position the terminals around your room as needed");
        Debug.Log("   • Test the system using 'Test Terminal System'");
        Debug.Log("   • Run your game and walk up to terminals, press E to interact!");
    }

    [ContextMenu("🚀 1. Clean Up AI Terminal")]
    private void CleanupAITerminal()
    {
        Debug.Log("🔧 Cleaning up AI Terminal...");

        GameObject aiTerminal = GameObject.Find("AITerminal");
        if (aiTerminal == null)
        {
            Debug.LogError("❌ AITerminal not found! Please create it first using the legacy options.");
            return;
        }

        // Remove old InteractableAITerminal component if it exists
        InteractableAITerminal oldComponent = aiTerminal.GetComponent<InteractableAITerminal>();
        if (oldComponent != null)
        {
            DestroyImmediate(oldComponent);
            Debug.Log("✅ Removed old InteractableAITerminal component");
        }

        // Set correct layer
        int interactableLayer = LayerMask.NameToLayer("Interactable");
        if (interactableLayer != -1)
        {
            aiTerminal.layer = interactableLayer;
            Debug.Log("✅ Set AITerminal to Interactable layer");
        }

        // Fix BoxCollider for proper raycasting
        BoxCollider collider = aiTerminal.GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.isTrigger = false; // This is important for raycasting!
            Debug.Log("✅ Set BoxCollider for proper raycasting (isTrigger = false)");
        }

        // Make sure it has InteractableAITerminalNew
        if (aiTerminal.GetComponent<InteractableAITerminalNew>() == null)
        {
            aiTerminal.AddComponent<InteractableAITerminalNew>();
            Debug.Log("✅ Added new InteractableAITerminalNew component");
        }

        Debug.Log("🎯 AI Terminal cleanup complete! Ready for player interaction.");
    }

    [ContextMenu("🎮 2. Setup Terminal System")]
    private void SetupTerminalSystem()
    {
        Debug.Log("🎮 Setting up complete terminal system...");

        // Create Terminal System Manager
        GameObject systemManager = GameObject.Find("TerminalSystemManager");
        if (systemManager == null)
        {
            systemManager = new GameObject("TerminalSystemManager");
            systemManager.AddComponent<TerminalSystemManager>();
            Debug.Log("✅ Created TerminalSystemManager");
        }

        // Add TerminalUIHelper if not exists
        if (GetComponent<TerminalUIHelper>() == null)
        {
            gameObject.AddComponent<TerminalUIHelper>();
            Debug.Log("✅ Added TerminalUIHelper to SetUpHelper");
        }

        // Add UIConnectionHelper if not exists
        if (GetComponent<UIConnectionHelper>() == null)
        {
            gameObject.AddComponent<UIConnectionHelper>();
            Debug.Log("✅ Added UIConnectionHelper to SetUpHelper");
        }

        Debug.Log("🎯 Terminal system setup complete!");
    }

    [ContextMenu("🔧 3. Create Puzzle Terminals")]
    private void CreatePuzzleTerminals()
    {
        Debug.Log("🔧 Creating individual puzzle terminals...");

        TerminalUIHelper uiHelper = GetComponent<TerminalUIHelper>();
        if (uiHelper == null)
        {
            Debug.LogError("❌ TerminalUIHelper not found! Run 'Setup Terminal System' first.");
            return;
        }

        // Check if puzzles exist
        GameObject cipherCanvas = GameObject.Find("CipherWheelCanvas");
        GameObject shadowCanvas = GameObject.Find("ShadowLogicCanvas");
        GameObject frequencyCanvas = GameObject.Find("FrequencyCanvas");

        if (cipherCanvas == null && shadowCanvas == null && frequencyCanvas == null)
        {
            Debug.LogWarning("⚠️ No puzzle canvases found! Create puzzles first using legacy options.");
            return;
        }

        // Create terminals for existing puzzles
        uiHelper.SetupExistingPuzzleUIs();

        // Position terminals around the room
        PositionPuzzleTerminals();

        Debug.Log("🎯 Puzzle terminals created! Position them as needed around your room.");
    }

    private void PositionPuzzleTerminals()
    {
        // Position Cipher Wheel Terminal
        GameObject cipherTerminal = GameObject.Find("CipherWheelTerminal");
        if (cipherTerminal != null)
        {
            cipherTerminal.transform.position = new Vector3(-3, 0.5f, 2);
            cipherTerminal.transform.rotation = Quaternion.Euler(0, 45, 0);
            AddTerminalVisuals(cipherTerminal, "Cipher Terminal");
        }

        // Position Shadow Logic Terminal
        GameObject shadowTerminal = GameObject.Find("ShadowLogicTerminal");
        if (shadowTerminal != null)
        {
            shadowTerminal.transform.position = new Vector3(3, 0.5f, 2);
            shadowTerminal.transform.rotation = Quaternion.Euler(0, -45, 0);
            AddTerminalVisuals(shadowTerminal, "Shadow Terminal");
        }

        // Position Frequency Terminal (if exists)
        GameObject freqTerminal = GameObject.Find("FrequencyResonanceTerminal");
        if (freqTerminal != null)
        {
            freqTerminal.transform.position = new Vector3(0, 0.5f, 3);
            freqTerminal.transform.rotation = Quaternion.identity;
            AddTerminalVisuals(freqTerminal, "Frequency Terminal");
        }

        Debug.Log("✅ Positioned all puzzle terminals around the room");
    }

    private void AddTerminalVisuals(GameObject terminal, string terminalType)
    {
        // Make it look like a proper terminal
        MeshRenderer renderer = terminal.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            // Create a simple dark material
            Material terminalMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            terminalMaterial.color = new Color(0.1f, 0.1f, 0.2f, 1f);
            terminalMaterial.SetFloat("_Metallic", 0.8f);
            terminalMaterial.SetFloat("_Smoothness", 0.6f);
            renderer.material = terminalMaterial;
        }

        // Add a light for visual feedback
        GameObject lightObj = new GameObject("TerminalLight");
        lightObj.transform.SetParent(terminal.transform);
        lightObj.transform.localPosition = new Vector3(0, 1, 0);
        
        Light terminalLight = lightObj.AddComponent<Light>();
        terminalLight.type = LightType.Point;
        terminalLight.color = Color.cyan;
        terminalLight.intensity = 0.5f;
        terminalLight.range = 3f;
        terminalLight.enabled = false; // Will be controlled by PuzzleTerminal script

        // Connect light to PuzzleTerminal component
        PuzzleTerminal puzzleTerminal = terminal.GetComponent<PuzzleTerminal>();
        if (puzzleTerminal != null)
        {
            puzzleTerminal.terminalLight = terminalLight;
        }

        Debug.Log($"✅ Added visuals to {terminalType}");
    }

    [ContextMenu("🎨 4. Style All Terminals")]
    private void StyleAllTerminals()
    {
        Debug.Log("🎨 Styling all terminals...");

        TerminalUIHelper uiHelper = GetComponent<TerminalUIHelper>();
        if (uiHelper != null)
        {
            uiHelper.StyleAllTerminalUIs();
        }

        // Style the main AI terminal too
        GameObject aiTerminal = GameObject.Find("AITerminal");
        if (aiTerminal != null)
        {
            AddTerminalVisuals(aiTerminal, "AI Terminal");
        }

        Debug.Log("🎯 All terminals styled consistently!");
    }

    [ContextMenu("🧪 5. Test Terminal System")]
    private void TestTerminalSystem()
    {
        Debug.Log("🧪 Testing terminal system...");

        // Find all terminals
        PuzzleTerminal[] puzzleTerminals = FindObjectsByType<PuzzleTerminal>(FindObjectsSortMode.None);
        InteractableAITerminalNew[] aiTerminals = FindObjectsByType<InteractableAITerminalNew>(FindObjectsSortMode.None);

        Debug.Log($"Found {puzzleTerminals.Length} puzzle terminals and {aiTerminals.Length} AI terminals");

        // Test puzzle terminals
        foreach (PuzzleTerminal terminal in puzzleTerminals)
        {
            if (terminal.terminalCanvas == null)
                Debug.LogWarning($"⚠️ {terminal.name} has no terminal canvas assigned!");
            
            if (terminal.terminalLight == null)
                Debug.LogWarning($"⚠️ {terminal.name} has no terminal light assigned!");
            else
                Debug.Log($"✅ {terminal.name} is properly configured");
        }

        // Test AI terminals
        foreach (InteractableAITerminalNew aiTerminal in aiTerminals)
        {
            if (aiTerminal.terminalController == null)
                Debug.LogWarning($"⚠️ {aiTerminal.name} has no terminal controller assigned!");
            else
                Debug.Log($"✅ {aiTerminal.name} is properly configured");
        }

        // Check player setup
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        if (players.Length == 0)
        {
            Debug.LogWarning("⚠️ No PlayerController found! Make sure you have a player in the scene.");
        }
        else
        {
            Debug.Log($"✅ Found {players.Length} PlayerController(s)");
        }

        Debug.Log("🎯 Terminal system test complete! Check warnings above if any.");
    }

    // ============================================================================
    // 🔧 LEGACY SYSTEM METHODS (Kept for compatibility)
    // ============================================================================

    private void CreateEscapeCodeManager()
    {
        GameObject escapeManager = new GameObject("EscapeCodeManager");
        escapeManager.AddComponent<EscapeCodeManager>();
        
        // Create UI Canvas for status display
        GameObject statusCanvas = new GameObject("EscapeStatusCanvas");
        Canvas canvas = statusCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        statusCanvas.AddComponent<CanvasScaler>();
        statusCanvas.AddComponent<GraphicRaycaster>();
        
        // Create status panel
        GameObject statusPanel = CreateUIPanel(statusCanvas.transform, "StatusPanel", new Vector2(300, 200));
        statusPanel.transform.SetAsFirstSibling();
        RectTransform statusRect = statusPanel.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0, 1);
        statusRect.anchorMax = new Vector2(0, 1);
        statusRect.anchoredPosition = new Vector2(150, -100);
        
        // Create status texts
        GameObject progressText = CreateText(statusPanel.transform, "ProgressDisplay", "Progress: 0/3 puzzles completed");
        GameObject numbersText = CreateText(statusPanel.transform, "CollectedNumbersDisplay", "No numbers collected yet");
        GameObject codeText = CreateText(statusPanel.transform, "FinalCodeDisplay", "Complete all puzzles to reveal code");
        
        // Position texts
        PositionText(progressText, new Vector2(0, 50));
        PositionText(numbersText, new Vector2(0, 0));
        PositionText(codeText, new Vector2(0, -50));
        
        // Connect references
        EscapeCodeManager manager = escapeManager.GetComponent<EscapeCodeManager>();
        manager.progressDisplay = progressText.GetComponent<TMP_Text>();
        manager.collectedNumbersDisplay = numbersText.GetComponent<TMP_Text>();
        manager.finalCodeDisplay = codeText.GetComponent<TMP_Text>();
        
        Debug.Log("Created EscapeCodeManager with status UI!");
    }

    private void CreateAITerminal()
    {
        // Create terminal object
        GameObject terminal = GameObject.CreatePrimitive(PrimitiveType.Cube);
        terminal.name = "AITerminal";
        terminal.transform.position = new Vector3(0, 1, 0);
        terminal.transform.localScale = new Vector3(2, 1, 0.2f);
        
        // Add components
        terminal.AddComponent<TerminalControllerNew>();
        terminal.AddComponent<InteractableAITerminal>();
        
        // Set up collider for interaction
        BoxCollider collider = terminal.GetComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(1.5f, 1.5f, 2f);
        
        // Create terminal UI canvas
        GameObject terminalCanvas = new GameObject("TerminalCanvas");
        Canvas canvas = terminalCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        terminalCanvas.AddComponent<CanvasScaler>();
        terminalCanvas.AddComponent<GraphicRaycaster>();
        terminalCanvas.SetActive(false);
        
        // Create terminal panel
        GameObject terminalPanel = CreateUIPanel(terminalCanvas.transform, "TerminalPanel", new Vector2(800, 600));
        terminalPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.9f);
        
        // Create terminal elements
        GameObject header = CreateText(terminalPanel.transform, "Header", "ESCAPE ROOM AI ASSISTANT");
        GameObject instructions = CreateText(terminalPanel.transform, "Instructions", "Select a puzzle for guidance");
        GameObject hintText = CreateText(terminalPanel.transform, "HintText", "");
        
        // Create buttons
        GameObject button1 = CreateButton(terminalPanel.transform, "Button1", "1. Cipher Wheel Puzzle");
        GameObject button2 = CreateButton(terminalPanel.transform, "Button2", "2. Frequency Resonance Puzzle");
        GameObject button3 = CreateButton(terminalPanel.transform, "Button3", "3. Shadow Logic Puzzle");
        GameObject closeBtn = CreateButton(terminalPanel.transform, "CloseButton", "Close");
        
        // Position elements
        PositionText(header, new Vector2(0, 250));
        PositionText(instructions, new Vector2(0, 200));
        PositionText(hintText, new Vector2(0, 0));
        
        PositionButton(button1, new Vector2(-200, 150));
        PositionButton(button2, new Vector2(0, 150));
        PositionButton(button3, new Vector2(200, 150));
        PositionButton(closeBtn, new Vector2(0, -250));
        
        // Set up hint text area
        TMP_Text hintTMP = hintText.GetComponent<TMP_Text>();
        hintTMP.fontSize = 14;
        hintTMP.alignment = TextAlignmentOptions.TopLeft;
        RectTransform hintRect = hintText.GetComponent<RectTransform>();
        hintRect.sizeDelta = new Vector2(700, 300);
        
        // Connect references to TerminalController
        TerminalControllerNew controller = terminal.GetComponent<TerminalControllerNew>();
        controller.terminalUI = terminalCanvas;
        controller.hintText = hintTMP;
        controller.button1 = button1.GetComponent<Button>();
        controller.button2 = button2.GetComponent<Button>();
        controller.button3 = button3.GetComponent<Button>();
        controller.buttonText1 = button1.GetComponentInChildren<TMP_Text>();
        controller.buttonText2 = button2.GetComponentInChildren<TMP_Text>();
        controller.buttonText3 = button3.GetComponentInChildren<TMP_Text>();
        controller.closeButton = closeBtn.GetComponent<Button>();
        controller.terminalHeader = header.GetComponent<TMP_Text>();
        controller.instructionText = instructions.GetComponent<TMP_Text>();
        
        // Create interaction prompt
        GameObject promptCanvas = new GameObject("InteractionCanvas");
        promptCanvas.transform.SetParent(terminal.transform);
        Canvas promptCanvasComp = promptCanvas.AddComponent<Canvas>();
        promptCanvasComp.renderMode = RenderMode.WorldSpace;
        promptCanvasComp.sortingOrder = 1;
        promptCanvas.AddComponent<CanvasScaler>();
        
        RectTransform promptRect = promptCanvas.GetComponent<RectTransform>();
        promptRect.sizeDelta = new Vector2(300, 100);
        promptRect.localPosition = new Vector3(0, 2, 0);
        promptRect.localScale = Vector3.one * 0.01f;
        
        GameObject promptText = CreateText(promptCanvas.transform, "InteractionPrompt", "Press E to access terminal");
        promptText.GetComponent<TMP_Text>().fontSize = 24;
        promptText.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;
        promptText.SetActive(false);
        
        // Connect to InteractableAITerminal
        InteractableAITerminal interactable = terminal.GetComponent<InteractableAITerminal>();
        interactable.terminalController = controller;
        interactable.interactionPrompt = promptText.GetComponent<TMP_Text>();
        
        Debug.Log("Created AI Terminal with full UI!");
    }

    private void CreateCipherWheelUI()
    {
        // Create cipher wheel puzzle object
        GameObject cipherPuzzle = new GameObject("CipherWheelPuzzle");
        cipherPuzzle.AddComponent<CipherWheelPuzzle>();
        
        // Create UI canvas
        GameObject canvas = CreatePuzzleCanvas("CipherWheelCanvas");
        GameObject panel = CreateUIPanel(canvas.transform, "CipherWheelPanel", new Vector2(600, 400));
        
        // Create UI elements
        GameObject symbolDisplay = CreateText(panel.transform, "SymbolDisplay", "△");
        GameObject letterDisplay = CreateText(panel.transform, "LetterDisplay", "A");
        GameObject symbolBtn = CreateButton(panel.transform, "SymbolButton", "Next Symbol");
        GameObject letterBtn = CreateButton(panel.transform, "LetterButton", "Next Letter");
        GameObject testBtn = CreateButton(panel.transform, "TestMatchButton", "Test Match");
        GameObject inputField = CreateInputField(panel.transform, "CodeInputField", "Enter decoded password");
        GameObject submitBtn = CreateButton(panel.transform, "SubmitButton", "Submit");
        GameObject resetBtn = CreateButton(panel.transform, "ResetButton", "Reset");
        GameObject sequenceDisplay = CreateText(panel.transform, "SequenceDisplay", "Find symbol sequences around the room");
        GameObject mappingsDisplay = CreateText(panel.transform, "MappingsDisplay", "No mappings discovered yet");
        GameObject feedback = CreateText(panel.transform, "FeedbackText", "");
        
        // Position elements
        PositionText(symbolDisplay, new Vector2(-200, 120));
        PositionText(letterDisplay, new Vector2(200, 120));
        PositionButton(symbolBtn, new Vector2(-200, 80));
        PositionButton(letterBtn, new Vector2(200, 80));
        PositionButton(testBtn, new Vector2(0, 40));
        PositionInputField(inputField, new Vector2(0, 0));
        PositionButton(submitBtn, new Vector2(-80, -40));
        PositionButton(resetBtn, new Vector2(80, -40));
        PositionText(sequenceDisplay, new Vector2(-150, -80));
        PositionText(mappingsDisplay, new Vector2(150, -80));
        PositionText(feedback, new Vector2(0, -150));
        
        // Connect references
        CipherWheelPuzzle puzzle = cipherPuzzle.GetComponent<CipherWheelPuzzle>();
        puzzle.puzzleUI = canvas;
        puzzle.symbolDisplay = symbolDisplay.GetComponent<TMP_Text>();
        puzzle.letterDisplay = letterDisplay.GetComponent<TMP_Text>();
        puzzle.symbolButton = symbolBtn.GetComponent<Button>();
        puzzle.letterButton = letterBtn.GetComponent<Button>();
        puzzle.testMatchButton = testBtn.GetComponent<Button>();
        puzzle.codeInputField = inputField.GetComponent<TMP_InputField>();
        puzzle.submitButton = submitBtn.GetComponent<Button>();
        puzzle.resetButton = resetBtn.GetComponent<Button>();
        puzzle.symbolSequenceDisplay = sequenceDisplay.GetComponent<TMP_Text>();
        puzzle.discoveredMappingsDisplay = mappingsDisplay.GetComponent<TMP_Text>();
        puzzle.feedbackText = feedback.GetComponent<TMP_Text>();
        
        canvas.SetActive(false);
        Debug.Log("Created Cipher Wheel Puzzle UI!");
    }

    private void CreateFrequencyUI()
    {
        GameObject freqPuzzle = new GameObject("FrequencyResonancePuzzle");
        freqPuzzle.AddComponent<FrequencyResonancePuzzle>();
        
        GameObject canvas = CreatePuzzleCanvas("FrequencyCanvas");
        GameObject panel = CreateUIPanel(canvas.transform, "FrequencyPanel", new Vector2(700, 500));
        
        // Create frequency slider
        GameObject freqSlider = CreateSlider(panel.transform, "FrequencySlider", 0f, 1f, 0.5f);
        GameObject freqDisplay = CreateText(panel.transform, "FrequencyDisplay", "440 Hz");
        
        // Create frequency buttons (5 buttons)
        GameObject[] freqButtons = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            freqButtons[i] = CreateButton(panel.transform, $"FrequencyButton{i + 1}", $"Tone {i + 1}");
            PositionButton(freqButtons[i], new Vector2(-200 + i * 100, 100));
        }
        
        GameObject submitBtn = CreateButton(panel.transform, "SubmitSequence", "Submit Sequence");
        GameObject clearBtn = CreateButton(panel.transform, "ClearSequence", "Clear");
        GameObject sequenceDisplay = CreateText(panel.transform, "SequenceDisplay", "Sequence: None");
        GameObject feedback = CreateText(panel.transform, "FeedbackText", "");
        
        // Position elements
        PositionSlider(freqSlider, new Vector2(0, 150));
        PositionText(freqDisplay, new Vector2(0, 180));
        PositionButton(submitBtn, new Vector2(-50, 0));
        PositionButton(clearBtn, new Vector2(50, 0));
        PositionText(sequenceDisplay, new Vector2(0, -50));
        PositionText(feedback, new Vector2(0, -100));
        
        // Create audio sources
        GameObject[] audioSources = new GameObject[5];
        for (int i = 0; i < 5; i++)
        {
            audioSources[i] = new GameObject($"FrequencySource{i + 1}");
            audioSources[i].transform.SetParent(freqPuzzle.transform);
            audioSources[i].AddComponent<AudioSource>();
        }
        
        // Connect references
        FrequencyResonancePuzzle puzzle = freqPuzzle.GetComponent<FrequencyResonancePuzzle>();
        puzzle.puzzleUI = canvas;
        puzzle.frequencySlider = freqSlider.GetComponent<Slider>();
        puzzle.frequencyDisplay = freqDisplay.GetComponent<TMP_Text>();
        puzzle.submitSequenceButton = submitBtn.GetComponent<Button>();
        puzzle.clearSequenceButton = clearBtn.GetComponent<Button>();
        puzzle.sequenceDisplay = sequenceDisplay.GetComponent<TMP_Text>();
        puzzle.feedbackText = feedback.GetComponent<TMP_Text>();
        
        // Set up button and audio arrays
        Button[] buttonArray = new Button[5];
        AudioSource[] audioArray = new AudioSource[5];
        for (int i = 0; i < 5; i++)
        {
            buttonArray[i] = freqButtons[i].GetComponent<Button>();
            audioArray[i] = audioSources[i].GetComponent<AudioSource>();
        }
        puzzle.frequencyButtons = buttonArray;
        puzzle.frequencySources = audioArray;
        
        canvas.SetActive(false);
        Debug.Log("Created Frequency Resonance Puzzle UI!");
    }

    private void CreateShadowLogicUI()
    {
        GameObject shadowPuzzle = new GameObject("ShadowLogicPuzzle");
        shadowPuzzle.AddComponent<ShadowLogicPuzzle>();
        
        GameObject canvas = CreatePuzzleCanvas("ShadowLogicCanvas");
        GameObject panel = CreateUIPanel(canvas.transform, "ShadowLogicPanel", new Vector2(800, 600));
        
        // Create light control sliders (3 sets)
        GameObject[] rotationSliders = new GameObject[3];
        GameObject[] intensitySliders = new GameObject[3];
        GameObject[] toggleButtons = new GameObject[3];
        
        for (int i = 0; i < 3; i++)
        {
            rotationSliders[i] = CreateSlider(panel.transform, $"LightRotationSlider{i + 1}", 0f, 360f, 0f);
            intensitySliders[i] = CreateSlider(panel.transform, $"LightIntensitySlider{i + 1}", 0f, 2f, 1f);
            toggleButtons[i] = CreateButton(panel.transform, $"LightToggle{i + 1}", $"Light {i + 1} ON/OFF");
            
            PositionSlider(rotationSliders[i], new Vector2(-200 + i * 200, 200));
            PositionSlider(intensitySliders[i], new Vector2(-200 + i * 200, 150));
            PositionButton(toggleButtons[i], new Vector2(-200 + i * 200, 100));
        }
        
        // Create object move buttons (3 buttons)
        GameObject[] moveButtons = new GameObject[3];
        for (int i = 0; i < 3; i++)
        {
            moveButtons[i] = CreateButton(panel.transform, $"MoveObject{i + 1}", $"Move Object {i + 1}");
            PositionButton(moveButtons[i], new Vector2(-100 + i * 100, 50));
        }
        
        GameObject codeDisplay = CreateText(panel.transform, "ShadowCodeDisplay", "----");
        GameObject statusDisplay = CreateText(panel.transform, "AlignmentStatus", "Lights: 0/3, Objects: 0/3");
        GameObject submitBtn = CreateButton(panel.transform, "SubmitButton", "Submit Solution");
        GameObject feedback = CreateText(panel.transform, "FeedbackText", "");
        
        PositionText(codeDisplay, new Vector2(0, 0));
        PositionText(statusDisplay, new Vector2(0, -50));
        PositionButton(submitBtn, new Vector2(0, -100));
        PositionText(feedback, new Vector2(0, -150));
        
        // Create lights and objects
        GameObject[] lights = new GameObject[3];
        GameObject[] shadowObjects = new GameObject[3];
        GameObject[] objectTargets = new GameObject[3];
        
        for (int i = 0; i < 3; i++)
        {
            // Create lights
            lights[i] = new GameObject($"AdjustableLight{i + 1}");
            lights[i].transform.SetParent(shadowPuzzle.transform);
            lights[i].transform.position = new Vector3(-2 + i * 2, 3, 0);
            Light lightComp = lights[i].AddComponent<Light>();
            lightComp.type = LightType.Spot;
            lightComp.intensity = 1f;
            
            // Create shadow objects
            shadowObjects[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shadowObjects[i].name = $"ShadowObject{i + 1}";
            shadowObjects[i].transform.SetParent(shadowPuzzle.transform);
            shadowObjects[i].transform.position = new Vector3(-1 + i * 1, 1, -2);
            shadowObjects[i].transform.localScale = new Vector3(0.5f, 1f, 0.5f);
            
            // Create target positions
            objectTargets[i] = new GameObject($"ShadowObjectTarget{i + 1}");
            objectTargets[i].transform.SetParent(shadowPuzzle.transform);
            objectTargets[i].transform.position = new Vector3(-1 + i * 1, 1, -1);
        }
        
        // Connect references
        ShadowLogicPuzzle puzzle = shadowPuzzle.GetComponent<ShadowLogicPuzzle>();
        puzzle.puzzleUI = canvas;
        puzzle.shadowCodeDisplay = codeDisplay.GetComponent<TMP_Text>();
        puzzle.alignmentStatusDisplay = statusDisplay.GetComponent<TMP_Text>();
        puzzle.submitButton = submitBtn.GetComponent<Button>();
        puzzle.feedbackText = feedback.GetComponent<TMP_Text>();
        
        // Set up arrays
        Slider[] rotSliders = new Slider[3];
        Slider[] intSliders = new Slider[3];
        Button[] toggleBtns = new Button[3];
        Button[] moveBtns = new Button[3];
        Light[] lightArray = new Light[3];
        Transform[] objArray = new Transform[3];
        Transform[] targetArray = new Transform[3];
        
        for (int i = 0; i < 3; i++)
        {
            rotSliders[i] = rotationSliders[i].GetComponent<Slider>();
            intSliders[i] = intensitySliders[i].GetComponent<Slider>();
            toggleBtns[i] = toggleButtons[i].GetComponent<Button>();
            moveBtns[i] = moveButtons[i].GetComponent<Button>();
            lightArray[i] = lights[i].GetComponent<Light>();
            objArray[i] = shadowObjects[i].transform;
            targetArray[i] = objectTargets[i].transform;
        }
        
        puzzle.lightRotationSliders = rotSliders;
        puzzle.lightIntensitySliders = intSliders;
        puzzle.lightToggleButtons = toggleBtns;
        puzzle.moveObjectButtons = moveBtns;
        puzzle.adjustableLights = lightArray;
        puzzle.shadowObjects = objArray;
        puzzle.shadowObjectTargets = targetArray;
        
        canvas.SetActive(false);
        Debug.Log("Created Shadow Logic Puzzle with lights and objects!");
    }

    private void CreateWinScreen()
    {
        GameObject winCanvas = new GameObject("WinScreen");
        Canvas canvas = winCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        winCanvas.AddComponent<CanvasScaler>();
        winCanvas.AddComponent<GraphicRaycaster>();
        winCanvas.AddComponent<WinScreen>();
        
        GameObject winPanel = CreateUIPanel(winCanvas.transform, "WinPanel", new Vector2(600, 400));
        winPanel.GetComponent<Image>().color = new Color(0, 0.2f, 0, 0.9f);
        
        GameObject congratsText = CreateText(winPanel.transform, "CongratsText", "CONGRATULATIONS!\nYou have escaped!");
        GameObject timeText = CreateText(winPanel.transform, "EscapeTimeText", "Escape Time: 00:00");
        GameObject numbersText = CreateText(winPanel.transform, "CollectedNumbersText", "Numbers collected: ");
        GameObject codeText = CreateText(winPanel.transform, "FinalCodeText", "Final code: ");
        
        GameObject restartBtn = CreateButton(winPanel.transform, "RestartButton", "Restart");
        GameObject quitBtn = CreateButton(winPanel.transform, "QuitButton", "Quit");
        
        // Position elements
        PositionText(congratsText, new Vector2(0, 150));
        PositionText(timeText, new Vector2(0, 100));
        PositionText(numbersText, new Vector2(0, 50));
        PositionText(codeText, new Vector2(0, 0));
        PositionButton(restartBtn, new Vector2(-100, -100));
        PositionButton(quitBtn, new Vector2(100, -100));
        
        // Connect references
        WinScreen winScreen = winCanvas.GetComponent<WinScreen>();
        winScreen.winPanel = winPanel;
        winScreen.congratsText = congratsText.GetComponent<TMP_Text>();
        winScreen.escapeTimeText = timeText.GetComponent<TMP_Text>();
        winScreen.collectedNumbersText = numbersText.GetComponent<TMP_Text>();
        winScreen.finalCodeText = codeText.GetComponent<TMP_Text>();
        winScreen.restartButton = restartBtn.GetComponent<Button>();
        winScreen.quitButton = quitBtn.GetComponent<Button>();
        
        winCanvas.SetActive(false);
        Debug.Log("Created Win Screen!");
    }

    // Helper methods for UI creation
    private GameObject CreatePuzzleCanvas(string name)
    {
        GameObject canvas = new GameObject(name);
        Canvas canvasComp = canvas.AddComponent<Canvas>();
        canvasComp.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasComp.sortingOrder = 50;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private GameObject CreateUIPanel(Transform parent, string name, Vector2 size)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent);
        panel.AddComponent<CanvasRenderer>();
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;
        
        return panel;
    }

    private GameObject CreateText(Transform parent, string name, string text)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent);
        textObj.AddComponent<CanvasRenderer>();
        TMP_Text tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = 16;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = Color.white;
        
        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 30);
        
        return textObj;
    }

    private GameObject CreateButton(Transform parent, string name, string text)
    {
        GameObject button = new GameObject(name);
        button.transform.SetParent(parent);
        button.AddComponent<CanvasRenderer>();
        Image image = button.AddComponent<Image>();
        image.color = new Color(0.3f, 0.3f, 0.8f, 1f);
        
        Button btn = button.AddComponent<Button>();
        
        GameObject buttonText = CreateText(button.transform, "Text", text);
        buttonText.GetComponent<TMP_Text>().color = Color.white;
        
        RectTransform rect = button.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(150, 40);
        
        return button;
    }

    private GameObject CreateInputField(Transform parent, string name, string placeholder)
    {
        GameObject inputField = new GameObject(name);
        inputField.transform.SetParent(parent);
        inputField.AddComponent<CanvasRenderer>();
        Image image = inputField.AddComponent<Image>();
        image.color = Color.white;
        
        TMP_InputField input = inputField.AddComponent<TMP_InputField>();
        
        // Create text area
        GameObject textArea = new GameObject("Text Area");
        textArea.transform.SetParent(inputField.transform);
        textArea.AddComponent<CanvasRenderer>();
        RectMask2D mask = textArea.AddComponent<RectMask2D>();
        
        // Create placeholder
        GameObject placeholderObj = CreateText(textArea.transform, "Placeholder", placeholder);
        TMP_Text placeholderText = placeholderObj.GetComponent<TMP_Text>();
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        placeholderText.alignment = TextAlignmentOptions.Left;
        
        // Create input text
        GameObject inputText = CreateText(textArea.transform, "Text", "");
        TMP_Text inputTMP = inputText.GetComponent<TMP_Text>();
        inputTMP.color = Color.black;
        inputTMP.alignment = TextAlignmentOptions.Left;
        
        input.textComponent = inputTMP;
        input.placeholder = placeholderText;
        
        RectTransform rect = inputField.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 30);
        
        return inputField;
    }

    private GameObject CreateSlider(Transform parent, string name, float min, float max, float value)
    {
        GameObject slider = new GameObject(name);
        slider.transform.SetParent(parent);
        slider.AddComponent<CanvasRenderer>();
        
        Slider sliderComp = slider.AddComponent<Slider>();
        sliderComp.minValue = min;
        sliderComp.maxValue = max;
        sliderComp.value = value;
        
        // Create background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(slider.transform);
        background.AddComponent<CanvasRenderer>();
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        // Create handle
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(slider.transform);
        handle.AddComponent<CanvasRenderer>();
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        
        sliderComp.handleRect = handle.GetComponent<RectTransform>();
        sliderComp.targetGraphic = handleImage;
        
        RectTransform rect = slider.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(150, 20);
        
        return slider;
    }

    // Helper positioning methods
    private void PositionText(GameObject obj, Vector2 position)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
    }

    private void PositionButton(GameObject obj, Vector2 position)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
    }

    private void PositionInputField(GameObject obj, Vector2 position)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
    }

    private void PositionSlider(GameObject obj, Vector2 position)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
    }
}
#endif