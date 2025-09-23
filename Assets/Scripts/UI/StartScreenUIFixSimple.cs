using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Simple fix for UI interaction issues in the start screen
/// </summary>
public class StartScreenUIFixSimple : MonoBehaviour
{
    [Header("Fix Options")]
    [Tooltip("Auto-fix issues on Start")]
    public bool autoFixOnStart = true;
    
    [Tooltip("Enable debug logging")]
    public bool enableDebugLogs = true;
    
    [Tooltip("Test key for manual fixing")]
    public KeyCode testKey = KeyCode.F1;
    
    void Start()
    {
        if (autoFixOnStart)
        {
            Invoke(nameof(FixUIIssues), 0.1f);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            FixUIIssues();
        }
    }
    
    public void FixUIIssues()
    {
        if (enableDebugLogs)
            Debug.Log("🔧 Fixing UI interaction issues...");
        
        FixCursor();
        FixEventSystem();
        FixBlockingImages();
        FixButtons();
        
        if (enableDebugLogs)
            Debug.Log("✅ UI fix completed!");
    }
    
    void FixCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (enableDebugLogs)
            Debug.Log("✅ Cursor fixed");
    }
    
    void FixEventSystem()
    {
        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
        
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            
            if (enableDebugLogs)
                Debug.Log("✅ Created EventSystem");
        }
        else
        {
            // Ensure InputSystemUIInputModule exists for Unity 6
            if (eventSystem.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                
                if (enableDebugLogs)
                    Debug.Log("✅ Added InputSystemUIInputModule");
            }
        }
    }
    
    void FixBlockingImages()
    {
        Image[] images = FindObjectsByType<Image>(FindObjectsSortMode.None);
        int fixedCount = 0;
        
        foreach (Image image in images)
        {
            // Skip button images
            if (image.GetComponent<Button>() != null) continue;
            
            // Check if image might be blocking interactions
            if (image.raycastTarget)
            {
                RectTransform rect = image.GetComponent<RectTransform>();
                
                // Check if it's a large background image
                bool isLargeImage = false;
                
                // Check by size
                if (rect.sizeDelta.x >= 1800 || rect.sizeDelta.y >= 1000)
                    isLargeImage = true;
                
                // Check by anchors (full screen)
                if (rect.anchorMin == Vector2.zero && rect.anchorMax == Vector2.one)
                    isLargeImage = true;
                
                if (isLargeImage)
                {
                    image.raycastTarget = false;
                    fixedCount++;
                    
                    if (enableDebugLogs)
                        Debug.Log($"✅ Fixed blocking image: {image.name}");
                }
            }
        }
        
        if (enableDebugLogs)
            Debug.Log($"✅ Fixed {fixedCount} blocking images");
    }
    
    void FixButtons()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        StartScreenManager manager = FindFirstObjectByType<StartScreenManager>();
        
        foreach (Button button in buttons)
        {
            // Make sure button is interactable
            if (!button.interactable)
            {
                button.interactable = true;
                if (enableDebugLogs)
                    Debug.Log($"✅ Made button interactable: {button.name}");
            }
            
            // Set target graphic if missing
            if (button.targetGraphic == null)
            {
                Image buttonImage = button.GetComponent<Image>();
                if (buttonImage != null)
                {
                    button.targetGraphic = buttonImage;
                    if (enableDebugLogs)
                        Debug.Log($"✅ Set target graphic for: {button.name}");
                }
            }
            
            // Add listeners if manager exists and button has no listeners
            if (manager != null && button.onClick.GetPersistentEventCount() == 0)
            {
                if (button.name.ToLower().Contains("play"))
                {
                    button.onClick.AddListener(manager.OnPlayButtonClicked);
                    if (enableDebugLogs)
                        Debug.Log($"✅ Added play listener to: {button.name}");
                }
                else if (button.name.ToLower().Contains("quit"))
                {
                    button.onClick.AddListener(manager.OnQuitButtonClicked);
                    if (enableDebugLogs)
                        Debug.Log($"✅ Added quit listener to: {button.name}");
                }
            }
        }
        
        if (enableDebugLogs)
            Debug.Log($"✅ Fixed {buttons.Length} buttons");
    }
    
    [ContextMenu("Fix UI Issues")]
    public void ManualFix()
    {
        FixUIIssues();
    }
    
    [ContextMenu("Test Buttons")]
    public void TestButtons()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        
        Debug.Log($"🧪 Testing {buttons.Length} buttons:");
        
        foreach (Button button in buttons)
        {
            bool canClick = button.interactable && button.gameObject.activeInHierarchy;
            string status = canClick ? "✅" : "❌";
            
            Debug.Log($"{status} {button.name} - Interactable: {button.interactable}, Active: {button.gameObject.activeInHierarchy}, Listeners: {button.onClick.GetPersistentEventCount()}");
        }
    }
}