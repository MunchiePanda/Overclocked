using UnityEngine;

/// <summary>
/// Quick tester for colorblindness effect
/// </summary>
public class TestColorblindness : MonoBehaviour
{
    [ContextMenu("🧪 Test Deuteranopia")]
    void TestDeuteranopia()
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetColorBlindnessEnabled(true);
            GameSettings.Instance.SetColorBlindnessType(ColorBlindnessType.Deuteranopia);
            Debug.Log("🧪 Deuteranopia (Green-blind) mode enabled!");
        }
    }
    
    [ContextMenu("🧪 Test Protanopia")]
    void TestProtanopia()
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetColorBlindnessEnabled(true);
            GameSettings.Instance.SetColorBlindnessType(ColorBlindnessType.Protanopia);
            Debug.Log("🧪 Protanopia (Red-blind) mode enabled!");
        }
    }
    
    [ContextMenu("🧪 Disable Effect")]
    void DisableEffect()
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetColorBlindnessEnabled(false);
            Debug.Log("🧪 Colorblindness effect disabled");
        }
    }
}