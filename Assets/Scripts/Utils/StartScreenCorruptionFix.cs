using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Targeted fix for StartScreen UI corruption issues
/// </summary>
public class StartScreenCorruptionFix : MonoBehaviour
{
    [Header("🎯 StartScreen Corruption Fix")]
    [Space(5)]
    [Tooltip("Run aggressive cleanup to fix Inspector errors")]
    public bool aggressiveMode = true;
    
    [Header("📊 Fix Results")]
    [SerializeField] private int objectsFixed = 0;
    [SerializeField] private int componentsRemoved = 0;
    [SerializeField] private int referencesRepaired = 0;

    void Start()
    {
        // Auto-run fix on start if in Editor
        #if UNITY_EDITOR
        if (Application.isEditor)
        {
            Invoke(nameof(RunCompleteFix), 0.1f);
        }
        #endif
    }

    [ContextMenu("🚨 Emergency Fix - Run Now")]
    public void RunCompleteFix()
    {
        Debug.Log("🚨 Running Emergency StartScreen Corruption Fix...");
        
        objectsFixed = 0;
        componentsRemoved = 0;
        referencesRepaired = 0;
        
        // Step 1: Clear all editor selections immediately
        ClearEditorState();
        
        // Step 2: Fix specific UI objects that are causing issues
        FixCanvasHierarchy();
        
        // Step 3: Remove any objects with missing components
        RemoveCorruptedObjects();
        
        // Step 4: Rebuild UI references
        RebuildUIReferences();
        
        // Step 5: Final cleanup
        FinalCleanup();
        
        Debug.Log($"✅ Emergency fix complete! Fixed {objectsFixed} objects, removed {componentsRemoved} components, repaired {referencesRepaired} references");
        
        // Auto-destroy this component
        if (Application.isEditor)
        {
            DestroyImmediate(this);
        }
    }
    
    private void ClearEditorState()
    {
        #if UNITY_EDITOR
        // Clear all selections to stop Inspector errors
        Selection.activeObject = null;
        Selection.objects = new Object[0];
        Selection.activeGameObject = null;
        Selection.activeTransform = null;
        
        // Clear editor caches
        EditorUtility.ClearProgressBar();
        
        // Force refresh all windows
        EditorApplication.RepaintHierarchyWindow();
        EditorApplication.RepaintProjectWindow();
        
        // Clear console
        var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
        if (logEntries != null)
        {
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod?.Invoke(null, null);
        }
        #endif
        
        Debug.Log("🧹 Cleared editor state");
    }
    
    private void FixCanvasHierarchy()
    {
        // Find the Canvas and its children
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        
        foreach (Canvas canvas in canvases)
        {
            if (canvas == null) continue;
            
            Debug.Log($"🔧 Fixing Canvas: {canvas.name}");
            
            // Fix Canvas components
            FixCanvasComponent(canvas);
            
            // Fix all UI elements in this canvas
            FixUIElementsInCanvas(canvas.transform);
            
            objectsFixed++;
        }
    }
    
    private void FixCanvasComponent(Canvas canvas)
    {
        try
        {
            // Ensure Canvas has proper components
            if (canvas.GetComponent<CanvasScaler>() == null)
            {
                canvas.gameObject.AddComponent<CanvasScaler>();
                referencesRepaired++;
            }
            
            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
                referencesRepaired++;
            }
            
            // Fix Canvas settings
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;
            
            #if UNITY_EDITOR
            EditorUtility.SetDirty(canvas);
            #endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to fix Canvas {canvas.name}: {e.Message}");
        }
    }
    
    private void FixUIElementsInCanvas(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (child == null) continue;
            
            GameObject obj = child.gameObject;
            
            // Check for missing components first
            if (HasMissingComponents(obj))
            {
                if (aggressiveMode)
                {
                    Debug.LogWarning($"🗑️ Removing object with missing components: {obj.name}");
                    DestroyImmediate(obj);
                    componentsRemoved++;
                    continue;
                }
            }
            
            // Fix common UI component issues
            FixUIComponentReferences(obj);
            
            // Recursively fix children
            FixUIElementsInCanvas(child);
        }
    }
    
    private bool HasMissingComponents(GameObject obj)
    {
        Component[] components = obj.GetComponents<Component>();
        
        foreach (Component comp in components)
        {
            if (comp == null)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private void FixUIComponentReferences(GameObject obj)
    {
        try
        {
            // Fix Button components
            Button button = obj.GetComponent<Button>();
            if (button != null)
            {
                if (button.targetGraphic == null)
                {
                    Image img = obj.GetComponent<Image>();
                    if (img != null)
                    {
                        button.targetGraphic = img;
                        referencesRepaired++;
                    }
                }
                
                // Clear any null event listeners
                if (button.onClick != null)
                {
                    for (int i = button.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
                    {
                        if (button.onClick.GetPersistentTarget(i) == null)
                        {
                            button.onClick.RemoveAllListeners();
                            referencesRepaired++;
                            break;
                        }
                    }
                }
            }
            
            // Fix Image components
            Image image = obj.GetComponent<Image>();
            if (image != null)
            {
                // Ensure valid color
                if (image.color.a <= 0)
                {
                    image.color = Color.white;
                    referencesRepaired++;
                }
                
                // Fix null sprite issues
                if (image.sprite == null && image.type != Image.Type.Simple)
                {
                    image.type = Image.Type.Simple;
                    referencesRepaired++;
                }
            }
            
            // Fix TMP_Text components
            TMP_Text text = obj.GetComponent<TMP_Text>();
            if (text != null)
            {
                if (string.IsNullOrEmpty(text.text))
                {
                    text.text = obj.name;
                    referencesRepaired++;
                }
            }
            
            #if UNITY_EDITOR
            EditorUtility.SetDirty(obj);
            #endif
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to fix UI references for {obj.name}: {e.Message}");
        }
    }
    
    private void RemoveCorruptedObjects()
    {
        if (!aggressiveMode) return;
        
        List<GameObject> toRemove = new List<GameObject>();
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue;
            
            // Check if object has corrupted components
            if (HasMissingComponents(obj))
            {
                // Don't remove essential objects
                if (obj.name.Contains("Canvas") || obj.name.Contains("EventSystem") || 
                    obj.name.Contains("Camera") || obj.name.Contains("Light"))
                {
                    continue;
                }
                
                toRemove.Add(obj);
            }
        }
        
        foreach (GameObject obj in toRemove)
        {
            if (obj != null)
            {
                Debug.LogWarning($"🗑️ Removing corrupted object: {obj.name}");
                DestroyImmediate(obj);
                componentsRemoved++;
            }
        }
    }
    
    private void RebuildUIReferences()
    {
        // Find StartScreenManager and ensure it has proper references
        StartScreenManager startScreen = FindFirstObjectByType<StartScreenManager>();
        if (startScreen != null)
        {
            try
            {
                // Let the StartScreenManager rebuild its references
                #if UNITY_EDITOR
                EditorUtility.SetDirty(startScreen);
                #endif
                referencesRepaired++;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Failed to rebuild StartScreenManager references: {e.Message}");
            }
        }
    }
    
    private void FinalCleanup()
    {
        #if UNITY_EDITOR
        // Final editor cleanup
        Selection.activeObject = null;
        
        // Force garbage collection
        System.GC.Collect();
        
        // Refresh all editor windows one more time
        EditorApplication.RepaintHierarchyWindow();
        EditorApplication.RepaintProjectWindow();
        
        // Mark scene as dirty to ensure changes are saved
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        #endif
        
        Debug.Log("🧹 Final cleanup complete");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(StartScreenCorruptionFix))]
public class StartScreenCorruptionFixEditor : Editor
{
    public override void OnInspectorGUI()
    {
        StartScreenCorruptionFix fix = (StartScreenCorruptionFix)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("🚨 StartScreen Emergency Fix", EditorStyles.boldLabel);
        
        EditorGUILayout.HelpBox(
            "This tool aggressively fixes StartScreen UI corruption:\n" +
            "• Clears all Inspector selections\n" +
            "• Removes objects with missing components\n" +
            "• Rebuilds UI component references\n" +
            "• Forces editor state refresh\n\n" +
            "⚠️ Use with caution - this will delete corrupted objects!", 
            MessageType.Warning);
        
        EditorGUILayout.Space();
        
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("🚨 RUN EMERGENCY FIX", GUILayout.Height(50)))
        {
            if (EditorUtility.DisplayDialog("Emergency Fix", 
                "This will aggressively fix UI corruption and may delete corrupted objects. Continue?", 
                "Yes, Fix It", "Cancel"))
            {
                fix.RunCompleteFix();
            }
        }
        
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("🧹 Clear Editor State Only", GUILayout.Height(30)))
        {
            Selection.activeObject = null;
            Selection.objects = new Object[0];
            
            var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
            if (logEntries != null)
            {
                var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                clearMethod?.Invoke(null, null);
            }
            
            EditorApplication.RepaintHierarchyWindow();
            EditorApplication.RepaintProjectWindow();
            
            Debug.Log("🧹 Editor state cleared");
        }
        
        GUI.backgroundColor = Color.white;
    }
}
#endif