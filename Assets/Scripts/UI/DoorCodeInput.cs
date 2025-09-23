using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Handles door code input interface with keypad-style input
/// </summary>
public class DoorCodeInput : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Input field for code entry")]
    public TMP_InputField codeInputField;
    
    [Tooltip("Submit button to try the code")]
    public Button submitButton;
    
    [Tooltip("Clear button to reset input")]
    public Button clearButton;
    
    [Tooltip("Feedback text for showing results")]
    public TMP_Text feedbackText;
    
    [Tooltip("Instructions text")]
    public TMP_Text instructionsText;
    
    [Tooltip("Main door input panel")]
    public GameObject doorInputPanel;
    
    [Header("Keypad Buttons")]
    [Tooltip("Array of number buttons (0-9)")]
    public Button[] numberButtons = new Button[10];
    
    [Header("Integration References")]
    [Tooltip("Reference to the door to unlock")]
    public Door targetDoor;
    
    [Tooltip("Reference to escape code manager")]
    public EscapeCodeManager escapeCodeManager;
    
    [Header("Visual Settings")]
    [Tooltip("Color for correct code feedback")]
    public Color correctColor = Color.green;
    
    [Tooltip("Color for incorrect code feedback")]
    public Color incorrectColor = Color.red;
    
    [Tooltip("Color for normal state")]
    public Color normalColor = Color.white;
    
    [Tooltip("Color for waiting state")]
    public Color waitingColor = Color.yellow;
    
    [Header("Animation Settings")]
    [Tooltip("Shake intensity for wrong code")]
    public float shakeIntensity = 10f;
    
    [Tooltip("Shake duration")]
    public float shakeDuration = 0.5f;
    
    [Tooltip("Feedback display duration")]
    public float feedbackDuration = 2f;
    
    [Header("Sound Settings")]
    [Tooltip("Sound for button press")]
    public AudioClip buttonPressSound;
    
    [Tooltip("Sound for correct code")]
    public AudioClip correctCodeSound;
    
    [Tooltip("Sound for incorrect code")]
    public AudioClip incorrectCodeSound;
    
    [Tooltip("Audio source for playing sounds")]
    public AudioSource audioSource;
    
    // Internal state
    private string currentInput = "";
    private bool isWaitingForCode = false;
    private Vector3 originalPanelPosition;
    private Coroutine feedbackCoroutine;
    private Coroutine shakeCoroutine;
    
    void Start()
    {
        Initialize();
    }
    
    private void Initialize()
    {
        // Find references if not assigned
        FindMissingReferences();
        
        // Setup UI event handlers
        SetupEventHandlers();
        
        // Store original position for shake effect
        if (doorInputPanel != null)
        {
            originalPanelPosition = doorInputPanel.transform.localPosition;
        }
        
        // Initial UI state
        UpdateUI();
        
        Debug.Log("DoorCodeInput initialized");
    }
    
    private void FindMissingReferences()
    {
        if (targetDoor == null)
        {
            targetDoor = FindFirstObjectByType<Door>();
        }
        
        if (escapeCodeManager == null)
        {
            escapeCodeManager = FindFirstObjectByType<EscapeCodeManager>();
        }
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        // Auto-find UI elements if not assigned
        if (doorInputPanel == null)
        {
            doorInputPanel = transform.Find("DoorInputPanel")?.gameObject;
        }
        
        if (codeInputField == null)
        {
            codeInputField = GetComponentInChildren<TMP_InputField>();
        }
        
        if (feedbackText == null)
        {
            // Look for feedback text by name
            Transform feedbackTransform = transform.Find("FeedbackText") ?? 
                                        transform.Find("Feedback") ?? 
                                        transform.Find("conText");
            if (feedbackTransform != null)
            {
                feedbackText = feedbackTransform.GetComponent<TMP_Text>();
            }
        }
    }
    
    private void SetupEventHandlers()
    {
        // Setup submit button
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(SubmitCode);
        }
        
        // Setup clear button
        if (clearButton != null)
        {
            clearButton.onClick.AddListener(ClearInput);
        }
        
        // Setup number buttons
        for (int i = 0; i < numberButtons.Length; i++)
        {
            if (numberButtons[i] != null)
            {
                int number = i; // Capture for closure
                numberButtons[i].onClick.AddListener(() => InputNumber(number));
            }
        }
        
        // Setup input field
        if (codeInputField != null)
        {
            codeInputField.onValueChanged.AddListener(OnInputChanged);
            codeInputField.onEndEdit.AddListener(OnInputEndEdit);
        }
    }
    
    private void UpdateUI()
    {
        // Update input field
        if (codeInputField != null)
        {
            codeInputField.text = currentInput;
        }
        
        // Update instructions
        if (instructionsText != null)
        {
            if (escapeCodeManager != null && escapeCodeManager.IsEscapeCodeReady())
            {
                instructionsText.text = "Enter the escape code to unlock the door";
                instructionsText.color = waitingColor;
            }
            else
            {
                instructionsText.text = "Complete all puzzles to reveal the escape code";
                instructionsText.color = normalColor;
            }
        }
        
        // Update submit button interactability
        if (submitButton != null)
        {
            bool canSubmit = !string.IsNullOrEmpty(currentInput) && 
                           escapeCodeManager != null && 
                           escapeCodeManager.IsEscapeCodeReady();
            submitButton.interactable = canSubmit;
        }
        
        // Show/hide input panel based on code availability
        if (doorInputPanel != null)
        {
            bool shouldShow = escapeCodeManager != null && escapeCodeManager.IsEscapeCodeReady();
            doorInputPanel.SetActive(shouldShow);
        }
    }
    
    public void InputNumber(int number)
    {
        if (isWaitingForCode) return;
        
        // Limit input length (typically 4-6 digits)
        if (currentInput.Length < 8)
        {
            currentInput += number.ToString();
            UpdateUI();
            PlaySound(buttonPressSound);
        }
    }
    
    public void ClearInput()
    {
        if (isWaitingForCode) return;
        
        currentInput = "";
        UpdateUI();
        ClearFeedback();
        PlaySound(buttonPressSound);
    }
    
    public void SubmitCode()
    {
        if (isWaitingForCode || string.IsNullOrEmpty(currentInput)) return;
        
        if (escapeCodeManager == null || !escapeCodeManager.IsEscapeCodeReady())
        {
            ShowFeedback("Complete all puzzles first!", incorrectColor);
            return;
        }
        
        if (targetDoor == null)
        {
            ShowFeedback("Door system not found!", incorrectColor);
            return;
        }
        
        isWaitingForCode = true;
        ShowFeedback("Checking code...", waitingColor);
        
        // Add slight delay for suspense
        StartCoroutine(CheckCodeWithDelay(currentInput));
    }
    
    private IEnumerator CheckCodeWithDelay(string code)
    {
        yield return new WaitForSeconds(0.5f);
        
        string correctCode = escapeCodeManager.GetFinalEscapeCode();
        
        if (code == correctCode)
        {
            // Correct code!
            ShowFeedback("ACCESS GRANTED!", correctColor);
            PlaySound(correctCodeSound);
            
            // Unlock the door
            targetDoor.UnlockDoor(code);
            
            // Disable input after success
            DisableInput();
        }
        else
        {
            // Wrong code
            ShowFeedback("ACCESS DENIED!", incorrectColor);
            PlaySound(incorrectCodeSound);
            ShakePanel();
            
            // Clear input after wrong attempt
            yield return new WaitForSeconds(1f);
            ClearInput();
        }
        
        isWaitingForCode = false;
    }
    
    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null) return;
        
        // Stop existing feedback coroutine
        if (feedbackCoroutine != null)
        {
            StopCoroutine(feedbackCoroutine);
        }
        
        feedbackText.text = message;
        feedbackText.color = color;
        
        feedbackCoroutine = StartCoroutine(ClearFeedbackAfterDelay());
    }
    
    private IEnumerator ClearFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDuration);
        ClearFeedback();
    }
    
    private void ClearFeedback()
    {
        if (feedbackText != null)
        {
            feedbackText.text = "";
            feedbackText.color = normalColor;
        }
    }
    
    private void ShakePanel()
    {
        if (doorInputPanel == null) return;
        
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        
        shakeCoroutine = StartCoroutine(ShakeEffect());
    }
    
    private IEnumerator ShakeEffect()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeIntensity;
            randomOffset.z = 0; // Keep in 2D
            
            doorInputPanel.transform.localPosition = originalPanelPosition + randomOffset;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        doorInputPanel.transform.localPosition = originalPanelPosition;
    }
    
    private void DisableInput()
    {
        // Disable all input buttons
        if (submitButton != null) submitButton.interactable = false;
        if (clearButton != null) clearButton.interactable = false;
        if (codeInputField != null) codeInputField.interactable = false;
        
        foreach (var button in numberButtons)
        {
            if (button != null) button.interactable = false;
        }
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    private void OnInputChanged(string value)
    {
        if (isWaitingForCode) return;
        
        // Filter to only allow numbers
        string filteredValue = "";
        foreach (char c in value)
        {
            if (char.IsDigit(c) && filteredValue.Length < 8)
            {
                filteredValue += c;
            }
        }
        
        currentInput = filteredValue;
        
        // Update the input field if it was filtered
        if (codeInputField != null && codeInputField.text != currentInput)
        {
            codeInputField.text = currentInput;
        }
    }
    
    private void OnInputEndEdit(string value)
    {
        // Auto-submit if user presses Enter and code is ready
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (escapeCodeManager != null && escapeCodeManager.IsEscapeCodeReady())
            {
                SubmitCode();
            }
        }
    }
    
    void Update()
    {
        // Handle keyboard input for numbers
        if (!isWaitingForCode && (codeInputField == null || !codeInputField.isFocused))
        {
            for (int i = 0; i <= 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i))
                {
                    InputNumber(i);
                }
            }
            
            // Handle backspace
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (currentInput.Length > 0)
                {
                    currentInput = currentInput.Substring(0, currentInput.Length - 1);
                    UpdateUI();
                }
            }
            
            // Handle enter
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SubmitCode();
            }
        }
        
        // Update UI state
        UpdateUI();
    }
    
    // Public methods for external control
    public void ShowDoorInput()
    {
        if (doorInputPanel != null)
        {
            doorInputPanel.SetActive(true);
        }
        
        // Enable cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void HideDoorInput()
    {
        if (doorInputPanel != null)
        {
            doorInputPanel.SetActive(false);
        }
        
        // Restore cursor state (this might need adjustment based on your game state)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void ResetDoorInput()
    {
        currentInput = "";
        isWaitingForCode = false;
        ClearFeedback();
        
        // Re-enable input
        if (submitButton != null) submitButton.interactable = true;
        if (clearButton != null) clearButton.interactable = true;
        if (codeInputField != null) codeInputField.interactable = true;
        
        foreach (var button in numberButtons)
        {
            if (button != null) button.interactable = true;
        }
        
        UpdateUI();
    }
    
    // Context menu for testing
    [ContextMenu("🧪 Test Show Input")]
    public void TestShowInput()
    {
        ShowDoorInput();
        Debug.Log("Door input shown for testing");
    }
    
    [ContextMenu("🔑 Test Auto-Fill Code")]
    public void TestAutoFillCode()
    {
        if (escapeCodeManager != null && escapeCodeManager.IsEscapeCodeReady())
        {
            currentInput = escapeCodeManager.GetFinalEscapeCode();
            UpdateUI();
            Debug.Log($"Auto-filled with escape code: {currentInput}");
        }
        else
        {
            Debug.LogWarning("Escape code not ready yet!");
        }
    }
}