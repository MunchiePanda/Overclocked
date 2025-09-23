using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Utility to detect and fix corrupted UI references that cause Inspector errors
/// </summary>
public class UICorruptionFixer : MonoBehaviour
{
    [Header("🔧 UI Corruption Fixer")]
    [Space(5)]
    [Tooltip("Enable detailed logging during the fix process")]
    public bool enableDetailedLogging = true;
    
    [Header("📊 Detection Results")]
    [SerializeField] private int corruptedObjectsFound = 0;
    [SerializeField] private int nullReferencesFound = 0;
    [SerializeField] private int fixedReferences = 0;

    [ContextMenu("🔍 Detect UI Corruption")]
    public void DetectUICorruption()
    {
        Debug.Log("🔍 Starting UI corruption detection...");
        
        corruptedObjectsFound = 0;
        nullReferencesFound = 0;
        fixedReferences = 0;
        
        // Find all UI components in the scene
        Image[] images = FindObjectsByType<Image>(FindObjectsSortMode.None);
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        TMP_Text[] texts = FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);
        RectTransform[] rectTransforms = FindObjectsByType<RectTransform>(FindObjectsSortMode.None);
        
        LogDetailed($"🔍 Found {images.Length} Images, {buttons.Length} Buttons, {texts.Length} TMP_Text, {rectTransforms.Length} RectTransforms");
        
        // Check Images
        CheckUIComponents(images, "Image");
        
        // Check Buttons  
        CheckUIComponents(buttons, "Button");
        
        // Check TMP_Text
        CheckUIComponents(texts, "TMP_Text");
        
        // Check RectTransforms
        CheckUIComponents(rectTransforms, "RectTransform");
        
        Debug.Log($"🎯 Detection complete! Found {corruptedObjectsFound} corrupted objects and {nullReferencesFound} null references");
    }
    
    [ContextMenu("🔧 Fix UI Corruption")]
    public void FixUICorruption()
    {
        Debug.Log("🔧 Starting UI corruption fix...");
        
        DetectUICorruption(); // Run detection first
        
        if (corruptedObjectsFound == 0 && nullReferencesFound == 0)
        {
            Debug.Log("✅ No corruption detected - your UI is clean!");
            return;
        }
        
        // Fix corrupted UI objects
        FixCorruptedImages();
        FixCorruptedButtons();
        FixCorruptedTexts();
        
        // Clear Inspector selection to refresh
        #if UNITY_EDITOR
        Selection.activeObject = null;
        EditorUtility.SetDirty(this);
        #endif
        
        Debug.Log($"✅ Fix complete! Fixed {fixedReferences} references. Try selecting UI objects again.");
    }
    
    [ContextMenu("🗑️ Remove Corrupted Objects")]
    public void RemoveCorruptedObjects()
    {
        Debug.LogWarning("🗑️ Removing corrupted UI objects...");
        
        List<GameObject> toDestroy = new List<GameObject>();
        
        // Find objects with missing components
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue;
            
            Component[] components = obj.GetComponents<Component>();
            bool hasCorruption = false;
            
            foreach (Component comp in components)
            {
                if (comp == null)
                {
                    hasCorruption = true;
                    LogDetailed($"❌ Found null component on {obj.name}");
                    break;
                }
            }
            
            if (hasCorruption)
            {
                toDestroy.Add(obj);
                LogDetailed($"🗑️ Marking {obj.name} for removal");
            }
        }
        
        // Destroy corrupted objects
        foreach (GameObject obj in toDestroy)
        {
            if (obj != null)
            {
                Debug.LogWarning($"🗑️ Removing corrupted object: {obj.name}");
                DestroyImmediate(obj);
                fixedReferences++;
            }
        }
        
        Debug.Log($"✅ Removed {toDestroy.Count} corrupted objects");
    }
    
    private void CheckUIComponents<T>(T[] components, string componentType) where T : Component
    {
        foreach (T component in components)
        {
            if (component == null)
            {
                nullReferencesFound++;
                LogDetailed($"❌ Found null {componentType} reference");
                continue;
            }
            
            if (component.gameObject == null)
            {
                corruptedObjectsFound++;
                LogDetailed($"❌ {componentType} component has null GameObject: {component.name}");
                continue;
            }
            
            // Check if the component has missing references
            if (component is Image img)
            {
                if (img.sprite == null && img.material == null)
                {
                    LogDetailed($"⚠️ Image {img.name} has no sprite or material");
                }
            }
            else if (component is Button btn)
            {
                if (btn.targetGraphic == null)
                {
                    LogDetailed($"⚠️ Button {btn.name} has no target graphic");
                }
            }
            else if (component is TMP_Text txt)
            {
                if (txt.font == null)
                {
                    LogDetailed($"⚠️ TMP_Text {txt.name} has no font");
                }
            }
        }
    }
    
    private void FixCorruptedImages()
    {
        Image[] images = FindObjectsByType<Image>(FindObjectsSortMode.None);
        
        foreach (Image img in images)
        {
            if (img == null || img.gameObject == null) continue;
            
            try
            {
                // Ensure the Image has proper references
                if (img.sprite == null)
                {
                    // Set to default UI sprite if available
                    #if UNITY_EDITOR
                    img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
                    #endif
                    LogDetailed($"🔧 Fixed Image sprite for {img.name}");
                    fixedReferences++;
                }
                
                // Ensure proper color
                if (img.color.a == 0)
                {
                    img.color = Color.white;
                    LogDetailed($"🔧 Fixed Image color for {img.name}");
                    fixedReferences++;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Failed to fix Image {img.name}: {e.Message}");
            }
        }
    }
    
    private void FixCorruptedButtons()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        
        foreach (Button btn in buttons)
        {
            if (btn == null || btn.gameObject == null) continue;
            
            try
            {
                // Ensure the Button has a target graphic
                if (btn.targetGraphic == null)
                {
                    Image img = btn.GetComponent<Image>();
                    if (img != null)
                    {
                        btn.targetGraphic = img;
                        LogDetailed($"🔧 Fixed Button target graphic for {btn.name}");
                        fixedReferences++;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Failed to fix Button {btn.name}: {e.Message}");
            }
        }
    }
    
    private void FixCorruptedTexts()
    {
        TMP_Text[] texts = FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);
        
        foreach (TMP_Text txt in texts)
        {
            if (txt == null || txt.gameObject == null) continue;
            
            try
            {
                // Ensure the text has content
                if (string.IsNullOrEmpty(txt.text))
                {
                    txt.text = txt.name;
                    LogDetailed($"🔧 Fixed TMP_Text content for {txt.name}");
                    fixedReferences++;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Failed to fix TMP_Text {txt.name}: {e.Message}");
            }
        }
    }
    
    private void LogDetailed(string message)
    {
        if (enableDetailedLogging)
        {
            Debug.Log(message);
        }
    }
    
    [ContextMenu("📋 Generate UI Report")]
    public void GenerateUIReport()
    {
        Debug.Log("📋 Generating UI Health Report...");
        
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        Image[] images = FindObjectsByType<Image>(FindObjectsSortMode.None);
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        TMP_Text[] texts = FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);
        
        Debug.Log($"📊 UI Report:\n" +
                 $"• Canvases: {canvases.Length}\n" +
                 $"• Images: {images.Length}\n" +
                 $"• Buttons: {buttons.Length}\n" +
                 $"• TMP_Texts: {texts.Length}\n" +
                 $"• Last scan found: {corruptedObjectsFound} corrupted objects, {nullReferencesFound} null references\n" +
                 $"• Fixed references: {fixedReferences}");
    }
}