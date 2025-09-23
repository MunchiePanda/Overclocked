using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Complete Door Creator - Creates everything needed for an escape room door system
/// Includes: Mesh, UI, Puzzle Integration, Win Screen, Interaction System
/// </summary>
public class CompleteDoorCreator : MonoBehaviour
{
    [Header("🚪 Door Settings")]
    [Tooltip("Door code required to open")]
    public string doorCode = "1234";
    
    [Tooltip("Door size and appearance")]
    public Vector3 doorSize = new Vector3(2f, 3f, 0.2f);
    
    [Tooltip("Door material (leave null for default)")]
    public Material doorMaterial;
    
    [Tooltip("Door position in the scene")]
    public Vector3 doorPosition = new Vector3(0f, 1.5f, 5f);
    
    [Header("🎨 Door Appearance")]
    [Tooltip("Door color if no material is provided")]
    public Color doorColor = new Color(0.6f, 0.4f, 0.2f); // Brown wood color
    
    [Tooltip("Add a door frame")]
    public bool createDoorFrame = true;
    
    [Tooltip("Frame color")]
    public Color frameColor = new Color(0.3f, 0.3f, 0.3f); // Dark gray
    
    [Header("🎮 Interaction Settings")]
    [Tooltip("Distance player can interact with door")]
    public float interactionDistance = 3f;
    
    [Tooltip("Key to press for interaction")]
    public KeyCode interactionKey = KeyCode.E;
    
    [Tooltip("Show interaction prompt")]
    public bool showInteractionPrompt = true;
    
    [Header("🧩 Puzzle Integration")]
    [Tooltip("Require all puzzles to be solved before door opens")]
    public bool requireAllPuzzlesSolved = true;
    
    [Tooltip("Auto-find puzzle systems in scene")]
    public bool autoFindPuzzles = true;
    
    [Header("🏆 Win Screen Settings")]
    [Tooltip("Message to display when player escapes")]
    [TextArea(3, 5)]
    public string winMessage = "🎉 CONGRATULATIONS! 🎉\n\nYou have successfully escaped the room!\n\nYour puzzle-solving skills have proven superior.\nThe door to freedom is now open.\n\nTime to return to reality...";
    
    [Tooltip("Win screen display duration (0 = manual close)")]
    public float winScreenDuration = 5f;
    
    [Header("🔊 Audio Settings")]
    [Tooltip("Sound when door opens")]
    public AudioClip doorOpenSound;
    
    [Tooltip("Sound when wrong code is entered")]
    public AudioClip wrongCodeSound;
    
    [Tooltip("Sound when correct code is entered")]
    public AudioClip correctCodeSound;
    
    [Tooltip("Sound when player escapes")]
    public AudioClip escapeSuccessSound;

    [ContextMenu("🚀 Create Complete Door System")]
    public void CreateCompleteDoorSystem()
    {
        Debug.Log("🚀 Creating complete door system...");
        
        StartCoroutine(CreateDoorSystemCoroutine());
    }
    
    IEnumerator CreateDoorSystemCoroutine()
    {
        int step = 1;
        int totalSteps = 6;
        
        // Step 1: Create Door GameObject and Mesh
        Debug.Log($"[{step}/{totalSteps}] 🚪 Creating door mesh...");
        GameObject doorGameObject = CreateDoorMesh();
        yield return null;
        step++;
        
        // Step 2: Add Door Components
        Debug.Log($"[{step}/{totalSteps}] 🔧 Adding door components...");
        AddDoorComponents(doorGameObject);
        yield return null;
        step++;
        
        // Step 3: Create Door UI
        Debug.Log($"[{step}/{totalSteps}] 🖥️ Creating door UI...");
        GameObject doorUI = CreateDoorUI();
        yield return null;
        step++;
        
        // Step 4: Create Win Screen
        Debug.Log($"[{step}/{totalSteps}] 🏆 Creating win screen...");
        GameObject winScreen = CreateWinScreen();
        yield return null;
        step++;
        
        // Step 5: Setup Interactions
        Debug.Log($"[{step}/{totalSteps}] 🎮 Setting up interactions...");
        SetupDoorInteraction(doorGameObject, doorUI, winScreen);
        yield return null;
        step++;
        
        // Step 6: Integrate with Puzzles
        Debug.Log($"[{step}/{totalSteps}] 🧩 Integrating with puzzles...");
        IntegrateWithPuzzles(doorGameObject);
        yield return null;
        
        Debug.Log("✅ Complete door system created successfully!");
        Debug.Log("🎮 Test your door:");
        Debug.Log("  • Walk close to the door");
        Debug.Log("  • Press E to interact");
        Debug.Log($"  • Enter code: {doorCode}");
        Debug.Log("  • Solve all puzzles to unlock escape!");
        
        // Clean up this creator component
        Debug.Log("🧹 Removing creator component...");
        yield return new WaitForSeconds(1f);
        DestroyImmediate(this);
    }
    
    GameObject CreateDoorMesh()
    {
        // Create main door object
        GameObject doorObject = new GameObject("EscapeDoor");
        doorObject.transform.position = doorPosition;
        
        // Create door mesh
        GameObject doorMesh = CreateDoorMeshObject(doorObject, "DoorPanel", doorSize, doorColor);
        
        // Create door frame if requested
        if (createDoorFrame)
        {
            CreateDoorFrame(doorObject);
        }
        
        // Add collider for interaction
        BoxCollider doorCollider = doorObject.AddComponent<BoxCollider>();
        doorCollider.size = new Vector3(doorSize.x + 0.5f, doorSize.y, doorSize.z + 0.5f);
        doorCollider.isTrigger = true;
        
        return doorObject;
    }
    
    GameObject CreateDoorMeshObject(GameObject parent, string name, Vector3 size, Color color)
    {
        GameObject meshObject = new GameObject(name);
        meshObject.transform.SetParent(parent.transform);
        meshObject.transform.localPosition = Vector3.zero;
        
        // Create mesh
        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
        
        // Generate cube mesh
        Mesh mesh = new Mesh();
        mesh.name = name + "_Mesh";
        
        // Vertices for a cube
        Vector3[] vertices = new Vector3[24];
        int[] triangles = new int[36];
        Vector2[] uvs = new Vector2[24];
        
        // Front face
        vertices[0] = new Vector3(-size.x/2, -size.y/2, size.z/2);
        vertices[1] = new Vector3(size.x/2, -size.y/2, size.z/2);
        vertices[2] = new Vector3(size.x/2, size.y/2, size.z/2);
        vertices[3] = new Vector3(-size.x/2, size.y/2, size.z/2);
        
        // Back face
        vertices[4] = new Vector3(-size.x/2, -size.y/2, -size.z/2);
        vertices[5] = new Vector3(-size.x/2, size.y/2, -size.z/2);
        vertices[6] = new Vector3(size.x/2, size.y/2, -size.z/2);
        vertices[7] = new Vector3(size.x/2, -size.y/2, -size.z/2);
        
        // Top face
        vertices[8] = new Vector3(-size.x/2, size.y/2, -size.z/2);
        vertices[9] = new Vector3(-size.x/2, size.y/2, size.z/2);
        vertices[10] = new Vector3(size.x/2, size.y/2, size.z/2);
        vertices[11] = new Vector3(size.x/2, size.y/2, -size.z/2);
        
        // Bottom face
        vertices[12] = new Vector3(-size.x/2, -size.y/2, -size.z/2);
        vertices[13] = new Vector3(size.x/2, -size.y/2, -size.z/2);
        vertices[14] = new Vector3(size.x/2, -size.y/2, size.z/2);
        vertices[15] = new Vector3(-size.x/2, -size.y/2, size.z/2);
        
        // Right face
        vertices[16] = new Vector3(size.x/2, -size.y/2, -size.z/2);
        vertices[17] = new Vector3(size.x/2, size.y/2, -size.z/2);
        vertices[18] = new Vector3(size.x/2, size.y/2, size.z/2);
        vertices[19] = new Vector3(size.x/2, -size.y/2, size.z/2);
        
        // Left face
        vertices[20] = new Vector3(-size.x/2, -size.y/2, -size.z/2);
        vertices[21] = new Vector3(-size.x/2, -size.y/2, size.z/2);
        vertices[22] = new Vector3(-size.x/2, size.y/2, size.z/2);
        vertices[23] = new Vector3(-size.x/2, size.y/2, -size.z/2);
        
        // Triangles (2 triangles per face, 6 faces)
        int[] tris = {
            0,1,2, 0,2,3,   // Front
            4,5,6, 4,6,7,   // Back
            8,9,10, 8,10,11, // Top
            12,13,14, 12,14,15, // Bottom
            16,17,18, 16,18,19, // Right
            20,21,22, 20,22,23  // Left
        };
        
        // UVs
        for (int i = 0; i < 24; i += 4)
        {
            uvs[i] = new Vector2(0, 0);
            uvs[i + 1] = new Vector2(1, 0);
            uvs[i + 2] = new Vector2(1, 1);
            uvs[i + 3] = new Vector2(0, 1);
        }
        
        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        
        meshFilter.mesh = mesh;
        
        // Create material
        Material material;
        if (doorMaterial != null)
        {
            material = doorMaterial;
        }
        else
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.color = color;
            material.name = name + "_Material";
        }
        
        meshRenderer.material = material;
        
        return meshObject;
    }
    
    void CreateDoorFrame(GameObject doorObject)
    {
        Vector3 frameSize = new Vector3(0.1f, doorSize.y + 0.4f, doorSize.z + 0.1f);
        
        // Left frame
        GameObject leftFrame = CreateDoorMeshObject(doorObject, "LeftFrame", 
            new Vector3(frameSize.x, frameSize.y, frameSize.z), frameColor);
        leftFrame.transform.localPosition = new Vector3(-(doorSize.x/2 + frameSize.x/2), 0, 0);
        
        // Right frame
        GameObject rightFrame = CreateDoorMeshObject(doorObject, "RightFrame", 
            new Vector3(frameSize.x, frameSize.y, frameSize.z), frameColor);
        rightFrame.transform.localPosition = new Vector3(doorSize.x/2 + frameSize.x/2, 0, 0);
        
        // Top frame
        GameObject topFrame = CreateDoorMeshObject(doorObject, "TopFrame", 
            new Vector3(doorSize.x + frameSize.x * 2, frameSize.x, frameSize.z), frameColor);
        topFrame.transform.localPosition = new Vector3(0, doorSize.y/2 + frameSize.x/2, 0);
    }
    
    void AddDoorComponents(GameObject doorObject)
    {
        // Add Door component
        Door door = doorObject.AddComponent<Door>();
        door.correctCode = doorCode;
        
        // Add AudioSource
        AudioSource audioSource = doorObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound
        
        // Add door sounds to the Door component if available
        if (doorOpenSound != null || wrongCodeSound != null || correctCodeSound != null)
        {
            // We'll handle audio in the interaction script
        }
    }
    
    GameObject CreateDoorUI()
    {
        // Create Canvas
        GameObject canvasObject = new GameObject("DoorInputPanel");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;
        
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObject.AddComponent<GraphicRaycaster>();
        
        // Create background panel
        GameObject panelObject = new GameObject("Panel");
        panelObject.transform.SetParent(canvasObject.transform);
        
        RectTransform panelRect = panelObject.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(400, 300);
        panelRect.anchoredPosition = Vector2.zero;
        
        Image panelImage = panelObject.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        
        // Create title
        GameObject titleObject = new GameObject("Title");
        titleObject.transform.SetParent(panelObject.transform);
        
        RectTransform titleRect = titleObject.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 0.8f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.offsetMin = new Vector2(20, -10);
        titleRect.offsetMax = new Vector2(-20, -10);
        
        TMP_Text titleText = titleObject.AddComponent<TMP_Text>();
        titleText.text = "🚪 DOOR ACCESS PANEL";
        titleText.fontSize = 24;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;
        
        // Create status text
        GameObject statusObject = new GameObject("StatusText");
        statusObject.transform.SetParent(panelObject.transform);
        
        RectTransform statusRect = statusObject.AddComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0f, 0.5f);
        statusRect.anchorMax = new Vector2(1f, 0.8f);
        statusRect.offsetMin = new Vector2(20, 0);
        statusRect.offsetMax = new Vector2(-20, 0);
        
        TMP_Text statusText = statusObject.AddComponent<TMP_Text>();
        statusText.text = "Enter access code:";
        statusText.fontSize = 16;
        statusText.color = Color.cyan;
        statusText.alignment = TextAlignmentOptions.Center;
        
        // Create input field
        GameObject inputObject = new GameObject("CodeInput");
        inputObject.transform.SetParent(panelObject.transform);
        
        RectTransform inputRect = inputObject.AddComponent<RectTransform>();
        inputRect.anchorMin = new Vector2(0.1f, 0.35f);
        inputRect.anchorMax = new Vector2(0.9f, 0.5f);
        inputRect.offsetMin = Vector2.zero;
        inputRect.offsetMax = Vector2.zero;
        
        Image inputImage = inputObject.AddComponent<Image>();
        inputImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        TMP_InputField inputField = inputObject.AddComponent<TMP_InputField>();
        inputField.characterLimit = 8;
        inputField.contentType = TMP_InputField.ContentType.Standard;
        
        // Input field text
        GameObject inputTextObject = new GameObject("Text");
        inputTextObject.transform.SetParent(inputObject.transform);
        
        RectTransform inputTextRect = inputTextObject.AddComponent<RectTransform>();
        inputTextRect.anchorMin = Vector2.zero;
        inputTextRect.anchorMax = Vector2.one;
        inputTextRect.offsetMin = new Vector2(10, 0);
        inputTextRect.offsetMax = new Vector2(-10, 0);
        
        TMP_Text inputTextComponent = inputTextObject.AddComponent<TMP_Text>();
        inputTextComponent.text = "";
        inputTextComponent.fontSize = 18;
        inputTextComponent.color = Color.white;
        inputTextComponent.alignment = TextAlignmentOptions.Left;
        
        inputField.textComponent = inputTextComponent;
        
        // Create Submit button
        GameObject submitObject = new GameObject("SubmitButton");
        submitObject.transform.SetParent(panelObject.transform);
        
        RectTransform submitRect = submitObject.AddComponent<RectTransform>();
        submitRect.anchorMin = new Vector2(0.1f, 0.15f);
        submitRect.anchorMax = new Vector2(0.45f, 0.3f);
        submitRect.offsetMin = Vector2.zero;
        submitRect.offsetMax = Vector2.zero;
        
        Image submitImage = submitObject.AddComponent<Image>();
        submitImage.color = new Color(0.2f, 0.6f, 0.2f, 1f);
        
        Button submitButton = submitObject.AddComponent<Button>();
        
        GameObject submitTextObject = new GameObject("Text");
        submitTextObject.transform.SetParent(submitObject.transform);
        
        RectTransform submitTextRect = submitTextObject.AddComponent<RectTransform>();
        submitTextRect.anchorMin = Vector2.zero;
        submitTextRect.anchorMax = Vector2.one;
        submitTextRect.offsetMin = Vector2.zero;
        submitTextRect.offsetMax = Vector2.zero;
        
        TMP_Text submitText = submitTextObject.AddComponent<TMP_Text>();
        submitText.text = "SUBMIT";
        submitText.fontSize = 14;
        submitText.color = Color.white;
        submitText.alignment = TextAlignmentOptions.Center;
        submitText.fontStyle = FontStyles.Bold;
        
        // Create Cancel button
        GameObject cancelObject = new GameObject("CancelButton");
        cancelObject.transform.SetParent(panelObject.transform);
        
        RectTransform cancelRect = cancelObject.AddComponent<RectTransform>();
        cancelRect.anchorMin = new Vector2(0.55f, 0.15f);
        cancelRect.anchorMax = new Vector2(0.9f, 0.3f);
        cancelRect.offsetMin = Vector2.zero;
        cancelRect.offsetMax = Vector2.zero;
        
        Image cancelImage = cancelObject.AddComponent<Image>();
        cancelImage.color = new Color(0.6f, 0.2f, 0.2f, 1f);
        
        Button cancelButton = cancelObject.AddComponent<Button>();
        
        GameObject cancelTextObject = new GameObject("Text");
        cancelTextObject.transform.SetParent(cancelObject.transform);
        
        RectTransform cancelTextRect = cancelTextObject.AddComponent<RectTransform>();
        cancelTextRect.anchorMin = Vector2.zero;
        cancelTextRect.anchorMax = Vector2.one;
        cancelTextRect.offsetMin = Vector2.zero;
        cancelTextRect.offsetMax = Vector2.zero;
        
        TMP_Text cancelText = cancelTextObject.AddComponent<TMP_Text>();
        cancelText.text = "CANCEL";
        cancelText.fontSize = 14;
        cancelText.color = Color.white;
        cancelText.alignment = TextAlignmentOptions.Center;
        cancelText.fontStyle = FontStyles.Bold;
        
        // Hide UI initially
        canvasObject.SetActive(false);
        
        return canvasObject;
    }
    
    GameObject CreateWinScreen()
    {
        // Create Canvas
        GameObject canvasObject = new GameObject("WinScreen");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObject.AddComponent<GraphicRaycaster>();
        
        // Create background
        GameObject backgroundObject = new GameObject("Background");
        backgroundObject.transform.SetParent(canvasObject.transform);
        
        RectTransform backgroundRect = backgroundObject.AddComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;
        
        Image backgroundImage = backgroundObject.AddComponent<Image>();
        backgroundImage.color = new Color(0f, 0f, 0f, 0.8f);
        
        // Create win panel
        GameObject panelObject = new GameObject("WinPanel");
        panelObject.transform.SetParent(canvasObject.transform);
        
        RectTransform panelRect = panelObject.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.2f, 0.2f);
        panelRect.anchorMax = new Vector2(0.8f, 0.8f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        Image panelImage = panelObject.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.3f, 0.1f, 0.95f);
        
        // Create win text
        GameObject textObject = new GameObject("WinText");
        textObject.transform.SetParent(panelObject.transform);
        
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.05f, 0.3f);
        textRect.anchorMax = new Vector2(0.95f, 0.9f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TMP_Text winText = textObject.AddComponent<TMP_Text>();
        winText.text = winMessage;
        winText.fontSize = 36;
        winText.color = Color.white;
        winText.alignment = TextAlignmentOptions.Center;
        winText.fontStyle = FontStyles.Bold;
        
        // Create continue button (if manual close)
        if (winScreenDuration <= 0)
        {
            GameObject buttonObject = new GameObject("ContinueButton");
            buttonObject.transform.SetParent(panelObject.transform);
            
            RectTransform buttonRect = buttonObject.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.35f, 0.1f);
            buttonRect.anchorMax = new Vector2(0.65f, 0.25f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            
            Image buttonImage = buttonObject.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.6f, 0.2f, 1f);
            
            Button button = buttonObject.AddComponent<Button>();
            button.onClick.AddListener(() => canvasObject.SetActive(false));
            
            GameObject buttonTextObject = new GameObject("Text");
            buttonTextObject.transform.SetParent(buttonObject.transform);
            
            RectTransform buttonTextRect = buttonTextObject.AddComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.offsetMin = Vector2.zero;
            buttonTextRect.offsetMax = Vector2.zero;
            
            TMP_Text buttonText = buttonTextObject.AddComponent<TMP_Text>();
            buttonText.text = "CONTINUE";
            buttonText.fontSize = 24;
            buttonText.color = Color.white;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.fontStyle = FontStyles.Bold;
        }
        
        // Hide initially
        canvasObject.SetActive(false);
        
        return canvasObject;
    }
    
    void SetupDoorInteraction(GameObject doorObject, GameObject doorUI, GameObject winScreen)
    {
        // Add the complete door interaction component
        CompleteDoorInteraction interaction = doorObject.AddComponent<CompleteDoorInteraction>();
        
        // Configure interaction
        interaction.doorUICanvas = doorUI;
        interaction.winScreen = winScreen;
        interaction.interactionDistance = interactionDistance;
        interaction.interactionKey = interactionKey;
        interaction.showInteractionPrompt = showInteractionPrompt;
        interaction.requireAllPuzzlesSolved = requireAllPuzzlesSolved;
        interaction.autoFindPuzzles = autoFindPuzzles;
        interaction.winScreenDuration = winScreenDuration;
        
        // Set audio clips
        interaction.doorOpenSound = doorOpenSound;
        interaction.wrongCodeSound = wrongCodeSound;
        interaction.correctCodeSound = correctCodeSound;
        interaction.escapeSuccessSound = escapeSuccessSound;
        
        // Setup UI interactions
        SetupUIInteractions(doorUI, interaction);
    }
    
    void SetupUIInteractions(GameObject doorUI, CompleteDoorInteraction interaction)
    {
        // Find UI components
        TMP_InputField inputField = doorUI.GetComponentInChildren<TMP_InputField>();
        Button[] buttons = doorUI.GetComponentsInChildren<Button>();
        TMP_Text statusText = doorUI.transform.Find("Panel/StatusText").GetComponent<TMP_Text>();
        
        Button submitButton = null;
        Button cancelButton = null;
        
        foreach (Button button in buttons)
        {
            if (button.name == "SubmitButton")
                submitButton = button;
            else if (button.name == "CancelButton")
                cancelButton = button;
        }
        
        // Configure interaction component with UI references
        interaction.SetUIReferences(inputField, submitButton, cancelButton, statusText);
    }
    
    void IntegrateWithPuzzles(GameObject doorObject)
    {
        if (!autoFindPuzzles) return;
        
        CompleteDoorInteraction interaction = doorObject.GetComponent<CompleteDoorInteraction>();
        if (interaction != null)
        {
            interaction.FindPuzzleReferences();
        }
    }
}

/// <summary>
/// Complete door interaction system with puzzle integration
/// </summary>
public class CompleteDoorInteraction : MonoBehaviour
{
    [Header("UI References")]
    public GameObject doorUICanvas;
    public GameObject winScreen;
    public TMP_InputField codeInputField;
    public Button submitButton;
    public Button cancelButton;
    public TMP_Text statusText;
    
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public KeyCode interactionKey = KeyCode.E;
    public bool showInteractionPrompt = true;
    public bool requireAllPuzzlesSolved = true;
    public bool autoFindPuzzles = true;
    public float winScreenDuration = 5f;
    
    [Header("Audio")]
    public AudioClip doorOpenSound;
    public AudioClip wrongCodeSound;
    public AudioClip correctCodeSound;
    public AudioClip escapeSuccessSound;
    
    [Header("Puzzle References")]
    public RiddlePuzzle riddlePuzzle;
    public ShapeCipherPuzzle cipherPuzzle;
    public ColorLightPuzzle colorLightPuzzle;
    
    private Door door;
    private AudioSource audioSource;
    private GameObject player;
    private bool isUIOpen = false;
    private bool doorIsOpen = false;
    
    void Start()
    {
        door = GetComponent<Door>();
        audioSource = GetComponent<AudioSource>();
        
        if (autoFindPuzzles)
        {
            FindPuzzleReferences();
        }
        
        FindPlayer();
        
        // Setup UI initially hidden
        if (doorUICanvas != null)
            doorUICanvas.SetActive(false);
        if (winScreen != null)
            winScreen.SetActive(false);
    }
    
    public void FindPuzzleReferences()
    {
        if (riddlePuzzle == null)
            riddlePuzzle = FindFirstObjectByType<RiddlePuzzle>();
        
        if (cipherPuzzle == null)
            cipherPuzzle = FindFirstObjectByType<ShapeCipherPuzzle>();
        
        if (colorLightPuzzle == null)
            colorLightPuzzle = FindFirstObjectByType<ColorLightPuzzle>();
        
        Debug.Log($"🧩 Found puzzles: Riddle={riddlePuzzle != null}, Cipher={cipherPuzzle != null}, Color={colorLightPuzzle != null}");
    }
    
    void FindPlayer()
    {
        // Look for player by common names and tags
        player = GameObject.FindWithTag("Player");
        if (player == null)
            player = GameObject.Find("Player");
        if (player == null)
            player = GameObject.Find("FPSController");
        if (player == null)
        {
            // Look for any object with CharacterController or similar movement component
            CharacterController charController = FindFirstObjectByType<CharacterController>();
            if (charController != null)
                player = charController.gameObject;
        }
        
        if (player != null)
            Debug.Log($"🎮 Found player: {player.name}");
        else
            Debug.LogWarning("⚠️ Player not found - door interaction may not work properly");
    }
    
    public void SetUIReferences(TMP_InputField inputField, Button submit, Button cancel, TMP_Text status)
    {
        codeInputField = inputField;
        submitButton = submit;
        cancelButton = cancel;
        statusText = status;
        
        // Setup button events
        if (submitButton != null)
            submitButton.onClick.AddListener(SubmitCode);
        
        if (cancelButton != null)
            cancelButton.onClick.AddListener(CloseUI);
        
        // Setup input field events
        if (codeInputField != null)
        {
            codeInputField.onSubmit.AddListener((string value) => SubmitCode());
        }
    }
    
    void Update()
    {
        if (doorIsOpen) return;
        
        HandlePlayerInteraction();
        HandleUIInput();
    }
    
    void HandlePlayerInteraction()
    {
        if (player == null) return;
        
        float distance = Vector3.Distance(player.transform.position, transform.position);
        
        if (distance <= interactionDistance && !isUIOpen)
        {
            if (showInteractionPrompt)
            {
                // Could add a UI prompt here
            }
            
            if (Input.GetKeyDown(interactionKey))
            {
                OpenDoorUI();
            }
        }
    }
    
    void HandleUIInput()
    {
        if (isUIOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUI();
        }
    }
    
    void OpenDoorUI()
    {
        if (doorUICanvas == null) return;
        
        isUIOpen = true;
        doorUICanvas.SetActive(true);
        
        // Enable cursor
        PauseManager.RequestCursor("DoorInteraction", CursorLockMode.None, true, 60);
        
        // Clear previous input
        if (codeInputField != null)
        {
            codeInputField.text = "";
            codeInputField.Select();
        }
        
        // Update status based on puzzle completion
        if (statusText != null)
        {
            if (requireAllPuzzlesSolved && !AreAllPuzzlesSolved())
            {
                statusText.text = "🔒 Solve all puzzles first!\n" + GetPuzzleStatus();
                statusText.color = Color.red;
            }
            else
            {
                statusText.text = "Enter access code:";
                statusText.color = Color.cyan;
            }
        }
    }
    
    void CloseUI()
    {
        isUIOpen = false;
        
        if (doorUICanvas != null)
            doorUICanvas.SetActive(false);
        
        // Release cursor
        PauseManager.ReleaseCursor("DoorInteraction");
    }
    
    void SubmitCode()
    {
        if (codeInputField == null || door == null) return;
        
        string enteredCode = codeInputField.text.Trim();
        
        // Check if puzzles need to be solved first
        if (requireAllPuzzlesSolved && !AreAllPuzzlesSolved())
        {
            if (statusText != null)
            {
                statusText.text = "🚫 ACCESS DENIED\nComplete all puzzles first!";
                statusText.color = Color.red;
            }
            
            PlaySound(wrongCodeSound);
            return;
        }
        
        // Check door code
        if (enteredCode == door.correctCode)
        {
            // Correct code!
            if (statusText != null)
            {
                statusText.text = "✅ ACCESS GRANTED\nOpening door...";
                statusText.color = Color.green;
            }
            
            PlaySound(correctCodeSound);
            StartCoroutine(OpenDoorSequence());
        }
        else
        {
            // Wrong code
            if (statusText != null)
            {
                statusText.text = "❌ ACCESS DENIED\nIncorrect code!";
                statusText.color = Color.red;
            }
            
            PlaySound(wrongCodeSound);
            
            // Clear input field
            codeInputField.text = "";
        }
    }
    
    IEnumerator OpenDoorSequence()
    {
        yield return new WaitForSeconds(1f);
        
        CloseUI();
        
        // Play door opening sound
        PlaySound(doorOpenSound);
        
        // Animate door opening (simple rotation)
        float openAngle = 90f;
        float openDuration = 2f;
        Vector3 startRotation = transform.eulerAngles;
        Vector3 endRotation = startRotation + new Vector3(0, openAngle, 0);
        
        float elapsedTime = 0;
        while (elapsedTime < openDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / openDuration;
            progress = Mathf.SmoothStep(0, 1, progress); // Smooth animation
            
            transform.eulerAngles = Vector3.Lerp(startRotation, endRotation, progress);
            yield return null;
        }
        
        doorIsOpen = true;
        
        // Show win screen
        yield return new WaitForSeconds(0.5f);
        ShowWinScreen();
    }
    
    void ShowWinScreen()
    {
        if (winScreen == null) return;
        
        winScreen.SetActive(true);
        
        // Play escape success sound
        PlaySound(escapeSuccessSound);
        
        // Auto-hide win screen if duration is set
        if (winScreenDuration > 0)
        {
            StartCoroutine(AutoHideWinScreen());
        }
    }
    
    IEnumerator AutoHideWinScreen()
    {
        yield return new WaitForSeconds(winScreenDuration);
        
        if (winScreen != null)
            winScreen.SetActive(false);
    }
    
    bool AreAllPuzzlesSolved()
    {
        bool riddleSolved = riddlePuzzle == null || riddlePuzzle.IsCompleted();
        bool cipherSolved = cipherPuzzle == null || cipherPuzzle.IsCompleted();
        bool colorSolved = colorLightPuzzle == null || colorLightPuzzle.IsCompleted();
        
        return riddleSolved && cipherSolved && colorSolved;
    }
    
    string GetPuzzleStatus()
    {
        string status = "Puzzle Status:\n";
        
        if (riddlePuzzle != null)
            status += $"🧩 Riddles: {(riddlePuzzle.IsCompleted() ? "✅" : "❌")}\n";
        
        if (cipherPuzzle != null)
            status += $"🔐 Cipher: {(cipherPuzzle.IsCompleted() ? "✅" : "❌")}\n";
        
        if (colorLightPuzzle != null)
            status += $"🌈 Colors: {(colorLightPuzzle.IsCompleted() ? "✅" : "❌")}";
        
        return status;
    }
    
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (showInteractionPrompt)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionDistance);
        }
    }
}