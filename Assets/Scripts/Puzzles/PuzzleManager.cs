using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages all puzzles in the escape room game - Updated for new system
/// </summary>
public class PuzzleManager : MonoBehaviour
{
    [Header("Puzzle References")]
    [Tooltip("List of all puzzles in the game")]
    public List<BasePuzzle> puzzles = new List<BasePuzzle>();

    [Header("System References")]
    [Tooltip("Reference to the new AI Terminal Controller")]
    public TerminalControllerNew terminalController;
    
    [Tooltip("Reference to the Escape Code Manager")]
    public EscapeCodeManager escapeCodeManager;

    [Header("Auto-Discovery")]
    [Tooltip("Automatically find puzzles in the scene on start")]
    public bool autoDiscoverPuzzles = true;

    private int currentPuzzleIndex = -1;

    /// <summary>
    /// Initialize the puzzle manager
    /// </summary>
    void Start()
    {
        if (autoDiscoverPuzzles)
        {
            DiscoverPuzzlesInScene();
        }

        // Initialize all puzzles
        foreach (BasePuzzle puzzle in puzzles)
        {
            if (puzzle != null)
            {
                puzzle.Initialize();
                puzzle.onPuzzleCompleted.AddListener(() => OnPuzzleCompleted(puzzle));
            }
        }

        // Find system components if not assigned
        if (terminalController == null)
            terminalController = FindObjectOfType<TerminalControllerNew>();
            
        if (escapeCodeManager == null)
            escapeCodeManager = FindObjectOfType<EscapeCodeManager>();

        Debug.Log($"PuzzleManager initialized with {puzzles.Count} puzzles");
    }

    /// <summary>
    /// Automatically discover puzzles in the scene
    /// </summary>
    private void DiscoverPuzzlesInScene()
    {
        // Find all puzzle types
        CipherWheelPuzzle[] cipherPuzzles = FindObjectsOfType<CipherWheelPuzzle>();
        FrequencyResonancePuzzle[] frequencyPuzzles = FindObjectsOfType<FrequencyResonancePuzzle>();
        ShadowLogicPuzzle[] shadowPuzzles = FindObjectsOfType<ShadowLogicPuzzle>();

        // Add them to the list if not already present
        foreach (var puzzle in cipherPuzzles)
        {
            if (!puzzles.Contains(puzzle))
                puzzles.Add(puzzle);
        }
        
        foreach (var puzzle in frequencyPuzzles)
        {
            if (!puzzles.Contains(puzzle))
                puzzles.Add(puzzle);
        }
        
        foreach (var puzzle in shadowPuzzles)
        {
            if (!puzzles.Contains(puzzle))
                puzzles.Add(puzzle);
        }

        Debug.Log($"Auto-discovered {puzzles.Count} puzzles in scene");
    }

    /// <summary>
    /// Called when a puzzle is completed
    /// </summary>
    private void OnPuzzleCompleted(BasePuzzle completedPuzzle)
    {
        Debug.Log($"Puzzle completed: {completedPuzzle.puzzleName}");

        // Update the terminal to show this puzzle as solved
        if (terminalController != null)
        {
            terminalController.OnPuzzleSolved();
        }

        // Check if all puzzles are completed
        CheckGameCompletion();
    }

    /// <summary>
    /// Check if all puzzles are completed
    /// </summary>
    private void CheckGameCompletion()
    {
        int completedCount = 0;
        foreach (BasePuzzle puzzle in puzzles)
        {
            if (puzzle != null && puzzle.IsCompleted())
            {
                completedCount++;
            }
        }

        Debug.Log($"Puzzles completed: {completedCount}/{puzzles.Count}");

        if (completedCount >= puzzles.Count && escapeCodeManager != null)
        {
            Debug.Log("All puzzles completed! Escape code should be ready.");
        }
    }

    /// <summary>
    /// Show a specific puzzle by index
    /// </summary>
    /// <param name="index">Index of the puzzle to show</param>
    public void ShowPuzzle(int index)
    {
        // Hide the current puzzle if one is active
        if (currentPuzzleIndex >= 0 && currentPuzzleIndex < puzzles.Count)
        {
            puzzles[currentPuzzleIndex].HidePuzzle();
        }

        // Show the new puzzle
        if (index >= 0 && index < puzzles.Count)
        {
            currentPuzzleIndex = index;
            puzzles[currentPuzzleIndex].ShowPuzzle();
            Debug.Log($"Showing puzzle: {puzzles[currentPuzzleIndex].puzzleName}");
        }
        else
        {
            Debug.LogError($"Invalid puzzle index: {index}");
        }
    }

    /// <summary>
    /// Hide the current puzzle
    /// </summary>
    public void HideCurrentPuzzle()
    {
        if (currentPuzzleIndex >= 0 && currentPuzzleIndex < puzzles.Count)
        {
            puzzles[currentPuzzleIndex].HidePuzzle();
            currentPuzzleIndex = -1;
            Debug.Log("Hid current puzzle");
        }
    }

    /// <summary>
    /// Check if a puzzle is completed
    /// </summary>
    /// <param name="index">Index of the puzzle to check</param>
    /// <returns>True if the puzzle is completed</returns>
    public bool IsPuzzleCompleted(int index)
    {
        if (index >= 0 && index < puzzles.Count)
        {
            return puzzles[index].IsCompleted();
        }
        return false;
    }

    /// <summary>
    /// Get the name of a puzzle
    /// </summary>
    /// <param name="index">Index of the puzzle</param>
    /// <returns>Name of the puzzle</returns>
    public string GetPuzzleName(int index)
    {
        if (index >= 0 && index < puzzles.Count)
        {
            return puzzles[index].puzzleName;
        }
        return "Unknown Puzzle";
    }

    /// <summary>
    /// Get the description of a puzzle
    /// </summary>
    /// <param name="index">Index of the puzzle</param>
    /// <returns>Description of the puzzle</returns>
    public string GetPuzzleDescription(int index)
    {
        if (index >= 0 && index < puzzles.Count)
        {
            return puzzles[index].puzzleDescription;
        }
        return "No description available";
    }

    /// <summary>
    /// Reset all puzzles
    /// </summary>
    public void ResetAllPuzzles()
    {
        foreach (BasePuzzle puzzle in puzzles)
        {
            if (puzzle != null)
            {
                puzzle.ResetPuzzle();
            }
        }
        
        currentPuzzleIndex = -1;
        
        // Reset the escape code manager too
        if (escapeCodeManager != null)
        {
            escapeCodeManager.ResetEscapeProgress();
        }
        
        Debug.Log("All puzzles reset");
    }

    /// <summary>
    /// Get total number of puzzles
    /// </summary>
    public int GetTotalPuzzleCount()
    {
        return puzzles.Count;
    }

    /// <summary>
    /// Get number of completed puzzles
    /// </summary>
    public int GetCompletedPuzzleCount()
    {
        int count = 0;
        foreach (BasePuzzle puzzle in puzzles)
        {
            if (puzzle != null && puzzle.IsCompleted())
                count++;
        }
        return count;
    }
}
