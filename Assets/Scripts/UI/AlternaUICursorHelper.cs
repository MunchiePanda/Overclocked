using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Simple helper to manage cursor state when interacting with Alteruna UI
/// This component monitors UI interactions and ensures proper cursor restoration
/// </summary>
public class AlternaUICursorHelper : MonoBehaviour
{
    [Header("🌐 Alteruna UI Cursor Helper")]
    [Tooltip("Automatically monitor for Alteruna UI interactions")]
    public bool autoMonitorUI = true;
    
    [Tooltip("Delay before restoring cursor after UI interaction")]
    public float cursorRestoreDelay = 0.2f;
    
    [Tooltip("Enable debug logging")]
    public bool enableDebugLogs = true;
    
    // Private variables
    private bool wasOverUI = false;
    private Coroutine restoreCursorCoroutine;
    
    void Update()
    {
        if (!autoMonitorUI)
            return;
            
        MonitorUIInteractions();
    }
    
    /// <summary>
    /// Monitor UI interactions and manage cursor accordingly
    /// </summary>
    private void MonitorUIInteractions()
    {
        bool isOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        
        // Check if we just started hovering over UI
        if (isOverUI && !wasOverUI)
        {
            OnUIEnter();
        }
        // Check if we just left UI
        else if (!isOverUI && wasOverUI)
        {
            OnUIExit();
        }
        
        wasOverUI = isOverUI;
    }
    
    /// <summary>
    /// Called when mouse enters UI
    /// </summary>
    private void OnUIEnter()
    {
        // Cancel any pending cursor restore
        if (restoreCursorCoroutine != null)
        {
            StopCoroutine(restoreCursorCoroutine);
            restoreCursorCoroutine = null;
        }
        
        // Request cursor for UI interaction (lower priority than terminals/pause)
        PauseManager.RequestCursor("AlternaUI", CursorLockMode.None, true, 60);
        
        if (enableDebugLogs)
        {
            Debug.Log("🌐 Mouse entered Alteruna UI - showing cursor");
        }
    }
    
    /// <summary>
    /// Called when mouse exits UI
    /// </summary>
    private void OnUIExit()
    {
        // Start delayed cursor restore
        restoreCursorCoroutine = StartCoroutine(DelayedCursorRestore());
        
        if (enableDebugLogs)
        {
            Debug.Log("🌐 Mouse left Alteruna UI - scheduling cursor restore");
        }
    }
    
    /// <summary>
    /// Restore cursor after a delay (allows for UI transitions)
    /// </summary>
    private IEnumerator DelayedCursorRestore()
    {
        yield return new WaitForSecondsRealtime(cursorRestoreDelay);
        
        // Only restore if we're still not over UI
        bool isStillOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        
        if (!isStillOverUI)
        {
            PauseManager.ReleaseCursor("AlternaUI");
            
            if (enableDebugLogs)
            {
                Debug.Log("🌐 Restored cursor after Alteruna UI interaction");
            }
        }
        
        restoreCursorCoroutine = null;
    }
    
    /// <summary>
    /// Force release cursor (call this when Alteruna UI closes)
    /// </summary>
    public void ForceReleaseCursor()
    {
        // Cancel any pending restore
        if (restoreCursorCoroutine != null)
        {
            StopCoroutine(restoreCursorCoroutine);
            restoreCursorCoroutine = null;
        }
        
        // Release cursor immediately
        PauseManager.ReleaseCursor("AlternaUI");
        
        if (enableDebugLogs)
        {
            Debug.Log("🌐 Force released Alteruna UI cursor");
        }
    }
    
    /// <summary>
    /// Emergency cursor reset (call if cursor gets stuck)
    /// </summary>
    [ContextMenu("🚨 Emergency Cursor Reset")]
    public void EmergencyCursorReset()
    {
        PauseManager.ClearAllCursorRequests();
        Debug.Log("🚨 Emergency cursor reset performed!");
    }
    
    /// <summary>
    /// Test UI cursor request
    /// </summary>
    [ContextMenu("🧪 Test UI Cursor")]
    public void TestUICursor()
    {
        PauseManager.RequestCursor("TEST_UI", CursorLockMode.None, true, 60);
        Debug.Log("🧪 Requested test UI cursor");
    }
    
    /// <summary>
    /// Test cursor release
    /// </summary>
    [ContextMenu("🔄 Test Cursor Release")]
    public void TestCursorRelease()
    {
        PauseManager.ReleaseCursor("TEST_UI");
        Debug.Log("🔄 Released test cursor");
    }
    
    void OnDestroy()
    {
        // Clean up when destroyed
        PauseManager.ReleaseCursor("AlternaUI");
    }
}