using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Quick test script to create and test riddle terminals
/// </summary>
public class QuickRiddleTerminalTest : MonoBehaviour
{
    [Header("Quick Terminal Test")]
    [Tooltip("Position where to create the test terminal")]
    public Vector3 terminalPosition = new Vector3(0, 0, 5f);
    
    [Tooltip("Enable debug logging")]
    public bool enableDebugLogs = true;

    [Header("Test Status")]
    [SerializeField]
    private bool terminalCreated = false;
    
    [SerializeField]
    private bool riddlePuzzleFound = false;
    
    [SerializeField]
    private GameObject testTerminal;

    void Start()
    {
        if (enableDebugLogs)
        {
            Debug.Log("QuickRiddleTerminalTest: Starting test...");
        }
        
        CheckForRiddlePuzzle();
    }

    void CheckForRiddlePuzzle()
    {
        RiddlePuzzle riddlePuzzle = FindFirstObjectByType<RiddlePuzzle>();
        riddlePuzzleFound = riddlePuzzle != null;
        
        if (enableDebugLogs)
        {
            if (riddlePuzzleFound)
            {
                Debug.Log($"✅ Found RiddlePuzzle: {riddlePuzzle.puzzleName}");
            }
            else
            {
                Debug.LogWarning("⚠️ No RiddlePuzzle found in scene. Terminal will be created but won't be functional until you create a riddle puzzle.");
            }
        }
    }

    [ContextMenu("Create Test Terminal")]
    public void CreateTestTerminal()
    {
        if (terminalCreated && testTerminal != null)
        {
            Debug.LogWarning("Test terminal already exists! Use 'Remove Test Terminal' first.");
            return;
        }

        Debug.Log("Creating test riddle terminal...");

        try
        {
            // Create terminal setup helper
            GameObject setupHelper = new GameObject("TestTerminalSetup");
            setupHelper.transform.position = terminalPosition;
            
            RiddleTerminalSetup setup = setupHelper.AddComponent<RiddleTerminalSetup>();
            setup.terminalName = "Test Riddle Terminal";
            setup.terminalPosition = terminalPosition;
            setup.terminalMessage = "TEST TERMINAL\n\nPress E to test riddle system\n\nStatus: TESTING";
            
            // Create the terminal
            setup.CreateRiddleTerminal();
            
            // Store reference
            testTerminal = setup.createdTerminal;
            terminalCreated = testTerminal != null;
            
            // Clean up the setup helper
            DestroyImmediate(setupHelper);
            
            if (terminalCreated)
            {
                Debug.Log("✅ Test terminal created successfully!");
                Debug.Log("Walk up to the terminal and press E to test the interaction.");
                
                // Move the terminal slightly forward from this object
                if (testTerminal != null)
                {
                    testTerminal.transform.position = transform.position + Vector3.forward * 3f;
                }
            }
            else
            {
                Debug.LogError("❌ Failed to create test terminal");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error creating test terminal: {e.Message}");
        }
    }

    [ContextMenu("Remove Test Terminal")]
    public void RemoveTestTerminal()
    {
        if (testTerminal != null)
        {
            DestroyImmediate(testTerminal);
            Debug.Log("🗑️ Test terminal removed");
        }
        
        terminalCreated = false;
        testTerminal = null;
    }

    [ContextMenu("Test Terminal Interaction")]
    public void TestTerminalInteraction()
    {
        if (testTerminal != null)
        {
            RiddleTerminal terminal = testTerminal.GetComponent<RiddleTerminal>();
            if (terminal != null)
            {
                Debug.Log("🧪 Testing terminal interaction...");
                
                // Simulate player interaction
                terminal.OnHover();
                Debug.Log("Simulated hover...");
                
                terminal.OnInteract();
                Debug.Log("Simulated interaction...");
                
                Debug.Log("✅ Terminal interaction test completed. Check console for results.");
            }
            else
            {
                Debug.LogError("❌ No RiddleTerminal component found on test terminal");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No test terminal exists. Create one first.");
        }
    }

    [ContextMenu("Check Terminal Status")]
    public void CheckTerminalStatus()
    {
        Debug.Log("=== RIDDLE TERMINAL TEST STATUS ===");
        Debug.Log($"Terminal Created: {terminalCreated}");
        Debug.Log($"Riddle Puzzle Found: {riddlePuzzleFound}");
        
        if (testTerminal != null)
        {
            RiddleTerminal terminal = testTerminal.GetComponent<RiddleTerminal>();
            if (terminal != null)
            {
                Debug.Log($"Terminal Name: {terminal.terminalName}");
                Debug.Log($"Linked Puzzle: {(terminal.GetLinkedRiddlePuzzle() != null ? "Yes" : "No")}");
                Debug.Log($"Player Nearby: {terminal.IsPlayerNearby()}");
                Debug.Log($"Terminal Active: {terminal.IsTerminalActive()}");
                Debug.Log($"Riddle Solved: {terminal.IsRiddleSolved()}");
                Debug.Log($"Riddle Active: {terminal.IsRiddleActive()}");
            }
        }
        else
        {
            Debug.Log("Test Terminal: None");
        }
        Debug.Log("=================================");
    }

    void OnDrawGizmos()
    {
        // Draw where the test terminal will be placed
        Gizmos.color = terminalCreated ? Color.green : Color.yellow;
        Gizmos.DrawWireCube(terminalPosition, Vector3.one);
        
        if (terminalCreated)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, terminalPosition);
        }
    }
}

/// <summary>
/// ReadOnly attribute for inspector
/// </summary>
public class ReadOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;
    }
}
#endif