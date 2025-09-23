using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper script to automatically setup timer UI elements and configurations
/// </summary>
public class TimerSetupHelper : MonoBehaviour
{
    [Header("Timer Configuration")]
    [Tooltip("Default timer duration in seconds")]
    public float defaultDuration = 300f; // 5 minutes
    
    [Tooltip("Timer format style")]
    public TimerFormat timerFormat = TimerFormat.MinutesSeconds;
    
    [Tooltip("Should the timer count down or up?")]
    public TimerDirection timerDirection = TimerDirection.CountDown;
    
    [Header("Visual Settings")]
    [Tooltip("Normal timer color")]
    public Color normalColor = Color.white;
    
    [Tooltip("Warning color (when time is low)")]
    public Color warningColor = Color.red;
    
    [Tooltip("Critical color (very low time)")]
    public Color criticalColor = new Color(1f, 0.3f, 0.3f, 1f);
    
    [Tooltip("Warning threshold (seconds)")]
    public float warningThreshold = 60f;
    
    [Tooltip("Critical threshold (seconds)")]
    public float criticalThreshold = 30f;
    
    [Header("Auto-Setup Options")]
    [Tooltip("Automatically find timer text components")]
    public bool autoFindTimerTexts = true;
    
    [Tooltip("Create timer display if none found")]
    public bool createTimerIfMissing = true;
    
    [Tooltip("Setup riddle puzzle timers automatically")]
    public bool setupRiddlePuzzleTimers = true;
    
    [Tooltip("Setup all puzzle timers automatically")]
    public bool setupAllPuzzleTimers = true;
    
    [Header("Timer Prefab Settings")]
    [Tooltip("Timer text prefab to instantiate")]
    public GameObject timerTextPrefab;
    
    [Tooltip("Parent canvas for new timer displays")]
    public Canvas targetCanvas;
    
    [Header("Manual Timer References")]
    [Tooltip("Timer text components found or created")]
    public List<TMP_Text> timerTexts = new List<TMP_Text>();
    
    [Tooltip("Associated puzzle components")]
    public List<BasePuzzle> puzzleComponents = new List<BasePuzzle>();
    
    // Internal tracking
    private Dictionary<BasePuzzle, TimerData> puzzleTimers = new Dictionary<BasePuzzle, TimerData>();
    
    [System.Serializable]
    public class TimerData
    {
        public TMP_Text timerDisplay;
        public float duration;
        public float currentTime;
        public bool isActive;
        public TimerDirection direction;
        public Color originalColor;
    }
    
    public enum TimerFormat
    {
        Seconds,           // "120"
        MinutesSeconds,    // "02:00"
        HoursMinutesSeconds, // "00:02:00"
        Decimal            // "2.0m"
    }
    
    public enum TimerDirection
    {
        CountDown,
        CountUp
    }
    
    void Start()
    {
        if (autoFindTimerTexts)
        {
            FindAllTimerComponents();
        }
        
        if (setupRiddlePuzzleTimers)
        {
            SetupRiddlePuzzleTimers();
        }
        
        if (setupAllPuzzleTimers)
        {
            SetupAllPuzzleTimers();
        }
    }
    
    [ContextMenu("🔍 Find All Timer Components")]
    public void FindAllTimerComponents()
    {
        // Find all TMP_Text components that might be timers
        TMP_Text[] allTexts = FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);
        timerTexts.Clear();
        
        foreach (var text in allTexts)
        {
            // Check if the text component is likely a timer
            if (IsLikelyTimerText(text))
            {
                timerTexts.Add(text);
                Debug.Log($"✅ Found timer text: {text.name}");
            }
        }
        
        // Find all puzzle components
        BasePuzzle[] allPuzzles = FindObjectsByType<BasePuzzle>(FindObjectsSortMode.None);
        puzzleComponents.Clear();
        puzzleComponents.AddRange(allPuzzles);
        
        Debug.Log($"📊 Found {timerTexts.Count} timer texts and {puzzleComponents.Count} puzzles");
    }
    
    [ContextMenu("🎯 Setup Riddle Puzzle Timers")]
    public void SetupRiddlePuzzleTimers()
    {
        RiddlePuzzle[] riddlePuzzles = FindObjectsByType<RiddlePuzzle>(FindObjectsSortMode.None);
        
        foreach (var puzzle in riddlePuzzles)
        {
            SetupPuzzleTimer(puzzle);
        }
        
        Debug.Log($"🎯 Setup timers for {riddlePuzzles.Length} riddle puzzles");
    }
    
    [ContextMenu("🧩 Setup All Puzzle Timers")]
    public void SetupAllPuzzleTimers()
    {
        BasePuzzle[] allPuzzles = FindObjectsByType<BasePuzzle>(FindObjectsSortMode.None);
        
        foreach (var puzzle in allPuzzles)
        {
            SetupPuzzleTimer(puzzle);
        }
        
        Debug.Log($"🧩 Setup timers for {allPuzzles.Length} puzzles");
    }
    
    [ContextMenu("🏗️ Create Timer Display")]
    public void CreateTimerDisplay()
    {
        if (targetCanvas == null)
        {
            targetCanvas = FindFirstObjectByType<Canvas>();
        }
        
        if (targetCanvas == null)
        {
            Debug.LogError("❌ No canvas found to create timer display!");
            return;
        }
        
        GameObject timerObj;
        
        if (timerTextPrefab != null)
        {
            timerObj = Instantiate(timerTextPrefab, targetCanvas.transform);
        }
        else
        {
            timerObj = CreateDefaultTimerDisplay(targetCanvas);
        }
        
        TMP_Text timerText = timerObj.GetComponent<TMP_Text>();
        if (timerText != null)
        {
            timerTexts.Add(timerText);
            ConfigureTimerText(timerText);
            Debug.Log($"✅ Created timer display: {timerObj.name}");
        }
    }
    
    private GameObject CreateDefaultTimerDisplay(Canvas canvas)
    {
        // Create timer GameObject
        GameObject timerObj = new GameObject("Timer Display");
        timerObj.transform.SetParent(canvas.transform, false);
        
        // Add RectTransform
        RectTransform rectTransform = timerObj.AddComponent<RectTransform>();
        
        // Position at top-right corner
        rectTransform.anchorMin = new Vector2(1, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.anchoredPosition = new Vector2(-100, -50);
        rectTransform.sizeDelta = new Vector2(200, 50);
        
        // Add TMP_Text component
        TMP_Text timerText = timerObj.AddComponent<TextMeshProUGUI>();
        timerText.text = "Time: 05:00";
        timerText.fontSize = 24;
        timerText.color = normalColor;
        timerText.alignment = TextAlignmentOptions.Center;
        timerText.fontStyle = FontStyles.Bold;
        
        // Add outline for better visibility
        timerText.enableAutoSizing = true;
        timerText.fontSizeMin = 12;
        timerText.fontSizeMax = 32;
        
        return timerObj;
    }
    
    private void SetupPuzzleTimer(BasePuzzle puzzle)
    {
        if (puzzle == null) return;
        
        TimerData timerData = new TimerData
        {
            duration = defaultDuration,
            currentTime = defaultDuration,
            direction = timerDirection,
            isActive = false
        };
        
        // Try to find existing timer display for this puzzle
        TMP_Text timerDisplay = FindTimerDisplayForPuzzle(puzzle);
        
        if (timerDisplay == null && createTimerIfMissing)
        {
            timerDisplay = CreateTimerForPuzzle(puzzle);
        }
        
        if (timerDisplay != null)
        {
            timerData.timerDisplay = timerDisplay;
            timerData.originalColor = timerDisplay.color;
            ConfigureTimerText(timerDisplay);
            
            // Special setup for RiddlePuzzle
            if (puzzle is RiddlePuzzle riddlePuzzle)
            {
                SetupRiddlePuzzleSpecific(riddlePuzzle, timerData);
            }
        }
        
        puzzleTimers[puzzle] = timerData;
        
        if (!puzzleComponents.Contains(puzzle))
        {
            puzzleComponents.Add(puzzle);
        }
        
        Debug.Log($"⚙️ Setup timer for puzzle: {puzzle.puzzleName}");
    }
    
    private void SetupRiddlePuzzleSpecific(RiddlePuzzle riddlePuzzle, TimerData timerData)
    {
        // Use reflection to set timer properties if they exist
        var timerDisplayField = typeof(RiddlePuzzle).GetField("timerDisplay");
        var timeLimitField = typeof(RiddlePuzzle).GetField("timeLimit");
        var warningColorField = typeof(RiddlePuzzle).GetField("warningColor");
        var normalTimerColorField = typeof(RiddlePuzzle).GetField("normalTimerColor");
        
        if (timerDisplayField != null && timerData.timerDisplay != null)
        {
            timerDisplayField.SetValue(riddlePuzzle, timerData.timerDisplay);
        }
        
        if (timeLimitField != null)
        {
            timeLimitField.SetValue(riddlePuzzle, defaultDuration);
            timerData.duration = defaultDuration;
        }
        
        if (warningColorField != null)
        {
            warningColorField.SetValue(riddlePuzzle, warningColor);
        }
        
        if (normalTimerColorField != null)
        {
            normalTimerColorField.SetValue(riddlePuzzle, normalColor);
        }
        
        Debug.Log($"🎯 Configured RiddlePuzzle timer settings for: {riddlePuzzle.puzzleName}");
    }
    
    private TMP_Text FindTimerDisplayForPuzzle(BasePuzzle puzzle)
    {
        // Look for timer display in puzzle's UI hierarchy
        if (puzzle.puzzleUI != null)
        {
            TMP_Text[] texts = puzzle.puzzleUI.GetComponentsInChildren<TMP_Text>();
            foreach (var text in texts)
            {
                if (IsLikelyTimerText(text))
                {
                    return text;
                }
            }
        }
        
        // Look for timer display with similar name
        foreach (var timerText in timerTexts)
        {
            if (timerText.name.ToLower().Contains(puzzle.name.ToLower()) ||
                timerText.name.ToLower().Contains("timer"))
            {
                return timerText;
            }
        }
        
        return null;
    }
    
    private TMP_Text CreateTimerForPuzzle(BasePuzzle puzzle)
    {
        Canvas canvas = null;
        
        // Try to find canvas in puzzle UI
        if (puzzle.puzzleUI != null)
        {
            canvas = puzzle.puzzleUI.GetComponentInParent<Canvas>();
        }
        
        // Fallback to main canvas
        if (canvas == null)
        {
            canvas = targetCanvas ?? FindFirstObjectByType<Canvas>();
        }
        
        if (canvas == null)
        {
            Debug.LogWarning($"⚠️ No canvas found for puzzle: {puzzle.puzzleName}");
            return null;
        }
        
        GameObject timerObj = CreateDefaultTimerDisplay(canvas);
        timerObj.name = $"{puzzle.puzzleName} Timer";
        
        // Position relative to puzzle UI if available
        if (puzzle.puzzleUI != null)
        {
            RectTransform timerRect = timerObj.GetComponent<RectTransform>();
            timerRect.SetParent(puzzle.puzzleUI.transform, false);
            
            // Position at top of puzzle UI
            timerRect.anchorMin = new Vector2(0.5f, 1f);
            timerRect.anchorMax = new Vector2(0.5f, 1f);
            timerRect.anchoredPosition = new Vector2(0, -30);
        }
        
        return timerObj.GetComponent<TMP_Text>();
    }
    
    private void ConfigureTimerText(TMP_Text timerText)
    {
        timerText.color = normalColor;
        timerText.fontSize = Mathf.Max(timerText.fontSize, 18);
        timerText.fontStyle = FontStyles.Bold;
        timerText.alignment = TextAlignmentOptions.Center;
        
        // Set initial timer display
        UpdateTimerDisplay(timerText, defaultDuration);
    }
    
    private bool IsLikelyTimerText(TMP_Text text)
    {
        if (text == null) return false;
        
        string name = text.name.ToLower();
        string textContent = text.text.ToLower();
        
        // Check for timer-related keywords
        string[] timerKeywords = { "timer", "time", "countdown", "clock", "duration" };
        string[] timePatterns = { ":", "00", "sec", "min", "hour" };
        
        foreach (var keyword in timerKeywords)
        {
            if (name.Contains(keyword)) return true;
        }
        
        foreach (var pattern in timePatterns)
        {
            if (textContent.Contains(pattern)) return true;
        }
        
        return false;
    }
    
    public void UpdateTimerDisplay(TMP_Text timerText, float timeValue)
    {
        if (timerText == null) return;
        
        string timeString = FormatTime(timeValue, timerFormat);
        timerText.text = $"Time: {timeString}";
        
        // Update color based on time remaining (for countdown)
        if (timerDirection == TimerDirection.CountDown)
        {
            if (timeValue <= criticalThreshold)
            {
                timerText.color = criticalColor;
            }
            else if (timeValue <= warningThreshold)
            {
                timerText.color = warningColor;
            }
            else
            {
                timerText.color = normalColor;
            }
        }
    }
    
    public string FormatTime(float seconds, TimerFormat format)
    {
        seconds = Mathf.Max(0, seconds);
        
        switch (format)
        {
            case TimerFormat.Seconds:
                return Mathf.CeilToInt(seconds).ToString();
                
            case TimerFormat.MinutesSeconds:
                int minutes = Mathf.FloorToInt(seconds / 60);
                int secs = Mathf.FloorToInt(seconds % 60);
                return $"{minutes:00}:{secs:00}";
                
            case TimerFormat.HoursMinutesSeconds:
                int hours = Mathf.FloorToInt(seconds / 3600);
                int mins = Mathf.FloorToInt((seconds % 3600) / 60);
                int s = Mathf.FloorToInt(seconds % 60);
                return $"{hours:00}:{mins:00}:{s:00}";
                
            case TimerFormat.Decimal:
                if (seconds >= 60)
                    return $"{(seconds / 60):F1}m";
                else
                    return $"{seconds:F1}s";
                    
            default:
                return seconds.ToString("F1");
        }
    }
    
    [ContextMenu("🧪 Test Timer Setup")]
    public void TestTimerSetup()
    {
        Debug.Log("🧪 Testing Timer Setup:");
        Debug.Log($"📊 Found {timerTexts.Count} timer displays");
        Debug.Log($"🧩 Found {puzzleComponents.Count} puzzle components");
        Debug.Log($"⚙️ Configured {puzzleTimers.Count} puzzle timers");
        
        foreach (var timerText in timerTexts)
        {
            if (timerText != null)
            {
                UpdateTimerDisplay(timerText, defaultDuration);
                Debug.Log($"✅ Updated timer: {timerText.name}");
            }
        }
    }
    
    [ContextMenu("🔄 Reset All Timers")]
    public void ResetAllTimers()
    {
        foreach (var kvp in puzzleTimers)
        {
            var timerData = kvp.Value;
            timerData.currentTime = timerData.duration;
            timerData.isActive = false;
            
            if (timerData.timerDisplay != null)
            {
                UpdateTimerDisplay(timerData.timerDisplay, timerData.currentTime);
                timerData.timerDisplay.color = timerData.originalColor;
            }
        }
        
        Debug.Log("🔄 Reset all timers");
    }
    
    // Public methods for runtime use
    public void StartTimer(BasePuzzle puzzle)
    {
        if (puzzleTimers.TryGetValue(puzzle, out TimerData timerData))
        {
            timerData.isActive = true;
            timerData.currentTime = timerData.duration;
        }
    }
    
    public void StopTimer(BasePuzzle puzzle)
    {
        if (puzzleTimers.TryGetValue(puzzle, out TimerData timerData))
        {
            timerData.isActive = false;
        }
    }
    
    public float GetTimeRemaining(BasePuzzle puzzle)
    {
        if (puzzleTimers.TryGetValue(puzzle, out TimerData timerData))
        {
            return timerData.currentTime;
        }
        return 0f;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TimerSetupHelper))]
public class TimerSetupHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Timer Setup Tools", EditorStyles.boldLabel);
        
        TimerSetupHelper helper = (TimerSetupHelper)target;
        
        if (GUILayout.Button("🔍 Find All Timer Components", GUILayout.Height(40)))
        {
            helper.FindAllTimerComponents();
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🎯 Setup Riddle Timers", GUILayout.Height(30)))
        {
            helper.SetupRiddlePuzzleTimers();
        }
        
        if (GUILayout.Button("🧩 Setup All Puzzle Timers", GUILayout.Height(30)))
        {
            helper.SetupAllPuzzleTimers();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("🏗️ Create Timer Display", GUILayout.Height(30)))
        {
            helper.CreateTimerDisplay();
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🧪 Test Timer Setup", GUILayout.Height(25)))
        {
            helper.TestTimerSetup();
        }
        
        if (GUILayout.Button("🔄 Reset All Timers", GUILayout.Height(25)))
        {
            helper.ResetAllTimers();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This helper automatically finds and configures timer displays for your puzzles. Use the buttons above to setup timers across your project.", MessageType.Info);
        
        // Show summary
        if (helper.timerTexts.Count > 0 || helper.puzzleComponents.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Setup Summary", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Timer Displays: {helper.timerTexts.Count}");
            EditorGUILayout.LabelField($"Puzzle Components: {helper.puzzleComponents.Count}");
        }
    }
}
#endif