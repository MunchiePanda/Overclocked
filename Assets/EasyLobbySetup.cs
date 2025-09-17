using UnityEngine;

/// <summary>
/// Easy lobby setup without conflicts
/// </summary>
public class EasyLobbySetup : MonoBehaviour
{
    [Header("🏢 EASY LOBBY SETUP")]
    [Space(10)]
    [Tooltip("✅ Click this to create your complete lobby!")]
    public bool setupLobby = false;

    void OnValidate()
    {
        if (setupLobby)
        {
            CreateLobby();
            setupLobby = false;
        }
    }

    [ContextMenu("🏢 Create Lobby")]
    public void CreateLobby()
    {
        Debug.Log("🏢 Creating lobby setup...");

        // Use the MultiSceneSetupHelper
        GameObject helper = new GameObject("MultiSceneSetupHelper");
        MultiSceneSetupHelper setup = helper.AddComponent<MultiSceneSetupHelper>();
        
        // Set the scene names (these properties exist)
        setup.gameSceneName = "SampleScene";
        setup.lobbySceneName = "LobbyScene";
        
        // Create the lobby
        setup.SetupLobbyScene();
        
        // Clean up
        DestroyImmediate(helper);
        DestroyImmediate(this);
        
        Debug.Log("✅ Lobby setup complete!");
    }
}