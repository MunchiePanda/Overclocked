using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// Utility to fix Unity 6 API deprecation warnings across all scripts
/// </summary>
public class Unity6APIFixer : MonoBehaviour
{
    [Header("🔧 UNITY 6 API FIXER")]
    [Space(10)]
    [Tooltip("Fix all FindObjectOfType deprecation warnings in project")]
    public bool fixAllScripts = false;

    void OnValidate()
    {
        if (fixAllScripts)
        {
            FixAllProjectScripts();
            fixAllScripts = false;
        }
    }

    [ContextMenu("🔧 Fix All Unity 6 API Issues")]
    public void FixAllProjectScripts()
    {
        #if UNITY_EDITOR
        Debug.Log("🔧 Starting Unity 6 API fixes...");

        int fixedFiles = 0;

        // Get all C# scripts in the project
        string[] scriptPaths = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

        foreach (string scriptPath in scriptPaths)
        {
            // Skip Alteruna scripts - they'll update themselves
            if (scriptPath.Contains("Alteruna"))
                continue;

            if (FixScript(scriptPath))
                fixedFiles++;
        }

        Debug.Log($"✅ Fixed {fixedFiles} script files for Unity 6 compatibility");
        
        // Refresh the asset database
        UnityEditor.AssetDatabase.Refresh();
        #else
        Debug.LogWarning("⚠️ API fixer only works in Editor");
        #endif
    }

    #if UNITY_EDITOR
    private bool FixScript(string scriptPath)
    {
        try
        {
            string content = File.ReadAllText(scriptPath);
            string originalContent = content;

            // Fix FindObjectOfType -> FindFirstObjectByType
            content = Regex.Replace(content, 
                @"FindObjectOfType<([^>]+)>\(\)", 
                "FindFirstObjectByType<$1>()");

            // Fix FindObjectsOfType -> FindObjectsByType with None sort mode
            content = Regex.Replace(content, 
                @"FindObjectsOfType<([^>]+)>\(\)", 
                "FindObjectsByType<$1>(FindObjectsSortMode.None)");

            // Check if changes were made
            if (content != originalContent)
            {
                File.WriteAllText(scriptPath, content);
                
                string relativePath = scriptPath.Substring(Application.dataPath.Length - 6); // Remove "Assets" from dataPath
                Debug.Log($"🔧 Fixed: {relativePath}");
                return true;
            }

            return false;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error fixing script {scriptPath}: {e.Message}");
            return false;
        }
    }
    #endif
}