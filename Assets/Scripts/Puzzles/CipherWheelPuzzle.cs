using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CipherWheelPuzzle : BasePuzzle
{
    [Header("Cipher Wheel Settings")]
    [Tooltip("Button to rotate the outer ring (symbols)")]
    public Button outerRingButton;
    
    [Tooltip("Button to rotate the inner ring (letters)")]
    public Button innerRingButton;
    
    [Tooltip("Display for current symbol")]
    public TMP_Text symbolDisplay;
    
    [Tooltip("Display for current letter")]
    public TMP_Text letterDisplay;
    
    [Tooltip("Input field for the decoded password")]
    public TMP_InputField codeInputField;
    
    [Tooltip("Button to submit the decoded password")]
    public Button submitButton;
    
    [Tooltip("Reset button to clear input")]
    public Button resetButton;

    [Header("Puzzle Configuration")]
    [Tooltip("Symbol sequences found around the room")]
    public string[] symbolSequences = {"△○□", "●◇▽", "◆⬟⬢"};
    
    [Tooltip("The correct password when symbols are decoded")]
    public string correctPassword = "CODE";
    
    [Tooltip("Display showing symbol sequences found in room")]
    public TMP_Text symbolSequenceDisplay;

    private int outerRingPosition = 0;
    private int innerRingPosition = 0;
    private string[] symbols = {"△", "○", "□", "●", "◇", "▽", "◆", "⬟", "⬢"};
    private string[] letters = {"A", "B", "C", "D", "E", "F", "G", "H", "I"};
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
    }

    // Test method to manually show the puzzle (can be called from inspector or other scripts)
    [ContextMenu("Test Show Puzzle")]
    public void TestShowPuzzle()
    {
        ShowPuzzle();
    }

    private void SetupButtons()
    {
        if (outerRingButton != null)
        {
            outerRingButton.onClick.AddListener(RotateOuterRing);
            Debug.Log("Outer ring button connected successfully");
        }
        else
        {
            Debug.LogError("Outer ring button is not assigned in the inspector!");
        }
            
        if (innerRingButton != null)
        {
            innerRingButton.onClick.AddListener(RotateInnerRing);
            Debug.Log("Inner ring button connected successfully");
        }
        else
        {
            Debug.LogError("Inner ring button is not assigned in the inspector!");
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

    private void RotateOuterRing()
    {
        outerRingPosition = (outerRingPosition + 1) % symbols.Length;
        UpdateDisplay();
        GiveFeedback($"Outer ring position: {outerRingPosition + 1}");
    }

    private void RotateInnerRing()
    {
        innerRingPosition = (innerRingPosition + 1) % letters.Length;
        UpdateDisplay();
        GiveFeedback($"Inner ring position: {innerRingPosition + 1}");
    }

    private void UpdateDisplay()
    {
        if (symbolDisplay != null)
        {
            symbolDisplay.text = symbols[outerRingPosition];
            Debug.Log($"Updated symbol display to: {symbols[outerRingPosition]}");
        }
        else
        {
            Debug.LogError("Symbol display is not assigned in the inspector!");
        }
            
        if (letterDisplay != null)
        {
            letterDisplay.text = letters[innerRingPosition];
            Debug.Log($"Updated letter display to: {letters[innerRingPosition]}");
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

    private void ResetInput()
    {
        if (codeInputField != null)
            codeInputField.text = "";
        GiveFeedback("Input cleared. Continue decoding.");
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
        outerRingPosition = 0;
        innerRingPosition = 0;
        
        if (codeInputField != null)
            codeInputField.text = "";
            
        UpdateDisplay();
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

    // Helper method to get current symbol-letter mapping
    public string GetCurrentMapping()
    {
        return $"{symbols[outerRingPosition]} → {letters[innerRingPosition]}";
    }
}