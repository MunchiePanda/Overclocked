using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages all terminals in the escape room and their interactions
/// </summary>
public class TerminalSystemManager : MonoBehaviour
{
    [Header("Terminal System")]
    [Tooltip("Main AI terminal that provides hints")]
    public InteractableAITerminalNew mainAITerminal;
    
    [Tooltip("All puzzle terminals in the room")]
    public List<PuzzleTerminal> puzzleTerminals = new List<PuzzleTerminal>();
    
    [Header("UI Feedback")]
    [Tooltip("UI element to show current terminal status")]
    public TMPro.TMP_Text statusText;
    
    [Tooltip("Sound to play when accessing any terminal")]
    public AudioClip terminalAccessSound;

    private static TerminalSystemManager instance;
    public static TerminalSystemManager Instance => instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeTerminals();
    }

    void InitializeTerminals()
    {
        // Find main AI terminal if not assigned
        if (mainAITerminal == null)
            mainAITerminal = FindFirstObjectByType<InteractableAITerminalNew>();

        // Find all puzzle terminals
        PuzzleTerminal[] foundPuzzleTerminals = FindObjectsByType<PuzzleTerminal>(FindObjectsSortMode.None);
        foreach (PuzzleTerminal terminal in foundPuzzleTerminals)
        {
            if (!puzzleTerminals.Contains(terminal))
                puzzleTerminals.Add(terminal);
        }

        Debug.Log($"Terminal System initialized: 1 main terminal, {puzzleTerminals.Count} puzzle terminals");
    }

    public void OnTerminalAccessed(string terminalName, string terminalType)
    {
        if (statusText != null)
        {
            statusText.text = $"Accessing: {terminalName}";
        }

        Debug.Log($"Terminal accessed: {terminalName} ({terminalType})");
    }

    public void OnTerminalClosed(string terminalName)
    {
        if (statusText != null)
        {
            statusText.text = "";
        }

        Debug.Log($"Terminal closed: {terminalName}");
    }

    public void PlayTerminalSound()
    {
        if (terminalAccessSound != null)
        {
            AudioSource.PlayClipAtPoint(terminalAccessSound, Camera.main.transform.position);
        }
    }

    // Helper methods for other scripts
    public bool IsAnyTerminalOpen()
    {
        if (mainAITerminal != null && mainAITerminal.IsTerminalOpen())
            return true;

        foreach (PuzzleTerminal terminal in puzzleTerminals)
        {
            if (terminal != null && terminal.IsTerminalOpen())
                return true;
        }

        return false;
    }

    public void CloseAllTerminals()
    {
        if (mainAITerminal != null && mainAITerminal.IsTerminalOpen())
        {
            mainAITerminal.OnInteract();
        }

        foreach (PuzzleTerminal terminal in puzzleTerminals)
        {
            if (terminal != null && terminal.IsTerminalOpen())
            {
                terminal.OnInteract();
            }
        }
    }

    [ContextMenu("Clean Up All Terminals")]
    public void CleanUpAllTerminals()
    {
        // Remove duplicate components from main terminal
        if (mainAITerminal != null)
        {
            InteractableAITerminal oldComponent = mainAITerminal.GetComponent<InteractableAITerminal>();
            if (oldComponent != null)
            {
                DestroyImmediate(oldComponent);
                Debug.Log("Removed old InteractableAITerminal component from main terminal");
            }
        }

        Debug.Log("Terminal cleanup complete");
    }
}