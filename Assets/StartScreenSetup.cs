using UnityEngine;

/// <summary>
/// Quick setup for start screen - just click the checkbox!
/// </summary>
public class StartScreenSetup : MonoBehaviour
{
    [Header("🚀 QUICK START SCREEN SETUP")]
    [Space(10)]
    [Tooltip("✅ Click this to create your complete start screen!")]
    public bool createStartScreen = false;

    void OnValidate()
    {
        if (createStartScreen)
        {
            // Use the safe scene setup creator
            GameObject setup = new GameObject("SafeStartScreenCreator");
            SafeStartScreenCreator safeCreator = setup.AddComponent<SafeStartScreenCreator>();
            
            // Set the scene names
            safeCreator.lobbySceneName = "LobbyScene";
            safeCreator.gameSceneName = "SampleScene";
            
            // Create the start screen safely
            safeCreator.CreateSafeStartScreen();
            
            // Clean up
            DestroyImmediate(setup);
            DestroyImmediate(this);
            
            createStartScreen = false;
        }
    }
}