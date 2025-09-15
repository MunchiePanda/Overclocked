using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PowerGridPuzzle : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public int[] correctSequence = { 0, 1, 2, 3, 4 }; // Default sequence: 0, 1, 2, 3, 4
    public float resetDelay = 1f; // Delay before resetting after failure

    [Header("Feedback")]
    public UnityEvent onPuzzleSuccess;
    public UnityEvent onPuzzleFailure;

    private List<int> playerSequence = new List<int>();
    private PuzzleButton[] buttons;

    void Start()
    {
        buttons = FindObjectsByType<PuzzleButton>(FindObjectsSortMode.None);
    }

    public void OnButtonPressed(int buttonId)
    {
        playerSequence.Add(buttonId);
        Debug.Log($"Button {buttonId} added to sequence. Current sequence: {string.Join(", ", playerSequence)}");
        CheckSequence();
    }

    private void CheckSequence()
    {
        // Check if the player's sequence matches the correct sequence so far
        for (int i = 0; i < playerSequence.Count; i++)
        {
            if (playerSequence[i] != correctSequence[i])
            {
                // Incorrect sequence: trigger failure
                Debug.Log($"Incorrect sequence! Expected: {string.Join(", ", correctSequence)}, Got: {string.Join(", ", playerSequence)}");
                Invoke(nameof(ResetPuzzle), resetDelay);
                onPuzzleFailure?.Invoke();
                return;
            }
        }

        // Check if the player has completed the sequence
        if (playerSequence.Count == correctSequence.Length)
        {
            // Correct sequence: trigger success
            Debug.Log($"Correct sequence completed! {string.Join(", ", playerSequence)}");
            onPuzzleSuccess?.Invoke();
        }
    }

    private void ResetPuzzle()
    {
        playerSequence.Clear();
        foreach (PuzzleButton button in buttons)
        {
            button.ResetButton();
        }
    }
}
