using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;

/// <summary>
/// Helper script to clean up obsolete puzzle files that are no longer needed
/// </summary>
public class CleanupObsoleteFiles : MonoBehaviour
{
    [Header("Cleanup Control")]
    [Tooltip("Click to delete all obsolete puzzle files")]
    public bool deleteObsoleteFiles = false;

    [Header("Files to Delete")]
    [Tooltip("These files will be permanently deleted")]
    public string[] filesToDelete = {
        "Assets/Scripts/Puzzles/BattleshipPuzzle.cs",
        "Assets/Scripts/Puzzles/ChessController.cs", 
        "Assets/Scripts/Puzzles/MathRiddlePuzzle.cs",
        "Assets/Scripts/Puzzles/StackingPuzzle.cs",
        "Assets/Scripts/Puzzles/PressurePlate.cs",
        "Assets/Scripts/Puzzles/PressurePlatePuzzle", // This is a file, not directory
        "Assets/Scripts/Puzzles/PressurePlatePuzzleManager.cs",
        "Assets/Scripts/Puzzles/PressurePlatePuzzleSetupGuide.md",
        "Assets/Scripts/Puzzles/InteractableTerminal.cs", // Replaced by InteractableAITerminal
        "Assets/Scripts/Puzzles/TerminalController.cs", // Replaced by TerminalControllerNew
        "Assets/Scripts/Puzzles/Pzl1/PowerGridPuzzle.cs",
        "Assets/Scripts/Puzzles/Pzl1/PuzzleButton.cs"
    };

    void OnValidate()
    {
        if (deleteObsoleteFiles)
        {
            DeleteObsoleteFiles();
            deleteObsoleteFiles = false;
        }
    }

    [ContextMenu("Delete Obsolete Files")]
    private void DeleteObsoleteFiles()
    {
        int deletedCount = 0;
        
        foreach (string filePath in filesToDelete)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                File.Delete(filePath + ".meta"); // Delete the meta file too
                deletedCount++;
                Debug.Log($"Deleted: {filePath}");
            }
            else
            {
                Debug.LogWarning($"File not found: {filePath}");
            }
        }

        // Check if Pzl1 directory is empty and delete it
        string pzl1Directory = "Assets/Scripts/Puzzles/Pzl1";
        if (Directory.Exists(pzl1Directory))
        {
            string[] remainingFiles = Directory.GetFiles(pzl1Directory);
            if (remainingFiles.Length == 0)
            {
                Directory.Delete(pzl1Directory);
                File.Delete(pzl1Directory + ".meta");
                Debug.Log("Deleted empty Pzl1 directory");
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"Cleanup complete! Deleted {deletedCount} obsolete files.");
        
        // Show summary of what's left
        ShowRemainingFiles();
    }

    private void ShowRemainingFiles()
    {
        Debug.Log("=== REMAINING PUZZLE SCRIPTS ===");
        Debug.Log("✓ BasePuzzle.cs - Base class for all puzzles");
        Debug.Log("✓ CipherWheelPuzzle.cs - New escape room puzzle");
        Debug.Log("✓ FrequencyResonancePuzzle.cs - New escape room puzzle");
        Debug.Log("✓ ShadowLogicPuzzle.cs - New escape room puzzle");
        Debug.Log("✓ EscapeCodeManager.cs - Manages escape room system");
        Debug.Log("✓ TerminalControllerNew.cs - New AI terminal system");
        Debug.Log("✓ InteractableAITerminal.cs - New terminal interaction");
        Debug.Log("✓ WinScreen.cs - Victory screen");
        Debug.Log("✓ Door.cs - Updated door system");
        Debug.Log("✓ CodeDisplay.cs - Code display system");
        Debug.Log("✓ PuzzleManager.cs - Can be updated to work with new system");
        Debug.Log("✓ CameraSwitcher.cs - Camera switching utility");
        Debug.Log("✓ EscapeRoomSetupHelper.cs - Setup automation tool");
        Debug.Log("✓ EscapeRoomSetupGuide.md - Setup documentation");
        Debug.Log("================================");
    }

    [ContextMenu("Show File Status")]
    private void ShowFileStatus()
    {
        Debug.Log("=== OBSOLETE FILE STATUS ===");
        foreach (string filePath in filesToDelete)
        {
            bool exists = File.Exists(filePath);
            Debug.Log($"{(exists ? "❌" : "✅")} {filePath} - {(exists ? "Still exists" : "Deleted")}");
        }
        Debug.Log("============================");
    }
}

#else
// Empty class for builds
public class CleanupObsoleteFiles : MonoBehaviour { }
#endif