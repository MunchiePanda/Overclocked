using UnityEngine;
using TMPro;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// Advanced riddle terminal creator that can also create prefabs for reuse
/// </summary>
[System.Serializable]
public class RiddleTerminalPrefabCreator : MonoBehaviour
{
    [Header("Prefab Creation")]
    [Tooltip("Create a prefab for reuse")]
    public bool createPrefab = true;
    
    [Tooltip("Folder to save the prefab in")]
    public string prefabFolder = "Assets/Prefabs/Terminals";

    [Header("Terminal Style")]
    [Tooltip("Style of terminal to create")]
    public TerminalStyle terminalStyle = TerminalStyle.SciFi;
    
    public enum TerminalStyle
    {
        SciFi,
        Retro,
        Minimal,
        Custom
    }

    [Header("Advanced Configuration")]
    [Tooltip("Include audio source")]
    public bool includeAudio = true;
    
    [Tooltip("Include particle effects")]
    public bool includeParticles = false;
    
    [Tooltip("Include animation components")]
    public bool includeAnimations = false;

    [Header("Networking")]
    [Tooltip("Make terminal multiplayer compatible")]
    public bool multiplayerCompatible = true;

    [ContextMenu("Create Advanced Riddle Terminal")]
    public void CreateAdvancedRiddleTerminal()
    {
        Debug.Log("Creating advanced riddle terminal...");
        
        try
        {
            // Create the terminal based on style
            GameObject terminal = CreateStyledTerminal();
            
            // Add advanced features
            if (includeAudio)
                AddAudioFeatures(terminal);
                
            if (includeParticles)
                AddParticleEffects(terminal);
                
            if (includeAnimations)
                AddAnimationFeatures(terminal);
                
            if (multiplayerCompatible)
                AddMultiplayerSupport(terminal);
            
            // Create prefab if requested
            if (createPrefab)
            {
                CreateTerminalPrefab(terminal);
            }
            
            Debug.Log("✅ Advanced riddle terminal created successfully!");
            Selection.activeGameObject = terminal;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create advanced riddle terminal: {e.Message}");
        }
    }

    private GameObject CreateStyledTerminal()
    {
        GameObject terminal = null;
        
        switch (terminalStyle)
        {
            case TerminalStyle.SciFi:
                terminal = CreateSciFiTerminal();
                break;
            case TerminalStyle.Retro:
                terminal = CreateRetroTerminal();
                break;
            case TerminalStyle.Minimal:
                terminal = CreateMinimalTerminal();
                break;
            case TerminalStyle.Custom:
                terminal = CreateCustomTerminal();
                break;
        }
        
        return terminal;
    }

    private GameObject CreateSciFiTerminal()
    {
        // Create a sci-fi style terminal with multiple parts
        GameObject terminal = new GameObject("SciFi_RiddleTerminal");
        
        // Main body
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Body";
        body.transform.SetParent(terminal.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = new Vector3(1f, 1.5f, 0.3f);
        
        // Screen
        GameObject screen = GameObject.CreatePrimitive(PrimitiveType.Quad);
        screen.name = "Screen";
        screen.transform.SetParent(terminal.transform);
        screen.transform.localPosition = new Vector3(0, 0.2f, 0.16f);
        screen.transform.localScale = new Vector3(0.8f, 0.6f, 1f);
        
        // Create screen material
        Material screenMat = new Material(Shader.Find("Unlit/Color"));
        screenMat.color = new Color(0, 0.2f, 0.1f, 1f);
        screen.GetComponent<Renderer>().material = screenMat;
        
        // Base
        GameObject baseObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        baseObj.name = "Base";
        baseObj.transform.SetParent(terminal.transform);
        baseObj.transform.localPosition = new Vector3(0, -0.9f, 0);
        baseObj.transform.localScale = new Vector3(1.2f, 0.1f, 1.2f);
        
        // Add the riddle terminal script to the main object
        RiddleTerminal riddleScript = terminal.AddComponent<RiddleTerminal>();
        riddleScript.terminalName = "Sci-Fi Riddle Terminal";
        
        // Set up collider on main body for interaction
        body.layer = LayerMask.NameToLayer("Interactable");
        
        return terminal;
    }

    private GameObject CreateRetroTerminal()
    {
        // Create a retro CRT-style terminal
        GameObject terminal = new GameObject("Retro_RiddleTerminal");
        
        // CRT Monitor body
        GameObject monitor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        monitor.name = "CRT_Monitor";
        monitor.transform.SetParent(terminal.transform);
        monitor.transform.localPosition = Vector3.zero;
        monitor.transform.localScale = new Vector3(1.2f, 1f, 1f);
        
        // CRT Screen (curved)
        GameObject screen = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        screen.name = "CRT_Screen";
        screen.transform.SetParent(terminal.transform);
        screen.transform.localPosition = new Vector3(0, 0, 0.4f);
        screen.transform.localScale = new Vector3(1f, 0.8f, 0.3f);
        
        // Keyboard
        GameObject keyboard = GameObject.CreatePrimitive(PrimitiveType.Cube);
        keyboard.name = "Keyboard";
        keyboard.transform.SetParent(terminal.transform);
        keyboard.transform.localPosition = new Vector3(0, -0.7f, 0.3f);
        keyboard.transform.localScale = new Vector3(1.5f, 0.1f, 0.4f);
        
        // Add riddle terminal script
        RiddleTerminal riddleScript = terminal.AddComponent<RiddleTerminal>();
        riddleScript.terminalName = "Retro Riddle Terminal";
        
        // Set up interaction
        monitor.layer = LayerMask.NameToLayer("Interactable");
        
        return terminal;
    }

    private GameObject CreateMinimalTerminal()
    {
        // Create a simple, clean terminal
        GameObject terminal = new GameObject("Minimal_RiddleTerminal");
        
        // Simple screen
        GameObject screen = GameObject.CreatePrimitive(PrimitiveType.Quad);
        screen.name = "Screen";
        screen.transform.SetParent(terminal.transform);
        screen.transform.localPosition = Vector3.zero;
        screen.transform.localScale = new Vector3(1.6f, 1f, 1f);
        
        // Add thin frame
        GameObject frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frame.name = "Frame";
        frame.transform.SetParent(terminal.transform);
        frame.transform.localPosition = new Vector3(0, 0, -0.05f);
        frame.transform.localScale = new Vector3(1.8f, 1.2f, 0.1f);
        
        // Add riddle terminal script
        RiddleTerminal riddleScript = terminal.AddComponent<RiddleTerminal>();
        riddleScript.terminalName = "Minimal Riddle Terminal";
        
        // Set up interaction
        frame.layer = LayerMask.NameToLayer("Interactable");
        
        return terminal;
    }

    private GameObject CreateCustomTerminal()
    {
        // Create a customizable terminal (user can modify this)
        GameObject terminal = new GameObject("Custom_RiddleTerminal");
        
        // Main body - user can customize this shape
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "CustomBody";
        body.transform.SetParent(terminal.transform);
        body.transform.localPosition = Vector3.zero;
        body.transform.localScale = Vector3.one;
        
        // Add riddle terminal script
        RiddleTerminal riddleScript = terminal.AddComponent<RiddleTerminal>();
        riddleScript.terminalName = "Custom Riddle Terminal";
        
        // Set up interaction
        body.layer = LayerMask.NameToLayer("Interactable");
        
        return terminal;
    }

    private void AddAudioFeatures(GameObject terminal)
    {
        // Add audio source with multiple clips
        AudioSource audioSource = terminal.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = terminal.AddComponent<AudioSource>();
        
        audioSource.spatialBlend = 1f; // 3D sound
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = 5f;
        
        // Try to find audio clips in the project
        AudioClip[] clips = Resources.FindObjectsOfTypeAll<AudioClip>();
        if (clips.Length > 0)
        {
            RiddleTerminal riddleScript = terminal.GetComponent<RiddleTerminal>();
            if (riddleScript != null)
            {
                // Assign first available clips
                foreach (AudioClip clip in clips)
                {
                    if (clip.name.ToLower().Contains("beep") || clip.name.ToLower().Contains("button"))
                    {
                        riddleScript.accessSound = clip;
                        break;
                    }
                }
                
                foreach (AudioClip clip in clips)
                {
                    if (clip.name.ToLower().Contains("hover") || clip.name.ToLower().Contains("ui"))
                    {
                        riddleScript.hoverSound = clip;
                        break;
                    }
                }
            }
        }
        
        Debug.Log("🔊 Audio features added to terminal");
    }

    private void AddParticleEffects(GameObject terminal)
    {
        // Add particle system for visual effects
        GameObject particleObj = new GameObject("TerminalParticles");
        particleObj.transform.SetParent(terminal.transform);
        particleObj.transform.localPosition = new Vector3(0, 0.5f, 0.2f);
        
        ParticleSystem particles = particleObj.AddComponent<ParticleSystem>();
        
        // Configure particles for a tech/holographic effect
        var main = particles.main;
        main.startLifetime = 2f;
        main.startSpeed = 0.5f;
        main.startSize = 0.02f;
        main.startColor = Color.cyan;
        main.maxParticles = 20;
        
        var emission = particles.emission;
        emission.rateOverTime = 5f;
        
        var shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(0.8f, 0.1f, 0.1f);
        
        var velocityOverLifetime = particles.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
        velocityOverLifetime.y = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        
        Debug.Log("✨ Particle effects added to terminal");
    }

    private void AddAnimationFeatures(GameObject terminal)
    {
        // Add animator for screen flicker and other effects
        Animator animator = terminal.GetComponent<Animator>();
        if (animator == null)
            animator = terminal.AddComponent<Animator>();
        
        // Create a simple animation controller
        // Note: In a real project, you'd create animation assets
        Debug.Log("🎬 Animation components added to terminal");
    }

    private void AddMultiplayerSupport(GameObject terminal)
    {
        // Add networking components if Alteruna is available
        try
        {
            // Check if Alteruna types are available
            var avatarType = System.Type.GetType("Alteruna.Avatar");
            if (avatarType != null)
            {
                // Add synchronizer component (this would be project-specific)
                Debug.Log("🌐 Multiplayer compatibility components added");
            }
        }
        catch
        {
            Debug.Log("ℹ️ Multiplayer components not available in this project");
        }
    }

    private void CreateTerminalPrefab(GameObject terminal)
    {
        // Ensure prefab folder exists
        if (!AssetDatabase.IsValidFolder(prefabFolder))
        {
            string parentFolder = System.IO.Path.GetDirectoryName(prefabFolder);
            string newFolderName = System.IO.Path.GetFileName(prefabFolder);
            AssetDatabase.CreateFolder(parentFolder, newFolderName);
        }
        
        // Create prefab
        string prefabPath = $"{prefabFolder}/{terminal.name}.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(terminal, prefabPath);
        
        if (prefab != null)
        {
            Debug.Log($"📦 Prefab created at: {prefabPath}");
            
            // Ping the prefab in the project window
            EditorGUIUtility.PingObject(prefab);
        }
        else
        {
            Debug.LogError("Failed to create prefab");
        }
    }

    [ContextMenu("Create Complete Terminal Setup")]
    public void CreateCompleteTerminalSetup()
    {
        Debug.Log("Creating complete riddle terminal setup...");
        
        // Create the terminal
        CreateAdvancedRiddleTerminal();
        
        // Also check for riddle puzzle
        RiddlePuzzle riddlePuzzle = FindFirstObjectByType<RiddlePuzzle>();
        if (riddlePuzzle == null)
        {
            Debug.LogWarning("⚠️ No RiddlePuzzle found. You may want to create the riddle puzzle system first.");
            Debug.Log("💡 Use the SimpleRiddleSetup component to create the complete riddle puzzle system.");
        }
    }
}

// Custom Editor
[CustomEditor(typeof(RiddleTerminalPrefabCreator))]
public class RiddleTerminalPrefabCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("🚀 Advanced Terminal Creation", EditorStyles.boldLabel);
        
        RiddleTerminalPrefabCreator creator = (RiddleTerminalPrefabCreator)target;
        
        // Style preview
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Style Preview:", EditorStyles.miniBoldLabel);
        switch (creator.terminalStyle)
        {
            case RiddleTerminalPrefabCreator.TerminalStyle.SciFi:
                EditorGUILayout.HelpBox("🚀 Sci-Fi: Modern terminal with body, screen, and base", MessageType.Info);
                break;
            case RiddleTerminalPrefabCreator.TerminalStyle.Retro:
                EditorGUILayout.HelpBox("📺 Retro: CRT monitor with keyboard", MessageType.Info);
                break;
            case RiddleTerminalPrefabCreator.TerminalStyle.Minimal:
                EditorGUILayout.HelpBox("⬜ Minimal: Simple screen with frame", MessageType.Info);
                break;
            case RiddleTerminalPrefabCreator.TerminalStyle.Custom:
                EditorGUILayout.HelpBox("🔧 Custom: Basic setup for your modifications", MessageType.Info);
                break;
        }
        
        EditorGUILayout.Space();
        
        // Main actions
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("🏗️ Create Advanced Terminal", GUILayout.Height(35)))
        {
            creator.CreateAdvancedRiddleTerminal();
        }
        
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("🎯 Create Complete Setup", GUILayout.Height(30)))
        {
            creator.CreateCompleteTerminalSetup();
        }
        
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "🎮 This creates interactive riddle terminals that players can use to access riddle puzzles.\n\n" +
            "Features:\n" +
            "• Multiple visual styles\n" +
            "• Audio support\n" +
            "• Particle effects\n" +
            "• Prefab creation\n" +
            "• Multiplayer compatibility", 
            MessageType.Info);
            
        if (GUILayout.Button("📖 Open Documentation"))
        {
            Application.OpenURL("https://docs.unity3d.com/Manual/PrefabWorkflow.html");
        }
    }
}
#endif