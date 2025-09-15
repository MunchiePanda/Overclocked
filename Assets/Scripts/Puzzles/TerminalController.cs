using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class TerminalController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject terminalUI;          // The root UI panel
    public TMP_Text hintText;             // Text display for hints
    public Button button1;                // Button for puzzle 1
    public Button button2;                // Button for puzzle 2
    public Button button3;                // Button for puzzle 3
    public TMP_Text buttonText1;          // Text for button 1
    public TMP_Text buttonText2;          // Text for button 2
    public TMP_Text buttonText3;          // Text for button 3
    public Button closeButton;            // Button to close the terminal
    public TMP_Text terminalHeader;       // Header text for the terminal
    public TMP_Text instructionText;      // Instruction text

    [Header("Puzzle Settings")]
    public List<string> puzzleNames = new List<string>
    {
        "Main Reactor Puzzle",
        "Security Door Puzzle",
        "Navigation System Puzzle"
    };

    public List<string> puzzleHints = new List<string>
    {
        "MAIN REACTOR PUZZLE:\n\nTo stabilize the reactor core, you must align the three control rods in the correct sequence. " +
        "First, locate the control panel in the reactor room. The rods are color-coded: Red, Blue, and Green. " +
        "The correct sequence is Green, Red, Blue. Insert them in this order, but beware: inserting them incorrectly " +
        "will trigger a 30-second lockdown. You can find the sequence hint on the wall near the reactor entrance, " +
        "but it's partially obscured by steam. Use the environmental controls to clear the steam for a better view.\n\n" +
        "Press ENTER when you've completed this puzzle.",

        "SECURITY DOOR PUZZLE:\n\nThe security door requires a 4-digit access code. This code changes daily, " +
        "but you can find today's code by examining the security logs in the main office. Look for the document " +
        "labeled 'Access Codes - Current Month' on the desk near the terminal. The code is written in red ink. " +
        "If you can't find it, check the whiteboard in the break room, where someone might have written it down. " +
        "Remember, entering the wrong code three times will lock the system for 5 minutes.\n\n" +
        "Press ENTER when you've completed this puzzle.",

        "NAVIGATION SYSTEM PUZZLE:\n\nTo recalibrate the navigation system, you need to input the correct " +
        "stellar coordinates. These coordinates can be found in the captain's log, located in the bridge terminal. " +
        "However, the log is encrypted. To decrypt it, you'll need to find the decryption key stored in the engineering " +
        "database. The key is a 6-digit alphanumeric code labeled 'NAV_KEY'. Once decrypted, the coordinates will " +
        "appear as three sets of numbers (e.g., 45.78, 12.34, 78.90). Input these in the navigation console in the same " +
        "order they appear in the log.\n\n" +
        "Press ENTER when you've completed this puzzle."
    };

    [Header("Terminal Settings")]
    public float typingSpeed = 0.03f;       // Speed for the typing effect
    public AudioClip buttonPressSound;    // Sound for button presses
    public AudioClip terminalOpenSound;   // Sound for terminal opening
    public AudioClip terminalCloseSound;  // Sound for terminal closing
    public AudioClip puzzleCompleteSound; // Sound for puzzle completion

    [Header("Camera Settings")]
    public Camera mainCamera;             // Main game camera
    public Camera terminalCamera;         // Camera for terminal view

    private AudioSource audioSource;
    private bool isTerminalActive = false;
    private Coroutine typingCoroutine;
    private int currentPuzzleIndex = -1;
    private bool[] puzzlesCompleted = new bool[3]; // Track which puzzles are completed

    void Start()
    {
        // Initialize the terminal UI
        if (terminalUI != null)
            terminalUI.SetActive(false);

        // Set up button texts
        UpdateButtonTexts();

        // Set up button listeners
        if (button1 != null) button1.onClick.AddListener(() => OnPuzzleButtonClicked(0));
        if (button2 != null) button2.onClick.AddListener(() => OnPuzzleButtonClicked(1));
        if (button3 != null) button3.onClick.AddListener(() => OnPuzzleButtonClicked(2));
        if (closeButton != null) closeButton.onClick.AddListener(ToggleTerminal);

        // Set up audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Initialize camera settings
        if (mainCamera != null) mainCamera.enabled = true;
        if (terminalCamera != null) terminalCamera.enabled = false;
    }

    void Update()
    {
        // Close terminal when pressing Escape
        if (isTerminalActive && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleTerminal();
        }

        // Handle keyboard input for puzzle selection
        if (isTerminalActive)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                OnPuzzleButtonClicked(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                OnPuzzleButtonClicked(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                OnPuzzleButtonClicked(2);
        }
    }

    public void ToggleTerminal()
    {
        isTerminalActive = !isTerminalActive;
        terminalUI.SetActive(isTerminalActive);

        if (isTerminalActive)
        {
            // Open terminal
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Switch cameras
            if (mainCamera != null) mainCamera.enabled = false;
            if (terminalCamera != null) terminalCamera.enabled = true;

            if (terminalOpenSound != null)
                audioSource.PlayOneShot(terminalOpenSound);

            // Clear any existing text
            if (hintText != null)
                hintText.text = "";

            // Set header text
            if (terminalHeader != null)
                terminalHeader.text = "COMPUTER TERMINAL - PUZZLE ASSISTANCE SYSTEM";

            // Set instruction text
            if (instructionText != null)
                instructionText.text = "Select a puzzle (1-3) for assistance. Press ESC to exit.";
        }
        else
        {
            // Close terminal
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Switch cameras
            if (mainCamera != null) mainCamera.enabled = true;
            if (terminalCamera != null) terminalCamera.enabled = false;

            if (terminalCloseSound != null)
                audioSource.PlayOneShot(terminalCloseSound);

            // Stop any typing animation
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            // Reset current puzzle selection
            currentPuzzleIndex = -1;
        }
    }

    public bool IsTerminalActive()
    {
        return isTerminalActive;
    }

    public void OnPuzzleButtonClicked(int puzzleIndex)
    {
        if (puzzleIndex >= 0 && puzzleIndex < puzzleHints.Count)
        {
            // Play button sound
            if (buttonPressSound != null)
                audioSource.PlayOneShot(buttonPressSound);

            // Update current puzzle index
            currentPuzzleIndex = puzzleIndex;

            // Start typing effect
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            // Check if puzzle is already completed
            if (puzzlesCompleted[puzzleIndex])
            {
                typingCoroutine = StartCoroutine(TypeText("PUZZLE COMPLETED:\n\nYou have already solved this puzzle. Select another puzzle or press ESC to exit."));
            }
            else
            {
                typingCoroutine = StartCoroutine(TypeText(puzzleHints[puzzleIndex]));
            }
        }
    }

    public void OnPuzzleSolved()
    {
        if (currentPuzzleIndex >= 0 && currentPuzzleIndex < puzzlesCompleted.Length)
        {
            puzzlesCompleted[currentPuzzleIndex] = true;
            UpdateButtonTexts();

            if (puzzleCompleteSound != null)
                audioSource.PlayOneShot(puzzleCompleteSound);

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            typingCoroutine = StartCoroutine(TypeText("PUZZLE COMPLETION CONFIRMED:\n\nThis puzzle has been marked as solved. The terminal will no longer provide hints for this puzzle.\n\nPress ESC to exit or select another puzzle."));
        }
    }

    private void UpdateButtonTexts()
    {
        if (buttonText1 != null && puzzleNames.Count > 0)
            buttonText1.text = "1. " + puzzleNames[0] + (puzzlesCompleted[0] ? " [SOLVED]" : "");

        if (buttonText2 != null && puzzleNames.Count > 1)
            buttonText2.text = "2. " + puzzleNames[1] + (puzzlesCompleted[1] ? " [SOLVED]" : "");

        if (buttonText3 != null && puzzleNames.Count > 2)
            buttonText3.text = "3. " + puzzleNames[2] + (puzzlesCompleted[2] ? " [SOLVED]" : "");
    }

    private string GetPuzzleHint(int puzzleIndex)
    {
        if (puzzleIndex >= 0 && puzzleIndex < puzzleHints.Count)
        {
            return puzzleHints[puzzleIndex];
        }
        return "No hint available for this puzzle.";
    }

    private System.Collections.IEnumerator TypeText(string text)
    {
        if (hintText == null) yield break;

        hintText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            hintText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
