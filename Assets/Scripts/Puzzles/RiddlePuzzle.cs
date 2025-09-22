using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class RiddlePuzzle : BasePuzzle
{
    [Header("Riddle Settings")]
    [Tooltip("Input field for player to enter riddle answers")]
    public TMP_InputField riddleAnswerInput;
    
    [Tooltip("Button to submit riddle answer")]
    public Button submitAnswerButton;
    
    [Tooltip("Button to show next riddle")]
    public Button nextRiddleButton;
    
    [Tooltip("Button to show previous riddle")]
    public Button previousRiddleButton;
    
    [Tooltip("Display for the current riddle")]
    public TMP_Text riddleDisplay;
    
    [Tooltip("Display for riddle progress (1 of 4)")]
    public TMP_Text riddleProgressDisplay;
    
    [Tooltip("Display for solved riddles")]
    public TMP_Text solvedRiddlesDisplay;
    
    [Tooltip("Reset button to clear progress")]
    public Button resetButton;
    
    [Tooltip("Close button to hide the puzzle")]
    public Button closeButton;

    [Header("Riddle Configuration")]
    [Tooltip("All riddles with their answers")]
    public List<RiddleData> riddles = new List<RiddleData>
    {
        new RiddleData
        {
            riddle = "I have keys but no locks. I have space but no room. You can enter, but can't go outside. What am I?",
            answer = "KEYBOARD",
            hint = "Something you use every day to communicate digitally."
        },
        new RiddleData
        {
            riddle = "I am taken from a mine, and shut up in a wooden case, from which I am never released, and yet I am used by almost everyone. What am I?",
            answer = "PENCIL",
            hint = "Writing tool made from graphite."
        },
        new RiddleData
        {
            riddle = "I speak without a mouth and hear without ears. I have no body, but come alive with wind. What am I?",
            answer = "ECHO",
            hint = "Sound that bounces back to you."
        },
        new RiddleData
        {
            riddle = "The more you take, the more you leave behind. What am I?",
            answer = "FOOTSTEPS",
            hint = "Think about walking somewhere."
        }
    };

    [Header("AI Terminal Integration")]
    [Tooltip("Reference to the terminal controller for generating nonsense responses")]
    public TerminalControllerNew aiTerminal;
    
    [Tooltip("Nonsense text templates that will contain hidden answers")]
    public List<string> nonsenseTemplates = new List<string>
    {
        "QUANTUM FLUX DETECTED: Analyzing temporal displacement matrices... {0} patterns emerging from dimensional vectors... Error 404: Logic.exe has stopped working... Calibrating nonsense generators... {0} frequencies detected in the void... Please try turning reality off and on again...",
        "CRYPTOGRAPHIC BABEL INITIATED: Processing linguistic entropy... {0} algorithms dancing with mathematical chaos... Warning: Sanity levels approaching zero... {0} variables compiled in nonsensical order... Have you tried feeding the AI more coffee?...",
        "NEURAL NETWORK MALFUNCTION: Synapses misfiring in comedic harmony... {0} patterns discovered in random noise... Alert: The AI is having an existential crisis... {0} solutions found, all involving rubber ducks... Rebooting common sense module...",
        "RECURSIVE PARADOX ENGAGED: Calculating the square root of purple... {0} answers hiding in plain sight... Error: Cannot divide by happiness... {0} results processed through the nonsense engine... The computer is laughing at your confusion..."
    };

    private int currentRiddleIndex = 0;
    private List<bool> riddlesSolved = new List<bool>();
    private int randomNumber;
    private static RiddlePuzzle instance;

    [System.Serializable]
    public class RiddleData
    {
        public string riddle;
        public string answer;
        public string hint;
    }

    void Awake()
    {
        instance = this;
    }

    public static RiddlePuzzle Instance => instance;

    private void Start()
    {
        Initialize();
        
        // Make sure the puzzle UI reference is set
        if (puzzleUI == null)
        {
            puzzleUI = GameObject.Find("RiddleCanvas");
            if (puzzleUI != null)
            {
                Debug.Log("Found and assigned RiddleCanvas as puzzleUI");
            }
            else
            {
                Debug.LogError("Could not find RiddleCanvas!");
            }
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        randomNumber = Random.Range(1000, 9999);
        
        // Initialize solved status for all riddles
        riddlesSolved.Clear();
        for (int i = 0; i < riddles.Count; i++)
        {
            riddlesSolved.Add(false);
        }
        
        SetupButtons();
        UpdateDisplay();
        
        // Find AI terminal if not assigned
        if (aiTerminal == null)
        {
            aiTerminal = FindFirstObjectByType<TerminalControllerNew>();
        }
    }

    [ContextMenu("Test Show Puzzle")]
    public void TestShowPuzzle()
    {
        ShowPuzzle();
    }

    private void SetupButtons()
    {
        if (submitAnswerButton != null)
        {
            submitAnswerButton.onClick.AddListener(CheckAnswer);
            Debug.Log("Submit answer button connected successfully");
        }
        else
        {
            Debug.LogError("Submit answer button is not assigned in the inspector!");
        }
            
        if (nextRiddleButton != null)
        {
            nextRiddleButton.onClick.AddListener(NextRiddle);
            Debug.Log("Next riddle button connected successfully");
        }
        else
        {
            Debug.LogError("Next riddle button is not assigned in the inspector!");
        }
        
        if (previousRiddleButton != null)
        {
            previousRiddleButton.onClick.AddListener(PreviousRiddle);
            Debug.Log("Previous riddle button connected successfully");
        }
        else
        {
            Debug.LogError("Previous riddle button is not assigned in the inspector!");
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
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HidePuzzle);
            Debug.Log("Close button connected successfully");
        }
        else
        {
            Debug.LogError("Close button is not assigned in the inspector!");
        }
    }

    private void NextRiddle()
    {
        currentRiddleIndex = (currentRiddleIndex + 1) % riddles.Count;
        UpdateDisplay();
        GiveFeedback($"Showing riddle {currentRiddleIndex + 1} of {riddles.Count}");
    }

    private void PreviousRiddle()
    {
        currentRiddleIndex = (currentRiddleIndex - 1 + riddles.Count) % riddles.Count;
        UpdateDisplay();
        GiveFeedback($"Showing riddle {currentRiddleIndex + 1} of {riddles.Count}");
    }

    private void UpdateDisplay()
    {
        if (riddleDisplay != null && currentRiddleIndex < riddles.Count)
        {
            string status = riddlesSolved[currentRiddleIndex] ? " [SOLVED]" : "";
            riddleDisplay.text = riddles[currentRiddleIndex].riddle + status;
        }
        
        if (riddleProgressDisplay != null)
        {
            riddleProgressDisplay.text = $"Riddle {currentRiddleIndex + 1} of {riddles.Count}";
        }
        
        UpdateSolvedRiddlesDisplay();
    }

    private void UpdateSolvedRiddlesDisplay()
    {
        if (solvedRiddlesDisplay != null)
        {
            int solvedCount = 0;
            string display = "Solved Riddles:\n";
            
            for (int i = 0; i < riddles.Count; i++)
            {
                if (riddlesSolved[i])
                {
                    display += $"{i + 1}. {riddles[i].answer}\n";
                    solvedCount++;
                }
            }
            
            if (solvedCount == 0)
            {
                display += "None solved yet";
            }
            
            display += $"\nProgress: {solvedCount}/{riddles.Count}";
            solvedRiddlesDisplay.text = display;
        }
    }

    private void CheckAnswer()
    {
        if (riddleAnswerInput == null) return;
        
        string playerAnswer = riddleAnswerInput.text.ToUpper().Trim();
        
        if (string.IsNullOrEmpty(playerAnswer))
        {
            GiveFeedback("Please enter an answer to the riddle.");
            return;
        }

        string correctAnswer = riddles[currentRiddleIndex].answer.ToUpper();
        
        if (playerAnswer == correctAnswer)
        {
            if (!riddlesSolved[currentRiddleIndex])
            {
                riddlesSolved[currentRiddleIndex] = true;
                GiveFeedback($"✅ Correct! The answer is {correctAnswer}.");
                UpdateDisplay();
                
                // Check if all riddles are solved
                bool allSolved = true;
                foreach (bool solved in riddlesSolved)
                {
                    if (!solved)
                    {
                        allSolved = false;
                        break;
                    }
                }
                
                if (allSolved)
                {
                    GiveFeedback($"🎉 All riddles solved! Your escape number is: {randomNumber}");
                    CompletePuzzle();
                    
                    // Notify the escape code manager
                    EscapeCodeManager escapeManager = FindFirstObjectByType<EscapeCodeManager>();
                    if (escapeManager != null)
                        escapeManager.OnPuzzleCompleted(randomNumber);
                }
            }
            else
            {
                GiveFeedback("You already solved this riddle!");
            }
        }
        else
        {
            GiveFeedback("❌ Incorrect. Try again or use the AI terminal for 'help'...");
        }
        
        riddleAnswerInput.text = "";
    }

    private void ResetInput()
    {
        if (riddleAnswerInput != null)
            riddleAnswerInput.text = "";
        GiveFeedback("Input cleared. Keep thinking!");
    }

    public override void ResetPuzzle()
    {
        base.ResetPuzzle();
        currentRiddleIndex = 0;
        
        for (int i = 0; i < riddlesSolved.Count; i++)
        {
            riddlesSolved[i] = false;
        }
        
        if (riddleAnswerInput != null)
            riddleAnswerInput.text = "";
            
        UpdateDisplay();
    }

    public override void ShowPuzzle()
    {
        base.ShowPuzzle();
        
        if (riddleAnswerInput != null)
        {
            riddleAnswerInput.ActivateInputField();
            riddleAnswerInput.Select();
        }
    }

    // This method will be called by the AI terminal when player asks for help
    public string GenerateNonsenseResponse(int riddleIndex)
    {
        if (riddleIndex < 0 || riddleIndex >= riddles.Count)
            return "ERROR: Riddle index out of bounds. Please reboot your brain.";
            
        string answer = riddles[riddleIndex].answer;
        string template = nonsenseTemplates[Random.Range(0, nonsenseTemplates.Count)];
        
        // Insert the answer into the nonsense text at the {0} placeholders
        return string.Format(template, answer);
    }

    // Helper method to get the current riddle's nonsense response
    public string GetCurrentRiddleNonsenseResponse()
    {
        return GenerateNonsenseResponse(currentRiddleIndex);
    }

    // Helper method for other scripts to check if this puzzle is active
    public bool IsRiddlePuzzleActive()
    {
        return puzzleUI != null && puzzleUI.activeSelf;
    }

    // Helper method to get current riddle info
    public RiddleData GetCurrentRiddle()
    {
        if (currentRiddleIndex >= 0 && currentRiddleIndex < riddles.Count)
            return riddles[currentRiddleIndex];
        return null;
    }

    // Helper method to get solved count
    public int GetSolvedCount()
    {
        int count = 0;
        foreach (bool solved in riddlesSolved)
        {
            if (solved) count++;
        }
        return count;
    }
}