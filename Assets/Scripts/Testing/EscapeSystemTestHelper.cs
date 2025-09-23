using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Complete test helper for the entire escape room system
/// </summary>
public class EscapeSystemTestHelper : MonoBehaviour
{
    [Header("System References")]
    [Tooltip("Escape code manager")]
    public EscapeCodeManager escapeCodeManager;
    
    [Tooltip("Door component")]
    public Door escapeDoor;
    
    [Tooltip("Door code input")]
    public DoorCodeInput doorCodeInput;
    
    [Tooltip("Win screen")]
    public GameObject winScreen;
    
    [Header("Auto-Find Settings")]
    [Tooltip("Automatically find components")]
    public bool autoFindComponents = true;
    
    [Tooltip("Enable debug logging")]
    public bool enableDebugLogging = true;
    
    void Start()
    {
        if (autoFindComponents)
        {
            FindAllComponents();
        }
    }
    
    private void FindAllComponents()
    {
        if (escapeCodeManager == null)
        {
            escapeCodeManager = FindFirstObjectByType<EscapeCodeManager>();
            if (escapeCodeManager != null && enableDebugLogging)
                Debug.Log("✅ Found EscapeCodeManager");
        }
        
        if (escapeDoor == null)
        {
            escapeDoor = FindFirstObjectByType<Door>();
            if (escapeDoor != null && enableDebugLogging)
                Debug.Log("✅ Found Door");
        }
        
        if (doorCodeInput == null)
        {
            doorCodeInput = FindFirstObjectByType<DoorCodeInput>();
            if (doorCodeInput != null && enableDebugLogging)
                Debug.Log("✅ Found DoorCodeInput");
        }
        
        if (winScreen == null && escapeCodeManager != null)
        {
            winScreen = escapeCodeManager.winScreen.gameObject;
            if (winScreen != null && enableDebugLogging)
                Debug.Log("✅ Found WinScreen from EscapeCodeManager");
        }
    }
    
    [ContextMenu("🧪 Test Complete Escape Flow")]
    public void TestCompleteEscapeFlow()
    {
        StartCoroutine(RunCompleteEscapeTest());
    }
    
    private IEnumerator RunCompleteEscapeTest()
    {
        FindAllComponents();
        
        Debug.Log("🧪 === TESTING COMPLETE ESCAPE FLOW ===");
        
        // Step 1: Reset everything
        Debug.Log("🔄 Step 1: Resetting all systems...");
        if (escapeCodeManager != null)
        {
            escapeCodeManager.ResetEscapeProgress();
        }
        yield return new WaitForSeconds(0.5f);
        
        // Step 2: Simulate completing 3 puzzles
        Debug.Log("🎯 Step 2: Simulating puzzle completions...");
        if (escapeCodeManager != null)
        {
            escapeCodeManager.OnPuzzleCompleted(1234); // Riddle puzzle
            yield return new WaitForSeconds(0.5f);
            
            escapeCodeManager.OnPuzzleCompleted(5678); // Cipher wheel
            yield return new WaitForSeconds(0.5f);
            
            escapeCodeManager.OnPuzzleCompleted(9012); // Color puzzle
            yield return new WaitForSeconds(0.5f);
        }
        
        // Step 3: Check escape code generation
        Debug.Log("🔑 Step 3: Checking escape code generation...");
        if (escapeCodeManager != null && escapeCodeManager.IsEscapeCodeReady())
        {
            string finalCode = escapeCodeManager.GetFinalEscapeCode();
            Debug.Log($"✅ Escape code generated: {finalCode}");
            
            // Step 4: Show door input
            Debug.Log("🚪 Step 4: Showing door input interface...");
            if (doorCodeInput != null)
            {
                doorCodeInput.ShowDoorInput();
            }
            yield return new WaitForSeconds(1f);
            
            // Step 5: Auto-enter the code
            Debug.Log("⌨️ Step 5: Auto-entering escape code...");
            if (doorCodeInput != null)
            {
                doorCodeInput.TestAutoFillCode();
                yield return new WaitForSeconds(1f);
                doorCodeInput.SubmitCode();
            }
            
            // Step 6: Wait for win screen
            Debug.Log("🏆 Step 6: Waiting for win screen...");
            yield return new WaitForSeconds(3f);
            
            if (winScreen != null && winScreen.activeSelf)
            {
                Debug.Log("🎉 SUCCESS! Win screen is showing!");
            }
            else
            {
                Debug.LogWarning("⚠️ Win screen not showing - check setup");
            }
        }
        else
        {
            Debug.LogError("❌ Escape code not generated - check EscapeCodeManager");
        }
        
        Debug.Log("🧪 === ESCAPE FLOW TEST COMPLETE ===");
    }
    
    [ContextMenu("🎯 Test Individual Components")]
    public void TestIndividualComponents()
    {
        FindAllComponents();
        
        Debug.Log("🔍 === TESTING INDIVIDUAL COMPONENTS ===");
        
        // Test EscapeCodeManager
        if (escapeCodeManager != null)
        {
            Debug.Log($"✅ EscapeCodeManager: Ready={escapeCodeManager.IsEscapeCodeReady()}, Count={escapeCodeManager.GetCollectedCount()}/{escapeCodeManager.GetRequiredCount()}");
            if (escapeCodeManager.IsEscapeCodeReady())
            {
                Debug.Log($"🔑 Final Code: {escapeCodeManager.GetFinalEscapeCode()}");
            }
        }
        else
        {
            Debug.LogError("❌ EscapeCodeManager not found!");
        }
        
        // Test Door
        if (escapeDoor != null)
        {
            Debug.Log($"✅ Door: Name={escapeDoor.name}, Correct Code={escapeDoor.correctCode}");
        }
        else
        {
            Debug.LogError("❌ Door not found!");
        }
        
        // Test DoorCodeInput
        if (doorCodeInput != null)
        {
            Debug.Log($"✅ DoorCodeInput: Found on {doorCodeInput.name}");
            if (doorCodeInput.doorInputPanel != null)
            {
                Debug.Log($"   Panel Active: {doorCodeInput.doorInputPanel.activeSelf}");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ DoorCodeInput not found - may need setup");
        }
        
        // Test WinScreen
        if (winScreen != null)
        {
            Debug.Log($"✅ WinScreen: Name={winScreen.name}, Active={winScreen.activeSelf}");
        }
        else
        {
            Debug.LogWarning("⚠️ WinScreen not found!");
        }
        
        // Test cursor state
        Debug.Log($"🖱️ Cursor: Lock={Cursor.lockState}, Visible={Cursor.visible}");
        Debug.Log($"⏱️ Time Scale: {Time.timeScale}");
        
        Debug.Log("🔍 === COMPONENT TEST COMPLETE ===");
    }
    
    [ContextMenu("🔑 Test Door Code Input")]
    public void TestDoorCodeInput()
    {
        FindAllComponents();
        
        if (doorCodeInput == null)
        {
            Debug.LogError("❌ DoorCodeInput not found!");
            return;
        }
        
        Debug.Log("🚪 Testing door code input...");
        
        // Show the door input
        doorCodeInput.ShowDoorInput();
        
        // Enable cursor for testing
        PauseManager.RequestCursor("EscapeSystemTest", CursorLockMode.None, true, 85);
        
        Debug.Log("✅ Door input shown - test manually or use 'Test Auto-Fill Code'");
    }
    
    [ContextMenu("🎉 Test Win Screen")]
    public void TestWinScreen()
    {
        FindAllComponents();
        
        if (escapeCodeManager != null)
        {
            escapeCodeManager.OnEscapeSuccessful();
            Debug.Log("🏆 Triggered win screen manually");
        }
        else
        {
            Debug.LogError("❌ EscapeCodeManager not found!");
        }
    }
    
    [ContextMenu("🔄 Reset Entire System")]
    public void ResetEntireSystem()
    {
        FindAllComponents();
        
        Debug.Log("🔄 Resetting entire escape system...");
        
        // Reset escape code manager
        if (escapeCodeManager != null)
        {
            escapeCodeManager.ResetEscapeProgress();
        }
        
        // Reset door code input
        if (doorCodeInput != null)
        {
            doorCodeInput.ResetDoorInput();
            doorCodeInput.HideDoorInput();
        }
        
        // Hide win screen
        if (winScreen != null)
        {
            winScreen.SetActive(false);
            Time.timeScale = 1f;
        }
        
        // Reset cursor
        PauseManager.ReleaseCursor("EscapeSystemTest");
        
        Debug.Log("✅ System reset complete");
    }
    
    [ContextMenu("📊 Show System Status")]
    public void ShowSystemStatus()
    {
        FindAllComponents();
        
        Debug.Log("📊 === ESCAPE ROOM SYSTEM STATUS ===");
        
        if (escapeCodeManager != null)
        {
            Debug.Log($"🎯 ESCAPE PROGRESS:");
            Debug.Log($"   • Puzzles: {escapeCodeManager.GetCollectedCount()}/{escapeCodeManager.GetRequiredCount()}");
            Debug.Log($"   • Code Ready: {escapeCodeManager.IsEscapeCodeReady()}");
            if (escapeCodeManager.IsEscapeCodeReady())
                Debug.Log($"   • Final Code: {escapeCodeManager.GetFinalEscapeCode()}");
        }
        
        if (escapeDoor != null)
        {
            Debug.Log($"🚪 DOOR STATUS:");
            Debug.Log($"   • Name: {escapeDoor.name}");
            Debug.Log($"   • Expected Code: {escapeDoor.correctCode}");
        }
        
        if (doorCodeInput != null)
        {
            Debug.Log($"⌨️ INPUT INTERFACE:");
            Debug.Log($"   • Component: Found");
            Debug.Log($"   • Panel Active: {doorCodeInput.doorInputPanel?.activeSelf}");
        }
        else
        {
            Debug.Log($"⌨️ INPUT INTERFACE: Not found");
        }
        
        if (winScreen != null)
        {
            Debug.Log($"🏆 WIN SCREEN:");
            Debug.Log($"   • Name: {winScreen.name}");
            Debug.Log($"   • Active: {winScreen.activeSelf}");
        }
        
        Debug.Log($"🖱️ GAME STATE:");
        Debug.Log($"   • Cursor Lock: {Cursor.lockState}");
        Debug.Log($"   • Cursor Visible: {Cursor.visible}");
        Debug.Log($"   • Time Scale: {Time.timeScale}");
        
        Debug.Log("=====================================");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EscapeSystemTestHelper))]
public class EscapeSystemTestHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Escape System Testing", EditorStyles.boldLabel);
        
        EscapeSystemTestHelper helper = (EscapeSystemTestHelper)target;
        
        if (GUILayout.Button("🧪 Test Complete Escape Flow", GUILayout.Height(40)))
        {
            helper.TestCompleteEscapeFlow();
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🎯 Test Components", GUILayout.Height(30)))
        {
            helper.TestIndividualComponents();
        }
        
        if (GUILayout.Button("📊 System Status", GUILayout.Height(30)))
        {
            helper.ShowSystemStatus();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🔑 Test Door Input", GUILayout.Height(30)))
        {
            helper.TestDoorCodeInput();
        }
        
        if (GUILayout.Button("🎉 Test Win Screen", GUILayout.Height(30)))
        {
            helper.TestWinScreen();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("🔄 Reset Entire System", GUILayout.Height(30)))
        {
            helper.ResetEntireSystem();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This helper tests the complete escape room flow from puzzle completion to win screen display.", MessageType.Info);
    }
}
#endif