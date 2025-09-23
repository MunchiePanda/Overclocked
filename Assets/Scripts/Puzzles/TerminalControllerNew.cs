using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class TerminalControllerNew : MonoBehaviour
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
        "CIPHER WHEEL ANALYSIS:\\n\\nAncient scribes understood that truth lies not in what is seen, but in the " +
        "relationship between observer and observed. When celestial bodies align with terrestrial markers, hidden " +
        "meanings emerge from chaos. Seek the symbols that dance in shadows throughout this facility, for they hold " +
        "keys to doors unopened. Remember: the wheel of knowledge turns, but only when the seeker finds the proper " +
        "perspective. The alignment of inner and outer rings reveals the cipher that transforms mystery into clarity. " +
        "Three sequences await discovery, scattered like stars across the room's horizon.\\n\\n" +
        "Query the wheel, decode the patterns, unlock the first fragment of your liberation.",

        "FREQUENCY RESONANCE ANALYSIS:\\n\\nHarmony emerges when disparate voices unite in purpose. The architects " +
        "of this place understood that matter vibrates at frequencies invisible to the naked eye. Listen not with " +
        "your ears alone, but with your understanding of the cosmic symphony. Some instruments must be tuned before " +
        "the orchestra can perform its destined melody. The pattern exists in waves that crash upon shores of " +
        "comprehension, each tone a note in the song of escape. Five voices must sing in sequence, but first they " +
        "must be tuned to resonate with the frequencies of liberation.\\n\\n" +
        "Align the harmonics, conduct the sequence, capture the second fragment of freedom.",

        "SHADOW LOGIC ANALYSIS:\\n\\nWhat is reality but the interplay of light and absence? Ancient philosophers " +
        "knew that truth often hides in the spaces between illumination. Position the sources of enlightenment such " +
        "that their absence creates presence. The answer you seek exists not in what is lit, but in what remains " +
        "dark when all elements align. Three suns must dance with earthbound forms to reveal what numbers guard the " +
        "final threshold. Intensity and angle, position and purpose - all must converge to cast the shadow of truth " +
        "upon the wall of understanding.\\n\\n" +
        "Master light and shadow, reveal the pattern, claim the final fragment of your escape."
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

    private AudioSource audioSource;
    private bool isTerminalActive = false;
    private Coroutine typingCoroutine;
    private int currentPuzzleIndex = -1;
    private bool[] puzzlesCompleted = new bool[4]; // Updated to support 4 puzzles

    void Start()
    {
        if (terminalUI != null)
            terminalUI.SetActive(false);

        UpdateButtonTexts();

        if (button1 != null) button1.onClick.AddListener(() => OnPuzzleButtonClicked(0));
        if (button2 != null) button2.onClick.AddListener(() => OnPuzzleButtonClicked(1));
        if (button3 != null) button3.onClick.AddListener(() => OnPuzzleButtonClicked(2));
        if (button4 != null) button4.onClick.AddListener(() => OnPuzzleButtonClicked(3));
        if (closeButton != null) closeButton.onClick.AddListener(ToggleTerminal);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (mainCamera != null) mainCamera.enabled = true;
        if (terminalCamera != null) terminalCamera.enabled = false;
    }

    void Update()
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

    public void ToggleTerminal()
    {
        isTerminalActive = !isTerminalActive;
        terminalUI.SetActive(isTerminalActive);

        if (isTerminalActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (mainCamera != null) mainCamera.enabled = false;
            if (terminalCamera != null) terminalCamera.enabled = true;

            if (terminalOpenSound != null)
                audioSource.PlayOneShot(terminalOpenSound);

            if (hintText != null)
                hintText.text = "";

            if (terminalHeader != null)
                terminalHeader.text = "ESCAPE ROOM AI ASSISTANT - PUZZLE ANALYSIS SYSTEM";

            if (instructionText != null)
                instructionText.text = "Select a puzzle (1-4) for cryptic guidance. The path to freedom requires interpretation.";
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

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
        }
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
                typingCoroutine = StartCoroutine(TypeText("PUZZLE ANALYSIS COMPLETE:\\n\\nThis challenge has been conquered. The fragment of your liberation has been claimed. Seek the remaining puzzles or proceed to the final threshold with your collected keys."));
            }
            else
            {
                // Special handling for riddle puzzle (index 3)
                if (puzzleIndex == 3)
                {
                    HandleRiddlePuzzleRequest();
                }
                else if (puzzleIndex < puzzleHints.Count)
                {
                    typingCoroutine = StartCoroutine(TypeText(puzzleHints[puzzleIndex]));
                }
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

            typingCoroutine = StartCoroutine(TypeText("ANALYSIS CONFIRMED:\\n\\nYour mastery of this puzzle has been recorded. The fragment of escape has been added to your collection. Continue your quest for liberation."));
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

    private void HandleRiddlePuzzleRequest()
    {
        RiddlePuzzle riddlePuzzle = RiddlePuzzle.Instance;
        
        if (riddlePuzzle != null && riddlePuzzle.IsRiddlePuzzleActive())
        {
            // Generate frustrating nonsense response that contains the answer
            string nonsenseResponse = riddlePuzzle.GetCurrentRiddleNonsenseResponse();
            typingCoroutine = StartCoroutine(TypeText(nonsenseResponse));
        }
        else
        {
            // Normal riddle hint when puzzle is not active
            string normalHint = "RIDDLE CHALLENGE ANALYSIS:\\\\n\\\\nAh, the seeker of words approaches the altar of wit! These ancient " +
                "puzzles have confounded minds for millennia. But fear not, for I shall provide... assistance... though " +
                "perhaps not in the form you expect. The AI terminal seems to be malfunctioning when processing riddle " +
                "queries - it outputs complete nonsense! However, legend says the answers hide within the chaos. Use the " +
                "terminal during your riddle solving for 'helpful' responses that may contain more than meets the eye...\\\\n\\\\n" +
                "Seek the wisdom hidden in digital madness, decode the chaos, unlock the final mystery.";
            typingCoroutine = StartCoroutine(TypeText(normalHint));
        }
    }
}