using UnityEngine;
using TMPro;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Setup helper for creating riddle terminals in Unity 6
/// Creates an interactable terminal that triggers the riddle puzzle UI
/// </summary>
[System.Serializable]
public class RiddleTerminalSetup : MonoBehaviour
{
    [Header("Terminal Configuration")]
    [Tooltip("Name for the riddle terminal")]
    public string terminalName = "Riddle Terminal";
    
    [Tooltip("Position for the terminal")]
    public Vector3 terminalPosition = Vector3.zero;
    
    [Tooltip("Rotation for the terminal")]
    public Vector3 terminalRotation = Vector3.zero;
    
    [Tooltip("Size of the terminal")]
    public Vector3 terminalSize = new Vector3(1f, 2f, 0.5f);

    [Header("Visual Settings")]
    [Tooltip("Material for the terminal (optional)")]
    public Material terminalMaterial;
    
    [Tooltip("Material for hover effect (optional)")]
    public Material hoverMaterial;
    
    [Tooltip("Color when riddle is solved")]
    public Color solvedColor = Color.green;
    
    [Tooltip("Color when riddle is active")]
    public Color activeColor = Color.cyan;
    
    [Tooltip("Color when riddle is unsolved")]
    public Color unsolvedColor = Color.blue;

    [Header("UI Settings")]
    [Tooltip("Text shown on terminal screen")]
    [TextArea(3, 5)]
    public string terminalMessage = "RIDDLE CHALLENGE TERMINAL\n\nPress E to begin mental exercise\n\nStatus: READY";

    [Header("Audio Settings")]
    [Tooltip("Sound when terminal is accessed (optional)")]
    public AudioClip accessSound;
    
    [Tooltip("Sound when hovering (optional)")]
    public AudioClip hoverSound;

    [Header("Status")]
    [SerializeField, HideInInspector]
    private bool terminalCreated = false;
    
    [SerializeField, HideInInspector]
    public GameObject createdTerminal;

    void Start()
    {
        // Auto-create if not already created
        if (!terminalCreated)
        {
            CreateRiddleTerminal();
        }
    }

    [ContextMenu("Create Riddle Terminal")]
    public void CreateRiddleTerminal()
    {
        if (terminalCreated && createdTerminal != null)
        {
            Debug.LogWarning("Riddle terminal already exists! Use 'Remove Riddle Terminal' first if you want to recreate it.");
            return;
        }

        Debug.Log("Creating riddle terminal system...");

        try
        {
            // Create the main terminal object
            GameObject terminal = CreateTerminalObject();
            
            // Add collider for interaction
            AddColliderToTerminal(terminal);
            
            // Add the riddle terminal script
            RiddleTerminal riddleTerminalScript = terminal.AddComponent<RiddleTerminal>();
            
            // Configure the terminal
            ConfigureTerminal(riddleTerminalScript, terminal);
            
            // Create UI elements
            CreateTerminalUI(riddleTerminalScript, terminal);
            
            // Find and link to riddle puzzle
            LinkToRiddlePuzzle(riddleTerminalScript);
            
            // Mark as created
            createdTerminal = terminal;
            terminalCreated = true;
            
            Debug.Log($"✅ Riddle terminal '{terminalName}' created successfully!");
            Debug.Log("Players can now interact with the terminal to access riddle puzzles.");
            
            // Select the created terminal
            Selection.activeGameObject = terminal;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create riddle terminal: {e.Message}");
        }
    }

    private GameObject CreateTerminalObject()
    {
        // Create the main terminal parent GameObject
        GameObject terminal = new GameObject(terminalName);
        terminal.transform.position = terminalPosition;
        terminal.transform.rotation = Quaternion.Euler(terminalRotation);
        
        // Create terminal base/pedestal
        GameObject terminalBase = CreateTerminalBase(terminal.transform);
        
        // Create terminal console/monitor
        GameObject terminalMonitor = CreateTerminalMonitor(terminal.transform);
        
        // Create keyboard/input panel
        GameObject terminalKeyboard = CreateTerminalKeyboard(terminal.transform);
        
        // Add some detail elements
        CreateTerminalDetails(terminal.transform);
        
        // Set up the main collider for interaction
        BoxCollider terminalCollider = terminal.AddComponent<BoxCollider>();
        terminalCollider.size = new Vector3(1.2f, 2.0f, 0.8f);
        terminalCollider.center = new Vector3(0, 1.0f, 0);
        terminalCollider.isTrigger = true;
        
        return terminal;
    }

    private void AddColliderToTerminal(GameObject terminal)
    {
        // Make sure it has a collider for interaction
        BoxCollider collider = terminal.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = terminal.AddComponent<BoxCollider>();
        }
        
        // Set to Interactable layer
        int interactableLayer = LayerMask.NameToLayer("Interactable");
        if (interactableLayer != -1)
        {
            terminal.layer = interactableLayer;
        }
        else
        {
            Debug.LogWarning("Interactable layer not found! Terminal may not work properly.");
        }
    }

    private void ConfigureTerminal(RiddleTerminal riddleTerminal, GameObject terminal)
    {
        // Configure basic settings
        riddleTerminal.terminalName = terminalName;
        riddleTerminal.hoverText = "Press E to access riddle challenge";
        riddleTerminal.defaultMessage = terminalMessage;
        
        // Configure colors
        riddleTerminal.solvedColor = solvedColor;
        riddleTerminal.activeColor = activeColor;
        riddleTerminal.unsolvedColor = unsolvedColor;
        
        // Set materials
        if (hoverMaterial != null)
            riddleTerminal.hoverMaterial = hoverMaterial;
            
        // Set audio clips
        if (accessSound != null)
            riddleTerminal.accessSound = accessSound;
        if (hoverSound != null)
            riddleTerminal.hoverSound = hoverSound;
    }

    private void CreateTerminalUI(RiddleTerminal riddleTerminal, GameObject terminal)
    {
        // Create interaction prompt UI
        GameObject promptUI = CreateInteractionPrompt(terminal);
        if (promptUI != null)
        {
            TMP_Text promptText = promptUI.GetComponent<TMP_Text>();
            riddleTerminal.interactionPrompt = promptText;
        }
        
        // Create terminal screen UI
        GameObject screenUI = CreateTerminalScreen(terminal);
        if (screenUI != null)
        {
            TMP_Text screenText = screenUI.GetComponent<TMP_Text>();
            riddleTerminal.terminalScreen = screenText;
        }
        
        // Create terminal light
        GameObject lightObj = CreateTerminalLight(terminal);
        if (lightObj != null)
        {
            Light terminalLight = lightObj.GetComponent<Light>();
            riddleTerminal.terminalLight = terminalLight;
        }
    }

    private GameObject CreateInteractionPrompt(GameObject parent)
    {
        // Create Canvas for UI
        GameObject canvasObj = new GameObject("InteractionCanvas");
        canvasObj.transform.SetParent(parent.transform);
        canvasObj.transform.localPosition = new Vector3(0, 0.8f, 0);
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.dynamicPixelsPerUnit = 10;
        
        GraphicRaycaster raycaster = canvasObj.AddComponent<GraphicRaycaster>();
        
        // Set canvas size
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(200, 50);
        
        // Create text for interaction prompt
        GameObject textObj = new GameObject("InteractionPrompt");
        textObj.transform.SetParent(canvasObj.transform);
        
        TMP_Text text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "Press E to interact";
        text.fontSize = 14;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.textWrappingMode = TextWrappingModes.Normal;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        // Initially hidden
        canvasObj.SetActive(false);
        
        return textObj;
    }

    private GameObject CreateTerminalScreen(GameObject parent)
    {
        // Find the physical screen object we created
        Transform screenTransform = parent.transform.Find("TerminalMonitor/TerminalScreen");
        if (screenTransform == null)
        {
            Debug.LogWarning("Physical terminal screen not found. Creating UI on main terminal.");
            screenTransform = parent.transform;
        }
        
        // Create Canvas for terminal screen on the physical screen surface
        GameObject canvasObj = new GameObject("TerminalScreenCanvas");
        canvasObj.transform.SetParent(screenTransform);
        canvasObj.transform.localPosition = new Vector3(0, 0, 0.51f); // Just in front of screen
        canvasObj.transform.localRotation = Quaternion.identity;
        canvasObj.transform.localScale = Vector3.one;
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.dynamicPixelsPerUnit = 100;
        
        // Set canvas size to match the physical screen dimensions
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(100, 80); // Adjusted for physical screen
        
        // Create text for terminal screen
        GameObject textObj = new GameObject("TerminalScreen");
        textObj.transform.SetParent(canvasObj.transform);
        
        TMP_Text text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = terminalMessage;
        text.fontSize = 6;
        text.color = new Color(0.2f, 1f, 0.2f, 1f); // Bright green terminal text
        text.alignment = TextAlignmentOptions.Center;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.fontStyle = FontStyles.Bold;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = new Vector2(-10, -10); // Padding
        textRect.anchoredPosition = Vector2.zero;
        
        // Add subtle background for better text visibility
        Image background = textObj.AddComponent<Image>();
        background.color = new Color(0, 0.05f, 0, 0.9f); // Very dark green background
        
        return textObj;
    }

    private GameObject CreateTerminalLight(GameObject parent)
    {
        // Create light object positioned to illuminate the terminal
        GameObject lightObj = new GameObject("TerminalLight");
        lightObj.transform.SetParent(parent.transform);
        lightObj.transform.localPosition = new Vector3(0, 1.6f, 0.4f); // Above the monitor
        
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = unsolvedColor;
        light.intensity = 0.5f;
        light.range = 3f;
        light.enabled = false; // Start disabled
        
        return lightObj;
    }

    private void LinkToRiddlePuzzle(RiddleTerminal riddleTerminal)
    {
        // Try to find existing riddle puzzle in the scene
        RiddlePuzzle riddlePuzzle = FindFirstObjectByType<RiddlePuzzle>();
        
        if (riddlePuzzle != null)
        {
            riddleTerminal.SetLinkedRiddlePuzzle(riddlePuzzle);
            Debug.Log($"✅ Terminal linked to existing riddle puzzle: {riddlePuzzle.puzzleName}");
        }
        else
        {
            Debug.LogWarning("⚠️ No RiddlePuzzle found in scene. Create a riddle puzzle first or assign manually.");
        }
    }

    [ContextMenu("Remove Riddle Terminal")]
    public void RemoveRiddleTerminal()
    {
        if (createdTerminal != null)
        {
            DestroyImmediate(createdTerminal);
            Debug.Log("Riddle terminal removed.");
        }
        
        terminalCreated = false;
        createdTerminal = null;
    }

    [ContextMenu("Test Terminal Interaction")]
    public void TestTerminalInteraction()
    {
        if (createdTerminal != null)
        {
            RiddleTerminal terminal = createdTerminal.GetComponent<RiddleTerminal>();
            if (terminal != null)
            {
                Debug.Log("Testing terminal interaction...");
                terminal.OnInteract();
            }
        }
        else
        {
            Debug.LogWarning("No terminal created yet. Use 'Create Riddle Terminal' first.");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw where the terminal will be placed
        if (!terminalCreated)
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = Matrix4x4.TRS(terminalPosition, Quaternion.Euler(terminalRotation), terminalSize);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            
            // Draw label
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.white;
        }
    }
    
    // Public properties for access
    public GameObject GetCreatedTerminal() => createdTerminal;
    public bool IsTerminalCreated() => terminalCreated;
    
    // Physical terminal creation methods
    private GameObject CreateTerminalBase(Transform parent)
    {
        // Create cylindrical base/pedestal
        GameObject terminalBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        terminalBase.name = "TerminalBase";
        terminalBase.transform.SetParent(parent, false);
        terminalBase.transform.localPosition = new Vector3(0, 0.25f, 0);
        terminalBase.transform.localScale = new Vector3(0.8f, 0.25f, 0.8f);
        
        // Apply dark metallic material
        Renderer baseRenderer = terminalBase.GetComponent<Renderer>();
        Material baseMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        baseMaterial.color = new Color(0.2f, 0.2f, 0.25f, 1f);
        baseMaterial.SetFloat("_Metallic", 0.8f);
        baseMaterial.SetFloat("_Smoothness", 0.6f);
        baseRenderer.material = baseMaterial;
        
        return terminalBase;
    }
    
    private GameObject CreateTerminalMonitor(Transform parent)
    {
        // Create main monitor/console unit
        GameObject monitor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        monitor.name = "TerminalMonitor";
        monitor.transform.SetParent(parent, false);
        monitor.transform.localPosition = new Vector3(0, 1.3f, 0);
        monitor.transform.localScale = new Vector3(0.7f, 0.6f, 0.4f);
        
        // Apply console material
        Renderer monitorRenderer = monitor.GetComponent<Renderer>();
        Material monitorMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        monitorMaterial.color = new Color(0.15f, 0.15f, 0.2f, 1f);
        monitorMaterial.SetFloat("_Metallic", 0.6f);
        monitorMaterial.SetFloat("_Smoothness", 0.4f);
        monitorRenderer.material = monitorMaterial;
        
        // Create the screen area
        GameObject screen = GameObject.CreatePrimitive(PrimitiveType.Cube);
        screen.name = "TerminalScreen";
        screen.transform.SetParent(monitor.transform, false);
        screen.transform.localPosition = new Vector3(0, 0.1f, 0.45f);
        screen.transform.localScale = new Vector3(0.8f, 0.7f, 0.1f);
        
        // Apply screen material (dark with slight emission)
        Renderer screenRenderer = screen.GetComponent<Renderer>();
        Material screenMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        screenMaterial.color = new Color(0.05f, 0.1f, 0.05f, 1f);
        screenMaterial.SetColor("_EmissionColor", new Color(0, 0.2f, 0, 1f));
        screenMaterial.EnableKeyword("_EMISSION");
        screenRenderer.material = screenMaterial;
        
        return monitor;
    }
    
    private GameObject CreateTerminalKeyboard(Transform parent)
    {
        // Create keyboard/input panel
        GameObject keyboard = GameObject.CreatePrimitive(PrimitiveType.Cube);
        keyboard.name = "TerminalKeyboard";
        keyboard.transform.SetParent(parent, false);
        keyboard.transform.localPosition = new Vector3(0, 0.8f, 0.3f);
        keyboard.transform.localScale = new Vector3(0.6f, 0.1f, 0.3f);
        keyboard.transform.localRotation = Quaternion.Euler(-15f, 0, 0);
        
        // Apply keyboard material
        Renderer keyboardRenderer = keyboard.GetComponent<Renderer>();
        Material keyboardMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        keyboardMaterial.color = new Color(0.1f, 0.1f, 0.15f, 1f);
        keyboardMaterial.SetFloat("_Metallic", 0.3f);
        keyboardMaterial.SetFloat("_Smoothness", 0.2f);
        keyboardRenderer.material = keyboardMaterial;
        
        return keyboard;
    }
    
    private void CreateTerminalDetails(Transform parent)
    {
        // Create support column
        GameObject column = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        column.name = "TerminalColumn";
        column.transform.SetParent(parent, false);
        column.transform.localPosition = new Vector3(0, 0.75f, 0);
        column.transform.localScale = new Vector3(0.15f, 0.5f, 0.15f);
        
        // Apply column material
        Renderer columnRenderer = column.GetComponent<Renderer>();
        Material columnMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        columnMaterial.color = new Color(0.25f, 0.25f, 0.3f, 1f);
        columnMaterial.SetFloat("_Metallic", 0.7f);
        columnMaterial.SetFloat("_Smoothness", 0.5f);
        columnRenderer.material = columnMaterial;
        
        // Create some indicator lights
        for (int i = 0; i < 3; i++)
        {
            GameObject light = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            light.name = $"StatusLight{i + 1}";
            light.transform.SetParent(parent, false);
            light.transform.localPosition = new Vector3(-0.25f + (i * 0.1f), 1.5f, 0.22f);
            light.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            
            // Apply light material with emission
            Renderer lightRenderer = light.GetComponent<Renderer>();
            Material lightMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            
            Color lightColor = i == 0 ? Color.red : (i == 1 ? Color.yellow : Color.green);
            lightMaterial.color = lightColor;
            lightMaterial.SetColor("_EmissionColor", lightColor * 0.5f);
            lightMaterial.EnableKeyword("_EMISSION");
            lightRenderer.material = lightMaterial;
        }
        
        // Create ventilation grilles
        for (int i = 0; i < 2; i++)
        {
            GameObject grille = GameObject.CreatePrimitive(PrimitiveType.Cube);
            grille.name = $"VentGrille{i + 1}";
            grille.transform.SetParent(parent, false);
            grille.transform.localPosition = new Vector3(0.25f * (i == 0 ? 1 : -1), 1.1f, -0.15f);
            grille.transform.localScale = new Vector3(0.15f, 0.3f, 0.05f);
            
            // Apply grille material
            Renderer grilleRenderer = grille.GetComponent<Renderer>();
            Material grilleMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            grilleMaterial.color = new Color(0.1f, 0.1f, 0.1f, 1f);
            grilleMaterial.SetFloat("_Metallic", 0.8f);
            grilleMaterial.SetFloat("_Smoothness", 0.3f);
            grilleRenderer.material = grilleMaterial;
        }
    }
}

#if UNITY_EDITOR
// Custom Editor for better workflow
[CustomEditor(typeof(RiddleTerminalSetup))]
public class RiddleTerminalSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Terminal Actions", EditorStyles.boldLabel);
        
        RiddleTerminalSetup setup = (RiddleTerminalSetup)target;
        
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("🔧 Create Riddle Terminal", GUILayout.Height(30)))
        {
            setup.CreateRiddleTerminal();
        }
        
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("🗑️ Remove Riddle Terminal", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("Remove Terminal", 
                "Are you sure you want to remove the riddle terminal?", 
                "Yes", "No"))
            {
                setup.RemoveRiddleTerminal();
            }
        }
        
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("🧪 Test Terminal Interaction", GUILayout.Height(25)))
        {
            setup.TestTerminalInteraction();
        }
        
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "This setup creates an interactable riddle terminal that follows the same pattern as your cipher terminals. " +
            "Players can interact with it to trigger the riddle puzzle UI.", 
            MessageType.Info);
    }
}
#endif