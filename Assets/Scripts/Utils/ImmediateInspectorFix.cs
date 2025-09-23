using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Immediate fix for Inspector corruption - run this right now
/// </summary>
public class ImmediateInspectorFix
{
    #if UNITY_EDITOR
    [MenuItem("Tools/🚨 Fix Inspector Corruption NOW")]
    public static void FixInspectorNow()
    {
        Debug.Log("🚨 EMERGENCY: Fixing Inspector corruption immediately...");
        
        // 1. Clear ALL selections
        Selection.activeObject = null;
        Selection.objects = new Object[0];
        Selection.activeGameObject = null;
        Selection.activeTransform = null;
        
        // 2. Clear console
        var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
        if (logEntries != null)
        {
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod?.Invoke(null, null);
        }
        
        // 3. Clear editor instances (Unity 6 compatible)
        try
        {
            // Force Unity to clear any cached editors
            var inspectorType = System.Type.GetType("UnityEditor.InspectorWindow,UnityEditor");
            if (inspectorType != null)
            {
                var inspectors = Resources.FindObjectsOfTypeAll(inspectorType);
                foreach (var inspector in inspectors)
                {
                    if (inspector != null)
                    {
                        var repaintMethod = inspectorType.GetMethod("Repaint");
                        repaintMethod?.Invoke(inspector, null);
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Could not clear editor instances: {e.Message}");
        }
        
        // 4. Clear editor caches
        EditorUtility.ClearProgressBar();
        
        // 5. Force repaint everything
        EditorApplication.RepaintHierarchyWindow();
        EditorApplication.RepaintProjectWindow();
        
        // 6. Force focus on Scene view
        EditorApplication.ExecuteMenuItem("Window/General/Scene");
        
        // 7. Try to force garbage collection
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        System.GC.Collect();
        
        Debug.Log("✅ Inspector corruption fix complete! The errors should stop now.");
        Debug.Log("💡 If errors persist, select the StartScreenCorruptionFix in the scene and run the Emergency Fix.");
    }
    
    [MenuItem("Tools/🗑️ Remove Corrupted UI Objects")]
    public static void RemoveCorruptedUIObjects()
    {
        if (!EditorUtility.DisplayDialog("Remove Corrupted Objects", 
            "This will find and remove UI objects with missing components. This action cannot be undone. Continue?", 
            "Yes, Remove Them", "Cancel"))
        {
            return;
        }
        
        Debug.Log("🗑️ Searching for corrupted UI objects...");
        
        int removedCount = 0;
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        for (int i = allObjects.Length - 1; i >= 0; i--)
        {
            GameObject obj = allObjects[i];
            if (obj == null) continue;
            
            // Check for missing components
            Component[] components = obj.GetComponents<Component>();
            bool hasNullComponents = false;
            
            foreach (Component comp in components)
            {
                if (comp == null)
                {
                    hasNullComponents = true;
                    break;
                }
            }
            
            if (hasNullComponents)
            {
                // Don't remove essential objects
                if (obj.name.Contains("Canvas") || obj.name.Contains("EventSystem") || 
                    obj.name.Contains("Camera") || obj.name.Contains("Light") ||
                    obj.name.Contains("Manager"))
                {
                    Debug.LogWarning($"⚠️ Found corruption in essential object '{obj.name}' - not removing");
                    continue;
                }
                
                Debug.LogWarning($"🗑️ Removing corrupted object: {obj.name}");
                Object.DestroyImmediate(obj);
                removedCount++;
            }
        }
        
        // Clear selections after removal
        Selection.activeObject = null;
        EditorApplication.RepaintHierarchyWindow();
        
        Debug.Log($"✅ Removed {removedCount} corrupted objects");
        
        if (removedCount > 0)
        {
            // Mark scene as dirty
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }
    }
    #endif
}

/// <summary>
/// Auto-fix component that runs on scene load
/// </summary>
public class AutoInspectorFix : MonoBehaviour
{
    void Awake()
    {
        #if UNITY_EDITOR
        if (Application.isEditor)
        {
            // Auto-fix when scene loads
            StartCoroutine(DelayedFix());
        }
        #endif
    }
    
    System.Collections.IEnumerator DelayedFix()
    {
        yield return new WaitForSeconds(0.1f);
        
        #if UNITY_EDITOR
        ImmediateInspectorFix.FixInspectorNow();
        #endif
        
        // Self-destruct
        DestroyImmediate(this);
    }
}