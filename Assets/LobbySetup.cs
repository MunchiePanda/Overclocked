using UnityEngine;

/// <summary>
/// Quick lobby setup - just click the checkbox! (RENAMED TO AVOID CONFLICTS)
/// </summary>
public class DuplicateLobbySetup : MonoBehaviour
{
    [Header("🏢 DUPLICATE - USE EasyLobbySetup INSTEAD")]
    [Space(10)]
    [Tooltip("⚠️ This is a duplicate! Use EasyLobbySetup instead")]
    public bool createLobby = false;

    [Header("⚙️ Settings")]
    public string gameSceneName = "SampleScene";
    public string startScreenScene = "StartScreen";

    void OnValidate()
    {
        if (createLobby)
        {
            Debug.LogWarning("⚠️ This is a duplicate script! Use EasyLobbySetup instead.");
            createLobby = false;
        }
    }
}