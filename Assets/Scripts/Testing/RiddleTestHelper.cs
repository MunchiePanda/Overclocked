using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper script to test the riddle puzzle system integration
/// </summary>
public class RiddleTestHelper : MonoBehaviour
{
    [Header("Test References")]
    [Tooltip("Reference to the riddle puzzle to test")]
    public RiddlePuzzle riddlePuzzle;
    
    [Tooltip("Reference to the escape code manager")]
    public EscapeCodeManager escapeCodeManager;
    
    [Header("Test Settings")]
    [Tooltip("Enable debug logging")]
    public bool enableDebugLogging = true;
    
    [Tooltip("Automatically find components if not assigned")]
    public bool autoFindComponents = true;
    
    void Start()
    {
        if (autoFindComponents)
        {
            FindComponents();
        }
    }
    
    private void FindComponents()
    {
        if (riddlePuzzle == null)
        {
            riddlePuzzle = FindFirstObjectByType<RiddlePuzzle>();
            if (riddlePuzzle != null && enableDebugLogging)
                Debug.Log("✅ Found RiddlePuzzle component");
        }
        
        if (escapeCodeManager == null)
        {
            escapeCodeManager = FindFirstObjectByType<EscapeCodeManager>();
            if (escapeCodeManager != null && enableDebugLogging)
                Debug.Log("✅ Found EscapeCodeManager component");
        }
    }
    
    [ContextMenu("🧪 Test Show Riddle Puzzle")]
    public void TestShowRiddlePuzzle()
    {
        FindComponents();
        
        if (riddlePuzzle != null)
        {
            riddlePuzzle.ShowPuzzle();
            
            if (enableDebugLogging)
            {
                Debug.Log("🎯 Riddle puzzle shown");
                Debug.Log($"📋 Cursor state: {Cursor.lockState}, Visible: {Cursor.visible}");
                Debug.Log($"⏱️ Timer: {riddlePuzzle.GetTimeRemaining():F1}s remaining");
            }
        }
        else
        {
            Debug.LogError("❌ RiddlePuzzle component not found!");
        }
    }
    
    [ContextMenu("🔍 Test System Integration")]
    public void TestSystemIntegration()
    {
        FindComponents();
        
        if (enableDebugLogging)
        {
            Debug.Log("🔍 Testing Riddle System Integration:");
            
            // Test riddle puzzle
            if (riddlePuzzle != null)
            {
                Debug.Log($"✅ RiddlePuzzle found: {riddlePuzzle.name}");
                Debug.Log($"📝 Riddle count: {riddlePuzzle.riddles.Count}");
                Debug.Log($"✔️ Solved count: {riddlePuzzle.GetSolvedCount()}");
                Debug.Log($"📊 Completion: {riddlePuzzle.GetCompletionPercentage() * 100:F1}%");
                Debug.Log($"⏱️ Time remaining: {riddlePuzzle.GetTimeRemaining():F1}s");
            }
            else
            {
                Debug.LogError("❌ RiddlePuzzle not found!");
            }
            
            // Test escape code manager
            if (escapeCodeManager != null)
            {
                Debug.Log($"✅ EscapeCodeManager found: {escapeCodeManager.name}");
                Debug.Log($"🔢 Required puzzles: {escapeCodeManager.GetRequiredCount()}");
                Debug.Log($"✔️ Completed puzzles: {escapeCodeManager.GetCollectedCount()}");
                Debug.Log($"🚪 Escape code ready: {escapeCodeManager.IsEscapeCodeReady()}");
                
                if (escapeCodeManager.IsEscapeCodeReady())
                {
                    Debug.Log($"🔑 Final escape code: {escapeCodeManager.GetFinalEscapeCode()}");
                }
            }
            else
            {
                Debug.LogError("❌ EscapeCodeManager not found!");
            }
        }
    }
    
    [ContextMenu("🎯 Test Auto-Solve Riddles")]
    public void TestAutoSolveRiddles()
    {
        FindComponents();
        
        if (riddlePuzzle != null)
        {
            Debug.Log("🎯 Auto-solving all riddles...");
            
            // Simulate solving each riddle
            for (int i = 0; i < riddlePuzzle.riddles.Count; i++)
            {
                var riddle = riddlePuzzle.riddles[i];
                
                if (riddlePuzzle.riddleAnswerInput != null)
                {
                    riddlePuzzle.riddleAnswerInput.text = riddle.answer;
                }
                
                // Simulate button click by calling CheckAnswer via reflection
                var checkAnswerMethod = typeof(RiddlePuzzle).GetMethod("CheckAnswer", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (checkAnswerMethod != null)
                {
                    checkAnswerMethod.Invoke(riddlePuzzle, null);
                    Debug.Log($"✅ Auto-solved riddle {i + 1}: {riddle.answer}");
                }
                
                // Move to next riddle if not the last one
                if (i < riddlePuzzle.riddles.Count - 1)
                {
                    var nextRiddleMethod = typeof(RiddlePuzzle).GetMethod("NextRiddle", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    nextRiddleMethod?.Invoke(riddlePuzzle, null);
                }
            }
            
            Debug.Log("🎉 All riddles auto-solved!");
        }
        else
        {
            Debug.LogError("❌ RiddlePuzzle not found!");
        }
    }
    
    [ContextMenu("🔄 Reset All Systems")]
    public void ResetAllSystems()
    {
        FindComponents();
        
        if (riddlePuzzle != null)
        {
            riddlePuzzle.ResetPuzzle();
            Debug.Log("🔄 RiddlePuzzle reset");
        }
        
        if (escapeCodeManager != null)
        {
            escapeCodeManager.ResetEscapeProgress();
            Debug.Log("🔄 EscapeCodeManager reset");
        }
        
        Debug.Log("✅ All systems reset!");
    }
    
    [ContextMenu("📊 Show System Status")]
    public void ShowSystemStatus()
    {
        FindComponents();
        
        Debug.Log("📊 === RIDDLE SYSTEM STATUS ===");
        
        if (riddlePuzzle != null)
        {
            Debug.Log($"🎯 RIDDLE PUZZLE:");
            Debug.Log($"   • Name: {riddlePuzzle.puzzleName}");
            Debug.Log($"   • Active: {riddlePuzzle.IsRiddlePuzzleActive()}");
            Debug.Log($"   • Completed: {riddlePuzzle.IsCompleted()}");
            Debug.Log($"   • Riddles: {riddlePuzzle.GetSolvedCount()}/{riddlePuzzle.riddles.Count}");
            Debug.Log($"   • Progress: {riddlePuzzle.GetCompletionPercentage() * 100:F1}%");
            Debug.Log($"   • Time: {riddlePuzzle.GetTimeRemaining():F1}s");
        }
        
        if (escapeCodeManager != null)
        {
            Debug.Log($"🚪 ESCAPE SYSTEM:");
            Debug.Log($"   • Puzzles: {escapeCodeManager.GetCollectedCount()}/{escapeCodeManager.GetRequiredCount()}");
            Debug.Log($"   • Code Ready: {escapeCodeManager.IsEscapeCodeReady()}");
            
            if (escapeCodeManager.IsEscapeCodeReady())
                Debug.Log($"   • Final Code: {escapeCodeManager.GetFinalEscapeCode()}");
        }
        
        Debug.Log($"🖱️ CURSOR STATE:");
        Debug.Log($"   • Lock Mode: {Cursor.lockState}");
        Debug.Log($"   • Visible: {Cursor.visible}");
        
        Debug.Log("================================");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RiddleTestHelper))]
public class RiddleTestHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Riddle System Testing", EditorStyles.boldLabel);
        
        RiddleTestHelper helper = (RiddleTestHelper)target;
        
        if (GUILayout.Button("🧪 Test Show Riddle Puzzle", GUILayout.Height(40)))
        {
            helper.TestShowRiddlePuzzle();
        }
        
        if (GUILayout.Button("🔍 Test System Integration", GUILayout.Height(30)))
        {
            helper.TestSystemIntegration();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("🎯 Auto-Solve All Riddles", GUILayout.Height(30)))
        {
            helper.TestAutoSolveRiddles();
        }
        
        if (GUILayout.Button("📊 Show System Status", GUILayout.Height(30)))
        {
            helper.ShowSystemStatus();
        }
        
        if (GUILayout.Button("🔄 Reset All Systems", GUILayout.Height(30)))
        {
            helper.ResetAllSystems();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Use these buttons to test the complete riddle puzzle integration with the escape code system.", MessageType.Info);
    }
}
#endif