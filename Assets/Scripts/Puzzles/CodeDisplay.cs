using UnityEngine;
using TMPro;

/// <summary>
/// Handles displaying the code when all pressure plates are activated
/// </summary>
public class CodeDisplay : MonoBehaviour
{
    [Header("Code Display Settings")]
    [Tooltip("The text component to display the code")]
    public TMP_Text codeText;

    [Tooltip("The code to display")]
    public string code = "1234";

    [Tooltip("The game object to show when active")]
    public GameObject codeDisplayObject;

    [Header("Visual Feedback")]
    [Tooltip("The color to use for the code text")]
    public Color codeColor = Color.green;

    [Tooltip("Should the code be hidden when inactive?")]
    public bool hideWhenInactive = true;

    private void Start()
    {
        // Initialize the display
        InitializeDisplay();
    }

    /// <summary>
    /// Initialize the code display
    /// </summary>
    private void InitializeDisplay()
    {
        if (codeText != null)
        {
            codeText.text = code;
            codeText.color = codeColor;
        }

        if (hideWhenInactive && codeDisplayObject != null)
        {
            codeDisplayObject.SetActive(false);
        }
    }

    /// <summary>
    /// Show the code display
    /// </summary>
    public void ShowCode()
    {
        if (codeDisplayObject != null)
        {
            codeDisplayObject.SetActive(true);
        }

        if (codeText != null)
        {
            codeText.text = code;
            codeText.color = codeColor;
        }

        Debug.Log("Showing code: " + code);
    }

    /// <summary>
    /// Hide the code display
    /// </summary>
    public void HideCode()
    {
        if (codeDisplayObject != null)
        {
            codeDisplayObject.SetActive(false);
        }

        Debug.Log("Hiding code");
    }

    /// <summary>
    /// Update the code to display
    /// </summary>
    /// <param name="newCode">The new code to display</param>
    public void UpdateCode(string newCode)
    {
        code = newCode;

        if (codeText != null)
        {
            codeText.text = code;
        }

        Debug.Log("Updated code to: " + code);
    }
}
