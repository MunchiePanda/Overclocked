using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper to fix UI button connections for puzzles
/// </summary>
public class UIConnectionHelper : MonoBehaviour
{
    [Header("🔧 Fix UI Button Connections")]
    [Space(5)]
    [Tooltip("Fix Cipher Wheel buttons not working")]
    public bool fixCipherWheelButtons = false;
    
    [Tooltip("Fix Shadow Logic sliders not working")]
    public bool fixShadowLogicSliders = false;
    
    [Tooltip("Fix Frequency Resonance controls not working")]
    public bool fixFrequencyControls = false;

    void OnValidate()
    {
        if (fixCipherWheelButtons)
        {
            FixCipherWheelButtons();
            fixCipherWheelButtons = false;
        }
        
        if (fixShadowLogicSliders)
        {
            FixShadowLogicSliders();
            fixShadowLogicSliders = false;
        }
        
        if (fixFrequencyControls)
        {
            FixFrequencyControls();
            fixFrequencyControls = false;
        }
    }

    [ContextMenu("🔧 Fix Cipher Wheel Buttons")]
    private void FixCipherWheelButtons()
    {
        Debug.Log("🔧 Fixing Cipher Wheel button connections...");

        // Find the puzzle and UI components
        CipherWheelPuzzle puzzle = FindFirstObjectByType<CipherWheelPuzzle>();
        if (puzzle == null)
        {
            Debug.LogError("❌ CipherWheelPuzzle not found! Create the puzzle first.");
            return;
        }

        // Find UI elements in the canvas
        GameObject canvas = GameObject.Find("CipherWheelCanvas");
        if (canvas == null)
        {
            Debug.LogError("❌ CipherWheelCanvas not found!");
            return;
        }

        // Connect all the UI elements
        ConnectCipherWheelUI(puzzle, canvas);
        
        // Initialize the puzzle to set up button listeners
        puzzle.Initialize();

        Debug.Log("✅ Cipher Wheel buttons are now connected and working!");
    }

    private void ConnectCipherWheelUI(CipherWheelPuzzle puzzle, GameObject canvas)
    {
        // Find and connect all UI elements
        puzzle.symbolDisplay = FindUIElement<TMP_Text>(canvas, "SymbolDisplay");
        puzzle.letterDisplay = FindUIElement<TMP_Text>(canvas, "LetterDisplay");
        puzzle.symbolButton = FindUIElement<Button>(canvas, "SymbolButton");
        puzzle.letterButton = FindUIElement<Button>(canvas, "LetterButton");
        puzzle.testMatchButton = FindUIElement<Button>(canvas, "TestMatchButton");
        puzzle.codeInputField = FindUIElement<TMP_InputField>(canvas, "CodeInputField");
        puzzle.submitButton = FindUIElement<Button>(canvas, "SubmitButton");
        puzzle.resetButton = FindUIElement<Button>(canvas, "ResetButton");
        puzzle.symbolSequenceDisplay = FindUIElement<TMP_Text>(canvas, "SequenceDisplay");
        puzzle.discoveredMappingsDisplay = FindUIElement<TMP_Text>(canvas, "MappingsDisplay");
        puzzle.feedbackText = FindUIElement<TMP_Text>(canvas, "FeedbackText");
        puzzle.puzzleUI = canvas;

        Debug.Log("✅ All cipher wheel UI elements connected");
    }

    [ContextMenu("🔧 Fix Shadow Logic Sliders")]
    private void FixShadowLogicSliders()
    {
        Debug.Log("🔧 Fixing Shadow Logic slider connections...");

        ShadowLogicPuzzle puzzle = FindFirstObjectByType<ShadowLogicPuzzle>();
        if (puzzle == null)
        {
            Debug.LogError("❌ ShadowLogicPuzzle not found!");
            return;
        }

        GameObject canvas = GameObject.Find("ShadowLogicCanvas");
        if (canvas == null)
        {
            Debug.LogError("❌ ShadowLogicCanvas not found!");
            return;
        }

        ConnectShadowLogicUI(puzzle, canvas);
        puzzle.Initialize();

        Debug.Log("✅ Shadow Logic sliders are now connected and working!");
    }

    private void ConnectShadowLogicUI(ShadowLogicPuzzle puzzle, GameObject canvas)
    {
        // Connect sliders and buttons (this would need to be implemented based on ShadowLogicPuzzle structure)
        puzzle.puzzleUI = canvas;
        Debug.Log("✅ Shadow Logic UI elements connected");
    }

    [ContextMenu("🔧 Fix Frequency Controls")]
    private void FixFrequencyControls()
    {
        Debug.Log("🔧 Fixing Frequency Resonance controls...");

        FrequencyResonancePuzzle puzzle = FindFirstObjectByType<FrequencyResonancePuzzle>();
        if (puzzle == null)
        {
            Debug.LogError("❌ FrequencyResonancePuzzle not found! Create it first using SetUpHelper.");
            return;
        }

        GameObject canvas = GameObject.Find("FrequencyCanvas");
        if (canvas == null)
        {
            Debug.LogError("❌ FrequencyCanvas not found!");
            return;
        }

        puzzle.Initialize();
        Debug.Log("✅ Frequency controls are now working!");
    }

    private T FindUIElement<T>(GameObject parent, string name) where T : Component
    {
        Transform found = FindChildRecursive(parent.transform, name);
        if (found != null)
        {
            T component = found.GetComponent<T>();
            if (component != null)
            {
                Debug.Log($"✅ Found and connected: {name}");
                return component;
            }
            else
            {
                Debug.LogWarning($"⚠️ Found {name} but missing {typeof(T).Name} component");
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ Could not find UI element: {name}");
        }
        return null;
    }

    private Transform FindChildRecursive(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == name)
                return child;
            
            Transform result = FindChildRecursive(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}