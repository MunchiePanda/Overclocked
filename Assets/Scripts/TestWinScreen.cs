using UnityEngine;

/// <summary>
/// Simple script to test the win screen functionality
/// </summary>
public class TestWinScreen : MonoBehaviour
{
    [ContextMenu("🏆 Test Show Win Screen")]
    public void TestShowWin()
    {
        // Find the escape code manager
        EscapeCodeManager escapeManager = FindFirstObjectByType<EscapeCodeManager>();
        if (escapeManager != null)
        {
            // Trigger the escape sequence
            escapeManager.OnEscapeSuccessful();
            Debug.Log("✅ Win screen test triggered!");
        }
        else
        {
            Debug.LogWarning("❌ No EscapeCodeManager found in scene!");
        }
    }
    
    [ContextMenu("🔓 Test Unlock Door")]
    public void TestUnlockDoor()
    {
        // Find the door
        Door door = FindFirstObjectByType<Door>();
        if (door != null)
        {
            EscapeCodeManager escapeManager = FindFirstObjectByType<EscapeCodeManager>();
            if (escapeManager != null && escapeManager.IsEscapeCodeReady())
            {
                // Unlock with the correct escape code
                door.UnlockDoor(escapeManager.GetFinalEscapeCode());
                Debug.Log("✅ Door unlocked with correct escape code!");
            }
            else
            {
                Debug.LogWarning("❌ Escape code not ready! Complete puzzles first.");
            }
        }
        else
        {
            Debug.LogWarning("❌ No Door found in scene!");
        }
    }
    
    [ContextMenu("🧪 Generate Test Escape Code")]
    public void GenerateTestCode()
    {
        EscapeCodeManager escapeManager = FindFirstObjectByType<EscapeCodeManager>();
        if (escapeManager != null)
        {
            // Add test puzzle numbers
            escapeManager.OnPuzzleCompleted(1234);
            escapeManager.OnPuzzleCompleted(5678);
            escapeManager.OnPuzzleCompleted(9012);
            
            Debug.Log("✅ Test puzzle numbers added! Escape code should be generated.");
        }
        else
        {
            Debug.LogWarning("❌ No EscapeCodeManager found in scene!");
        }
    }
}