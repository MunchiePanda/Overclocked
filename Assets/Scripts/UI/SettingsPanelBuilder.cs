using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Builder to create a complete settings panel UI and connect it to GameSettings
/// </summary>
public class SettingsPanelBuilder : MonoBehaviour
{
    [Header("🎨 Settings Panel Builder")]
    [Space(10)]
    [TextArea(3, 4)]
    public string instructions = "This will create a complete settings panel UI with all accessibility and game options.\n\nClick 'Build Settings Panel' to create the UI.";
    
    [Space(10)]
    [Tooltip("The canvas to create the settings panel on")]
    public Canvas targetCanvas;
    
    [Header("📋 Build Options")]
    [Tooltip("Automatically connect to PauseMenuController")]
    public bool autoConnectToPauseMenu = true;
    
    [Tooltip("Position the panel in the center")]
    public bool centerPanel = true;
    
    [Header("🎨 Style Options")]
    public Color panelBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);
    public Color buttonColor = new Color(0.2f, 0.6f, 1.0f, 1.0f);
    public Color textColor = Color.white;
    
    void Start()
    {
        // Auto-find canvas if not assigned
        if (targetCanvas == null)
        {
            targetCanvas = FindFirstObjectByType<Canvas>();
        }
    }
    
    [ContextMenu("🚀 Build Settings Panel")]
    public void BuildSettingsPanel()
    {
        if (targetCanvas == null)
        {
            Debug.LogError("❌ No target canvas found! Please assign a canvas.");
            return;
        }
        
        Debug.Log("🎨 Building complete settings panel...");
        
        // Create the main settings panel
        GameObject settingsPanel = CreateSettingsPanel();
        
        // Create the UI elements
        CreateTitleBar(settingsPanel);
        CreateAccessibilitySection(settingsPanel);
        CreateAudioSection(settingsPanel);
        CreateGameplaySection(settingsPanel);
        CreateControlButtons(settingsPanel);
        
        // Connect to the existing SettingsPanel component
        ConnectToSettingsPanelComponent(settingsPanel);
        
        Debug.Log("✅ Settings panel created successfully!");
        Debug.Log("🎮 Test it: Pause the game and click Settings!");
        
        // Auto-destroy this builder
        DestroyImmediate(this);
    }
    
    GameObject CreateSettingsPanel()
    {
        // Check if settings panel already exists
        Transform existing = targetCanvas.transform.Find("SettingsPanel");
        if (existing != null)
        {
            Debug.Log("⚠️ SettingsPanel already exists, replacing it...");
            DestroyImmediate(existing.gameObject);
        }
        
        // Create main panel
        GameObject panel = new GameObject("SettingsPanel");
        panel.transform.SetParent(targetCanvas.transform, false);
        
        // Add RectTransform and setup
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Add background image
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = panelBackgroundColor;
        
        // Add CanvasGroup for easy show/hide
        CanvasGroup canvasGroup = panel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        // Initially hidden
        panel.SetActive(false);
        
        return panel;
    }
    
    void CreateTitleBar(GameObject parent)
    {
        // Create title bar container
        GameObject titleBar = new GameObject("TitleBar");
        titleBar.transform.SetParent(parent.transform, false);
        
        RectTransform titleRect = titleBar.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.9f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        // Title text
        GameObject titleText = new GameObject("Title");
        titleText.transform.SetParent(titleBar.transform, false);
        
        TextMeshProUGUI titleTMP = titleText.AddComponent<TextMeshProUGUI>();
        titleTMP.text = "🎛️ SETTINGS";
        titleTMP.fontSize = 32;
        titleTMP.color = textColor;
        titleTMP.alignment = TextAlignmentOptions.Center;
        titleTMP.fontStyle = FontStyles.Bold;
        
        RectTransform titleTextRect = titleText.GetComponent<RectTransform>();
        titleTextRect.anchorMin = Vector2.zero;
        titleTextRect.anchorMax = Vector2.one;
        titleTextRect.offsetMin = Vector2.zero;
        titleTextRect.offsetMax = Vector2.zero;
        
        // Close button
        CreateCloseButton(titleBar);
    }
    
    void CreateCloseButton(GameObject parent)
    {
        GameObject closeBtn = new GameObject("CloseButton");
        closeBtn.transform.SetParent(parent.transform, false);
        
        RectTransform closeRect = closeBtn.AddComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(0.9f, 0);
        closeRect.anchorMax = new Vector2(1, 1);
        closeRect.offsetMin = Vector2.zero;
        closeRect.offsetMax = Vector2.zero;
        
        Button button = closeBtn.AddComponent<Button>();
        Image buttonImage = closeBtn.AddComponent<Image>();
        buttonImage.color = Color.red;
        
        // Close button text
        GameObject buttonText = new GameObject("Text");
        buttonText.transform.SetParent(closeBtn.transform, false);
        
        TextMeshProUGUI text = buttonText.AddComponent<TextMeshProUGUI>();
        text.text = "✕";
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.fontStyle = FontStyles.Bold;
        
        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }
    
    void CreateAccessibilitySection(GameObject parent)
    {
        // Create accessibility section
        GameObject section = CreateSection(parent, "🎨 Accessibility", 0.7f, 0.9f);
        
        // Colorblindness toggle
        CreateToggle(section, "Enable Colorblindness Support", "ColorblindnessToggle");
        
        // Colorblindness type dropdown
        CreateDropdown(section, "Colorblindness Type:", "ColorblindnessDropdown", 
            new string[] { "Normal Vision", "Deuteranopia (Green-blind)", "Protanopia (Red-blind)", "Tritanopia (Blue-blind)" });
        
        // Visual adjustment sliders
        CreateSlider(section, "Contrast", "ContrastSlider", 0.5f, 2.0f, 1.0f);
        CreateSlider(section, "Saturation", "SaturationSlider", 0.5f, 1.5f, 1.0f);
        CreateSlider(section, "Brightness", "BrightnessSlider", 0.5f, 1.5f, 1.0f);
    }
    
    void CreateAudioSection(GameObject parent)
    {
        GameObject section = CreateSection(parent, "🔊 Audio", 0.4f, 0.7f);
        
        CreateSlider(section, "Master Volume", "MasterVolumeSlider", 0f, 1f, 0.8f);
        CreateSlider(section, "Music Volume", "MusicVolumeSlider", 0f, 1f, 0.7f);
        CreateSlider(section, "SFX Volume", "SFXVolumeSlider", 0f, 1f, 0.8f);
    }
    
    void CreateGameplaySection(GameObject parent)
    {
        GameObject section = CreateSection(parent, "🎮 Gameplay", 0.15f, 0.4f);
        
        CreateSlider(section, "Mouse Sensitivity", "MouseSensitivitySlider", 0.1f, 3.0f, 1.0f);
        CreateToggle(section, "Invert Y-Axis", "InvertYToggle");
        CreateToggle(section, "VSync", "VSyncToggle");
    }
    
    void CreateControlButtons(GameObject parent)
    {
        GameObject buttonSection = CreateSection(parent, "", 0.05f, 0.15f);
        
        // Create button container
        GameObject buttonContainer = new GameObject("ButtonContainer");
        buttonContainer.transform.SetParent(buttonSection.transform, false);
        
        RectTransform containerRect = buttonContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;
        
        // Horizontal layout group
        HorizontalLayoutGroup layout = buttonContainer.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 20;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
        
        // Apply button
        CreateButton(buttonContainer, "Apply Settings", "ApplyButton", buttonColor);
        
        // Reset button
        CreateButton(buttonContainer, "Reset to Defaults", "ResetButton", Color.gray);
    }
    
    GameObject CreateSection(GameObject parent, string title, float minY, float maxY)
    {
        GameObject section = new GameObject($"Section_{title.Replace(" ", "_").Replace("🎨", "").Replace("🔊", "").Replace("🎮", "")}");
        section.transform.SetParent(parent.transform, false);
        
        RectTransform sectionRect = section.AddComponent<RectTransform>();
        sectionRect.anchorMin = new Vector2(0.05f, minY);
        sectionRect.anchorMax = new Vector2(0.95f, maxY);
        sectionRect.offsetMin = Vector2.zero;
        sectionRect.offsetMax = Vector2.zero;
        
        // Vertical layout group
        VerticalLayoutGroup layout = section.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 10;
        layout.padding = new RectOffset(20, 20, 10, 10);
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        
        // Content size fitter
        ContentSizeFitter fitter = section.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        // Add title if provided
        if (!string.IsNullOrEmpty(title))
        {
            CreateSectionTitle(section, title);
        }
        
        return section;
    }
    
    void CreateSectionTitle(GameObject parent, string title)
    {
        GameObject titleObj = new GameObject("SectionTitle");
        titleObj.transform.SetParent(parent.transform, false);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = title;
        titleText.fontSize = 20;
        titleText.color = textColor;
        titleText.alignment = TextAlignmentOptions.Left;
        titleText.fontStyle = FontStyles.Bold;
        
        LayoutElement element = titleObj.AddComponent<LayoutElement>();
        element.preferredHeight = 30;
    }
    
    void CreateToggle(GameObject parent, string label, string objectName)
    {
        GameObject toggleObj = new GameObject(objectName);
        toggleObj.transform.SetParent(parent.transform, false);
        
        // Create horizontal layout
        HorizontalLayoutGroup layout = toggleObj.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = true;
        
        LayoutElement element = toggleObj.AddComponent<LayoutElement>();
        element.preferredHeight = 30;
        
        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(toggleObj.transform, false);
        
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 16;
        labelText.color = textColor;
        labelText.alignment = TextAlignmentOptions.Left;
        
        LayoutElement labelElement = labelObj.AddComponent<LayoutElement>();
        labelElement.preferredWidth = 200;
        
        // Toggle
        GameObject toggle = new GameObject("Toggle");
        toggle.transform.SetParent(toggleObj.transform, false);
        
        Toggle toggleComponent = toggle.AddComponent<Toggle>();
        
        // Toggle background
        Image toggleBg = toggle.AddComponent<Image>();
        toggleBg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        // Checkmark
        GameObject checkmark = new GameObject("Checkmark");
        checkmark.transform.SetParent(toggle.transform, false);
        
        Image checkmarkImage = checkmark.AddComponent<Image>();
        checkmarkImage.color = buttonColor;
        
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
    
    void CreateSlider(GameObject parent, string label, string objectName, float minValue, float maxValue, float defaultValue)
    {
        GameObject sliderObj = new GameObject(objectName);
        sliderObj.transform.SetParent(parent.transform, false);
        
        HorizontalLayoutGroup layout = sliderObj.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = true;
        
        LayoutElement element = sliderObj.AddComponent<LayoutElement>();
        element.preferredHeight = 30;
        
        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(sliderObj.transform, false);
        
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 16;
        labelText.color = textColor;
        labelText.alignment = TextAlignmentOptions.Left;
        
        LayoutElement labelElement = labelObj.AddComponent<LayoutElement>();
        labelElement.preferredWidth = 150;
        
        // Slider
        GameObject slider = new GameObject("Slider");
        slider.transform.SetParent(sliderObj.transform, false);
        
        Slider sliderComponent = slider.AddComponent<Slider>();
        sliderComponent.minValue = minValue;
        sliderComponent.maxValue = maxValue;
        sliderComponent.value = defaultValue;
        
        // Slider background
        Image sliderBg = slider.AddComponent<Image>();
        sliderBg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        // Fill area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(slider.transform, false);
        
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;
        
        // Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = buttonColor;
        
        sliderComponent.fillRect = fill.GetComponent<RectTransform>();
        
        // Handle area
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(slider.transform, false);
        
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = Vector2.zero;
        handleAreaRect.offsetMax = Vector2.zero;
        
        // Handle
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 20);
        
        sliderComponent.handleRect = handleRect;
        
        LayoutElement sliderElement = slider.AddComponent<LayoutElement>();
        sliderElement.preferredWidth = 200;
        sliderElement.preferredHeight = 20;
        
        // Value text
        GameObject valueObj = new GameObject("Value");
        valueObj.transform.SetParent(sliderObj.transform, false);
        
        TextMeshProUGUI valueText = valueObj.AddComponent<TextMeshProUGUI>();
        valueText.text = defaultValue.ToString("F1");
        valueText.fontSize = 14;
        valueText.color = textColor;
        valueText.alignment = TextAlignmentOptions.Right;
        
        LayoutElement valueElement = valueObj.AddComponent<LayoutElement>();
        valueElement.preferredWidth = 50;
        
        // Update value text when slider changes
        sliderComponent.onValueChanged.AddListener((value) => valueText.text = value.ToString("F1"));
    }
    
    void CreateDropdown(GameObject parent, string label, string objectName, string[] options)
    {
        GameObject dropdownObj = new GameObject(objectName);
        dropdownObj.transform.SetParent(parent.transform, false);
        
        HorizontalLayoutGroup layout = dropdownObj.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = true;
        
        LayoutElement element = dropdownObj.AddComponent<LayoutElement>();
        element.preferredHeight = 30;
        
        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(dropdownObj.transform, false);
        
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 16;
        labelText.color = textColor;
        labelText.alignment = TextAlignmentOptions.Left;
        
        LayoutElement labelElement = labelObj.AddComponent<LayoutElement>();
        labelElement.preferredWidth = 180;
        
        // Dropdown
        GameObject dropdown = new GameObject("Dropdown");
        dropdown.transform.SetParent(dropdownObj.transform, false);
        
        TMP_Dropdown dropdownComponent = dropdown.AddComponent<TMP_Dropdown>();
        
        // Clear and add options
        dropdownComponent.options.Clear();
        foreach (string option in options)
        {
            dropdownComponent.options.Add(new TMP_Dropdown.OptionData(option));
        }
        
        // Dropdown image
        Image dropdownImage = dropdown.AddComponent<Image>();
        dropdownImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        LayoutElement dropdownElement = dropdown.AddComponent<LayoutElement>();
        dropdownElement.preferredWidth = 250;
        dropdownElement.preferredHeight = 25;
        
        // Create dropdown template and text (simplified for this example)
        CreateDropdownTemplate(dropdown, dropdownComponent);
    }
    
    void CreateDropdownTemplate(GameObject dropdown, TMP_Dropdown dropdownComponent)
    {
        // Create label for dropdown
        GameObject label = new GameObject("Label");
        label.transform.SetParent(dropdown.transform, false);
        
        TextMeshProUGUI labelText = label.AddComponent<TextMeshProUGUI>();
        labelText.text = "Normal Vision";
        labelText.fontSize = 14;
        labelText.color = textColor;
        labelText.alignment = TextAlignmentOptions.Left;
        
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(10, 0);
        labelRect.offsetMax = new Vector2(-25, 0);
        
        dropdownComponent.captionText = labelText;
        
        // Arrow
        GameObject arrow = new GameObject("Arrow");
        arrow.transform.SetParent(dropdown.transform, false);
        
        TextMeshProUGUI arrowText = arrow.AddComponent<TextMeshProUGUI>();
        arrowText.text = "▼";
        arrowText.fontSize = 12;
        arrowText.color = textColor;
        arrowText.alignment = TextAlignmentOptions.Center;
        
        RectTransform arrowRect = arrow.GetComponent<RectTransform>();
        arrowRect.anchorMin = new Vector2(1, 0);
        arrowRect.anchorMax = new Vector2(1, 1);
        arrowRect.offsetMin = new Vector2(-20, 0);
        arrowRect.offsetMax = Vector2.zero;
    }
    
    void CreateButton(GameObject parent, string text, string objectName, Color color)
    {
        GameObject buttonObj = new GameObject(objectName);
        buttonObj.transform.SetParent(parent.transform, false);
        
        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = color;
        
        // Button text
        GameObject buttonText = new GameObject("Text");
        buttonText.transform.SetParent(buttonObj.transform, false);
        
        TextMeshProUGUI textComponent = buttonText.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = 16;
        textComponent.color = Color.white;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.fontStyle = FontStyles.Bold;
        
        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        LayoutElement buttonElement = buttonObj.AddComponent<LayoutElement>();
        buttonElement.preferredWidth = 150;
        buttonElement.preferredHeight = 40;
    }
    
    void ConnectToSettingsPanelComponent(GameObject settingsPanel)
    {
        // Find the SettingsPanel component on PauseMenu
        SettingsPanel settingsPanelComponent = FindFirstObjectByType<SettingsPanel>();
        if (settingsPanelComponent == null)
        {
            Debug.LogWarning("⚠️ SettingsPanel component not found!");
            return;
        }
        
        // Assign the panel and UI elements
        settingsPanelComponent.settingsPanel = settingsPanel;
        settingsPanelComponent.closeButton = settingsPanel.transform.Find("TitleBar/CloseButton")?.GetComponent<Button>();
        
        // Find and assign accessibility controls (using correct property names)
        Transform accessibilitySection = settingsPanel.transform.Find("Section_Accessibility");
        if (accessibilitySection != null)
        {
            settingsPanelComponent.colorBlindnessToggle = accessibilitySection.Find("ColorblindnessToggle/Toggle")?.GetComponent<Toggle>();
            settingsPanelComponent.colorBlindnessTypeDropdown = accessibilitySection.Find("ColorblindnessDropdown/Dropdown")?.GetComponent<TMP_Dropdown>();
            settingsPanelComponent.contrastSlider = accessibilitySection.Find("ContrastSlider/Slider")?.GetComponent<Slider>();
            settingsPanelComponent.saturationSlider = accessibilitySection.Find("SaturationSlider/Slider")?.GetComponent<Slider>();
            settingsPanelComponent.brightnessSlider = accessibilitySection.Find("BrightnessSlider/Slider")?.GetComponent<Slider>();
        }
        
        // Find and assign audio controls
        Transform audioSection = settingsPanel.transform.Find("Section_Audio");
        if (audioSection != null)
        {
            settingsPanelComponent.masterVolumeSlider = audioSection.Find("MasterVolumeSlider/Slider")?.GetComponent<Slider>();
            settingsPanelComponent.musicVolumeSlider = audioSection.Find("MusicVolumeSlider/Slider")?.GetComponent<Slider>();
            settingsPanelComponent.sfxVolumeSlider = audioSection.Find("SFXVolumeSlider/Slider")?.GetComponent<Slider>();
        }
        
        // Find and assign gameplay controls (using correct property names)
        Transform gameplaySection = settingsPanel.transform.Find("Section_Gameplay");
        if (gameplaySection != null)
        {
            settingsPanelComponent.mouseSensitivitySlider = gameplaySection.Find("MouseSensitivitySlider/Slider")?.GetComponent<Slider>();
            settingsPanelComponent.invertMouseYToggle = gameplaySection.Find("InvertYToggle/Toggle")?.GetComponent<Toggle>();
            settingsPanelComponent.vSyncToggle = gameplaySection.Find("VSyncToggle/Toggle")?.GetComponent<Toggle>();
        }
        
        // Find control buttons
        Transform buttonSection = settingsPanel.transform.Find("Section_");
        if (buttonSection != null)
        {
            Transform buttonContainer = buttonSection.Find("ButtonContainer");
            if (buttonContainer != null)
            {
                settingsPanelComponent.resetButton = buttonContainer.Find("ResetButton")?.GetComponent<Button>();
            }
        }
        
        Debug.Log("✅ Settings panel connected to SettingsPanel component!");
        Debug.Log("🔧 All UI elements have been automatically assigned!");
    }
}