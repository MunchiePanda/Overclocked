using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quick fix for scroll view content visibility issues
/// </summary>
public class ScrollViewFix : MonoBehaviour
{
    [Header("🔧 Scroll View Fix")]
    [Space(10)]
    [TextArea(3, 4)]
    public string instructions = "This will fix scroll view content visibility issues.\n\nClick 'Fix Scroll View' to repair the layout.";
    
    [ContextMenu("🔧 Fix Scroll View")]
    public void FixScrollView()
    {
        Debug.Log("🔧 Fixing scroll view layout issues...");
        
        // Find the settings panel
        SettingsPanel settingsPanel = FindFirstObjectByType<SettingsPanel>();
        if (settingsPanel == null)
        {
            Debug.LogError("❌ No SettingsPanel found!");
            return;
        }
        
        // Find the scroll view content
        Transform content = settingsPanel.transform.Find("ScrollArea/Viewport/Content");
        if (content == null)
        {
            Debug.LogError("❌ Content not found in scroll view!");
            return;
        }
        
        Debug.Log($"🔍 Found content with {content.childCount} children");
        
        // Fix the content RectTransform
        RectTransform contentRect = content.GetComponent<RectTransform>();
        if (contentRect != null)
        {
            // Reset anchors and positioning
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.anchoredPosition = new Vector2(0, 0);
            contentRect.pivot = new Vector2(0.5f, 1);
        }
        
        // Fix VerticalLayoutGroup settings
        VerticalLayoutGroup layoutGroup = content.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.spacing = 20f;
            layoutGroup.padding = new RectOffset(40, 40, 20, 20);
            layoutGroup.childAlignment = TextAnchor.UpperCenter;
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;
        }
        
        // Fix ContentSizeFitter
        ContentSizeFitter sizeFitter = content.GetComponent<ContentSizeFitter>();
        if (sizeFitter != null)
        {
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
        
        // Fix each child section
        for (int i = 0; i < content.childCount; i++)
        {
            Transform child = content.GetChild(i);
            FixSectionLayout(child);
        }
        
        // Force layout rebuild
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        
        Debug.Log("✅ Scroll view fixed! Check your settings panel now.");
        Debug.Log($"📊 Content size: {contentRect.sizeDelta}, Position: {contentRect.anchoredPosition}");
        
        // Auto-destroy this fixer
        DestroyImmediate(this);
    }
    
    void FixSectionLayout(Transform section)
    {
        RectTransform sectionRect = section.GetComponent<RectTransform>();
        if (sectionRect != null)
        {
            // Fix section anchoring
            sectionRect.anchorMin = new Vector2(0, 1);
            sectionRect.anchorMax = new Vector2(1, 1);
            sectionRect.pivot = new Vector2(0.5f, 1);
        }
        
        // Add or fix LayoutElement
        LayoutElement layoutElement = section.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = section.gameObject.AddComponent<LayoutElement>();
        }
        
        layoutElement.preferredHeight = -1; // Use child preferred height
        layoutElement.flexibleHeight = 0;
        
        Debug.Log($"🔧 Fixed section: {section.name}");
    }
}