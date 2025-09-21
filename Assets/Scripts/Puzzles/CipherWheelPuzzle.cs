using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CipherWheelPuzzle : BasePuzzle
{
    [Header("Symbol-Letter Matching")]
    [Tooltip("Button to cycle through symbols")]
    public Button symbolButton;
    
    [Tooltip("Button to cycle through letters")]
    public Button letterButton;
    
    [Tooltip("Button to test if current symbol-letter combination is correct")]
    public Button testMatchButton;
    
    [Tooltip("Display for current selected symbol")]
    public TMP_Text symbolDisplay;
    
    [Tooltip("Display for current selected letter")]
    public TMP_Text letterDisplay;
    
    [Tooltip("Input field for the decoded password")]
    public TMP_InputField codeInputField;
    
    [Tooltip("Button to submit the decoded password")]
    public Button submitButton;
    
    [Tooltip("Reset button to clear input and discovered mappings")]
    public Button resetButton;

    [Header("Puzzle Configuration")]
    [Tooltip("Symbol sequences found around the room")]
    public string[] symbolSequences = {"△○□", "●◇▽", "◆⬟⬢"};
    
    [Tooltip("The correct password when symbols are decoded")]
    public string correctPassword = "CODE";
    
    [Tooltip("Display showing symbol sequences found in room")]
    public TMP_Text symbolSequenceDisplay;
    
    [Tooltip("Display showing discovered symbol-letter mappings")]
    public TMP_Text discoveredMappingsDisplay;

    private int currentSymbolIndex = 0;
    private int currentLetterIndex = 0;
    private string[] symbols = {"△", "○", "□", "●", "◇", "▽", "◆", "⬟", "⬢"};
    private string[] letters = {"A", "B", "C", "D", "E", "F", "G", "H", "I"};
    
    // Correct symbol-letter mappings (△=C, ○=O, □=D, ●=E for "CODE")
    private Dictionary<string, string> correctMappings = new Dictionary<string, string>
    {
        {"△", "C"}, {"○", "O"}, {"□", "D"}, {"●", "E"},
        {"◇", "A"}, {"▽", "F"}, {"◆", "G"}, {"⬟", "H"}, {"⬢", "I"}
    };
    
    private Dictionary<string, string> discoveredMappings = new Dictionary<string, string>();
    private int randomNumber;

    private void Start()
    {
        Initialize();
        
        // Make sure the puzzle UI reference is set
        if (puzzleUI == null)
        {
            puzzleUI = GameObject.Find("CipherWheelCanvas");
            if (puzzleUI != null)
            {
                Debug.Log("Found and assigned CipherWheelCanvas as puzzleUI");
            }
            else
            {
                Debug.LogError("Could not find CipherWheelCanvas!");
            }
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        randomNumber = Random.Range(1000, 9999);
        
        SetupButtons();
        UpdateDisplay();
        UpdateSymbolSequenceDisplay();
        UpdateDiscoveredMappingsDisplay();
    }

    // Test method to manually show the puzzle (can be called from inspector or other scripts)
    [ContextMenu("Test Show Puzzle")]
    public void TestShowPuzzle()
    {
        ShowPuzzle();
    }

    private void SetupButtons()
    {
        if (symbolButton != null)
        {
            symbolButton.onClick.AddListener(CycleSymbol);
            Debug.Log("Symbol button connected successfully");
        }
        else
        {
            Debug.LogError("Symbol button is not assigned in the inspector!");
        }
            
        if (letterButton != null)
        {
            letterButton.onClick.AddListener(CycleLetter);
            Debug.Log("Letter button connected successfully");
        }
        else
        {
            Debug.LogError("Letter button is not assigned in the inspector!");
        }
        
        if (testMatchButton != null)
        {
            testMatchButton.onClick.AddListener(TestMatch);
            Debug.Log("Test match button connected successfully");
        }
        else
        {
            Debug.LogError("Test match button is not assigned in the inspector!");
        }
            
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(CheckPassword);
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

    private void CycleSymbol()
    {
        currentSymbolIndex = (currentSymbolIndex + 1) % symbols.Length;
        UpdateDisplay();
        GiveFeedback($"Selected symbol: {symbols[currentSymbolIndex]}");
    }

    private void CycleLetter()
    {
        currentLetterIndex = (currentLetterIndex + 1) % letters.Length;
        UpdateDisplay();
        GiveFeedback($"Selected letter: {letters[currentLetterIndex]}");
    }

    private void TestMatch()
    {
        string currentSymbol = symbols[currentSymbolIndex];
        string currentLetter = letters[currentLetterIndex];
        
        if (correctMappings.ContainsKey(currentSymbol) && correctMappings[currentSymbol] == currentLetter)
        {
            // Correct mapping found!
            if (!discoveredMappings.ContainsKey(currentSymbol))
            {
                discoveredMappings[currentSymbol] = currentLetter;
                GiveFeedback($"✅ MATCH! {currentSymbol} = {currentLetter}");
                UpdateDiscoveredMappingsDisplay();
            }
            else
            {
                GiveFeedback($"✅ MATCH! {currentSymbol} = {currentLetter} (already discovered)");
            }
        }
        else
        {
            GiveFeedback($"❌ NO MATCH: {currentSymbol} ≠ {currentLetter}");
        }
    }

    private void UpdateDisplay()
    {
        if (symbolDisplay != null)
        {
            symbolDisplay.text = symbols[currentSymbolIndex];
            Debug.Log($"Updated symbol display to: {symbols[currentSymbolIndex]}");
        }
        else
        {
            Debug.LogError("Symbol display is not assigned in the inspector!");
        }
            
        if (letterDisplay != null)
        {
            letterDisplay.text = letters[currentLetterIndex];
            Debug.Log($"Updated letter display to: {letters[currentLetterIndex]}");
        }
        else
        {
            Debug.LogError("Letter display is not assigned in the inspector!");
        }
    }

    private void UpdateSymbolSequenceDisplay()
    {
        if (symbolSequenceDisplay != null)
        {
            string display = "Symbol Sequences Found:\n";
            for (int i = 0; i < symbolSequences.Length; i++)
            {
                display += $"Location {i + 1}: {symbolSequences[i]}\n";
            }
            symbolSequenceDisplay.text = display;
        }
    }

    private void UpdateDiscoveredMappingsDisplay()
    {
        if (discoveredMappingsDisplay != null)
        {
            string display = "Discovered Mappings:\n";
            if (discoveredMappings.Count == 0)
            {
                display += "None discovered yet";
            }
            else
            {
                foreach (var mapping in discoveredMappings)
                {
                    display += $"{mapping.Key} = {mapping.Value}\n";
                }
            }
            discoveredMappingsDisplay.text = display;
        }
    }

    private void ResetInput()
    {
        if (codeInputField != null)
            codeInputField.text = "";
        
        discoveredMappings.Clear();
        UpdateDiscoveredMappingsDisplay();
        GiveFeedback("Input cleared and mappings reset. Start testing matches again.");
    }

    private void CheckPassword()
    {
        if (codeInputField == null) return;
        
        string enteredPassword = codeInputField.text.ToUpper().Trim();
        
        if (string.IsNullOrEmpty(enteredPassword))
        {
            GiveFeedback("Please enter the decoded password.");
            return;
        }

        if (enteredPassword == correctPassword.ToUpper())
        {
            GiveFeedback($"Cipher decoded successfully! Your escape number is: {randomNumber}");
            CompletePuzzle();
            
            // Notify the escape code manager
            EscapeCodeManager escapeManager = FindFirstObjectByType<EscapeCodeManager>();
            if (escapeManager != null)
                escapeManager.OnPuzzleCompleted(randomNumber);
        }
        else
        {
            GiveFeedback("Incorrect password. Check your symbol-to-letter mappings and try again.");
        }
    }

    public override void ResetPuzzle()
    {
        base.ResetPuzzle();
        currentSymbolIndex = 0;
        currentLetterIndex = 0;
        discoveredMappings.Clear();
        
        if (codeInputField != null)
            codeInputField.text = "";
            
        UpdateDisplay();
        UpdateDiscoveredMappingsDisplay();
    }

    public override void ShowPuzzle()
    {
        base.ShowPuzzle();
        
        if (codeInputField != null)
        {
            codeInputField.ActivateInputField();
            codeInputField.Select();
        }
    }

    // Helper method to get current symbol-letter selection
    public string GetCurrentSelection()
    {
        return $"Testing: {symbols[currentSymbolIndex]} → {letters[currentLetterIndex]}";
    }
    
    // Helper method to get discovered mappings count
    public int GetDiscoveredMappingsCount()
    {
        return discoveredMappings.Count;
    }
}