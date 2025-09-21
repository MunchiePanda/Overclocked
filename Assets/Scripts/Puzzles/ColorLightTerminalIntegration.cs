using UnityEngine;

/// <summary>
/// Integrates color light puzzle with the AI terminal for riddles and hints
/// </summary>
public class ColorLightTerminalIntegration : MonoBehaviour
{
    [Header("Terminal Integration")]
    [Tooltip("The AI terminal to connect to")]
    public InteractableAITerminalNew aiTerminal;
    
    [Tooltip("The color light puzzle to provide hints for")]
    public ColorLightPuzzle colorPuzzle;

    [Header("Riddle Configuration")]
    [Tooltip("Riddles for each color in sequence")]
    public ColorRiddle[] colorRiddles = {
        new ColorRiddle { color = ColorType.Red, riddle = "I burn with passion in the artist's domain, where creativity flows through brush and pain." },
        new ColorRiddle { color = ColorType.Blue, riddle = "Deep as oceans, vast as sky, I illuminate data flowing by." },
        new ColorRiddle { color = ColorType.Green, riddle = "Life springs eternal in my glow, where nature's children grow and grow." },
        new ColorRiddle { color = ColorType.Yellow, riddle = "Golden warmth that banishes night, I am captured sunshine's light." }
    };

    [Header("Hint Levels")]
    [Tooltip("Progressive hint system")]
    public HintLevel[] hintLevels = {
        new HintLevel { level = 1, description = "Environmental Awareness", message = "Observe the colored objects placed throughout this chamber." },
        new HintLevel { level = 2, description = "Riddle Guidance", message = "I can provide riddles to guide your search..." },
        new HintLevel { level = 3, description = "Sequence Clues", message = "The order matters - look for numbered or positioned elements." },
        new HintLevel { level = 4, description = "Direct Assistance", message = "Your progress and remaining steps..." }
    };

    [System.Serializable]
    public class ColorRiddle
    {
        public ColorType color;
        [TextArea(2, 4)]
        public string riddle;
    }

    [System.Serializable]
    public class HintLevel
    {
        public int level;
        public string description;
        [TextArea(2, 4)]
        public string message;
    }

    private void Start()
    {
        SetupTerminalIntegration();
    }

    private void SetupTerminalIntegration()
    {
        // Auto-find components if not assigned
        if (aiTerminal == null)
            aiTerminal = FindFirstObjectByType<InteractableAITerminalNew>();
            
        if (colorPuzzle == null)
            colorPuzzle = FindFirstObjectByType<ColorLightPuzzle>();

        if (aiTerminal != null && colorPuzzle != null)
        {
            // Register this integration with the terminal
            Debug.Log("✅ Color light puzzle integrated with AI terminal");
        }
        else
        {
            Debug.LogWarning("⚠️ Missing AI terminal or color puzzle references");
        }
    }

    // Method called by AI terminal when player requests a hint for color puzzle
    public string RequestColorPuzzleHint(int hintLevel)
    {
        if (colorPuzzle == null)
            return "Error: Color puzzle system not found.";

        switch (hintLevel)
        {
            case 1:
                return GetEnvironmentalHint();
            case 2:
                return GetRiddleHint();
            case 3:
                return GetSequenceHint();
            case 4:
                return GetProgressHint();
            default:
                return GetGeneralHint();
        }
    }

    private string GetEnvironmentalHint()
    {
        string hint = "🔍 ENVIRONMENTAL ANALYSIS\n\n";
        hint += "Scanning chamber for color-coded elements...\n\n";
        hint += "• Examine paintings, displays, and illuminated objects\n";
        hint += "• Look for consistent color patterns\n";
        hint += "• Note the positioning of colored elements\n";
        hint += "• Each color represents one step in the sequence\n\n";
        hint += "💡 Tip: Some objects may require closer inspection to reveal their significance.";
        
        return hint;
    }

    private string GetRiddleHint()
    {
        // Get current progress to determine which riddle to give
        int currentStep = GetCurrentSequenceStep();
        
        string hint = "🧩 RIDDLE GUIDANCE\n\n";
        
        if (currentStep < colorRiddles.Length)
        {
            ColorRiddle riddle = colorRiddles[currentStep];
            hint += $"For your next step, ponder this riddle:\n\n";
            hint += $"💭 \"{riddle.riddle}\"\n\n";
            hint += $"This riddle points to the {currentStep + 1} element in the sequence.";
        }
        else
        {
            hint += "You have received all available riddles.\n";
            hint += "Review the clues you've discovered to complete the sequence.";
        }
        
        return hint;
    }

    private string GetSequenceHint()
    {
        string hint = "📊 SEQUENCE ANALYSIS\n\n";
        
        // Check discovered environmental clues
        EnvironmentalColorClue[] clues = FindObjectsByType<EnvironmentalColorClue>(FindObjectsSortMode.None);
        
        hint += "Discovered color elements:\n";
        
        int discoveredCount = 0;
        for (int i = 0; i < clues.Length; i++)
        {
            if (clues[i].IsDiscovered())
            {
                hint += $"• Position {clues[i].GetSequencePosition() + 1}: {clues[i].GetClueColor()}\n";
                discoveredCount++;
            }
        }
        
        if (discoveredCount == 0)
        {
            hint += "No elements discovered yet. Explore the chamber more thoroughly.\n";
        }
        else if (discoveredCount < clues.Length)
        {
            hint += $"\n💡 You've found {discoveredCount} of {clues.Length} elements.\n";
            hint += "Continue searching for the remaining color clues.";
        }
        else
        {
            hint += "\n✅ All color elements discovered!\n";
            hint += "Use the sequence revealed by their positions.";
        }
        
        return hint;
    }

    private string GetProgressHint()
    {
        // This would require access to the current player sequence from colorPuzzle
        string hint = "📈 PROGRESS REPORT\n\n";
        
        if (colorPuzzle != null)
        {
            // Call a method to get current progress
            int totalSteps = colorPuzzle.correctSequence.Length;
            
            hint += $"Sequence length: {totalSteps} colors\n";
            hint += "Current status: Analyzing input patterns...\n\n";
            
            hint += "💡 Remember:\n";
            hint += "• Each button press should match the environmental clues\n";
            hint += "• Wrong sequences will reset your progress\n";
            hint += "• Green flash = success, Red flash = failure\n";
            hint += "• The order must match the discovered sequence exactly";
        }
        
        return hint;
    }

    private string GetGeneralHint()
    {
        return "🤖 AI ASSISTANCE AVAILABLE\n\n" +
               "I can provide different levels of guidance:\n\n" +
               "Level 1: Environmental awareness\n" +
               "Level 2: Cryptic riddles\n" +
               "Level 3: Sequence analysis\n" +
               "Level 4: Progress tracking\n\n" +
               "What type of assistance do you require?";
    }

    private int GetCurrentSequenceStep()
    {
        // This would need to be implemented based on how colorPuzzle tracks progress
        // For now, return 0 as placeholder
        return 0;
    }

    // Method for the terminal to check if color puzzle is active
    public bool IsColorPuzzleActive()
    {
        return colorPuzzle != null && colorPuzzle.gameObject.activeInHierarchy;
    }

    // Method to provide contextual terminal messages
    public string GetContextualMessage()
    {
        if (!IsColorPuzzleActive())
            return "Color sequence system offline.";

        EnvironmentalColorClue[] clues = FindObjectsByType<EnvironmentalColorClue>(FindObjectsSortMode.None);
        int discoveredClues = 0;
        
        foreach (var clue in clues)
        {
            if (clue.IsDiscovered())
                discoveredClues++;
        }

        if (discoveredClues == 0)
        {
            return "🔍 Color sequence puzzle detected. Begin by exploring the chamber for color-coded elements.";
        }
        else if (discoveredClues < clues.Length)
        {
            return $"📊 Progress: {discoveredClues}/{clues.Length} color elements discovered. Continue searching.";
        }
        else
        {
            return "✅ All color elements located. Input the sequence using the illuminated buttons.";
        }
    }

    // Integration methods for the terminal to call
    public void ProvideHint(int level)
    {
        if (colorPuzzle != null)
        {
            colorPuzzle.ProvideHint(level);
        }
    }

    public void ShowEnvironmentalClueHint()
    {
        if (colorPuzzle != null)
        {
            string hint = colorPuzzle.GetEnvironmentalClueHint();
            colorPuzzle.ProvideFeedback(hint);
        }
    }

    // Debug methods
    [ContextMenu("🔍 Test Riddle System")]
    private void TestRiddleSystem()
    {
        for (int i = 0; i < colorRiddles.Length; i++)
        {
            Debug.Log($"Riddle {i + 1} ({colorRiddles[i].color}): {colorRiddles[i].riddle}");
        }
    }

    [ContextMenu("💡 Test All Hint Levels")]
    private void TestAllHintLevels()
    {
        for (int i = 1; i <= 4; i++)
        {
            string hint = RequestColorPuzzleHint(i);
            Debug.Log($"Hint Level {i}:\n{hint}\n");
        }
    }
}