using UnityEngine;

/// <summary>
/// Setup helper for creating color light puzzle components
/// </summary>
public class ColorLightPuzzleSetup : MonoBehaviour
{
    [Header("🏗️ Auto Setup")]
    [Tooltip("Click to create color button setup")]
    public bool createColorButtons = false;
    
    [Tooltip("Click to create environmental clues")]
    public bool createEnvironmentalClues = false;
    
    [Tooltip("Click to setup complete puzzle")]
    public bool setupCompletePuzzle = false;

    [Header("🎯 Configuration")]
    [Tooltip("Number of color buttons to create")]
    public int numberOfButtons = 4;
    
    [Tooltip("Colors for the buttons")]
    public ColorType[] buttonColors = { ColorType.Red, ColorType.Blue, ColorType.Green, ColorType.Yellow };
    
    [Tooltip("Spacing between buttons")]
    public float buttonSpacing = 3f;
    
    [Tooltip("Height of buttons above ground")]
    public float buttonHeight = 1f;

    [Header("🎨 Environmental Clue Settings")]
    [Tooltip("Colors for environmental clues (should match button sequence)")]
    public ColorType[] clueColors = { ColorType.Red, ColorType.Blue, ColorType.Green, ColorType.Yellow };
    
    [Tooltip("Clue descriptions")]
    public string[] clueDescriptions = {
        "A passionate red painting",
        "Deep blue research monitor", 
        "Living green plant display",
        "Warm yellow reading lamp"
    };
    
    [Tooltip("Clue examination texts")]
    public string[] clueExaminationTexts = {
        "This vibrant red canvas burns with artistic passion...",
        "The blue screen displays flowing data streams like ocean depths...",
        "This green plant thrives with natural life energy...", 
        "The yellow lamp radiates warmth like captured sunlight..."
    };

    private void OnValidate()
    {
        if (createColorButtons)
        {
            CreateColorButtons();
            createColorButtons = false;
        }
        
        if (createEnvironmentalClues)
        {
            CreateEnvironmentalClues();
            createEnvironmentalClues = false;
        }
        
        if (setupCompletePuzzle)
        {
            SetupCompletePuzzle();
            setupCompletePuzzle = false;
        }
    }

    [ContextMenu("🔘 Create Color Buttons")]
    public void CreateColorButtons()
    {
        Debug.Log("🔘 Creating color buttons...");

        GameObject buttonParent = GameObject.Find("ColorButtons");
        if (buttonParent == null)
        {
            buttonParent = new GameObject("ColorButtons");
            buttonParent.transform.SetParent(transform);
        }

        // Clear existing buttons
        foreach (Transform child in buttonParent.transform)
        {
            DestroyImmediate(child.gameObject);
        }

        // Create buttons in a line
        for (int i = 0; i < numberOfButtons && i < buttonColors.Length; i++)
        {
            GameObject buttonObj = CreateColorButton(i, buttonColors[i], buttonParent.transform);
            Debug.Log($"Created {buttonColors[i]} button");
        }

        Debug.Log($"✅ Created {numberOfButtons} color buttons");
    }

    private GameObject CreateColorButton(int index, ColorType color, Transform parent)
    {
        // Create button base
        GameObject buttonObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        buttonObj.name = $"ColorButton_{color}";
        buttonObj.transform.SetParent(parent);
        
        // Position button
        Vector3 position = new Vector3(index * buttonSpacing, buttonHeight, 0);
        buttonObj.transform.localPosition = position;
        buttonObj.transform.localScale = new Vector3(1f, 0.2f, 1f); // Flat button

        // Add ColorButton component
        ColorButton colorButton = buttonObj.AddComponent<ColorButton>();
        colorButton.buttonColor = color;

        // Create light
        GameObject lightObj = new GameObject("ButtonLight");
        lightObj.transform.SetParent(buttonObj.transform);
        lightObj.transform.localPosition = Vector3.up * 2f; // Above button

        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 5f;
        light.intensity = 1f;
        
        colorButton.buttonLight = light;
        colorButton.buttonRenderer = buttonObj.GetComponent<Renderer>();

        // Create simple button material
        Material buttonMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        buttonMaterial.SetFloat("_Metallic", 0.8f);
        buttonMaterial.SetFloat("_Smoothness", 0.9f);
        buttonObj.GetComponent<Renderer>().material = buttonMaterial;

        return buttonObj;
    }

    [ContextMenu("🎨 Create Environmental Clues")]
    public void CreateEnvironmentalClues()
    {
        Debug.Log("🎨 Creating environmental clues...");

        GameObject clueParent = GameObject.Find("EnvironmentalClues");
        if (clueParent == null)
        {
            clueParent = new GameObject("EnvironmentalClues");
            clueParent.transform.SetParent(transform);
        }

        // Clear existing clues
        foreach (Transform child in clueParent.transform)
        {
            DestroyImmediate(child.gameObject);
        }

        // Create clues around the room
        for (int i = 0; i < clueColors.Length; i++)
        {
            GameObject clueObj = CreateEnvironmentalClue(i, clueColors[i], clueParent.transform);
            Debug.Log($"Created {clueColors[i]} environmental clue");
        }

        Debug.Log($"✅ Created {clueColors.Length} environmental clues");
    }

    private GameObject CreateEnvironmentalClue(int index, ColorType color, Transform parent)
    {
        // Create clue object (painting/poster/display)
        GameObject clueObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
        clueObj.name = $"EnvironmentalClue_{color}";
        clueObj.transform.SetParent(parent);

        // Position clues around the room
        float angle = (360f / clueColors.Length) * index;
        float radius = 8f;
        
        Vector3 position = new Vector3(
            Mathf.Sin(angle * Mathf.Deg2Rad) * radius,
            2f, // Wall height
            Mathf.Cos(angle * Mathf.Deg2Rad) * radius
        );
        
        clueObj.transform.localPosition = position;
        clueObj.transform.LookAt(Vector3.zero); // Face inward
        clueObj.transform.localScale = Vector3.one * 2f; // Make it visible

        // Add EnvironmentalColorClue component
        EnvironmentalColorClue colorClue = clueObj.AddComponent<EnvironmentalColorClue>();
        colorClue.clueColor = color;
        colorClue.sequencePosition = index;
        
        if (index < clueDescriptions.Length)
            colorClue.clueDescription = clueDescriptions[index];
            
        if (index < clueExaminationTexts.Length)
            colorClue.examinationText = clueExaminationTexts[index];

        colorClue.colorRenderer = clueObj.GetComponent<Renderer>();

        // Create glowing material
        Material clueMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        clueMaterial.EnableKeyword("_EMISSION");
        clueObj.GetComponent<Renderer>().material = clueMaterial;

        // Add optional light
        GameObject lightObj = new GameObject("ClueLight");
        lightObj.transform.SetParent(clueObj.transform);
        lightObj.transform.localPosition = Vector3.forward * 0.5f;

        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Spot;
        light.range = 3f;
        light.intensity = 2f;
        light.spotAngle = 45f;
        
        colorClue.colorLight = light;

        return clueObj;
    }

    [ContextMenu("🏗️ Setup Complete Puzzle")]
    public void SetupCompletePuzzle()
    {
        Debug.Log("🏗️ Setting up complete color light puzzle...");

        // Add ColorLightPuzzle component if not present
        ColorLightPuzzle puzzle = GetComponent<ColorLightPuzzle>();
        if (puzzle == null)
        {
            puzzle = gameObject.AddComponent<ColorLightPuzzle>();
        }

        // Create buttons and clues
        CreateColorButtons();
        CreateEnvironmentalClues();

        // Auto-assign references
        AssignPuzzleReferences(puzzle);

        Debug.Log("✅ Complete color light puzzle setup finished!");
    }

    private void AssignPuzzleReferences(ColorLightPuzzle puzzle)
    {
        // Find all color buttons
        ColorButton[] buttons = FindObjectsOfType<ColorButton>();
        puzzle.colorButtons = buttons;

        // Setup environmental clues
        EnvironmentalColorClue[] clues = FindObjectsOfType<EnvironmentalColorClue>();
        
        puzzle.environmentalClues = new ColorLightPuzzle.EnvironmentalClue[clues.Length];
        for (int i = 0; i < clues.Length; i++)
        {
            puzzle.environmentalClues[i] = new ColorLightPuzzle.EnvironmentalClue
            {
                location = $"Wall position {i + 1}",
                colorHint = clues[i].GetClueColor(),
                sequencePosition = clues[i].GetSequencePosition(),
                visualDescription = clues[i].GetClueDescription()
            };
        }

        // Set correct sequence based on button colors
        puzzle.correctSequence = (ColorType[])buttonColors.Clone();

        Debug.Log("✅ Puzzle references assigned");
    }

    [ContextMenu("🎯 Test Button Sequence")]
    public void TestButtonSequence()
    {
        ColorButton[] buttons = FindObjectsOfType<ColorButton>();
        
        Debug.Log("🎯 Testing button sequence...");
        foreach (ColorButton button in buttons)
        {
            Debug.Log($"Button: {button.ButtonColor} at {button.transform.position}");
        }
    }

    [ContextMenu("🔍 List Environmental Clues")]
    public void ListEnvironmentalClues()
    {
        EnvironmentalColorClue[] clues = FindObjectsOfType<EnvironmentalColorClue>();
        
        Debug.Log("🔍 Environmental clues found:");
        foreach (EnvironmentalColorClue clue in clues)
        {
            Debug.Log($"Clue: {clue.GetClueColor()} at position {clue.GetSequencePosition()} - {clue.GetClueDescription()}");
        }
    }
}