using UnityEngine;

/// <summary>
/// Professional lobby setup - organized version
/// </summary>
public class ProfessionalLobbySetup : MonoBehaviour
{
    [Header("🏢 PROFESSIONAL LOBBY SETUP")]
    [Space(10)]
    [Tooltip("✅ Click this to create your complete lobby!")]
    public bool createLobby = false;

    [Header("⚙️ Settings")]
    public string gameSceneName = "SampleScene";

    void OnValidate()
    {
        if (createLobby)
        {
            CreateCompleteLobby();
            createLobby = false;
        }
    }

    [ContextMenu("🏢 Create Complete Lobby")]
    public void CreateCompleteLobby()
    {
        Debug.Log("🏢 Creating complete lobby setup...");

        // Use the MultiSceneSetupHelper to create lobby components
        GameObject setupHelper = new GameObject("MultiSceneSetupHelper");
        MultiSceneSetupHelper helper = setupHelper.AddComponent<MultiSceneSetupHelper>();
        
        // Set scene names (using existing properties)
        helper.gameSceneName = gameSceneName;
        helper.lobbySceneName = "LobbyScene";
        
        // Create the lobby scene setup
        helper.SetupLobbyScene();
        
        // Clean up the helper
        DestroyImmediate(setupHelper);
        
        // Remove this component after setup
        DestroyImmediate(this);
        
        Debug.Log("✅ Lobby setup complete!");
    }
}