using UnityEngine;

/// <summary>
/// Simple script to test if compilation issues are resolved
/// </summary>
public class BuildCompilationTest : MonoBehaviour
{
    [ContextMenu("🧪 Test Compilation")]
    public void TestCompilation()
    {
        Debug.Log("🧪 Testing compilation of previously problematic classes...");
        
        // Test RiddleTerminalSetup
        try
        {
            GameObject testObj = new GameObject("CompilationTest");
            RiddleTerminalSetup riddleSetup = testObj.AddComponent<RiddleTerminalSetup>();
            if (riddleSetup != null)
            {
                Debug.Log("✅ RiddleTerminalSetup class is accessible");
            }
            DestroyImmediate(testObj);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ RiddleTerminalSetup error: {e.Message}");
        }
        
        // Test EscapeRoomSetupHelper
        try
        {
            GameObject testObj = new GameObject("CompilationTest2");
            EscapeRoomSetupHelper escapeSetup = testObj.AddComponent<EscapeRoomSetupHelper>();
            if (escapeSetup != null)
            {
                Debug.Log("✅ EscapeRoomSetupHelper class is accessible");
            }
            DestroyImmediate(testObj);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ EscapeRoomSetupHelper error: {e.Message}");
        }
        
        Debug.Log("🎯 Compilation test complete! Classes should now build properly.");
        
        // Auto-destroy this test
        DestroyImmediate(this);
    }
}