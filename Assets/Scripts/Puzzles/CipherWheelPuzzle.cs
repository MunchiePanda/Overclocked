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

    public override void Initialize()
    {
        base.Initialize();
        randomNumber = Random.Range(1000, 9999);
        
        SetupButtons();
        UpdateDisplay();
        UpdateSymbolSequenceDisplay();
    }

    private void SetupButtons()
    {
        if (outerRingButton != null)
            outerRingButton.onClick.AddListener(RotateOuterRing);
            
        if (innerRingButton != null)
            innerRingButton.onClick.AddListener(RotateInnerRing);
            
        if (submitButton != null)
            submitButton.onClick.AddListener(CheckPassword);
            
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetInput);
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
            symbolDisplay.text = symbols[outerRingPosition];
            
        if (letterDisplay != null)
            letterDisplay.text = letters[innerRingPosition];
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
            EscapeCodeManager escapeManager = FindObjectOfType<EscapeCodeManager>();
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