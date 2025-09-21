using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Cleanup tool to remove unused scripts and components
/// </summary>
public class CleanupUnusedScripts : MonoBehaviour
{
    [Header("🧹 Cleanup Options")]
    [Tooltip("Remove frequency puzzle related scripts")]
    public bool removeFrequencyPuzzle = true;
    
    [Tooltip("Remove debug and temporary scripts")]
    public bool removeDebugScripts = true;
    
    [Tooltip("Remove duplicate/obsolete scripts")]
    public bool removeDuplicateScripts = true;
    
    [Tooltip("Click to perform cleanup")]
    public bool performCleanup = false;

    [Header("📋 Scripts to Remove")]
    [Tooltip("List of script files to delete (relative to Assets/)")]
    public string[] scriptsToDelete = {
        "Scripts/Puzzles/FrequencyResonancePuzzle.cs",
        "Scripts/CleanupAITerminal.cs",
        "Scripts/CleanupDuplicatePauseManagers.cs", 
        "Scripts/FixPauseMenuNow.cs",
        "Scripts/Puzzles/CleanupObsoleteFiles.cs",
        "Scripts/Puzzles/TempCleanup.cs",
        "Scripts/Puzzles/PuzzleManagerFixed.cs",
        "Scripts/Puzzles/SceneSetupHelper.cs",
        "Scripts/Puzzles/UIConnectionHelper.cs",
        "Scripts/Puzzles/InteractableAITerminal.cs", // If InteractableAITerminalNew is the final version
        "Scripts/Debug/FixTerminalSetup.cs",
        "Scripts/Debug/TerminalDebugHelper.cs",
        "Scripts/UI/SafeStartScreenCreator.cs",
        "Scripts/UI/StartScreenUICreator.cs",
        "Scripts/UI/MultiSceneSetupHelper.cs",
        "Scripts/Utils/Unity6APIFixer.cs"
    };

    private void OnValidate()
    {
        if (performCleanup)
        {
            PerformCleanup();
            performCleanup = false;
        }
    }

    [ContextMenu("🧹 Cleanup Unused Scripts")]
    public void PerformCleanup()
    {
        Debug.Log("🧹 Starting cleanup of unused scripts...");
        
        List<string> deletedFiles = new List<string>();
        List<string> notFoundFiles = new List<string>();
        
        foreach (string scriptPath in scriptsToDelete)
        {
            string fullPath = "/Assets/" + scriptPath;
            
            // Check if file exists
            if (System.IO.File.Exists(Application.dataPath + "/" + scriptPath))
            {
                try
                {
                    System.IO.File.Delete(Application.dataPath + "/" + scriptPath);
                    
                    // Also delete .meta file if it exists
                    string metaPath = Application.dataPath + "/" + scriptPath + ".meta";
                    if (System.IO.File.Exists(metaPath))
                    {
                        System.IO.File.Delete(metaPath);
                    }
                    
                    deletedFiles.Add(scriptPath);
                    Debug.Log($"✅ Deleted: {scriptPath}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"❌ Failed to delete {scriptPath}: {e.Message}");
                }
            }
            else
            {
                notFoundFiles.Add(scriptPath);
                Debug.Log($"⚠️ File not found: {scriptPath}");
            }
        }

        // Clean up empty directories
        CleanupEmptyDirectories();
        
        // Refresh the Asset Database
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif
        
        // Summary
        Debug.Log($"🎉 Cleanup complete!");
        Debug.Log($"✅ Deleted {deletedFiles.Count} files");
        Debug.Log($"⚠️ {notFoundFiles.Count} files not found");
        
        if (deletedFiles.Count > 0)
        {
            Debug.Log("📋 Deleted files:");
            foreach (string file in deletedFiles)
            {
                Debug.Log($"  • {file}");
            }
        }
    }

    private void CleanupEmptyDirectories()
    {
        string[] directoriesToCheck = {
            "Assets/Scripts/Debug",
            "Assets/Scripts/Utils"
        };

        foreach (string dirPath in directoriesToCheck)
        {
            if (System.IO.Directory.Exists(dirPath))
            {
                string[] files = System.IO.Directory.GetFiles(dirPath, "*", System.IO.SearchOption.AllDirectories);
                
                // Filter out .meta files
                int nonMetaFiles = 0;
                foreach (string file in files)
                {
                    if (!file.EndsWith(".meta"))
                        nonMetaFiles++;
                }
                
                if (nonMetaFiles == 0)
                {
                    try
                    {
                        System.IO.Directory.Delete(dirPath, true);
                        Debug.Log($"🗑️ Deleted empty directory: {dirPath}");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogWarning($"⚠️ Could not delete directory {dirPath}: {e.Message}");
                    }
                }
            }
        }
    }

    [ContextMenu("📋 List All Scripts")]
    public void ListAllScripts()
    {
        Debug.Log("📋 Listing all scripts in project:");
        
        string scriptsPath = Application.dataPath + "/Scripts";
        if (System.IO.Directory.Exists(scriptsPath))
        {
            string[] files = System.IO.Directory.GetFiles(scriptsPath, "*.cs", System.IO.SearchOption.AllDirectories);
            
            foreach (string file in files)
            {
                string relativePath = file.Replace(Application.dataPath + "/", "");
                Debug.Log($"  • {relativePath}");
            }
            
            Debug.Log($"Total scripts found: {files.Length}");
        }
    }

    [ContextMenu("🔍 Find Components In Scene")]
    public void FindComponentsInScene()
    {
        Debug.Log("🔍 Finding components that use scripts to be deleted:");
        
        // Find FrequencyResonancePuzzle components
        FrequencyResonancePuzzle[] frequencyPuzzles = FindObjectsByType<FrequencyResonancePuzzle>(FindObjectsSortMode.None);
        if (frequencyPuzzles.Length > 0)
        {
            Debug.Log($"⚠️ Found {frequencyPuzzles.Length} FrequencyResonancePuzzle components:");
            foreach (var puzzle in frequencyPuzzles)
            {
                Debug.Log($"  • {puzzle.gameObject.name} at path: {GetGameObjectPath(puzzle.gameObject)}");
            }
        }

        // Add checks for other components that might need cleanup
        MonoBehaviour[] allComponents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        List<string> problematicComponents = new List<string>();
        
        foreach (var component in allComponents)
        {
            string typeName = component.GetType().Name;
            if (ShouldRemoveComponent(typeName))
            {
                problematicComponents.Add($"{typeName} on {GetGameObjectPath(component.gameObject)}");
            }
        }
        
        if (problematicComponents.Count > 0)
        {
            Debug.Log("⚠️ Components that will become missing after cleanup:");
            foreach (string component in problematicComponents)
            {
                Debug.Log($"  • {component}");
            }
        }
        else
        {
            Debug.Log("✅ No problematic components found in scene");
        }
    }

    private bool ShouldRemoveComponent(string typeName)
    {
        string[] componentsToRemove = {
            "FrequencyResonancePuzzle",
            "CleanupAITerminal",
            "CleanupDuplicatePauseManagers",
            "FixPauseMenuNow",
            "CleanupObsoleteFiles",
            "TempCleanup",
            "PuzzleManagerFixed",
            "SceneSetupHelper",
            "UIConnectionHelper",
            "InteractableAITerminal", // Only if keeping InteractableAITerminalNew
            "FixTerminalSetup",
            "TerminalDebugHelper",
            "SafeStartScreenCreator",
            "StartScreenUICreator",
            "MultiSceneSetupHelper",
            "Unity6APIFixer"
        };
        
        foreach (string componentName in componentsToRemove)
        {
            if (typeName == componentName)
                return true;
        }
        
        return false;
    }

    private string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;
        
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        return "/" + path;
    }

    [ContextMenu("🔄 Replace Frequency Puzzle with Chess Puzzle")]
    public void ReplaceFrequencyPuzzleWithChess()
    {
        Debug.Log("🔄 Replacing FrequencyResonancePuzzle with ChessPuzzle...");
        
        FrequencyResonancePuzzle[] frequencyPuzzles = FindObjectsByType<FrequencyResonancePuzzle>(FindObjectsSortMode.None);
        
        foreach (FrequencyResonancePuzzle freqPuzzle in frequencyPuzzles)
        {
            GameObject puzzleObject = freqPuzzle.gameObject;
            
            // Remove the old component
            DestroyImmediate(freqPuzzle);
            
            // Add the new chess puzzle component
            ChessPuzzle chessPuzzle = puzzleObject.AddComponent<ChessPuzzle>();
            
            Debug.Log($"✅ Replaced FrequencyResonancePuzzle with ChessPuzzle on {puzzleObject.name}");
        }
        
        // Update puzzle manager references if needed
        UpdatePuzzleManagerReferences();
        
        Debug.Log("🎉 Frequency puzzle replacement complete!");
    }

    private void UpdatePuzzleManagerReferences()
    {
        // Find puzzle managers and update their references
        PuzzleManager[] puzzleManagers = FindObjectsByType<PuzzleManager>(FindObjectsSortMode.None);
        
        foreach (PuzzleManager manager in puzzleManagers)
        {
            // This would need to be customized based on how PuzzleManager stores references
            Debug.Log($"📝 Note: Check PuzzleManager on {manager.gameObject.name} for frequency puzzle references");
        }
    }
}