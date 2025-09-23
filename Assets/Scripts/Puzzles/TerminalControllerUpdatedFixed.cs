using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class TerminalControllerUpdatedFixed : MonoBehaviour
{
    [Header("UI References")]
    public GameObject terminalUI;
    public TMP_Text hintText;
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public TMP_Text buttonText1;
    public TMP_Text buttonText2;
    public TMP_Text buttonText3;
    public TMP_Text buttonText4;
    public Button closeButton;
    public TMP_Text terminalHeader;
    public TMP_Text instructionText;

    [Header("Puzzle Settings")]
    public List<string> puzzleNames = new List<string>
    {
        "Riddle Challenge",
        "Cipher Wheel Puzzle", 
        "Color Light Puzzle",
        "Advanced Terminal Access"
    };

    public List<string> puzzleHints = new List<string>
    {
        "RIDDLE CHALLENGE ANALYSIS:\\n\\nAh, the seeker of words approaches the altar of wit! These ancient " +
        "puzzles have confounded minds for millennia. But fear not, for I shall provide... assistance... though " +
        "perhaps not in the form you expect. The AI terminal seems to be malfunctioning when processing riddle " +
        "queries - it outputs complete nonsense! However, legend says the answers hide within the chaos. Use the " +
        "terminal during your riddle solving for 'helpful' responses that may contain more than meets the eye...\\n\\n" +
        "Seek the wisdom hidden in digital madness, decode the chaos, unlock the verbal mystery.",

        "CIPHER WHEEL ANALYSIS:\\n\\nAncient scribes understood that truth lies not in what is seen, but in the " +
        "relationship between observer and observed. When celestial bodies align with terrestrial markers, hidden " +
        "meanings emerge from chaos. Seek the symbols that dance in shadows throughout this facility, for they hold " +
        "keys to doors unopened. Remember: the wheel of knowledge turns, but only when the seeker finds the proper " +
        "perspective. The alignment of inner and outer rings reveals the cipher that transforms mystery into clarity. " +
        "Three sequences await discovery, scattered like stars across the room's horizon.\\n\\n" +
        "Query the wheel, decode the patterns, unlock the first fragment of your liberation.",

        "COLOR LIGHT ANALYSIS:\\n\\nThe spectrum holds more than beauty - it holds the frequencies of freedom! " +
        "Each hue vibrates with meaning, and when properly combined, they sing the song of your escape. Red, Green, " +
        "Blue - the trinity of digital truth. But beware, for the sequence matters as much as the selection. The " +
        "ancients knew that color is but wavelength, and wavelength is but energy, and energy unlocks all doors. " +
        "Seven combinations exist, but only one holds the key. Watch for the patterns, feel for the rhythm, and " +
        "let the colors guide you to enlightenment.\\n\\n" +
        "Master the spectrum, align the frequencies, illuminate the path to freedom.",

        "ADVANCED ACCESS ANALYSIS:\\n\\nYou have proven yourself worthy of deeper mysteries. The path to ultimate " +
        "liberation requires more than puzzle-solving - it demands understanding of the very systems that bind you. " +
        "When all other challenges have fallen before your intellect, return here for the final revelation. The " +
        "door to freedom awaits those who have collected all fragments of truth.\\n\\n" +
        "Complete all puzzles, gather all keys, then seek the ultimate exit code."
    };

    [Header("Terminal Settings")]
    public float typingSpeed = 0.03f;
    public AudioClip buttonPressSound;
    public AudioClip terminalOpenSound;
    public AudioClip terminalCloseSound;
    public AudioClip puzzleCompleteSound;

    [Header("Camera Settings")]
    public Camera mainCamera;
    public Camera terminalCamera;

    [Header("Puzzle References")]
    [Tooltip("Reference to the Riddle Puzzle system")]
    public RiddlePuzzle riddlePuzzle;
    
    [Tooltip("Reference to the Cipher Puzzle system")]
    public ShapeCipherPuzzle cipherPuzzle;
    
    [Tooltip("Reference to the Color Light Puzzle system")]
    public ColorLightPuzzle colorLightPuzzle;

    private AudioSource audioSource;
    private bool isTerminalActive = false;
    private Coroutine typingCoroutine;
    private int currentPuzzleIndex = -1;
    private bool[] puzzlesCompleted = new bool[4];

    void Start()
    {
        if (terminalUI != null)
            terminalUI.SetActive(false);

        UpdateButtonTexts();
        SetupButtons();
        InitializeAudio();
        SetupCameras();
        FindPuzzleReferences();

        Debug.Log("🖥️ Updated AI Terminal initialized with new puzzle set");
    }

    void SetupButtons()
    {
        if (button1 != null) button1.onClick.AddListener(() => OnPuzzleButtonClicked(0));
        if (button2 != null) button2.onClick.AddListener(() => OnPuzzleButtonClicked(1));
        if (button3 != null) button3.onClick.AddListener(() => OnPuzzleButtonClicked(2));
        if (button4 != null) button4.onClick.AddListener(() => OnPuzzleButtonClicked(3));
        if (closeButton != null) closeButton.onClick.AddListener(ToggleTerminal);
    }

    void InitializeAudio()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void SetupCameras()
    {
        if (mainCamera != null) mainCamera.enabled = true;
        if (terminalCamera != null) terminalCamera.enabled = false;
    }

    public void FindPuzzleReferences()
    {
        // Auto-find puzzle references if not assigned
        if (riddlePuzzle == null)
            riddlePuzzle = FindFirstObjectByType<RiddlePuzzle>();
        
        if (cipherPuzzle == null)
            cipherPuzzle = FindFirstObjectByType<ShapeCipherPuzzle>();
        
        if (colorLightPuzzle == null)
            colorLightPuzzle = FindFirstObjectByType<ColorLightPuzzle>();

        Debug.Log($"🔍 Puzzle references found: Riddle={riddlePuzzle != null}, Cipher={cipherPuzzle != null}, ColorLight={colorLightPuzzle != null}");
    }

    void Update()
    {
        HandleInput();
        CheckPuzzleCompletion();
    }

    void HandleInput()
    {
        if (isTerminalActive && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleTerminal();
        }

        if (isTerminalActive)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
                OnPuzzleButtonClicked(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
                OnPuzzleButtonClicked(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
                OnPuzzleButtonClicked(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
                OnPuzzleButtonClicked(3);
        }
    }

    void CheckPuzzleCompletion()
    {
        // Check if puzzles are completed and update status
        bool wasUpdated = false;

        // Check Riddle Puzzle - using IsCompleted() from BasePuzzle
        if (riddlePuzzle != null && riddlePuzzle.IsCompleted() && !puzzlesCompleted[0])
        {
            puzzlesCompleted[0] = true;
            wasUpdated = true;
            Debug.Log("🧩 Riddle Puzzle completed!");
        }

        // Check Cipher Puzzle - using IsCompleted() from BasePuzzle
        if (cipherPuzzle != null && cipherPuzzle.IsCompleted() && !puzzlesCompleted[1])
        {
            puzzlesCompleted[1] = true;
            wasUpdated = true;
            Debug.Log("🔐 Cipher Puzzle completed!");
        }

        // Check Color Light Puzzle - using IsCompleted() from BasePuzzle
        if (colorLightPuzzle != null && colorLightPuzzle.IsCompleted() && !puzzlesCompleted[2])
        {
            puzzlesCompleted[2] = true;
            wasUpdated = true;
            Debug.Log("🌈 Color Light Puzzle completed!");
        }

        // Check if all puzzles are complete for advanced access
        if (puzzlesCompleted[0] && puzzlesCompleted[1] && puzzlesCompleted[2] && !puzzlesCompleted[3])
        {
            puzzlesCompleted[3] = true;
            wasUpdated = true;
            Debug.Log("🔓 All puzzles completed! Advanced terminal access unlocked!");
        }

        if (wasUpdated)
        {
            UpdateButtonTexts();
        }
    }

    public void ToggleTerminal()
    {
        isTerminalActive = !isTerminalActive;
        terminalUI.SetActive(isTerminalActive);

        if (isTerminalActive)
        {
            OpenTerminal();
        }
        else
        {
            CloseTerminal();
        }
    }

    void OpenTerminal()
    {
        // Use central cursor management
        PauseManager.RequestCursor("AITerminal", CursorLockMode.None, true, 85);

        if (mainCamera != null) mainCamera.enabled = false;
        if (terminalCamera != null) terminalCamera.enabled = true;

        if (terminalOpenSound != null)
            audioSource.PlayOneShot(terminalOpenSound);

        if (hintText != null)
            hintText.text = "";

        if (terminalHeader != null)
            terminalHeader.text = "ESCAPE ROOM AI ASSISTANT - PUZZLE ANALYSIS SYSTEM v2.0";

        if (instructionText != null)
            instructionText.text = "Select a puzzle (1-4) for cryptic guidance. The path to freedom requires interpretation.";

        Debug.Log("🖥️ AI Terminal opened");
    }

    void CloseTerminal()
    {
        // Release cursor control
        PauseManager.ReleaseCursor("AITerminal");

        if (mainCamera != null) mainCamera.enabled = true;
        if (terminalCamera != null) terminalCamera.enabled = false;

        if (terminalCloseSound != null)
            audioSource.PlayOneShot(terminalCloseSound);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        currentPuzzleIndex = -1;
        Debug.Log("🖥️ AI Terminal closed");
    }

    public bool IsTerminalActive()
    {
        return isTerminalActive;
    }

    public void OnPuzzleButtonClicked(int puzzleIndex)
    {
        if (puzzleIndex >= 0 && puzzleIndex < puzzleNames.Count)
        {
            if (buttonPressSound != null)
                audioSource.PlayOneShot(buttonPressSound);

            currentPuzzleIndex = puzzleIndex;

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            if (puzzlesCompleted[puzzleIndex])
            {
                string completedMessage = "PUZZLE ANALYSIS COMPLETE:\\n\\nThis challenge has been conquered. The fragment of your liberation has been claimed. ";
                
                if (puzzleIndex == 3) // Advanced Access
                {
                    completedMessage += "You have achieved mastery over all systems. The door to ultimate freedom awaits your command.";
                }
                else
                {
                    completedMessage += "Seek the remaining puzzles or proceed to the final threshold with your collected keys.";
                }

                typingCoroutine = StartCoroutine(TypeText(completedMessage));
            }
            else
            {
                // Special handling for different puzzles
                switch (puzzleIndex)
                {
                    case 0: // Riddle Challenge
                        HandleRiddlePuzzleRequest();
                        break;
                    case 1: // Cipher Puzzle
                        HandleCipherPuzzleRequest();
                        break;
                    case 2: // Color Light Puzzle
                        HandleColorLightPuzzleRequest();
                        break;
                    case 3: // Advanced Access
                        HandleAdvancedAccessRequest();
                        break;
                    default:
                        if (puzzleIndex < puzzleHints.Count)
                        {
                            typingCoroutine = StartCoroutine(TypeText(puzzleHints[puzzleIndex]));
                        }
                        break;
                }
            }
        }
    }

    void HandleRiddlePuzzleRequest()
    {
        if (riddlePuzzle != null && riddlePuzzle.IsRiddlePuzzleActive())
        {
            // Generate frustrating nonsense response that contains the answer
            string nonsenseResponse = riddlePuzzle.GetCurrentRiddleNonsenseResponse();
            typingCoroutine = StartCoroutine(TypeText(nonsenseResponse));
        }
        else
        {
            // Normal riddle hint when puzzle is not active
            typingCoroutine = StartCoroutine(TypeText(puzzleHints[0]));
        }
    }

    void HandleCipherPuzzleRequest()
    {
        if (cipherPuzzle != null)
        {
            // Add special cipher-specific hints if puzzle UI is active
            string cipherHint = puzzleHints[1];
            if (cipherPuzzle.puzzleUI != null && cipherPuzzle.puzzleUI.activeInHierarchy)
            {
                cipherHint += "\\n\\nCIPHER STATUS: The wheel awaits your touch. Symbols hold meaning beyond their form.";
            }
            typingCoroutine = StartCoroutine(TypeText(cipherHint));
        }
        else
        {
            typingCoroutine = StartCoroutine(TypeText(puzzleHints[1]));
        }
    }

    void HandleColorLightPuzzleRequest()
    {
        if (colorLightPuzzle != null)
        {
            // Add special color light hints if puzzle UI is active
            string colorHint = puzzleHints[2];
            if (colorLightPuzzle.puzzleUI != null && colorLightPuzzle.puzzleUI.activeInHierarchy)
            {
                colorHint += "\\n\\nCOLOR STATUS: The spectrum awaits alignment. Seven paths, one destination.";
            }
            typingCoroutine = StartCoroutine(TypeText(colorHint));
        }
        else
        {
            typingCoroutine = StartCoroutine(TypeText(puzzleHints[2]));
        }
    }

    void HandleAdvancedAccessRequest()
    {
        if (puzzlesCompleted[0] && puzzlesCompleted[1] && puzzlesCompleted[2])
        {
            string finalHint = puzzleHints[3] + "\\n\\nSYSTEM STATUS: All fragments collected. Door unlock sequence available.";
            typingCoroutine = StartCoroutine(TypeText(finalHint));
        }
        else
        {
            int remaining = 0;
            string missing = "";
            if (!puzzlesCompleted[0]) { remaining++; missing += "Riddles "; }
            if (!puzzlesCompleted[1]) { remaining++; missing += "Cipher "; }
            if (!puzzlesCompleted[2]) { remaining++; missing += "Colors "; }

            string lockedMessage = $"ACCESS DENIED:\\n\\nAdvanced terminal functions require completion of all primary puzzles. " +
                                 $"Remaining challenges: {remaining}\\n\\nIncomplete systems: {missing}\\n\\n" +
                                 "Return when all fragments have been gathered.";
            typingCoroutine = StartCoroutine(TypeText(lockedMessage));
        }
    }

    public void OnPuzzleSolved(int puzzleIndex = -1)
    {
        if (puzzleIndex == -1)
            puzzleIndex = currentPuzzleIndex;

        if (puzzleIndex >= 0 && puzzleIndex < puzzlesCompleted.Length)
        {
            puzzlesCompleted[puzzleIndex] = true;
            UpdateButtonTexts();

            if (puzzleCompleteSound != null)
                audioSource.PlayOneShot(puzzleCompleteSound);

            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            string completionMessage = "ANALYSIS CONFIRMED:\\n\\nYour mastery of this puzzle has been recorded. The fragment of escape has been added to your collection. ";
            
            if (puzzlesCompleted[0] && puzzlesCompleted[1] && puzzlesCompleted[2])
            {
                completionMessage += "ALL PRIMARY SYSTEMS CONQUERED! Advanced terminal access now available. Your liberation is at hand!";
            }
            else
            {
                completionMessage += "Continue your quest for liberation.";
            }

            typingCoroutine = StartCoroutine(TypeText(completionMessage));
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

        if (buttonText4 != null && puzzleNames.Count > 3)
            buttonText4.text = "4. " + puzzleNames[3] + (puzzlesCompleted[3] ? " [UNLOCKED]" : " [LOCKED]");
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

    // Public methods for external access
    public bool[] GetPuzzleCompletionStatus() => puzzlesCompleted;
    public bool IsAllPuzzlesCompleted() => puzzlesCompleted[0] && puzzlesCompleted[1] && puzzlesCompleted[2];
    public void ForceUpdatePuzzleStatus() => CheckPuzzleCompletion();

    // Debug methods
    [ContextMenu("🔓 Mark All Puzzles Solved")]
    public void DebugSolveAllPuzzles()
    {
        for (int i = 0; i < puzzlesCompleted.Length; i++)
        {
            puzzlesCompleted[i] = true;
        }
        UpdateButtonTexts();
        Debug.Log("🔓 All puzzles marked as solved (DEBUG)");
    }

    [ContextMenu("🔄 Reset All Puzzles")]
    public void DebugResetAllPuzzles()
    {
        for (int i = 0; i < puzzlesCompleted.Length; i++)
        {
            puzzlesCompleted[i] = false;
        }
        UpdateButtonTexts();
        Debug.Log("🔄 All puzzles reset (DEBUG)");
    }
}