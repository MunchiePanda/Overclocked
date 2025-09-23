using UnityEngine;

/// <summary>
/// Simple test for the direct door unlock -> end screen approach
/// </summary>
public class SimpleEndScreenTest : MonoBehaviour
{
    [Header("Testing")]
    [Tooltip("Press this key to unlock the first door and show end screen")]
    public KeyCode testUnlockKey = KeyCode.F8;
    
    [Tooltip("Test door code to use")]
    public string testCode = "1234";
    
    void Update()
    {
        if (Input.GetKeyDown(testUnlockKey))
        {
            TestDirectDoorUnlock();
        }
    }
    
    void TestDirectDoorUnlock()
    {
        // Find the first door
        Door door = FindFirstObjectByType<Door>();
        
        if (door != null)
        {
            Debug.Log($"🧪 Testing direct door unlock with code: {testCode}");
            
            // Set a simple test code and unlock
            door.correctCode = testCode;
            door.UnlockDoor(testCode);
            
            Debug.Log($"🚪 Door unlock attempt completed. IsUnlocked: {door.IsUnlocked}");
        }
        else
        {
            Debug.LogError("❌ No Door component found!");
        }
    }
    
    void OnGUI()
    {
        GUI.color = Color.cyan;
        GUI.Label(new Rect(10, Screen.height - 40, 200, 30), $"Press {testUnlockKey} to test door unlock");
        GUI.color = Color.white;
    }
    
    [ContextMenu("Test Door Unlock")]
    public void TestDoorUnlockContext()
    {
        TestDirectDoorUnlock();
    }
}