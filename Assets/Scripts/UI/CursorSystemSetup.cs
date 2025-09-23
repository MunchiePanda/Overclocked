using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Easy setup component to fix all cursor management issues in Overclocked
/// </summary>
public class CursorSystemSetup : MonoBehaviour
{
    [Header("🖱️ Cursor System Setup")]
    [Tooltip("Automatically setup cursor management on Start")]
    public bool autoSetupOnStart = true;
    
    [Tooltip("Show setup results in console")]
    public bool showSetupLogs = true;
    
    [Header("🎯 Cursor Settings")]
    [Tooltip("Default cursor lock mode for FPS gameplay")]
    public CursorLockMode gameplayCursorLock = CursorLockMode.Locked;
    
    [Tooltip("Default cursor visibility for FPS gameplay")]
    public bool gameplayCursorVisible = false;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupCursorSystem();
        }
    }
    
    [ContextMenu("🚀 Setup Complete Cursor System")]
    public void SetupCursorSystem()
    {
        SetupCompleteCursorSystem();
    }
    
    public void SetupCompleteCursorSystem()
    {
        if (showSetupLogs)
            Debug.Log("🖱️ Setting up complete cursor management system...");
        
        // Step 1: Configure PauseManager
        SetupPauseManager();
        
        // Step 2: Add Alteruna UI helper
        SetupAlternaUIHelper();
        
        // Step 3: Add Lobby UI helper (for menu/lobby scenes)
        SetupLobbyUIHelper();
        
        // Step 4: Set initial cursor state
        SetInitialCursorState();
        
        if (showSetupLogs)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            bool isMenuScene = sceneName.ToLower().Contains("menu") || 
                              sceneName.ToLower().Contains("lobby") ||
                              sceneName.ToLower().Contains("start") ||
                              sceneName == "SampleScene";
            
            Debug.Log("✅ Cursor system setup complete!");
            
            if (isMenuScene)
            {
                Debug.Log("🌐 Lobby/Menu cursor: Unlocked & Visible");
                Debug.Log("🎯 UI interactions: Fully supported");
            }
            else
            {
                Debug.Log("🎮 Gameplay cursor: Locked & Hidden");
                Debug.Log("🖥️ UI cursor: Unlocked & Visible");
            }
            
            Debug.Log("⚙️ Priority system: Pause(100) > EndScreen(90) > Terminal(75) > LobbyUI(65) > AlternaUI(60) > Default(0)");
        }
    }
    
    private void SetupPauseManager()
    {
        PauseManager pauseManager = FindFirstObjectByType<PauseManager>();
        
        if (pauseManager == null)
        {
            if (showSetupLogs)
                Debug.LogWarning("⚠️ No PauseManager found in scene!");
            return;
        }
        
        // Configure cursor settings
        pauseManager.gameplayCursorLockMode = gameplayCursorLock;
        pauseManager.gameplayCursorVisible = gameplayCursorVisible;
        pauseManager.enableCentralCursorManagement = true;
        
        if (showSetupLogs)
            Debug.Log("✅ Configured PauseManager for central cursor control");
    }
    
    private void SetupAlternaUIHelper()
    {
        // Check if helper already exists
        AlternaUICursorHelper existingHelper = FindFirstObjectByType<AlternaUICursorHelper>();
        
        if (existingHelper == null)
        {
            // Create new helper
            GameObject helperObject = new GameObject("AlternaUICursorHelper");
            AlternaUICursorHelper helper = helperObject.AddComponent<AlternaUICursorHelper>();
            helper.autoMonitorUI = true;
            helper.enableDebugLogs = showSetupLogs;
            
            if (showSetupLogs)
                Debug.Log("✅ Created AlternaUICursorHelper");
        }
        else
        {
            // Configure existing helper
            existingHelper.autoMonitorUI = true;
            existingHelper.enableDebugLogs = showSetupLogs;
            
            if (showSetupLogs)
                Debug.Log("✅ Configured existing AlternaUICursorHelper");
        }
    }
    
    private void SetupLobbyUIHelper()
    {
        // Check if helper already exists
        LobbyUIHelper existingHelper = FindFirstObjectByType<LobbyUIHelper>();
        
        if (existingHelper == null)
        {
            // Create new helper
            GameObject helperObject = new GameObject("LobbyUIHelper");
            LobbyUIHelper helper = helperObject.AddComponent<LobbyUIHelper>();
            helper.autoSetupOnStart = true;
            helper.enableDebugLogs = showSetupLogs;
            
            if (showSetupLogs)
                Debug.Log("✅ Created LobbyUIHelper");
        }
        else
        {
            // Configure existing helper
            existingHelper.autoSetupOnStart = true;
            existingHelper.enableDebugLogs = showSetupLogs;
            
            if (showSetupLogs)
                Debug.Log("✅ Configured existing LobbyUIHelper");
        }
    }

    private void SetInitialCursorState()
    {
        // Set initial cursor state based on scene type
        PauseManager.ClearAllCursorRequests();
        
        // Check if we're in a menu/lobby scene
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        bool isMenuScene = currentSceneName.ToLower().Contains("menu") || 
                          currentSceneName.ToLower().Contains("lobby") ||
                          currentSceneName.ToLower().Contains("start") ||
                          currentSceneName == "SampleScene"; // Add your lobby scene name here
        
        if (isMenuScene)
        {
            // For menu/lobby scenes, ensure cursor is visible and unlocked
            if (showSetupLogs)
                Debug.Log("✅ Set initial cursor state for lobby/menu scene (visible & unlocked)");
        }
        else
        {
            // For gameplay scenes, the default state will be applied automatically
            if (showSetupLogs)
                Debug.Log("✅ Set initial cursor state for gameplay scene (locked & hidden)");
        }
    }
    
    [ContextMenu("🔍 Debug Cursor Status")]
    public void DebugCursorStatus()
    {
        Debug.Log("🖱️ === CURSOR SYSTEM STATUS ===");
        Debug.Log($"Current lock state: {Cursor.lockState}");
        Debug.Log($"Current visibility: {Cursor.visible}");
        Debug.Log($"Current controller: {PauseManager.GetCurrentCursorRequester()}");
        
        string[] requesters = PauseManager.GetActiveCursorRequesters();
        Debug.Log($"Active requesters ({requesters.Length}): {string.Join(", ", requesters)}");
        
        // Check for common issues
        if (Cursor.lockState != CursorLockMode.Locked && requesters.Length == 0)
        {
            Debug.LogWarning("⚠️ ISSUE: Cursor unlocked but no active requesters!");
        }
        
        if (Cursor.visible && requesters.Length == 0)
        {
            Debug.LogWarning("⚠️ ISSUE: Cursor visible but no active requesters!");
        }
    }
    
    [ContextMenu("🧹 Emergency Cursor Fix")]
    public void EmergencyCursorFix()
    {
        Debug.Log("🚨 Performing emergency cursor fix...");
        
        // Clear all requests
        PauseManager.ClearAllCursorRequests();
        
        // Force gameplay cursor
        PauseManager.ForceCursor("EMERGENCY", gameplayCursorLock, gameplayCursorVisible);
        
        // Wait a frame then release
        StartCoroutine(DelayedEmergencyRelease());
        
        Debug.Log("✅ Emergency cursor fix applied!");
    }
    
    private System.Collections.IEnumerator DelayedEmergencyRelease()
    {
        yield return null;
        PauseManager.ReleaseCursor("EMERGENCY");
    }
    
    [ContextMenu("🎮 Test Cursor System")]
    public void TestCursorSystem()
    {
        StartCoroutine(CursorSystemTest());
    }
    
    public void TestCompleteCursorSystem()
    {
        TestCursorSystem();
    }
    
    private System.Collections.IEnumerator CursorSystemTest()
    {
        Debug.Log("🧪 Testing cursor system...");
        
        // Test UI cursor
        Debug.Log("1. Testing UI cursor...");
        PauseManager.RequestCursor("TEST", CursorLockMode.None, true, 50);
        yield return new WaitForSeconds(1f);
        
        // Test high priority cursor
        Debug.Log("2. Testing high priority cursor...");
        PauseManager.RequestCursor("TEST_HIGH", CursorLockMode.Confined, true, 80);
        yield return new WaitForSeconds(1f);
        
        // Release high priority
        Debug.Log("3. Releasing high priority cursor...");
        PauseManager.ReleaseCursor("TEST_HIGH");
        yield return new WaitForSeconds(1f);
        
        // Release all
        Debug.Log("4. Releasing all test cursors...");
        PauseManager.ReleaseCursor("TEST");
        
        Debug.Log("✅ Cursor system test complete!");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CursorSystemSetup))]
public class CursorSystemSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("🖱️ Cursor System Tools", EditorStyles.boldLabel);
        
        CursorSystemSetup setup = (CursorSystemSetup)target;
        
        // Main setup button
        if (GUILayout.Button("🚀 Setup Complete Cursor System", GUILayout.Height(40)))
        {
            setup.SetupCursorSystem();
        }
        
        EditorGUILayout.Space();
        
        // Debug tools
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🔍 Debug Status", GUILayout.Height(30)))
        {
            setup.DebugCursorStatus();
        }
        
        if (GUILayout.Button("🧪 Test System", GUILayout.Height(30)))
        {
            setup.TestCursorSystem();
        }
        EditorGUILayout.EndHorizontal();
        
        if (GUILayout.Button("🚨 Emergency Fix", GUILayout.Height(30)))
        {
            setup.EmergencyCursorFix();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This component creates a centralized cursor management system that prevents conflicts between Alteruna UI, terminals, pause menu, and other UI systems.", MessageType.Info);
    }
}
#endif