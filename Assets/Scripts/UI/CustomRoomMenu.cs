using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Alteruna;
using Alteruna.Trinity;

/// <summary>
/// Modified Alteruna Room Menu that goes to game scene when room is joined
/// </summary>
public class CustomRoomMenu : CommunicationBridge
{
    [Header("🎮 UI References")]
    [SerializeField] public TMP_Text titleText;
    [SerializeField] public Button startButton;
    [SerializeField] public Button leaveButton;
    [SerializeField] public Button backToMenuButton;
    
    [Header("🎯 Game Scene Settings")]
    [Tooltip("Name of the scene to load when joining/creating a room")]
    public string gameSceneName = "SampleScene";
    
    [Tooltip("Delay before loading game scene (allows network setup)")]
    public float sceneLoadDelay = 1f;
    
    [Tooltip("Show countdown before loading game")]
    public bool showCountdown = true;

    [Header("🔧 Room Settings")]
    [Tooltip("Maximum players per room")]
    public int maxPlayers = 4;
    
    [Tooltip("Room name to create/join")]
    public string defaultRoomName = "EscapeRoom";

    private bool isInRoom = false;
    private bool isLoadingGame = false;

    private void Start()
    {
        SetupMultiplayer();
        SetupButtons();
        SetupUI();
    }

    private void SetupMultiplayer()
    {
        if (Multiplayer == null)
        {
            Multiplayer = FindFirstObjectByType<Multiplayer>();
        }

        if (Multiplayer == null)
        {
            Debug.LogError("❌ Unable to find Multiplayer component!");
            if (titleText != null) titleText.text = "Missing Multiplayer Component";
            enabled = false;
            return;
        }

        // Setup Alteruna events
        Multiplayer.OnConnected.AddListener(OnConnected);
        Multiplayer.OnDisconnected.AddListener(OnDisconnected);
        Multiplayer.OnRoomJoined.AddListener(OnRoomJoined);
        Multiplayer.OnRoomLeft.AddListener(OnRoomLeft);

        // If already connected
        if (Multiplayer.IsConnected)
        {
            OnConnected(Multiplayer, null);
        }
    }

    private void SetupButtons()
    {
        // Start/Create Room button
        if (startButton != null)
        {
            startButton.onClick.AddListener(CreateOrJoinRoom);
            startButton.interactable = false;
        }

        // Leave Room button
        if (leaveButton != null)
        {
            leaveButton.onClick.AddListener(LeaveRoom);
            leaveButton.interactable = false;
        }

        // Back to Menu button
        if (backToMenuButton != null)
        {
            backToMenuButton.onClick.AddListener(BackToStartScreen);
        }
    }

    private void SetupUI()
    {
        if (titleText != null)
        {
            titleText.text = "Connecting...";
        }

        // Make sure cursor is visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    #region Button Actions

    [ContextMenu("🚀 Create/Join Room")]
    public void CreateOrJoinRoom()
    {
        if (Multiplayer == null || !Multiplayer.IsConnected || isInRoom) return;

        Debug.Log($"🚀 Creating/Joining room: {defaultRoomName}");
        
        if (titleText != null)
            titleText.text = "Joining Room...";

        // Use Alteruna's method to join or create room on demand
        Multiplayer.JoinOnDemandRoom();
    }

    [ContextMenu("🚪 Leave Room")]
    public void LeaveRoom()
    {
        if (Multiplayer == null || !isInRoom) return;

        Debug.Log("🚪 Leaving room...");
        Multiplayer.CurrentRoom?.Leave();
    }

    [ContextMenu("🏠 Back to Start Screen")]
    public void BackToStartScreen()
    {
        Debug.Log("🏠 Going back to start screen...");
        
        // Leave room if in one
        if (isInRoom)
        {
            LeaveRoom();
        }

        // Go back to start screen
        GameSceneManager.GoToStartScreen();
    }

    #endregion

    #region Alteruna Events

    private void OnConnected(Multiplayer multiplayer, Endpoint endpoint)
    {
        Debug.Log("✅ Connected to Alteruna!");
        
        // If already in room
        if (multiplayer.InRoom)
        {
            OnRoomJoined(multiplayer, multiplayer.CurrentRoom, multiplayer.Me);
            return;
        }

        // Enable start button
        if (startButton != null)
            startButton.interactable = true;

        if (titleText != null)
            titleText.text = "Ready to Play!";
    }

    private void OnDisconnected(Multiplayer multiplayer, Endpoint endpoint)
    {
        Debug.LogWarning("⚠️ Disconnected from Alteruna");
        
        isInRoom = false;
        
        if (startButton != null)
            startButton.interactable = false;
            
        if (leaveButton != null)
            leaveButton.interactable = false;

        if (titleText != null)
            titleText.text = "Reconnecting...";
    }

    private void OnRoomJoined(Multiplayer multiplayer, Room room, User user)
    {
        Debug.Log($"🎮 Joined room: {room.Name} (Players: {room.GetUserCount()}/{room.MaxUsers})");
        
        isInRoom = true;

        // Update UI
        if (startButton != null)
            startButton.interactable = false;
            
        if (leaveButton != null)
            leaveButton.interactable = true;

        if (titleText != null)
            titleText.text = $"Room: {room.Name} ({room.GetUserCount()}/{room.MaxUsers})";

        // Start loading the game scene
        if (!isLoadingGame)
        {
            StartCoroutine(LoadGameSceneDelayed());
        }
    }

    private void OnRoomLeft(Multiplayer multiplayer)
    {
        Debug.Log("🚪 Left room");
        
        isInRoom = false;
        isLoadingGame = false;

        if (startButton != null)
            startButton.interactable = true;
            
        if (leaveButton != null)
            leaveButton.interactable = false;

        if (titleText != null)
            titleText.text = "Ready to Play!";
    }

    #endregion

    #region Scene Loading

    private IEnumerator LoadGameSceneDelayed()
    {
        isLoadingGame = true;
        
        Debug.Log($"🎯 Loading game scene in {sceneLoadDelay} seconds...");

        if (showCountdown && titleText != null)
        {
            for (int i = (int)sceneLoadDelay; i > 0; i--)
            {
                titleText.text = $"Starting Game in {i}...";
                yield return new WaitForSeconds(1f);
            }
        }
        else
        {
            if (titleText != null)
                titleText.text = "Loading Game...";
            yield return new WaitForSeconds(sceneLoadDelay);
        }

        // Load the game scene
        LoadGameScene();
    }

    private void LoadGameScene()
    {
        Debug.Log($"🎮 Loading game scene: {gameSceneName}");
        
        // Use GameSceneManager if available, otherwise direct load
        if (GameSceneManager.Instance != null)
        {
            GameSceneManager.Instance.LoadGameScene();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
        }
    }

    #endregion

    #region Public Methods for UI

    public void OnStartButtonClicked()
    {
        CreateOrJoinRoom();
    }

    public void OnLeaveButtonClicked()
    {
        LeaveRoom();
    }

    public void OnBackButtonClicked()
    {
        BackToStartScreen();
    }

    #endregion
}