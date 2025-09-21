using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Color light sequence puzzle where players click physical buttons with colored lights
/// </summary>
public class ColorLightPuzzle : BasePuzzle
{
    [Header("Color Button Configuration")]
    [Tooltip("Array of color buttons in the scene")]
    public ColorButton[] colorButtons;
    
    [Tooltip("Correct sequence of colors to press")]
    public ColorType[] correctSequence = { ColorType.Red, ColorType.Blue, ColorType.Green, ColorType.Yellow };
    
    [Tooltip("Environmental clue hints that appear around the room")]
    public EnvironmentalClue[] environmentalClues;

    [Header("Feedback Settings")]
    [Tooltip("Duration of success/failure light flash")]
    public float flashDuration = 1.5f;
    
    [Tooltip("Color for success flash")]
    public Color successColor = Color.green;
    
    [Tooltip("Color for failure flash")]
    public Color failureColor = Color.red;
    
    [Tooltip("Time to wait before allowing new input after wrong sequence")]
    public float wrongSequenceCooldown = 2f;

    [Header("Riddle System")]
    [Tooltip("Riddles that the AI terminal can give as hints")]
    public string[] riddleHints = {
        "The first hue burns like passion in the artist's corner",
        "The second flows like ocean depths near the research station", 
        "The third grows like nature's life beside the old oak frame",
        "The fourth shines like sun's warmth where time stands still"
    };

    private List<ColorType> playerSequence = new List<ColorType>();
    private bool isAcceptingInput = true;
    private int randomNumber;
    private bool isFlashing = false;

    [System.Serializable]
    public class EnvironmentalClue
    {
        [Tooltip("Description of where this clue is located")]
        public string location;
        
        [Tooltip("The color this clue represents")]
        public ColorType colorHint;
        
        [Tooltip("Position in the sequence (0-based)")]
        public int sequencePosition;
        
        [Tooltip("Visual description for players")]
        public string visualDescription;
    }

    public override void Initialize()
    {
        base.Initialize();
        randomNumber = Random.Range(2000, 9999);
        
        SetupColorButtons();
        InitializeButtonLights();
        ResetPuzzle();
        
        ProvideFeedback("Color sequence puzzle activated. Look for environmental color clues around the room.");
    }

    private void SetupColorButtons()
    {
        for (int i = 0; i < colorButtons.Length; i++)
        {
            if (colorButtons[i] != null)
            {
                colorButtons[i].Initialize(this);
            }
        }
    }

    private void InitializeButtonLights()
    {
        foreach (ColorButton button in colorButtons)
        {
            if (button != null)
            {
                button.SetLightToDefaultColor();
            }
        }
    }

    public void OnColorButtonPressed(ColorType color)
    {
        if (!isAcceptingInput || isFlashing)
        {
            return;
        }

        // Add color to player sequence
        playerSequence.Add(color);
        
        // Visual feedback for button press
        ColorButton pressedButton = GetButtonByColor(color);
        if (pressedButton != null)
        {
            pressedButton.PulseLight();
        }

        ProvideFeedback($"Pressed: {color}. Sequence: {GetSequenceString()}");

        // Check if sequence is complete
        if (playerSequence.Count >= correctSequence.Length)
        {
            CheckSequence();
        }
        else
        {
            // Check if current sequence is still valid
            if (!IsCurrentSequenceValid())
            {
                StartCoroutine(HandleWrongSequence());
            }
        }
    }

    // Public feedback method that can be called by environmental clues
    public void ProvideFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
        Debug.Log($"[ColorLightPuzzle] {message}");
    }

    private bool IsCurrentSequenceValid()
    {
        for (int i = 0; i < playerSequence.Count; i++)
        {
            if (i >= correctSequence.Length || playerSequence[i] != correctSequence[i])
            {
                return false;
            }
        }
        return true;
    }

    private void CheckSequence()
    {
        bool isCorrect = true;
        
        if (playerSequence.Count != correctSequence.Length)
        {
            isCorrect = false;
        }
        else
        {
            for (int i = 0; i < correctSequence.Length; i++)
            {
                if (playerSequence[i] != correctSequence[i])
                {
                    isCorrect = false;
                    break;
                }
            }
        }

        if (isCorrect)
        {
            StartCoroutine(HandleCorrectSequence());
        }
        else
        {
            StartCoroutine(HandleWrongSequence());
        }
    }

    private IEnumerator HandleCorrectSequence()
    {
        isAcceptingInput = false;
        isFlashing = true;

        // Flash all buttons green
        foreach (ColorButton button in colorButtons)
        {
            if (button != null)
            {
                button.FlashColor(successColor, flashDuration);
            }
        }

        yield return new WaitForSeconds(flashDuration);

        // Success feedback
        ProvideFeedback($"Perfect sequence! The colors align with the environmental clues. Your access code is: {randomNumber}");
        
        CompletePuzzle();
        
        // Notify escape code manager
        EscapeCodeManager escapeManager = FindObjectOfType<EscapeCodeManager>();
        if (escapeManager != null)
        {
            escapeManager.OnPuzzleCompleted(randomNumber);
        }

        isFlashing = false;
    }

    private IEnumerator HandleWrongSequence()
    {
        isAcceptingInput = false;
        isFlashing = true;

        // Flash all buttons red
        foreach (ColorButton button in colorButtons)
        {
            if (button != null)
            {
                button.FlashColor(failureColor, flashDuration);
            }
        }

        yield return new WaitForSeconds(flashDuration);

        // Reset and give feedback
        playerSequence.Clear();
        ProvideFeedback("Incorrect sequence. Study the environmental clues more carefully.");

        // Cooldown period
        yield return new WaitForSeconds(wrongSequenceCooldown - flashDuration);

        // Reset button lights
        InitializeButtonLights();
        isAcceptingInput = true;
        isFlashing = false;
    }

    private ColorButton GetButtonByColor(ColorType color)
    {
        foreach (ColorButton button in colorButtons)
        {
            if (button != null && button.ButtonColor == color)
            {
                return button;
            }
        }
        return null;
    }

    private string GetSequenceString()
    {
        if (playerSequence.Count == 0) return "None";
        
        string sequence = "";
        for (int i = 0; i < playerSequence.Count; i++)
        {
            sequence += playerSequence[i].ToString();
            if (i < playerSequence.Count - 1)
                sequence += " → ";
        }
        return sequence;
    }

    public string GetRiddleHint(int hintIndex)
    {
        if (hintIndex >= 0 && hintIndex < riddleHints.Length)
        {
            return riddleHints[hintIndex];
        }
        return "The sequence follows the pattern of colors found throughout this chamber.";
    }

    public string GetEnvironmentalClueHint()
    {
        string hint = "Environmental color clues discovered:\n";
        
        foreach (var clue in environmentalClues)
        {
            hint += $"• {clue.location}: {clue.visualDescription}\n";
        }
        
        return hint + "Observe the sequence these clues suggest.";
    }

    public override void ResetPuzzle()
    {
        base.ResetPuzzle();
        
        playerSequence.Clear();
        isAcceptingInput = true;
        isFlashing = false;
        
        StopAllCoroutines();
        InitializeButtonLights();
        
        ProvideFeedback("Color sequence reset. Find the environmental clues to determine the correct order.");
    }

    public override void ShowPuzzle()
    {
        base.ShowPuzzle();
        ProvideFeedback("Study the colored elements around the room. They hold the key to the sequence.");
    }

    // Method for AI terminal to call for hints
    public void ProvideHint(int level)
    {
        switch (level)
        {
            case 1:
                ProvideFeedback("Look around the room for objects, paintings, or displays that show specific colors.");
                break;
            case 2:
                if (playerSequence.Count < riddleHints.Length)
                {
                    ProvideFeedback($"Riddle for position {playerSequence.Count + 1}: {GetRiddleHint(playerSequence.Count)}");
                }
                else
                {
                    ProvideFeedback("You've received all available riddles. Observe your environment carefully.");
                }
                break;
            case 3:
                ProvideFeedback(GetEnvironmentalClueHint());
                break;
            case 4:
                ProvideFeedback($"The sequence has {correctSequence.Length} colors. Current progress: {playerSequence.Count}/{correctSequence.Length}");
                break;
        }
    }

    // Debug method to show correct sequence
    [ContextMenu("🔍 Show Correct Sequence")]
    private void DebugShowSequence()
    {
        string sequence = "Correct sequence: ";
        for (int i = 0; i < correctSequence.Length; i++)
        {
            sequence += correctSequence[i].ToString();
            if (i < correctSequence.Length - 1)
                sequence += " → ";
        }
        Debug.Log(sequence);
    }
}