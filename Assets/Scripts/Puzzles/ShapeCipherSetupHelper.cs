using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper to set up the Shape Cipher Puzzle system
/// </summary>
public class ShapeCipherSetupHelper : MonoBehaviour
{
    [Header("Setup Configuration")]
    [Tooltip("Word that players need to decode")]
    public string targetWord = "CODE";
    
    [Tooltip("Shapes to use for each letter")]
    public string[] shapes = {"△", "○", "□", "◇"};
    
    [Tooltip("Positions where to place shape clues")]
    public Transform[] cluePositions;
    
    [Tooltip("Prefab for shape clues (optional)")]
    public GameObject shapeClumePrefab;

    [Header("Puzzle References")]
    [Tooltip("The main shape cipher puzzle component")]
    public ShapeCipherPuzzle shapeCipherPuzzle;

    [ContextMenu("Setup Shape Cipher System")]
    public void SetupShapeCipherSystem()
    {
        Debug.Log("🔧 Setting up Shape Cipher Puzzle System...");

        // Create puzzle if it doesn't exist
        if (shapeCipherPuzzle == null)
        {
            CreateShapeCipherPuzzle();
        }

        // Create environmental shape clues
        CreateShapeClues();

        // Connect UI elements
        ConnectUIElements();

        Debug.Log("✅ Shape Cipher Puzzle System setup complete!");
    }

    void CreateShapeCipherPuzzle()
    {
        GameObject puzzleObj = GameObject.Find("CipherWheelPuzzle");
        if (puzzleObj == null)
        {
            puzzleObj = new GameObject("ShapeCipherPuzzle");
        }

        // Add or get the ShapeCipherPuzzle component
        shapeCipherPuzzle = puzzleObj.GetComponent<ShapeCipherPuzzle>();
        if (shapeCipherPuzzle == null)
        {
            shapeCipherPuzzle = puzzleObj.AddComponent<ShapeCipherPuzzle>();
        }

        // Set the target word
        shapeCipherPuzzle.correctWord = targetWord;

        Debug.Log($"✅ Shape Cipher Puzzle created with target word: {targetWord}");
    }

    void CreateShapeClues()
    {
        if (cluePositions == null || cluePositions.Length == 0)
        {
            Debug.LogWarning("⚠️ No clue positions assigned! You'll need to manually place shape clues.");
            return;
        }

        int wordLength = targetWord.Length;
        for (int i = 0; i < wordLength && i < cluePositions.Length && i < shapes.Length; i++)
        {
            CreateShapeClue(i, shapes[i], targetWord[i].ToString().ToUpper(), cluePositions[i]);
        }
    }

    void CreateShapeClue(int index, string shape, string letter, Transform position)
    {
        GameObject clueObj;
        
        if (shapeClumePrefab != null)
        {
            clueObj = Instantiate(shapeClumePrefab, position.position, position.rotation);
        }
        else
        {
            // Create a simple cube as the clue object
            clueObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            clueObj.transform.position = position.position;
            clueObj.transform.rotation = position.rotation;
            clueObj.transform.localScale = Vector3.one * 0.5f;
        }

        clueObj.name = $"ShapeClue_{shape}_{letter}_{index}";

        // Add the environmental shape clue component
        EnvironmentalShapeClue clueComponent = clueObj.GetComponent<EnvironmentalShapeClue>();
        if (clueComponent == null)
        {
            clueComponent = clueObj.AddComponent<EnvironmentalShapeClue>();
        }

        // Configure the clue
        clueComponent.shapeSymbol = shape;
        clueComponent.decodedLetter = letter;
        clueComponent.wordPosition = index;
        clueComponent.examinationText = $"You found a {shape} shape carved here.";
        clueComponent.clueHint = $"This symbol represents the {GetOrdinal(index + 1)} letter in the code word.";

        // Set layer for interaction
        clueObj.layer = LayerMask.NameToLayer("Interactable");

        Debug.Log($"✅ Created shape clue: {shape} → {letter} at position {index}");
    }

    void ConnectUIElements()
    {
        if (shapeCipherPuzzle == null) return;

        // Find UI elements in the cipher canvas
        GameObject canvas = GameObject.Find("CipherWheelCanvas");
        if (canvas == null)
        {
            Debug.LogError("❌ CipherWheelCanvas not found! UI elements won't be connected.");
            return;
        }

        // Connect buttons
        shapeCipherPuzzle.cycleShapeButton = FindUIElement<Button>(canvas, "OuterRingButton");
        if (shapeCipherPuzzle.cycleShapeButton != null)
        {
            // Update button text
            TMP_Text buttonText = shapeCipherPuzzle.cycleShapeButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
                buttonText.text = "Cycle Shape";
        }

        shapeCipherPuzzle.submitButton = FindUIElement<Button>(canvas, "SubmitButton");
        shapeCipherPuzzle.resetButton = FindUIElement<Button>(canvas, "ResetButton");

        // Connect displays
        shapeCipherPuzzle.shapeDisplay = FindUIElement<TMP_Text>(canvas, "SymbolDisplay");
        shapeCipherPuzzle.letterDisplay = FindUIElement<TMP_Text>(canvas, "LetterDisplay");
        shapeCipherPuzzle.wordInputField = FindUIElement<TMP_InputField>(canvas, "CodeInputField");
        shapeCipherPuzzle.feedbackText = FindUIElement<TMP_Text>(canvas, "FeedbackText");

        // Use sequence display for discovered shapes
        shapeCipherPuzzle.discoveredShapesDisplay = FindUIElement<TMP_Text>(canvas, "SequenceDisplay");

        // Set UI reference for base puzzle
        shapeCipherPuzzle.puzzleUI = canvas;

        Debug.Log("✅ UI elements connected to Shape Cipher Puzzle");
    }

    T FindUIElement<T>(GameObject parent, string name) where T : Component
    {
        Transform found = parent.transform.Find(name);
        if (found == null)
        {
            // Try recursive search
            found = FindChildRecursive(parent.transform, name);
        }

        if (found != null)
        {
            T component = found.GetComponent<T>();
            if (component != null)
            {
                Debug.Log($"✅ Found UI element: {name}");
                return component;
            }
        }

        Debug.LogWarning($"⚠️ UI element not found: {name}");
        return null;
    }

    Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform found = FindChildRecursive(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    string GetOrdinal(int number)
    {
        switch (number)
        {
            case 1: return "1st";
            case 2: return "2nd";
            case 3: return "3rd";
            default: return $"{number}th";
        }
    }

    [ContextMenu("Test Create Single Clue")]
    public void TestCreateSingleClue()
    {
        Vector3 testPosition = transform.position + Vector3.forward * 2f;
        GameObject testClue = GameObject.CreatePrimitive(PrimitiveType.Cube);
        testClue.transform.position = testPosition;
        testClue.name = "TestShapeClue";

        EnvironmentalShapeClue clueComponent = testClue.AddComponent<EnvironmentalShapeClue>();
        clueComponent.shapeSymbol = "△";
        clueComponent.decodedLetter = "C";
        clueComponent.wordPosition = 0;

        testClue.layer = LayerMask.NameToLayer("Interactable");

        Debug.Log("✅ Test shape clue created");
    }

    [ContextMenu("Clear All Shape Clues")]
    public void ClearAllShapeClues()
    {
        EnvironmentalShapeClue[] existingClues = FindObjectsByType<EnvironmentalShapeClue>(FindObjectsSortMode.None);
        for (int i = 0; i < existingClues.Length; i++)
        {
            if (Application.isPlaying)
                Destroy(existingClues[i].gameObject);
            else
                DestroyImmediate(existingClues[i].gameObject);
        }

        Debug.Log($"✅ Cleared {existingClues.Length} shape clues");
    }
}