using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Repairs and rebuilds the settings UI to fix visibility issues
/// </summary>
public class SettingsUIRepair : MonoBehaviour
{
    [Header("🔧 Settings UI Repair")]
    [Space(10)]
    [TextArea(3, 4)]
    public string instructions = "This will completely repair and rebuild your settings UI.\n\nClick 'Repair Settings UI' to fix all issues.";
    
    [ContextMenu("🔧 Repair Settings UI")]
    public void RepairSettingsUI()
    {
        Debug.Log("🔧 Starting comprehensive settings UI repair...");
        
        // Find the settings panel
        SettingsPanel settingsPanel = FindFirstObjectByType<SettingsPanel>();
        if (settingsPanel == null)
        {
            Debug.LogError("❌ No SettingsPanel found!");
            return;
        }
        
        // Clear existing content and rebuild
        ClearAndRebuildContent(settingsPanel.transform);
        
        Debug.Log("✅ Settings UI repair complete!");
        Debug.Log("🎮 Test it: Pause → Settings to see your fixed UI!");
        
        // Auto-destroy this repair tool
        DestroyImmediate(this);
    }
    
    void ClearAndRebuildContent(Transform settingsPanel)
    {
        // Find or create scroll area
        Transform scrollArea = settingsPanel.Find("ScrollArea");
        if (scrollArea == null)
        {
            Debug.LogError("❌ ScrollArea not found!");
            return;
        }
        
        Transform viewport = scrollArea.Find("Viewport");
        Transform content = viewport?.Find("Content");
        
        if (content == null)
        {
            Debug.LogError("❌ Content area not found!");
            return;
        }
        
        // Clear existing content
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(content.GetChild(i).gameObject);
        }
        
        Debug.Log("🧹 Cleared existing content");
        
        // Fix content layout
        SetupContentLayout(content);
        
        // Rebuild sections
        CreateAccessibilitySection(content);
        CreateAudioSection(content);
        CreateGameplaySection(content);
        
        // Force layout update
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        
        Debug.Log("🎨 Content rebuilt successfully!");
    }
    
    void SetupContentLayout(Transform content)
    {
        // Fix RectTransform
        RectTransform contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.sizeDelta = new Vector2(0, 0); // Let content size fitter handle this
        
        // Fix VerticalLayoutGroup
        VerticalLayoutGroup layout = content.GetComponent<VerticalLayoutGroup>();
        if (layout == null) layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
        
        layout.spacing = 30f;
        layout.padding = new RectOffset(40, 40, 30, 30);
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlHeight = false;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        
        // Fix ContentSizeFitter
        ContentSizeFitter fitter = content.GetComponent<ContentSizeFitter>();
        if (fitter == null) fitter = content.gameObject.AddComponent<ContentSizeFitter>();
        
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        Debug.Log("🔧 Content layout fixed");
    }
    
    void CreateAccessibilitySection(Transform content)
    {
        GameObject section = CreateSection(content, "Section_ACCESSIBILITY", "🔍 ACCESSIBILITY");
        
        CreateToggle(section, "Color Blindness Support", "colorBlindnessToggle");
        CreateDropdown(section, "Color Blindness Type", "colorBlindnessTypeDropdown", new string[] { "Normal Vision", "Protanopia", "Deuteranopia", "Tritanopia" });
        CreateSlider(section, "Contrast", "contrastSlider", 0.5f, 2f, 1f);
        CreateSlider(section, "Saturation", "saturationSlider", 0f, 2f, 1f);
        CreateSlider(section, "Brightness", "brightnessSlider", 0.5f, 1.5f, 1f);
        
        Debug.Log("✅ Accessibility section created");
    }
    
    void CreateAudioSection(Transform content)
    {
        GameObject section = CreateSection(content, "Section_AUDIO", "🔊 AUDIO");
        
        CreateSlider(section, "Master Volume", "masterVolumeSlider", 0f, 1f, 1f);
        CreateSlider(section, "Music Volume", "musicVolumeSlider", 0f, 1f, 0.7f);
        CreateSlider(section, "SFX Volume", "sfxVolumeSlider", 0f, 1f, 0.8f);
        
        Debug.Log("✅ Audio section created");
    }
    
    void CreateGameplaySection(Transform content)
    {
        GameObject section = CreateSection(content, "Section_GAMEPLAY", "🎮 GAMEPLAY");
        
        CreateSlider(section, "Mouse Sensitivity", "mouseSensitivitySlider", 0.1f, 3f, 1f);
        CreateToggle(section, "Invert Mouse Y", "invertMouseYToggle");
        CreateToggle(section, "V-Sync", "vSyncToggle");
        
        Debug.Log("✅ Gameplay section created");
    }
    
    GameObject CreateSection(Transform parent, string name, string title)
    {
        GameObject section = new GameObject(name);
        section.transform.SetParent(parent, false);
        
        RectTransform sectionRect = section.AddComponent<RectTransform>();
        sectionRect.anchorMin = new Vector2(0, 1);
        sectionRect.anchorMax = new Vector2(1, 1);
        sectionRect.pivot = new Vector2(0.5f, 1);
        
        VerticalLayoutGroup sectionLayout = section.AddComponent<VerticalLayoutGroup>();
        sectionLayout.spacing = 15f;
        sectionLayout.childAlignment = TextAnchor.UpperCenter;
        sectionLayout.childControlHeight = false;
        sectionLayout.childControlWidth = true;
        sectionLayout.childForceExpandHeight = false;
        sectionLayout.childForceExpandWidth = true;
        
        ContentSizeFitter sectionFitter = section.AddComponent<ContentSizeFitter>();
        sectionFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        // Create title
        GameObject titleObj = new GameObject("SectionTitle");
        titleObj.transform.SetParent(section.transform, false);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = title;
        titleText.fontSize = 24f;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;
        
        LayoutElement titleElement = titleObj.AddComponent<LayoutElement>();
        titleElement.preferredHeight = 40f;
        
        return section;
    }
    
    void CreateToggle(GameObject section, string label, string name)
    {
        GameObject toggleContainer = new GameObject(name);
        toggleContainer.transform.SetParent(section.transform, false);
        
        HorizontalLayoutGroup layout = toggleContainer.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10f;
        layout.childControlHeight = false;
        layout.childControlWidth = false;
        layout.childForceExpandWidth = false;
        
        LayoutElement containerElement = toggleContainer.AddComponent<LayoutElement>();
        containerElement.preferredHeight = 35f;
        
        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(toggleContainer.transform, false);
        
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 16f;
        labelText.color = Color.white;
        labelText.alignment = TextAlignmentOptions.Left;
        
        LayoutElement labelElement = labelObj.AddComponent<LayoutElement>();
        labelElement.flexibleWidth = 1f;
        
        // Toggle
        GameObject toggleObj = new GameObject("Toggle");
        toggleObj.transform.SetParent(toggleContainer.transform, false);
        
        Toggle toggle = toggleObj.AddComponent<Toggle>();
        Image toggleBg = toggleObj.AddComponent<Image>();
        toggleBg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        // Checkmark
        GameObject checkmark = new GameObject("Checkmark");
        checkmark.transform.SetParent(toggleObj.transform, false);
        
        Image checkImage = checkmark.AddComponent<Image>();
        checkImage.color = new Color(0.2f, 0.8f, 1f, 1f);
        
        RectTransform checkRect = checkmark.GetComponent<RectTransform>();
        checkRect.anchorMin = Vector2.zero;
        checkRect.anchorMax = Vector2.one;
        checkRect.offsetMin = Vector2.zero;
        checkRect.offsetMax = Vector2.zero;
        
        toggle.graphic = checkImage;
        
        LayoutElement toggleElement = toggleObj.AddComponent<LayoutElement>();
        toggleElement.preferredWidth = 50f;
        toggleElement.preferredHeight = 25f;
    }
    
    void CreateSlider(GameObject section, string label, string name, float min, float max, float value)
    {
        GameObject sliderContainer = new GameObject(name);
        sliderContainer.transform.SetParent(section.transform, false);
        
        HorizontalLayoutGroup layout = sliderContainer.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 15f;
        layout.childControlHeight = false;
        layout.childControlWidth = false;
        
        LayoutElement containerElement = sliderContainer.AddComponent<LayoutElement>();
        containerElement.preferredHeight = 35f;
        
        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(sliderContainer.transform, false);
        
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 16f;
        labelText.color = Color.white;
        labelText.alignment = TextAlignmentOptions.Left;
        
        LayoutElement labelElement = labelObj.AddComponent<LayoutElement>();
        labelElement.preferredWidth = 200f;
        
        // Slider
        GameObject sliderObj = new GameObject("Slider");
        sliderObj.transform.SetParent(sliderContainer.transform, false);
        
        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = value;
        
        Image sliderBg = sliderObj.AddComponent<Image>();
        sliderBg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillRect = fillArea.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.8f, 1f, 1f);
        slider.fillRect = fill.GetComponent<RectTransform>();
        
        // Handle
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform handleRect = handleArea.AddComponent<RectTransform>();
        handleRect.anchorMin = Vector2.zero;
        handleRect.anchorMax = Vector2.one;
        handleRect.offsetMin = Vector2.zero;
        handleRect.offsetMax = Vector2.zero;
        
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        slider.handleRect = handle.GetComponent<RectTransform>();
        
        LayoutElement sliderElement = sliderObj.AddComponent<LayoutElement>();
        sliderElement.flexibleWidth = 1f;
        
        // Value text
        GameObject valueObj = new GameObject(name.Replace("Slider", "Text"));
        valueObj.transform.SetParent(sliderContainer.transform, false);
        
        TextMeshProUGUI valueText = valueObj.AddComponent<TextMeshProUGUI>();
        valueText.text = value.ToString("F1");
        valueText.fontSize = 14f;
        valueText.color = Color.white;
        valueText.alignment = TextAlignmentOptions.Right;
        
        LayoutElement valueElement = valueObj.AddComponent<LayoutElement>();
        valueElement.preferredWidth = 50f;
    }
    
    void CreateDropdown(GameObject section, string label, string name, string[] options)
    {
        GameObject dropdownContainer = new GameObject(name);
        dropdownContainer.transform.SetParent(section.transform, false);
        
        HorizontalLayoutGroup layout = dropdownContainer.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 15f;
        layout.childControlHeight = false;
        layout.childControlWidth = false;
        
        LayoutElement containerElement = dropdownContainer.AddComponent<LayoutElement>();
        containerElement.preferredHeight = 35f;
        
        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(dropdownContainer.transform, false);
        
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 16f;
        labelText.color = Color.white;
        labelText.alignment = TextAlignmentOptions.Left;
        
        LayoutElement labelElement = labelObj.AddComponent<LayoutElement>();
        labelElement.preferredWidth = 200f;
        
        // Dropdown
        GameObject dropdownObj = new GameObject("Dropdown");
        dropdownObj.transform.SetParent(dropdownContainer.transform, false);
        
        TMP_Dropdown dropdown = dropdownObj.AddComponent<TMP_Dropdown>();
        Image dropdownBg = dropdownObj.AddComponent<Image>();
        dropdownBg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        dropdown.options.Clear();
        foreach (string option in options)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(option));
        }
        
        LayoutElement dropdownElement = dropdownObj.AddComponent<LayoutElement>();
        dropdownElement.flexibleWidth = 1f;
    }
}