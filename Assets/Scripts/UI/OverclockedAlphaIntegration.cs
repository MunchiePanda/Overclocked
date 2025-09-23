using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Easy integration helper for Overclocked alpha end screen
/// </summary>
public class OverclockedAlphaIntegration : MonoBehaviour
{
    [Header("Overclocked Alpha Settings")]
    [Tooltip("Current alpha version")]
    public string alphaVersion = "Alpha v0.1.0";
    
    [Tooltip("Your contact email for feedback")]
    public string feedbackEmail = "feedback@yourstudio.com";
    
    [Tooltip("Your social media handle")]
    public string socialHandle = "@YourGameDev";
    
    [Tooltip("Automatically setup everything")]
    public bool autoSetupOnStart = true;
    
    [Header("Component References")]
    public AlphaEndScreenHelper endScreenHelper;
    public EscapeCodeManager escapeCodeManager;
    public Canvas mainCanvas;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupOverclockedAlphaExperience();
        }
    }
    
    [ContextMenu("🚀 Setup Complete Overclocked Alpha Experience")]
    public void SetupOverclockedAlphaExperience()
    {
        Debug.Log("🚀 Setting up Overclocked alpha experience...");
        
        // Find components if not assigned
        FindComponents();
        
        // Create or setup the alpha end screen helper
        SetupAlphaEndScreenHelper();
        
        // Configure the settings
        ConfigureAlphaSettings();
        
        // Create the end screen UI
        CreateAlphaEndScreen();
        
        // Connect to escape system
        ConnectToEscapeSystem();
        
        Debug.Log("✅ Overclocked alpha experience setup complete!");
        Debug.Log($"🎮 Game: Overclocked {alphaVersion}");
        Debug.Log($"📧 Feedback: {feedbackEmail}");
        Debug.Log($"🐦 Social: {socialHandle}");
    }
    
    private void FindComponents()
    {
        if (mainCanvas == null)
        {
            mainCanvas = FindFirstObjectByType<Canvas>();
        }
        
        if (escapeCodeManager == null)
        {
            escapeCodeManager = FindFirstObjectByType<EscapeCodeManager>();
        }
        
        if (endScreenHelper == null)
        {
            endScreenHelper = FindFirstObjectByType<AlphaEndScreenHelper>();
        }
    }
    
    private void SetupAlphaEndScreenHelper()
    {
        if (endScreenHelper == null)
        {
            // Create a new GameObject for the helper
            GameObject helperObject = new GameObject("AlphaEndScreenHelper");
            endScreenHelper = helperObject.AddComponent<AlphaEndScreenHelper>();
            Debug.Log("✅ Created AlphaEndScreenHelper");
        }
    }
    
    private void ConfigureAlphaSettings()
    {
        if (endScreenHelper != null)
        {
            endScreenHelper.gameVersion = alphaVersion;
            endScreenHelper.gameTitle = "OVERCLOCKED";
            endScreenHelper.developerName = "Your Studio"; // You can customize this
            endScreenHelper.targetCanvas = mainCanvas;
            endScreenHelper.escapeCodeManager = escapeCodeManager;
            
            Debug.Log("✅ Configured alpha settings");
        }
    }
    
    private void CreateAlphaEndScreen()
    {
        if (endScreenHelper != null)
        {
            endScreenHelper.CreateAlphaEndScreen();
            
            // Customize the feedback text with your actual contact info
            UpdateFeedbackText();
            
            Debug.Log("✅ Created alpha end screen");
        }
    }
    
    private void UpdateFeedbackText()
    {
        // Find the feedback text and update it with actual contact info
        if (endScreenHelper.targetCanvas != null)
        {
            Transform endScreen = endScreenHelper.targetCanvas.transform.Find("AlphaEndScreen");
            if (endScreen != null)
            {
                Transform feedbackContainer = endScreen.Find("FeedbackContainer");
                if (feedbackContainer != null)
                {
                    Transform feedbackText = feedbackContainer.Find("FeedbackText");
                    if (feedbackText != null)
                    {
                        var textComponent = feedbackText.GetComponent<TMPro.TextMeshProUGUI>();
                        if (textComponent != null)
                        {
                            string customFeedbackText = "💭 Share Your Thoughts:\n" +
                                                      "• What did you enjoy most?\n" +
                                                      "• Which puzzles were your favorite?\n" +
                                                      "• What would you like to see in future updates?\n" +
                                                      "• Any bugs or issues you encountered?\n\n" +
                                                      $"📧 Send feedback to: {feedbackEmail}\n" +
                                                      $"🐦 Follow development: {socialHandle}";
                            
                            textComponent.text = customFeedbackText;
                        }
                    }
                }
            }
        }
    }
    
    private void ConnectToEscapeSystem()
    {
        if (endScreenHelper != null)
        {
            endScreenHelper.ConnectToEscapeManager();
            Debug.Log("✅ Connected to escape system");
        }
    }
    
    [ContextMenu("🎮 Test Alpha End Screen")]
    public void TestAlphaEndScreen()
    {
        FindComponents();
        
        if (endScreenHelper != null)
        {
            endScreenHelper.TestEndScreen();
        }
        else
        {
            Debug.LogError("❌ AlphaEndScreenHelper not found! Run setup first.");
        }
    }
    
    [ContextMenu("📧 Update Contact Information")]
    public void UpdateContactInformation()
    {
        UpdateFeedbackText();
        Debug.Log($"✅ Updated contact info: {feedbackEmail} | {socialHandle}");
    }
    
    [ContextMenu("🧹 Clean Reset")]
    public void CleanReset()
    {
        if (endScreenHelper != null)
        {
            endScreenHelper.RemoveEndScreen();
        }
        
        if (escapeCodeManager != null)
        {
            escapeCodeManager.ResetEscapeProgress();
        }
        
        Debug.Log("🧹 Clean reset complete");
    }
    
    // Quick validation
    void OnValidate()
    {
        // Ensure version format
        if (!alphaVersion.ToLower().Contains("alpha"))
        {
            alphaVersion = "Alpha " + alphaVersion;
        }
        
        // Ensure email format hint
        if (feedbackEmail == "feedback@yourstudio.com")
        {
            Debug.LogWarning("⚠️ Remember to update feedbackEmail with your actual contact!");
        }
        
        // Ensure social handle format
        if (!socialHandle.StartsWith("@"))
        {
            socialHandle = "@" + socialHandle;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(OverclockedAlphaIntegration))]
public class OverclockedAlphaIntegrationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("🎮 Overclocked Alpha Setup", EditorStyles.boldLabel);
        
        OverclockedAlphaIntegration integration = (OverclockedAlphaIntegration)target;
        
        // Main setup button
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Quick Setup", EditorStyles.boldLabel);
        if (GUILayout.Button("🚀 Setup Complete Alpha Experience", GUILayout.Height(45)))
        {
            integration.SetupOverclockedAlphaExperience();
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
        
        // Testing tools
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Testing Tools", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🎮 Test End Screen", GUILayout.Height(30)))
        {
            integration.TestAlphaEndScreen();
        }
        
        if (GUILayout.Button("📧 Update Contact", GUILayout.Height(30)))
        {
            integration.UpdateContactInformation();
        }
        EditorGUILayout.EndHorizontal();
        
        if (GUILayout.Button("🧹 Clean Reset", GUILayout.Height(30)))
        {
            integration.CleanReset();
        }
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
        
        // Info and warnings
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Setup Checklist", EditorStyles.boldLabel);
        
        bool hasEmail = integration.feedbackEmail != "feedback@yourstudio.com";
        bool hasSocial = integration.socialHandle != "@YourGameDev";
        bool hasVersion = !string.IsNullOrEmpty(integration.alphaVersion);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(hasEmail ? "✅" : "❌", GUILayout.Width(25));
        EditorGUILayout.LabelField("Feedback email configured");
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(hasSocial ? "✅" : "❌", GUILayout.Width(25));
        EditorGUILayout.LabelField("Social handle configured");
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(hasVersion ? "✅" : "❌", GUILayout.Width(25));
        EditorGUILayout.LabelField("Alpha version set");
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This creates a professional alpha end screen that asks for player feedback and builds community around Overclocked!", MessageType.Info);
    }
}
#endif