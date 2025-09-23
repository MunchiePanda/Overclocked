using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper to generate timer prefabs with consistent styling
/// </summary>
public class TimerPrefabGenerator : MonoBehaviour
{
    [Header("Timer Style Settings")]
    [Tooltip("Timer text font size")]
    public float fontSize = 24f;
    
    [Tooltip("Timer text color")]
    public Color textColor = Color.white;
    
    [Tooltip("Background color (if using background)")]
    public Color backgroundColor = new Color(0, 0, 0, 0.7f);
    
    [Tooltip("Add background panel")]
    public bool addBackground = true;
    
    [Tooltip("Add outline effect")]
    public bool addOutline = true;
    
    [Tooltip("Outline color")]
    public Color outlineColor = Color.black;
    
    [Tooltip("Outline thickness")]
    public float outlineThickness = 2f;
    
    [Header("Layout Settings")]
    [Tooltip("Timer display width")]
    public float displayWidth = 200f;
    
    [Tooltip("Timer display height")]
    public float displayHeight = 50f;
    
    [Tooltip("Background padding")]
    public Vector2 backgroundPadding = new Vector2(10f, 5f);
    
    [Header("Animation Settings")]
    [Tooltip("Add pulsing animation when time is low")]
    public bool addPulseAnimation = true;
    
    [Tooltip("Add fade in/out animations")]
    public bool addFadeAnimations = true;
    
    [ContextMenu("🎨 Generate Basic Timer Prefab")]
    public void GenerateBasicTimerPrefab()
    {
        GameObject timerPrefab = CreateBasicTimer("Timer_Basic");
        SaveAsPrefab(timerPrefab, "Timer_Basic");
    }
    
    [ContextMenu("🎯 Generate Riddle Timer Prefab")]
    public void GenerateRiddleTimerPrefab()
    {
        GameObject timerPrefab = CreateRiddleTimer("Timer_Riddle");
        SaveAsPrefab(timerPrefab, "Timer_Riddle");
    }
    
    [ContextMenu("🏆 Generate Puzzle Timer Prefab")]
    public void GeneratePuzzleTimerPrefab()
    {
        GameObject timerPrefab = CreatePuzzleTimer("Timer_Puzzle");
        SaveAsPrefab(timerPrefab, "Timer_Puzzle");
    }
    
    [ContextMenu("⚡ Generate All Timer Prefabs")]
    public void GenerateAllTimerPrefabs()
    {
        GenerateBasicTimerPrefab();
        GenerateRiddleTimerPrefab();
        GeneratePuzzleTimerPrefab();
        
        Debug.Log("✅ Generated all timer prefabs!");
    }
    
    private GameObject CreateBasicTimer(string name)
    {
        GameObject timerObj = new GameObject(name);
        
        // Add RectTransform
        RectTransform rectTransform = timerObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(displayWidth, displayHeight);
        
        // Add background if requested
        if (addBackground)
        {
            Image background = timerObj.AddComponent<Image>();
            background.color = backgroundColor;
            background.raycastTarget = false;
        }
        
        // Create text child
        GameObject textObj = new GameObject("Timer Text");
        textObj.transform.SetParent(timerObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Add TMP_Text
        TMP_Text timerText = textObj.AddComponent<TextMeshProUGUI>();
        timerText.text = "Time: 05:00";
        timerText.fontSize = fontSize;
        timerText.color = textColor;
        timerText.alignment = TextAlignmentOptions.Center;
        timerText.fontStyle = FontStyles.Bold;
        
        // Add outline if requested
        if (addOutline)
        {
            Outline outline = textObj.AddComponent<Outline>();
            outline.effectColor = outlineColor;
            outline.effectDistance = new Vector2(outlineThickness, -outlineThickness);
        }
        
        Debug.Log($"✅ Created basic timer: {name}");
        return timerObj;
    }
    
    private GameObject CreateRiddleTimer(string name)
    {
        GameObject timerObj = CreateBasicTimer(name);
        
        // Add special riddle timer styling
        TMP_Text timerText = timerObj.GetComponentInChildren<TMP_Text>();
        if (timerText != null)
        {
            timerText.color = new Color(0.9f, 0.9f, 0.3f); // Yellow tint
            timerText.fontSize = fontSize + 4f; // Slightly larger
        }
        
        // Add warning indicator
        GameObject warningIcon = new GameObject("Warning Icon");
        warningIcon.transform.SetParent(timerObj.transform, false);
        
        RectTransform warningRect = warningIcon.AddComponent<RectTransform>();
        warningRect.anchorMin = new Vector2(0f, 0.5f);
        warningRect.anchorMax = new Vector2(0f, 0.5f);
        warningRect.anchoredPosition = new Vector2(-25f, 0f);
        warningRect.sizeDelta = new Vector2(20f, 20f);
        
        Image warningImage = warningIcon.AddComponent<Image>();
        warningImage.color = Color.red;
        warningImage.raycastTarget = false;
        warningIcon.SetActive(false); // Hidden by default
        
        // Add pulse animation component if requested
        if (addPulseAnimation)
        {
            TimerPulseAnimation pulseAnim = timerObj.AddComponent<TimerPulseAnimation>();
            pulseAnim.targetText = timerText;
            pulseAnim.warningIcon = warningImage;
        }
        
        Debug.Log($"✅ Created riddle timer: {name}");
        return timerObj;
    }
    
    private GameObject CreatePuzzleTimer(string name)
    {
        GameObject timerObj = CreateBasicTimer(name);
        
        // Add progress bar
        GameObject progressObj = new GameObject("Progress Bar");
        progressObj.transform.SetParent(timerObj.transform, false);
        
        RectTransform progressRect = progressObj.AddComponent<RectTransform>();
        progressRect.anchorMin = new Vector2(0f, 0f);
        progressRect.anchorMax = new Vector2(1f, 0f);
        progressRect.anchoredPosition = new Vector2(0f, -5f);
        progressRect.sizeDelta = new Vector2(0f, 4f);
        
        Image progressBackground = progressObj.AddComponent<Image>();
        progressBackground.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // Progress fill
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(progressObj.transform, false);
        
        RectTransform fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        
        Image fillImage = fillObj.AddComponent<Image>();
        fillImage.color = Color.green;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        
        // Add timer progress component
        TimerProgressBar progressBar = timerObj.AddComponent<TimerProgressBar>();
        progressBar.progressFill = fillImage;
        
        Debug.Log($"✅ Created puzzle timer: {name}");
        return timerObj;
    }
    
    private void SaveAsPrefab(GameObject obj, string prefabName)
    {
#if UNITY_EDITOR
        // Create Prefabs directory if it doesn't exist
        string prefabPath = "Assets/Prefabs/UI/Timers/";
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/UI"))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "UI");
        }
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/UI/Timers"))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs/UI", "Timers");
        }
        
        string fullPath = prefabPath + prefabName + ".prefab";
        
        // Save as prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(obj, fullPath);
        if (prefab != null)
        {
            Debug.Log($"💾 Saved prefab: {fullPath}");
        }
        else
        {
            Debug.LogError($"❌ Failed to save prefab: {fullPath}");
        }
        
        // Clean up the scene object
        DestroyImmediate(obj);
        
        // Refresh the asset database
        AssetDatabase.Refresh();
#endif
    }
}

/// <summary>
/// Component to handle timer pulse animations
/// </summary>
public class TimerPulseAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    public TMP_Text targetText;
    public Image warningIcon;
    
    [Tooltip("Pulse scale multiplier")]
    public float pulseScale = 1.2f;
    
    [Tooltip("Pulse speed")]
    public float pulseSpeed = 2f;
    
    [Tooltip("Warning threshold in seconds")]
    public float warningThreshold = 30f;
    
    private Vector3 originalScale;
    private Color originalColor;
    private bool isPulsing = false;
    
    void Start()
    {
        if (targetText != null)
        {
            originalScale = targetText.transform.localScale;
            originalColor = targetText.color;
        }
    }
    
    void Update()
    {
        if (isPulsing && targetText != null)
        {
            float pulse = 1f + (Mathf.Sin(Time.time * pulseSpeed) * 0.1f);
            targetText.transform.localScale = originalScale * pulse;
            
            // Pulse color between original and red
            float colorPulse = (Mathf.Sin(Time.time * pulseSpeed * 2f) + 1f) * 0.5f;
            targetText.color = Color.Lerp(Color.red, originalColor, colorPulse);
        }
    }
    
    public void StartPulsing()
    {
        isPulsing = true;
        if (warningIcon != null)
        {
            warningIcon.gameObject.SetActive(true);
        }
    }
    
    public void StopPulsing()
    {
        isPulsing = false;
        if (targetText != null)
        {
            targetText.transform.localScale = originalScale;
            targetText.color = originalColor;
        }
        if (warningIcon != null)
        {
            warningIcon.gameObject.SetActive(false);
        }
    }
}

/// <summary>
/// Component to handle timer progress bars
/// </summary>
public class TimerProgressBar : MonoBehaviour
{
    [Header("Progress Settings")]
    public Image progressFill;
    
    [Tooltip("Progress color gradient")]
    public Gradient progressGradient = new Gradient()
    {
        colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(Color.green, 0f),
            new GradientColorKey(Color.yellow, 0.5f),
            new GradientColorKey(Color.red, 1f)
        }
    };
    
    public void UpdateProgress(float normalizedTime)
    {
        if (progressFill != null)
        {
            progressFill.fillAmount = normalizedTime;
            progressFill.color = progressGradient.Evaluate(1f - normalizedTime);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TimerPrefabGenerator))]
public class TimerPrefabGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Prefab Generation", EditorStyles.boldLabel);
        
        TimerPrefabGenerator generator = (TimerPrefabGenerator)target;
        
        if (GUILayout.Button("🎨 Generate Basic Timer", GUILayout.Height(35)))
        {
            generator.GenerateBasicTimerPrefab();
        }
        
        if (GUILayout.Button("🎯 Generate Riddle Timer", GUILayout.Height(35)))
        {
            generator.GenerateRiddleTimerPrefab();
        }
        
        if (GUILayout.Button("🏆 Generate Puzzle Timer", GUILayout.Height(35)))
        {
            generator.GeneratePuzzleTimerPrefab();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("⚡ Generate All Timer Prefabs", GUILayout.Height(40)))
        {
            generator.GenerateAllTimerPrefabs();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("These tools generate timer prefabs with consistent styling. The prefabs will be saved to Assets/Prefabs/UI/Timers/", MessageType.Info);
    }
}
#endif