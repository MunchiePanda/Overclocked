using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base class for all puzzles in the game
/// </summary>
public abstract class BasePuzzle : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [Tooltip("Name of the puzzle")]
    public string puzzleName = "New Puzzle";

    [Tooltip("Description of the puzzle")]
    [TextArea(3, 5)]
    public string puzzleDescription = "";

    [Tooltip("Is this puzzle completed?")]
    public bool isCompleted = false;

    [Tooltip("Event triggered when the puzzle is completed")]
    public UnityEvent onPuzzleCompleted;

    [Header("UI References")]
    [Tooltip("UI panel for this puzzle")]
    public GameObject puzzleUI;

    [Tooltip("Text component to display the puzzle description")]
    public TMPro.TMP_Text descriptionText;

    [Tooltip("Text component to display feedback to the player")]
    public TMPro.TMP_Text feedbackText;

    /// <summary>
    /// Initialize the puzzle
    /// </summary>
    public virtual void Initialize()
    {
        if (puzzleUI != null)
        {
            puzzleUI.SetActive(false);
        }

        if (descriptionText != null && !string.IsNullOrEmpty(puzzleDescription))
        {
            descriptionText.text = puzzleDescription;
        }
    }

    /// <summary>
    /// Show the puzzle UI
    /// </summary>
    public virtual void ShowPuzzle()
    {
        if (puzzleUI != null)
        {
            puzzleUI.SetActive(true);
            
            // Fix scale if it's zero (common issue with UI canvases)
            Transform puzzleTransform = puzzleUI.transform;
            if (puzzleTransform.localScale == Vector3.zero)
            {
                puzzleTransform.localScale = Vector3.one;
                Debug.Log($"Fixed zero scale for puzzle UI: {puzzleName}");
            }
            
            Debug.Log("Showing puzzle: " + puzzleName);
        }

        // Reset the puzzle state when shown
        ResetPuzzle();
    }

    /// <summary>
    /// Hide the puzzle UI
    /// </summary>
    public virtual void HidePuzzle()
    {
        if (puzzleUI != null)
        {
            puzzleUI.SetActive(false);
            Debug.Log("Hiding puzzle: " + puzzleName);
        }
    }

    /// <summary>
    /// Reset the puzzle to its initial state
    /// </summary>
    public virtual void ResetPuzzle()
    {
        isCompleted = false;
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }
        Debug.Log("Resetting puzzle: " + puzzleName);
    }

    /// <summary>
    /// Check if the puzzle is completed
    /// </summary>
    /// <returns>True if the puzzle is completed</returns>
    public virtual bool IsCompleted()
    {
        return isCompleted;
    }

    /// <summary>
    /// Complete the puzzle
    /// </summary>
    public virtual void CompletePuzzle()
    {
        isCompleted = true;
        if (feedbackText != null)
        {
            feedbackText.text = "Puzzle Completed!";
        }

        Debug.Log("Puzzle completed: " + puzzleName);
        onPuzzleCompleted.Invoke();
    }

    /// <summary>
    /// Provide feedback to the player
    /// </summary>
    /// <param name="message">The feedback message</param>
    protected void GiveFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
        Debug.Log("Puzzle feedback: " + message);
    }
}
