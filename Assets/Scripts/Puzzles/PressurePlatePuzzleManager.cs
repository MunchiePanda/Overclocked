using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the overall state of the pressure plate puzzle
/// </summary>
public class PressurePlatePuzzleManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [Tooltip("List of pressure plates in the puzzle")]
    public List<PressurePlate> pressurePlates = new List<PressurePlate>();

    [Tooltip("The code display object")]
    public CodeDisplay codeDisplay;

    [Tooltip("Should the puzzle reset when all plates are activated?")]
    public bool resetOnCompletion = false;

    [Tooltip("Delay before resetting the puzzle (seconds)")]
    public float resetDelay = 3f;

    [Header("Debug")]
    [Tooltip("Show debug messages in console")]
    public bool debugMode = true;

    private bool isPuzzleComplete = false;

    private void Start()
    {
        // Initialize the puzzle
        InitializePuzzle();
    }

    /// <summary>
    /// Initialize the puzzle
    /// </summary>
    private void InitializePuzzle()
    {
        // Set up event listeners for each pressure plate
        foreach (PressurePlate plate in pressurePlates)
        {
            if (plate != null)
            {
                plate.onActivate.AddListener(OnPlateActivated);
                plate.onDeactivate.AddListener(OnPlateDeactivated);
            }
        }

        // Hide the code display initially
        if (codeDisplay != null)
        {
            codeDisplay.HideCode();
        }

        Log("Pressure plate puzzle initialized");
    }

    /// <summary>
    /// Called when a pressure plate is activated
    /// </summary>
    public void OnPlateActivated()
    {
        CheckPuzzleCompletion();
    }

    /// <summary>
    /// Called when a pressure plate is deactivated
    /// </summary>
    public void OnPlateDeactivated()
    {
        // If any plate is deactivated, hide the code
        if (codeDisplay != null && !isPuzzleComplete)
        {
            codeDisplay.HideCode();
        }
    }

    /// <summary>
    /// Check if all pressure plates are activated
    /// </summary>
    private void CheckPuzzleCompletion()
    {
        // Check if all plates are active
        foreach (PressurePlate plate in pressurePlates)
        {
            if (plate != null && !plate.isActive)
            {
                // If any plate is inactive, the puzzle is not complete
                return;
            }
        }

        // All plates are active - puzzle is complete
        if (!isPuzzleComplete)
        {
            isPuzzleComplete = true;
            Log("All pressure plates activated! Puzzle complete.");

            // Show the code
            if (codeDisplay != null)
            {
                codeDisplay.ShowCode();
            }

            // Reset the puzzle if needed
            if (resetOnCompletion)
            {
                Invoke("ResetPuzzle", resetDelay);
            }
        }
    }

    /// <summary>
    /// Reset the puzzle to its initial state
    /// </summary>
    public void ResetPuzzle()
    {
        Log("Resetting pressure plate puzzle");

        // Reset all pressure plates
        foreach (PressurePlate plate in pressurePlates)
        {
            if (plate != null)
            {
                plate.ResetPlate();
            }
        }

        // Hide the code display
        if (codeDisplay != null)
        {
            codeDisplay.HideCode();
        }

        // Reset puzzle state
        isPuzzleComplete = false;
    }

    /// <summary>
    /// Log a message if debug mode is enabled
    /// </summary>
    /// <param name="message">The message to log</param>
    private void Log(string message)
    {
        if (debugMode)
        {
            Debug.Log("[PressurePlatePuzzle] " + message);
        }
    }
}
