using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Shape-based cipher puzzle where players find environmental shape clues
/// and use them to decode letters to form a word
/// </summary>
public class ShapeCipherPuzzle : BasePuzzle
{
    [Header("Shape Cipher Settings")]
    [Tooltip("Button to cycle through discovered shapes")]
    public Button cycleShapeButton;
    
    [Tooltip("Display for current shape")]
    public TMP_Text shapeDisplay;
    
    [Tooltip("Display for current letter")]
    public TMP_Text letterDisplay;
    
    [Tooltip("Input field for the decoded word")]
    public TMP_InputField wordInputField;
    
    [Tooltip("Button to submit the decoded word")]
    public Button submitButton;
    
    [Tooltip("Button to reset input")]
    public Button resetButton;

    [Header("Information Displays")]
    [Tooltip("Display showing discovered shapes")]
    public TMP_Text discoveredShapesDisplay;
    
    [Tooltip("Display showing progress")]
    public TMP_Text progressDisplay;
    
    [Tooltip("Display showing hints")]
    public TMP_Text hintDisplay;

    [Header("Puzzle Configuration")]
    [Tooltip("The correct word when shapes are decoded")]
    public string correctWord = "CODE";
    
    [Tooltip("Hint about what the word might be")]
    [TextArea(2, 3)]
    public string wordHint = "Find the shapes around the room and decode them to spell a 4-letter computer term.";

    private int currentShapeIndex = 0;
    private Dictionary<string, ShapeLetterPair> discoveredShapes = new Dictionary<string, ShapeLetterPair>();
    private List<ShapeLetterPair> shapesInOrder = new List<ShapeLetterPair>();
    private int randomNumber;

    [System.Serializable]
    public class ShapeLetterPair
    {
        public string shape;
        public string letter;
        public int position;
        public bool discovered;

        public ShapeLetterPair(string s, string l, int p)
        {
            shape = s;
            letter = l;
            position = p;
            discovered = true;
        }
    }

    private void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();
        randomNumber = Random.Range(1000, 9999);
        
        // Auto-assign UI references if not set
        if (puzzleUI == null)
        {
            puzzleUI = GameObject.Find("CipherWheelCanvas");
            if (puzzleUI != null)
            {
                Debug.Log("Found and assigned CipherWheelCanvas as puzzleUI for ShapeCipherPuzzle");
            }
        }
        
        SetupButtons();
        UpdateAllDisplays();
        ShowInitialHint();
    }

    private void SetupButtons()
    {
        if (cycleShapeButton != null)
        {
            cycleShapeButton.onClick.AddListener(CycleShape);
            Debug.Log("Cycle shape button connected successfully");
        }
        else
        {
            Debug.LogError("Cycle shape button is not assigned in the inspector!");
        }
            
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(CheckWord);
            Debug.Log("Submit button connected successfully");
        }
        else
        {
            Debug.LogError("Submit button is not assigned in the inspector!");
        }
            
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetInput);
            Debug.Log("Reset button connected successfully");
        }
        else
        {
            Debug.LogError("Reset button is not assigned in the inspector!");
        }
    }

    public void OnShapeDiscovered(string shape, string letter, int position)
    {
        if (!discoveredShapes.ContainsKey(shape))
        {
            ShapeLetterPair newPair = new ShapeLetterPair(shape, letter, position);
            discoveredShapes[shape] = newPair;
            
            Debug.Log($"Shape discovered: {shape} → {letter} at position {position}");
            
            // Update the ordered list
            RefreshShapeOrder();
            
            // Update all displays
            UpdateAllDisplays();
            
            // If this is the first discovery, show the current shape
            if (discoveredShapes.Count == 1)
            {
                currentShapeIndex = 0;
                UpdateShapeDisplay();
            }
        }
    }

    private void RefreshShapeOrder()
    {
        // Sort discovered shapes by their position in the word
        shapesInOrder = discoveredShapes.Values.OrderBy(pair => pair.position).ToList();
    }

    private void CycleShape()
    {
        if (shapesInOrder.Count == 0)
        {
            GiveFeedback("No shapes discovered yet. Explore the room to find shape clues!");
            return;
        }

        currentShapeIndex = (currentShapeIndex + 1) % shapesInOrder.Count;
        UpdateShapeDisplay();
        
        ShapeLetterPair currentPair = shapesInOrder[currentShapeIndex];
        GiveFeedback($"Viewing shape {currentShapeIndex + 1} of {shapesInOrder.Count}: {currentPair.shape} → {currentPair.letter}");
    }

    private void UpdateShapeDisplay()
    {
        if (shapesInOrder.Count == 0)
        {
            if (shapeDisplay != null)
                shapeDisplay.text = "?";
            if (letterDisplay != null)
                letterDisplay.text = "?";
            return;
        }

        ShapeLetterPair currentPair = shapesInOrder[currentShapeIndex];
        
        if (shapeDisplay != null)
        {
            shapeDisplay.text = currentPair.shape;
            Debug.Log($"Updated shape display to: {currentPair.shape}");
        }
        else
        {
            Debug.LogError("Shape display is not assigned in the inspector!");
        }
            
        if (letterDisplay != null)
        {
            letterDisplay.text = currentPair.letter;
            Debug.Log($"Updated letter display to: {currentPair.letter}");
        }
        else
        {
            Debug.LogError("Letter display is not assigned in the inspector!");
        }
    }

    private void UpdateAllDisplays()
    {
        UpdateShapeDisplay();
        UpdateDiscoveredShapesDisplay();
        UpdateProgressDisplay();
    }

    private void UpdateDiscoveredShapesDisplay()
    {
        if (discoveredShapesDisplay == null) return;

        if (discoveredShapes.Count == 0)
        {
            discoveredShapesDisplay.text = "Discovered Shapes: None\nExplore the room to find shape clues!";
            return;
        }

        string display = "Discovered Shapes:\n";
        
        // Show shapes in word order
        for (int i = 0; i < shapesInOrder.Count; i++)
        {
            ShapeLetterPair pair = shapesInOrder[i];
            string indicator = (i == currentShapeIndex) ? "► " : "   ";
            display += $"{indicator}Position {pair.position + 1}: {pair.shape} → {pair.letter}\n";
        }
        
        discoveredShapesDisplay.text = display;
    }

    private void UpdateProgressDisplay()
    {
        if (progressDisplay == null) return;

        int totalNeeded = correctWord.Length;
        int discovered = discoveredShapes.Count;
        
        string progress = $"Progress: {discovered}/{totalNeeded} shapes found\n";
        
        if (discovered == totalNeeded)
        {
            progress += "All shapes discovered! Try to decode the word.";
        }
        else
        {
            progress += $"Find {totalNeeded - discovered} more shapes to complete the puzzle.";
        }
        
        progressDisplay.text = progress;
    }

    private void ShowInitialHint()
    {
        if (hintDisplay != null)
        {
            hintDisplay.text = wordHint;
        }
        
        GiveFeedback("Welcome to the Shape Cipher! " + wordHint);
    }

    private void ResetInput()
    {
        if (wordInputField != null)
            wordInputField.text = "";
        GiveFeedback("Input cleared. Continue decoding the shapes.");
    }

    private void CheckWord()
    {
        if (wordInputField == null) return;
        
        string enteredWord = wordInputField.text.ToUpper().Trim();
        
        if (string.IsNullOrEmpty(enteredWord))
        {
            GiveFeedback("Please enter the decoded word.");
            return;
        }

        if (enteredWord == correctWord.ToUpper())
        {
            GiveFeedback($"Excellent! You've decoded the word correctly! Your escape number is: {randomNumber}");
            CompletePuzzle();
            
            // Notify the escape code manager
            EscapeCodeManager escapeManager = FindFirstObjectByType<EscapeCodeManager>();
            if (escapeManager != null)
                escapeManager.OnPuzzleCompleted(randomNumber);
        }
        else
        {
            GiveFeedback($"'{enteredWord}' is not correct. Check your shape-to-letter mappings and try again.");
            
            // Give a hint about what they might be missing
            if (discoveredShapes.Count < correctWord.Length)
            {
                GiveFeedback($"Hint: You still need to find {correctWord.Length - discoveredShapes.Count} more shape(s) in the room.");
            }
        }
    }

    public override void ResetPuzzle()
    {
        base.ResetPuzzle();
        
        currentShapeIndex = 0;
        discoveredShapes.Clear();
        shapesInOrder.Clear();
        
        if (wordInputField != null)
            wordInputField.text = "";
            
        UpdateAllDisplays();
        ShowInitialHint();
    }

    public override void ShowPuzzle()
    {
        base.ShowPuzzle();
        
        if (wordInputField != null)
        {
            wordInputField.ActivateInputField();
            wordInputField.Select();
        }
    }

    // Public method to provide feedback (called by environmental clues)
    public void ProvideFeedback(string message)
    {
        GiveFeedback(message);
    }

    // Helper method to get current progress
    public string GetCurrentProgress()
    {
        int discovered = discoveredShapes.Count;
        int total = correctWord.Length;
        return $"{discovered}/{total} shapes discovered";
    }

    // Helper method to check if all shapes are discovered
    public bool AllShapesDiscovered()
    {
        return discoveredShapes.Count >= correctWord.Length;
    }

    // Test method to manually add a shape (for testing)
    [ContextMenu("Test Add Shape")]
    public void TestAddShape()
    {
        OnShapeDiscovered("△", "C", 0);
    }

    // Method to get discovered shapes info
    public Dictionary<string, ShapeLetterPair> GetDiscoveredShapes()
    {
        return new Dictionary<string, ShapeLetterPair>(discoveredShapes);
    }
}