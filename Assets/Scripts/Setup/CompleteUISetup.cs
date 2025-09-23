using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// One-click setup to create and connect the complete settings UI to your existing system
/// </summary>
public class CompleteUISetup : MonoBehaviour
{
    [Header("🎨 Complete UI Setup")]
    [Space(10)]
    [TextArea(4, 6)]
    public string instructions = "This will create a complete settings panel UI and automatically connect it to your existing SettingsPanel component.\n\nYour PauseMenu already has the SettingsPanel component - this will just build the UI for it.\n\nClick 'Build Complete UI' to create everything!";
    
    [Space(10)]
    [Tooltip("Automatically connect after building")]
    public bool autoConnectAfterBuild = true;
    
    [ContextMenu("🚀 Build Complete UI")]
    public void BuildCompleteUI()
    {
        Debug.Log("🎨 Building complete settings UI...");
        
        // Find the existing SettingsPanel component
        SettingsPanel existingPanel = FindFirstObjectByType<SettingsPanel>();
        if (existingPanel == null)
        {
            Debug.LogError("❌ No SettingsPanel component found! Please run the accessibility setup first.");
            return;
        }
        
        // Get the canvas (PauseMenu)
        Canvas targetCanvas = existingPanel.GetComponent<Canvas>();
        if (targetCanvas == null)
        {
            targetCanvas = existingPanel.GetComponentInParent<Canvas>();
        }
        
        if (targetCanvas == null)
        {
            Debug.LogError("❌ No Canvas found for SettingsPanel!");
            return;
        }
        
        // Build the complete UI
        GameObject settingsUI = CreateSettingsUI(targetCanvas);
        
        // Connect everything automatically
        if (autoConnectAfterBuild)
        {
            ConnectUIToSettingsPanel(settingsUI, existingPanel);
        }
        
        Debug.Log("✅ Complete settings UI built and connected!");
        Debug.Log("🎮 Test it: Pause → Settings to see your new UI!");
        
        // Clean up this component
        DestroyImmediate(this);
    }
    
    GameObject CreateSettingsUI(Canvas targetCanvas)
    {
        // Remove existing settings panel if it exists
        Transform existingUI = targetCanvas.transform.Find("SettingsPanel");
        if (existingUI != null)
        {
            Debug.Log("🔄 Replacing existing SettingsPanel UI...");
            DestroyImmediate(existingUI.gameObject);
        }
        
        // Create main settings panel
        GameObject settingsPanel = CreateMainPanel(targetCanvas);
        
        // Create title section
        CreateTitleSection(settingsPanel);
        
        // Create scrollable content area
        GameObject scrollArea = CreateScrollArea(settingsPanel);
        GameObject content = scrollArea.transform.Find("Viewport/Content").gameObject;
        
        // Create all sections
        CreateAccessibilitySection(content);
        CreateAudioSection(content);
        CreateGameplaySection(content);
        
        // Create bottom button section
        CreateBottomButtons(settingsPanel);
        
        return settingsPanel;
    }
    
    GameObject CreateMainPanel(Canvas targetCanvas)
    {
        GameObject panel = new GameObject("SettingsPanel");
        panel.transform.SetParent(targetCanvas.transform, false);
        
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.1f, 0.1f);
        panelRect.anchorMax = new Vector2(0.9f, 0.9f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        
        // Add border
        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(0.2f, 0.6f, 1.0f, 1.0f);
        outline.effectDistance = new Vector2(2, 2);
        
        // Initially hidden
        panel.SetActive(false);
        
        return panel;
    }
    
    void CreateTitleSection(GameObject parent)
    {
        GameObject titleSection = new GameObject("TitleSection");
        titleSection.transform.SetParent(parent.transform, false);
        
        RectTransform titleRect = titleSection.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.9f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        // Title text
        GameObject titleText = new GameObject("TitleText");
        titleText.transform.SetParent(titleSection.transform, false);
        
        TextMeshProUGUI title = titleText.AddComponent<TextMeshProUGUI>();
        title.text = "🎛️ SETTINGS";
        title.fontSize = 28;
        title.color = Color.white;
        title.alignment = TextAlignmentOptions.Center;
        title.fontStyle = FontStyles.Bold;
        
        RectTransform titleTextRect = titleText.GetComponent<RectTransform>();
        titleTextRect.anchorMin = new Vector2(0, 0);
        titleTextRect.anchorMax = new Vector2(0.85f, 1);
        titleTextRect.offsetMin = Vector2.zero;
        titleTextRect.offsetMax = Vector2.zero;
        
        // Close button
        CreateCloseButton(titleSection);
    }
    
    void CreateCloseButton(GameObject parent)
    {
        GameObject closeBtn = new GameObject("CloseButton");
        closeBtn.transform.SetParent(parent.transform, false);
        
        RectTransform closeRect = closeBtn.AddComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(0.85f, 0.1f);
        closeRect.anchorMax = new Vector2(0.95f, 0.9f);
        closeRect.offsetMin = Vector2.zero;
        closeRect.offsetMax = Vector2.zero;
        
        Button button = closeBtn.AddComponent<Button>();
        Image buttonImage = closeBtn.AddComponent<Image>();
        buttonImage.color = new Color(0.8f, 0.2f, 0.2f, 1f);
        
        GameObject buttonText = new GameObject("Text");
        buttonText.transform.SetParent(closeBtn.transform, false);
        
        TextMeshProUGUI text = buttonText.AddComponent<TextMeshProUGUI>();
        text.text = "✕";
        text.fontSize = 20;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = FontStyles.Bold;
        
        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }
    
    GameObject CreateScrollArea(GameObject parent)
    {
        GameObject scrollArea = new GameObject("ScrollArea");
        scrollArea.transform.SetParent(parent.transform, false);
        
        RectTransform scrollRect = scrollArea.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0, 0.15f);
        scrollRect.anchorMax = new Vector2(1, 0.9f);
        scrollRect.offsetMin = Vector2.zero;
        scrollRect.offsetMax = Vector2.zero;
        
        ScrollRect scroll = scrollArea.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        
        // Viewport
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollArea.transform, false);
        
        RectTransform viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        
        Image viewportImage = viewport.AddComponent<Image>();
        viewportImage.color = Color.clear;
        
        Mask mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        
        // Content
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;
        
        VerticalLayoutGroup contentLayout = content.AddComponent<VerticalLayoutGroup>();
        contentLayout.spacing = 20;
        contentLayout.padding = new RectOffset(20, 20, 20, 20);
        contentLayout.childAlignment = TextAnchor.UpperCenter;
        contentLayout.childControlWidth = true;
        contentLayout.childControlHeight = false;
        contentLayout.childForceExpandWidth = true;
        
        ContentSizeFitter contentFitter = content.AddComponent<ContentSizeFitter>();
        contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        // Connect to ScrollRect
        scroll.viewport = viewportRect;
        scroll.content = contentRect;
        
        return scrollArea;
    }
    
    void CreateAccessibilitySection(GameObject content)
    {
        GameObject section = CreateSection(content, "🎨 ACCESSIBILITY");
        
        CreateToggle(section, "Enable Colorblindness Support", "colorBlindnessToggle");
        CreateDropdown(section, "Colorblindness Type", "colorBlindnessTypeDropdown", 
            new string[] { "Normal Vision", "Deuteranopia (Green-Blind)", "Protanopia (Red-Blind)", "Tritanopia (Blue-Blind)" });
        CreateSliderWithText(section, "Contrast", "contrastSlider", "contrastValueText", 0.5f, 2.0f, 1.0f);
        CreateSliderWithText(section, "Saturation", "saturationSlider", "saturationValueText", 0.5f, 1.5f, 1.0f);
        CreateSliderWithText(section, "Brightness", "brightnessSlider", "brightnessValueText", 0.5f, 1.5f, 1.0f);
    }
    
    void CreateAudioSection(GameObject content)
    {
        GameObject section = CreateSection(content, "🔊 AUDIO");
        
        CreateSliderWithText(section, "Master Volume", "masterVolumeSlider", "masterVolumeText", 0f, 1f, 0.8f);
        CreateSliderWithText(section, "Music Volume", "musicVolumeSlider", "musicVolumeText", 0f, 1f, 0.7f);
        CreateSliderWithText(section, "SFX Volume", "sfxVolumeSlider", "sfxVolumeText", 0f, 1f, 0.8f);
    }
    
    void CreateGameplaySection(GameObject content)
    {
        GameObject section = CreateSection(content, "🎮 GAMEPLAY");
        
        CreateSliderWithText(section, "Mouse Sensitivity", "mouseSensitivitySlider", "mouseSensitivityText", 0.1f, 3.0f, 1.0f);
        CreateToggle(section, "Invert Y-Axis", "invertMouseYToggle");
        CreateToggle(section, "VSync", "vSyncToggle");
    }
    
    void CreateBottomButtons(GameObject parent)
    {
        GameObject buttonSection = new GameObject("BottomButtons");
        buttonSection.transform.SetParent(parent.transform, false);
        
        RectTransform buttonRect = buttonSection.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0, 0);
        buttonRect.anchorMax = new Vector2(1, 0.15f);
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;
        
        HorizontalLayoutGroup layout = buttonSection.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 20;
        layout.padding = new RectOffset(20, 20, 20, 20);
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        
        CreateButton(buttonSection, "Reset to Defaults", "resetButton", new Color(0.6f, 0.6f, 0.6f, 1f));
    }
    
    GameObject CreateSection(GameObject parent, string title)
    {
        GameObject section = new GameObject($"Section_{title.Replace(" ", "_").Replace("🎨", "").Replace("🔊", "").Replace("🎮", "")}");
        section.transform.SetParent(parent.transform, false);
        
        VerticalLayoutGroup layout = section.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 10;
        layout.padding = new RectOffset(10, 10, 10, 10);
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        
        ContentSizeFitter fitter = section.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        // Section background
        Image sectionBg = section.AddComponent<Image>();
        sectionBg.color = new Color(0.2f, 0.2f, 0.2f, 0.3f);
        
        // Section title
        GameObject titleObj = new GameObject("SectionTitle");
        titleObj.transform.SetParent(section.transform, false);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = title;
        titleText.fontSize = 18;
        titleText.color = new Color(0.2f, 0.6f, 1.0f, 1f);
        titleText.alignment = TextAlignmentOptions.Left;
        titleText.fontStyle = FontStyles.Bold;
        
        LayoutElement titleElement = titleObj.AddComponent<LayoutElement>();
        titleElement.preferredHeight = 25;
        
        return section;
    }
    
    void CreateToggle(GameObject parent, string label, string toggleName)
    {
        GameObject toggleContainer = new GameObject(toggleName);
        toggleContainer.transform.SetParent(parent.transform, false);
        
        HorizontalLayoutGroup layout = toggleContainer.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = true;
        
        LayoutElement containerElement = toggleContainer.AddComponent<LayoutElement>();
        containerElement.preferredHeight = 25;
        
        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(toggleContainer.transform, false);
        
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 14;
        labelText.color = Color.white;
        labelText.alignment = TextAlignmentOptions.Left;
        
        LayoutElement labelElement = labelObj.AddComponent<LayoutElement>();
        labelElement.preferredWidth = 200;
        
        // Toggle
        GameObject toggle = new GameObject("Toggle");
        toggle.transform.SetParent(toggleContainer.transform, false);
        
        Toggle toggleComponent = toggle.AddComponent<Toggle>();
        
        Image toggleBg = toggle.AddComponent<Image>();
        toggleBg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        GameObject checkmark = new GameObject("Checkmark");
        checkmark.transform.SetParent(toggle.transform, false);
        
        Image checkmarkImage = checkmark.AddComponent<Image>();
        checkmarkImage.color = new Color(0.2f, 0.6f, 1.0f, 1f);
        
        RectTransform checkmarkRect = checkmark.GetComponent<RectTransform>();
        checkmarkRect.anchorMin = Vector2.zero;
        checkmarkRect.anchorMax = Vector2.one;
        checkmarkRect.offsetMin = Vector2.zero;
        checkmarkRect.offsetMax = Vector2.zero;
        
        toggleComponent.graphic = checkmarkImage;
        
        LayoutElement toggleElement = toggle.AddComponent<LayoutElement>();
        toggleElement.preferredWidth = 30;
        toggleElement.preferredHeight = 20;
    }
    
    void CreateSliderWithText(GameObject parent, string label, string sliderName, string textName, float min, float max, float defaultValue)
    {
        GameObject sliderContainer = new GameObject(sliderName);
        sliderContainer.transform.SetParent(parent.transform, false);
        
        HorizontalLayoutGroup layout = sliderContainer.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = true;
        
        LayoutElement containerElement = sliderContainer.AddComponent<LayoutElement>();
        containerElement.preferredHeight = 25;
        
        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(sliderContainer.transform, false);
        
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 14;
        labelText.color = Color.white;
        labelText.alignment = TextAlignmentOptions.Left;
        
        LayoutElement labelElement = labelObj.AddComponent<LayoutElement>();
        labelElement.preferredWidth = 120;
        
        // Slider
        GameObject slider = CreateSlider(sliderContainer, min, max, defaultValue);
        
        // Value text
        GameObject valueText = new GameObject(textName);
        valueText.transform.SetParent(sliderContainer.transform, false);
        
        TextMeshProUGUI valueLabel = valueText.AddComponent<TextMeshProUGUI>();
        valueLabel.text = defaultValue.ToString("F1");
        valueLabel.fontSize = 12;
        valueLabel.color = Color.white;
        valueLabel.alignment = TextAlignmentOptions.Right;
        
        LayoutElement valueElement = valueText.AddComponent<LayoutElement>();
        valueElement.preferredWidth = 40;
    }
    
    GameObject CreateSlider(GameObject parent, float min, float max, float defaultValue)
    {
        GameObject slider = new GameObject("Slider");
        slider.transform.SetParent(parent.transform, false);
        
        Slider sliderComponent = slider.AddComponent<Slider>();
        sliderComponent.minValue = min;
        sliderComponent.maxValue = max;
        sliderComponent.value = defaultValue;
        
        Image sliderBg = slider.AddComponent<Image>();
        sliderBg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        // Fill area and fill
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(slider.transform, false);
        
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;
        
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.6f, 1.0f, 1f);
        
        sliderComponent.fillRect = fill.GetComponent<RectTransform>();
        
        // Handle area and handle
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(slider.transform, false);
        
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = Vector2.zero;
        handleAreaRect.offsetMax = Vector2.zero;
        
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(16, 16);
        
        sliderComponent.handleRect = handleRect;
        
        LayoutElement sliderElement = slider.AddComponent<LayoutElement>();
        sliderElement.preferredWidth = 150;
        sliderElement.preferredHeight = 20;
        
        return slider;
    }
    
    void CreateDropdown(GameObject parent, string label, string dropdownName, string[] options)
    {
        GameObject dropdownContainer = new GameObject(dropdownName);
        dropdownContainer.transform.SetParent(parent.transform, false);
        
        HorizontalLayoutGroup layout = dropdownContainer.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = true;
        
        LayoutElement containerElement = dropdownContainer.AddComponent<LayoutElement>();
        containerElement.preferredHeight = 25;
        
        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(dropdownContainer.transform, false);
        
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 14;
        labelText.color = Color.white;
        labelText.alignment = TextAlignmentOptions.Left;
        
        LayoutElement labelElement = labelObj.AddComponent<LayoutElement>();
        labelElement.preferredWidth = 150;
        
        // Dropdown
        GameObject dropdown = new GameObject("Dropdown");
        dropdown.transform.SetParent(dropdownContainer.transform, false);
        
        TMP_Dropdown dropdownComponent = dropdown.AddComponent<TMP_Dropdown>();
        dropdownComponent.options.Clear();
        foreach (string option in options)
        {
            dropdownComponent.options.Add(new TMP_Dropdown.OptionData(option));
        }
        
        Image dropdownImage = dropdown.AddComponent<Image>();
        dropdownImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        // Simple label for dropdown
        GameObject dropdownLabel = new GameObject("Label");
        dropdownLabel.transform.SetParent(dropdown.transform, false);
        
        TextMeshProUGUI dropdownLabelText = dropdownLabel.AddComponent<TextMeshProUGUI>();
        dropdownLabelText.text = options[0];
        dropdownLabelText.fontSize = 12;
        dropdownLabelText.color = Color.white;
        dropdownLabelText.alignment = TextAlignmentOptions.Left;
        
        RectTransform dropdownLabelRect = dropdownLabel.GetComponent<RectTransform>();
        dropdownLabelRect.anchorMin = Vector2.zero;
        dropdownLabelRect.anchorMax = Vector2.one;
        dropdownLabelRect.offsetMin = new Vector2(8, 0);
        dropdownLabelRect.offsetMax = new Vector2(-20, 0);
        
        dropdownComponent.captionText = dropdownLabelText;
        
        LayoutElement dropdownElement = dropdown.AddComponent<LayoutElement>();
        dropdownElement.preferredWidth = 200;
        dropdownElement.preferredHeight = 20;
    }
    
    void CreateButton(GameObject parent, string text, string buttonName, Color color)
    {
        GameObject button = new GameObject(buttonName);
        button.transform.SetParent(parent.transform, false);
        
        Button buttonComponent = button.AddComponent<Button>();
        Image buttonImage = button.AddComponent<Image>();
        buttonImage.color = color;
        
        GameObject buttonText = new GameObject("Text");
        buttonText.transform.SetParent(button.transform, false);
        
        TextMeshProUGUI textComponent = buttonText.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = 14;
        textComponent.color = Color.white;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.fontStyle = FontStyles.Bold;
        
        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        LayoutElement buttonElement = button.AddComponent<LayoutElement>();
        buttonElement.preferredWidth = 150;
        buttonElement.preferredHeight = 30;
    }
    
    void ConnectUIToSettingsPanel(GameObject settingsUI, SettingsPanel settingsPanel)
    {
        Debug.Log("🔌 Connecting UI elements to SettingsPanel component...");
        
        // Main panel reference
        settingsPanel.settingsPanel = settingsUI;
        
        // Close button
        settingsPanel.closeButton = settingsUI.transform.Find("TitleSection/CloseButton")?.GetComponent<Button>();
        
        // Reset button
        settingsPanel.resetButton = settingsUI.transform.Find("BottomButtons/resetButton")?.GetComponent<Button>();
        
        // Accessibility controls (note: using correct property names from SettingsPanel)
        Transform accessibilitySection = settingsUI.transform.Find("ScrollArea/Viewport/Content/Section_ACCESSIBILITY");
        if (accessibilitySection != null)
        {
            settingsPanel.colorBlindnessToggle = accessibilitySection.Find("colorBlindnessToggle/Toggle")?.GetComponent<Toggle>();
            settingsPanel.colorBlindnessTypeDropdown = accessibilitySection.Find("colorBlindnessTypeDropdown/Dropdown")?.GetComponent<TMP_Dropdown>();
            settingsPanel.contrastSlider = accessibilitySection.Find("contrastSlider/Slider")?.GetComponent<Slider>();
            settingsPanel.saturationSlider = accessibilitySection.Find("saturationSlider/Slider")?.GetComponent<Slider>();
            settingsPanel.brightnessSlider = accessibilitySection.Find("brightnessSlider/Slider")?.GetComponent<Slider>();
            settingsPanel.contrastValueText = accessibilitySection.Find("contrastSlider/contrastValueText")?.GetComponent<TextMeshProUGUI>();
            settingsPanel.saturationValueText = accessibilitySection.Find("saturationSlider/saturationValueText")?.GetComponent<TextMeshProUGUI>();
            settingsPanel.brightnessValueText = accessibilitySection.Find("brightnessSlider/brightnessValueText")?.GetComponent<TextMeshProUGUI>();
        }
        
        // Audio controls
        Transform audioSection = settingsUI.transform.Find("ScrollArea/Viewport/Content/Section_AUDIO");
        if (audioSection != null)
        {
            settingsPanel.masterVolumeSlider = audioSection.Find("masterVolumeSlider/Slider")?.GetComponent<Slider>();
            settingsPanel.musicVolumeSlider = audioSection.Find("musicVolumeSlider/Slider")?.GetComponent<Slider>();
            settingsPanel.sfxVolumeSlider = audioSection.Find("sfxVolumeSlider/Slider")?.GetComponent<Slider>();
            settingsPanel.masterVolumeText = audioSection.Find("masterVolumeSlider/masterVolumeText")?.GetComponent<TextMeshProUGUI>();
            settingsPanel.musicVolumeText = audioSection.Find("musicVolumeSlider/musicVolumeText")?.GetComponent<TextMeshProUGUI>();
            settingsPanel.sfxVolumeText = audioSection.Find("sfxVolumeSlider/sfxVolumeText")?.GetComponent<TextMeshProUGUI>();
        }
        
        // Gameplay controls (note: using correct property names from SettingsPanel)
        Transform gameplaySection = settingsUI.transform.Find("ScrollArea/Viewport/Content/Section_GAMEPLAY");
        if (gameplaySection != null)
        {
            settingsPanel.mouseSensitivitySlider = gameplaySection.Find("mouseSensitivitySlider/Slider")?.GetComponent<Slider>();
            settingsPanel.invertMouseYToggle = gameplaySection.Find("invertMouseYToggle/Toggle")?.GetComponent<Toggle>();
            settingsPanel.vSyncToggle = gameplaySection.Find("vSyncToggle/Toggle")?.GetComponent<Toggle>();
            settingsPanel.mouseSensitivityText = gameplaySection.Find("mouseSensitivitySlider/mouseSensitivityText")?.GetComponent<TextMeshProUGUI>();
        }
        
        Debug.Log("✅ All UI elements connected to SettingsPanel component!");
        
        // Verify connections
        int connectedElements = 0;
        if (settingsPanel.settingsPanel != null) connectedElements++;
        if (settingsPanel.closeButton != null) connectedElements++;
        if (settingsPanel.colorBlindnessToggle != null) connectedElements++;
        if (settingsPanel.masterVolumeSlider != null) connectedElements++;
        
        Debug.Log($"🔌 Connected {connectedElements} main elements successfully!");
    }
}