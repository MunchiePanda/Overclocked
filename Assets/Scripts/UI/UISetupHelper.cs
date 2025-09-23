using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class UISetupHelper : MonoBehaviour
{
    [Header("Canvas Settings")]
    [Tooltip("Name for the canvas")]
    public string canvasName = "UI Canvas";
    
    [Tooltip("Canvas render mode")]
    public RenderMode renderMode = RenderMode.ScreenSpaceOverlay;
    
    [Tooltip("Canvas sorting order")]
    public int sortingOrder = 0;
    
    [Header("Panel Settings")]
    [Tooltip("Name for the panel")]
    public string panelName = "Main Panel";
    
    [Tooltip("Panel background color")]
    public Color panelColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    
    [Tooltip("Panel anchor preset")]
    public PanelAnchor panelAnchor = PanelAnchor.FullScreen;
    
    [Header("Options")]
    [Tooltip("Add GraphicRaycaster to canvas")]
    public bool addGraphicRaycaster = true;
    
    [Tooltip("Create EventSystem if none exists")]
    public bool createEventSystem = true;
    
    // Created UI references
    [HideInInspector] public GameObject createdCanvas;
    [HideInInspector] public GameObject createdPanel;
    [HideInInspector] public bool uiCreated = false;
    
    public enum PanelAnchor
    {
        FullScreen,
        Center,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        TopStretch,
        BottomStretch,
        LeftStretch,
        RightStretch
    }
    
    [ContextMenu("Create Canvas with Panel")]
    public void CreateCanvasWithPanel()
    {
        try
        {
            Debug.Log("🎨 Creating Canvas with Panel...");
            
            // Check if UI already exists
            if (uiCreated && createdCanvas != null)
            {
                Debug.LogWarning("UI already exists. Use 'Remove UI' first to create new one.");
                return;
            }
            
            // Create Canvas
            GameObject canvas = CreateCanvas();
            
            // Create Panel
            GameObject panel = CreatePanel(canvas);
            
            // Create EventSystem if needed
            if (createEventSystem)
            {
                CreateEventSystemIfNeeded();
            }
            
            // Store references
            createdCanvas = canvas;
            createdPanel = panel;
            uiCreated = true;
            
            Debug.Log("✅ Canvas with Panel created successfully!");
            Debug.Log($"📦 Created: {canvas.name} with {panel.name}");
            
#if UNITY_EDITOR
            // Select the created canvas
            Selection.activeGameObject = canvas;
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create UI: {e.Message}");
        }
    }
    
    private GameObject CreateCanvas()
    {
        // Create canvas GameObject
        GameObject canvasObj = new GameObject(canvasName);
        
        // Add Canvas component
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = renderMode;
        canvas.sortingOrder = sortingOrder;
        
        // Add CanvasScaler for responsive UI
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        // Add GraphicRaycaster if requested
        if (addGraphicRaycaster)
        {
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        return canvasObj;
    }
    
    private GameObject CreatePanel(GameObject parentCanvas)
    {
        // Create panel GameObject
        GameObject panelObj = new GameObject(panelName);
        panelObj.transform.SetParent(parentCanvas.transform, false);
        
        // Add Image component for background
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = panelColor;
        panelImage.type = Image.Type.Sliced;
        
        // Set up RectTransform based on anchor preset
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        SetPanelAnchors(panelRect, panelAnchor);
        
        return panelObj;
    }
    
    private void SetPanelAnchors(RectTransform rectTransform, PanelAnchor anchor)
    {
        switch (anchor)
        {
            case PanelAnchor.FullScreen:
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero;
                break;
                
            case PanelAnchor.Center:
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.sizeDelta = new Vector2(400, 300);
                rectTransform.anchoredPosition = Vector2.zero;
                break;
                
            case PanelAnchor.TopLeft:
                rectTransform.anchorMin = new Vector2(0f, 1f);
                rectTransform.anchorMax = new Vector2(0f, 1f);
                rectTransform.sizeDelta = new Vector2(300, 200);
                rectTransform.anchoredPosition = new Vector2(150, -100);
                break;
                
            case PanelAnchor.TopRight:
                rectTransform.anchorMin = new Vector2(1f, 1f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                rectTransform.sizeDelta = new Vector2(300, 200);
                rectTransform.anchoredPosition = new Vector2(-150, -100);
                break;
                
            case PanelAnchor.BottomLeft:
                rectTransform.anchorMin = new Vector2(0f, 0f);
                rectTransform.anchorMax = new Vector2(0f, 0f);
                rectTransform.sizeDelta = new Vector2(300, 200);
                rectTransform.anchoredPosition = new Vector2(150, 100);
                break;
                
            case PanelAnchor.BottomRight:
                rectTransform.anchorMin = new Vector2(1f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 0f);
                rectTransform.sizeDelta = new Vector2(300, 200);
                rectTransform.anchoredPosition = new Vector2(-150, 100);
                break;
                
            case PanelAnchor.TopStretch:
                rectTransform.anchorMin = new Vector2(0f, 1f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                rectTransform.sizeDelta = new Vector2(0, 100);
                rectTransform.anchoredPosition = new Vector2(0, -50);
                break;
                
            case PanelAnchor.BottomStretch:
                rectTransform.anchorMin = new Vector2(0f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 0f);
                rectTransform.sizeDelta = new Vector2(0, 100);
                rectTransform.anchoredPosition = new Vector2(0, 50);
                break;
                
            case PanelAnchor.LeftStretch:
                rectTransform.anchorMin = new Vector2(0f, 0f);
                rectTransform.anchorMax = new Vector2(0f, 1f);
                rectTransform.sizeDelta = new Vector2(200, 0);
                rectTransform.anchoredPosition = new Vector2(100, 0);
                break;
                
            case PanelAnchor.RightStretch:
                rectTransform.anchorMin = new Vector2(1f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                rectTransform.sizeDelta = new Vector2(200, 0);
                rectTransform.anchoredPosition = new Vector2(-100, 0);
                break;
        }
    }
    
    private void CreateEventSystemIfNeeded()
    {
        // Check if EventSystem already exists
        EventSystem existingEventSystem = FindFirstObjectByType<EventSystem>();
        if (existingEventSystem == null)
        {
            // Create EventSystem
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
            
            Debug.Log("📱 EventSystem created");
        }
        else
        {
            Debug.Log("📱 EventSystem already exists");
        }
    }
    
    [ContextMenu("Remove UI")]
    public void RemoveUI()
    {
        if (createdCanvas != null)
        {
#if UNITY_EDITOR
            DestroyImmediate(createdCanvas);
#else
            Destroy(createdCanvas);
#endif
            Debug.Log("🗑️ UI Canvas removed");
        }
        
        // Reset references
        createdCanvas = null;
        createdPanel = null;
        uiCreated = false;
    }
    
    [ContextMenu("Test UI Setup")]
    public void TestUISetup()
    {
        if (createdCanvas != null && createdPanel != null)
        {
            Debug.Log($"✅ UI Test: Canvas '{createdCanvas.name}' with Panel '{createdPanel.name}' is working correctly!");
            
            // Flash the panel color briefly
            StartCoroutine(FlashPanel());
        }
        else
        {
            Debug.LogWarning("⚠️ No UI created yet. Use 'Create Canvas with Panel' first.");
        }
    }
    
    private System.Collections.IEnumerator FlashPanel()
    {
        if (createdPanel != null)
        {
            Image panelImage = createdPanel.GetComponent<Image>();
            if (panelImage != null)
            {
                Color originalColor = panelImage.color;
                panelImage.color = Color.yellow;
                yield return new WaitForSeconds(0.2f);
                panelImage.color = originalColor;
            }
        }
    }
    
    // Public accessors
    public Canvas GetCreatedCanvas() => createdCanvas?.GetComponent<Canvas>();
    public Image GetCreatedPanel() => createdPanel?.GetComponent<Image>();
    public bool IsUICreated() => uiCreated && createdCanvas != null;
}

#if UNITY_EDITOR
[CustomEditor(typeof(UISetupHelper))]
public class UISetupHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("UI Setup Actions", EditorStyles.boldLabel);
        
        UISetupHelper helper = (UISetupHelper)target;
        
        GUI.enabled = !helper.uiCreated;
        if (GUILayout.Button("🎨 Create Canvas with Panel", GUILayout.Height(40)))
        {
            helper.CreateCanvasWithPanel();
        }
        GUI.enabled = true;
        
        GUI.enabled = helper.uiCreated;
        if (GUILayout.Button("🎮 Test UI Setup", GUILayout.Height(30)))
        {
            helper.TestUISetup();
        }
        
        if (GUILayout.Button("🗑️ Remove UI", GUILayout.Height(30)))
        {
            helper.RemoveUI();
        }
        GUI.enabled = true;
        
        // Show status
        EditorGUILayout.Space();
        if (helper.uiCreated && helper.createdCanvas != null)
        {
            EditorGUILayout.HelpBox($"✅ UI Created: {helper.createdCanvas.name}", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("No UI created yet", MessageType.None);
        }
    }
}
#endif