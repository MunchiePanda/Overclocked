using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages all puzzles in the game
/// </summary>
public class PuzzleManager : MonoBehaviour
{
    [Tooltip("List of all puzzles in the game")]
    public List<BasePuzzle> puzzles = new List<BasePuzzle>();

    [Tooltip("Index of the current active puzzle")]
    private int currentPuzzleIndex = -1;

    [Tooltip("Reference to the TerminalController")]
    public TerminalController terminalController;

    /// <summary>
    /// Initialize the puzzle manager
    /// </summary>
    void Start()
    {
        // Initialize all puzzles
        foreach (BasePuzzle puzzle in puzzles)
        {
            if (puzzle != null)
            {
                puzzle.Initialize();
                puzzle.onPuzzleCompleted.AddListener(OnPuzzleCompleted);
            }
        }

        Debug.Log("PuzzleManager initialized with " + puzzles.Count + " puzzles");
    }

    /// <summary>
    /// Called when a puzzle is completed
    /// </summary>
    private void OnPuzzleCompleted()
    {
        Debug.Log("Puzzle completed! Updating terminal...");

        if (terminalController != null)
        {
            // Update the terminal to reflect the completed puzzle
            // This will be implemented based on your terminal system
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
            Debug.Log("Showing puzzle: " + puzzles[currentPuzzleIndex].puzzleName);
        }
        else
        {
            Debug.LogError("Invalid puzzle index: " + index);
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
        Debug.Log("All puzzles reset");
    }
}
