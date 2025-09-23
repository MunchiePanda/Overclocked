using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class EscapeCodeManager : MonoBehaviour
{
    [Header("Code Collection")]
    [Tooltip("List of numbers collected from completed puzzles")]
    public List<int> collectedNumbers = new List<int>();
    
    [Tooltip("The door that will be unlocked with the final code")]
    public Door escapeDoor;
    
    [Tooltip("Win screen to show when escape is successful")]
    public WinScreen winScreen;

    [Header("UI References")]
    [Tooltip("Display showing collected numbers")]
    public TMP_Text collectedNumbersDisplay;
    
    [Tooltip("Display showing the final escape code")]
    public TMP_Text finalCodeDisplay;
    
    [Tooltip("Progress display showing puzzle completion status")]
    public TMP_Text progressDisplay;
    
    [Tooltip("Instructions panel")]
    public GameObject instructionsPanel;

    [Header("Code Generation Settings")]
    [Tooltip("Number of puzzles required to generate escape code")]
    public int requiredPuzzleCount = 3;
    
    [Tooltip("Method for generating final code")]
    public CodeGenerationMethod codeMethod = CodeGenerationMethod.LastTwoDigits;

    public enum CodeGenerationMethod
    {
        LastTwoDigits,      // Take last 2 digits of each number
        FirstTwoDigits,     // Take first 2 digits of each number
        Sum,                // Sum all numbers and use as code
        Product,            // Multiply all numbers (mod 10000)
        Concatenate         // Just put all numbers together
    }

    private string finalEscapeCode = "";
    private bool isEscapeCodeGenerated = false;

    void Start()
    {
        UpdateAllDisplays();
        
        // Show instructions at start
        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);
    }

    public void OnPuzzleCompleted(int randomNumber)
    {
        // Prevent duplicate numbers
        if (!collectedNumbers.Contains(randomNumber))
        {
            collectedNumbers.Add(randomNumber);
            Debug.Log($"Puzzle completed! Number collected: {randomNumber}");
            
            UpdateAllDisplays();
            
            // Check if we have enough numbers to generate the escape code
            if (collectedNumbers.Count >= requiredPuzzleCount && !isEscapeCodeGenerated)
            {
                GenerateFinalEscapeCode();
            }
        }
        else
        {
            Debug.LogWarning($"Number {randomNumber} already collected!");
        }
    }

    private void GenerateFinalEscapeCode()
    {
        if (collectedNumbers.Count < requiredPuzzleCount)
        {
            Debug.LogWarning("Not enough numbers collected to generate escape code!");
            return;
        }

        // Sort numbers to ensure consistent code generation
        var sortedNumbers = collectedNumbers.OrderBy(x => x).ToList();
        
        switch (codeMethod)
        {
            case CodeGenerationMethod.LastTwoDigits:
                finalEscapeCode = GenerateFromLastDigits(sortedNumbers);
                break;
                
            case CodeGenerationMethod.FirstTwoDigits:
                finalEscapeCode = GenerateFromFirstDigits(sortedNumbers);
                break;
                
            case CodeGenerationMethod.Sum:
                finalEscapeCode = GenerateFromSum(sortedNumbers);
                break;
                
            case CodeGenerationMethod.Product:
                finalEscapeCode = GenerateFromProduct(sortedNumbers);
                break;
                
            case CodeGenerationMethod.Concatenate:
                finalEscapeCode = GenerateFromConcatenation(sortedNumbers);
                break;
        }

        isEscapeCodeGenerated = true;

        // Update the escape door with the final code
        if (escapeDoor != null)
        {
            escapeDoor.correctCode = finalEscapeCode;
            Debug.Log($"Escape door code set to: {finalEscapeCode}");
        }

        UpdateAllDisplays();
        
        // Hide instructions panel
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);

        Debug.Log($"Final escape code generated: {finalEscapeCode}");
    }

    private string GenerateFromLastDigits(List<int> numbers)
    {
        string code = "";
        foreach (int number in numbers)
        {
            code += (number % 100).ToString("D2");
        }
        return code;
    }

    private string GenerateFromFirstDigits(List<int> numbers)
    {
        string code = "";
        foreach (int number in numbers)
        {
            string numStr = number.ToString();
            if (numStr.Length >= 2)
                code += numStr.Substring(0, 2);
            else
                code += numStr.PadLeft(2, '0');
        }
        return code;
    }

    private string GenerateFromSum(List<int> numbers)
    {
        int sum = numbers.Sum();
        return (sum % 10000).ToString("D4"); // Ensure 4-digit code
    }

    private string GenerateFromProduct(List<int> numbers)
    {
        long product = 1;
        foreach (int number in numbers)
        {
            product *= number;
            product %= 1000000; // Prevent overflow
        }
        return (product % 10000).ToString("D4");
    }

    private string GenerateFromConcatenation(List<int> numbers)
    {
        string code = "";
        foreach (int number in numbers)
        {
            code += number.ToString();
        }
        
        // If too long, take first 6 digits
        if (code.Length > 6)
            code = code.Substring(0, 6);
            
        return code;
    }

    private void UpdateAllDisplays()
    {
        UpdateCollectedNumbersDisplay();
        UpdateProgressDisplay();
        UpdateFinalCodeDisplay();
    }

    private void UpdateCollectedNumbersDisplay()
    {
        if (collectedNumbersDisplay == null) return;

        if (collectedNumbers.Count == 0)
        {
            collectedNumbersDisplay.text = "Puzzle Numbers Collected:\nNone yet...";
        }
        else
        {
            string display = "Puzzle Numbers Collected:\n";
            for (int i = 0; i < collectedNumbers.Count; i++)
            {
                display += $"Puzzle {i + 1}: {collectedNumbers[i]}\n";
            }
            collectedNumbersDisplay.text = display;
        }
    }

    private void UpdateProgressDisplay()
    {
        if (progressDisplay == null) return;

        string progress = $"Escape Progress: {collectedNumbers.Count}/{requiredPuzzleCount} puzzles completed";
        
        if (isEscapeCodeGenerated)
            progress += "\nESCAPE CODE READY!";
        else if (collectedNumbers.Count > 0)
            progress += $"\nNeed {requiredPuzzleCount - collectedNumbers.Count} more puzzle(s)";

        progressDisplay.text = progress;
    }

    private void UpdateFinalCodeDisplay()
    {
        if (finalCodeDisplay == null) return;

        if (isEscapeCodeGenerated)
        {
            finalCodeDisplay.text = $"FINAL ESCAPE CODE: {finalEscapeCode}";
            finalCodeDisplay.color = Color.green;
        }
        else
        {
            finalCodeDisplay.text = "Complete all puzzles to reveal escape code";
            finalCodeDisplay.color = Color.gray;
        }
    }

    public void OnEscapeSuccessful()
    {
        Debug.Log("Escape successful! Showing win screen.");
        
        if (winScreen != null)
        {
            winScreen.ShowWinScreen();
        }
        else
        {
            Debug.LogWarning("Win screen component not assigned!");
        }
    }

    public void ResetEscapeProgress()
    {
        collectedNumbers.Clear();
        finalEscapeCode = "";
        isEscapeCodeGenerated = false;
        
        if (escapeDoor != null)
        {
            escapeDoor.ResetDoor();
            escapeDoor.correctCode = "0000"; // Placeholder
        }

        if (winScreen != null)
        {
            winScreen.HideWinScreen();
        }

        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);

        UpdateAllDisplays();
        Debug.Log("Escape progress reset.");
    }

    // Public getters for other scripts
    public bool IsEscapeCodeReady() => isEscapeCodeGenerated;
    public string GetFinalEscapeCode() => finalEscapeCode;
    public int GetCollectedCount() => collectedNumbers.Count;
    public int GetRequiredCount() => requiredPuzzleCount;

    // For debugging/testing
    [ContextMenu("Test Generate Code")]
    private void TestGenerateCode()
    {
        // Add some test numbers
        collectedNumbers.Clear();
        collectedNumbers.AddRange(new int[] { 1234, 5678, 9012 });
        GenerateFinalEscapeCode();
    }
}