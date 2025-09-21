using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FrequencyResonancePuzzle : BasePuzzle
{
    [Header("Frequency Settings")]
    [Tooltip("Audio sources for different frequency tones")]
    public AudioSource[] frequencySources;
    
    [Tooltip("Buttons to play different frequencies")]
    public Button[] frequencyButtons;
    
    [Tooltip("Slider to adjust frequency tuning")]
    public Slider frequencySlider;
    
    [Tooltip("Display showing current frequency")]
    public TMP_Text frequencyDisplay;
    
    [Tooltip("Button to submit the frequency sequence")]
    public Button submitSequenceButton;
    
    [Tooltip("Button to clear the current sequence")]
    public Button clearSequenceButton;
    
    [Tooltip("Display showing the current sequence")]
    public TMP_Text sequenceDisplay;
    
    [Tooltip("Visual waveform display")]
    public Image[] waveformIndicators;

    [Header("Target Configuration")]
    [Tooltip("Target frequencies for each tone")]
    public float[] targetFrequencies = {220f, 440f, 659f, 880f, 1100f};
    
    [Tooltip("Correct sequence order (0-based indices)")]
    public int[] correctSequence = {1, 3, 0, 4, 2}; // B, D, A, E, C
    
    [Tooltip("Frequency tolerance for matching")]
    public float frequencyTolerance = 20f;

    private List<int> playerSequence = new List<int>();
    private float currentFrequency = 440f;
    private int randomNumber;
    private bool isFrequencyMatched = false;

    public override void Initialize()
    {
        base.Initialize();
        randomNumber = Random.Range(2000, 9999);
        
        SetupButtons();
        SetupFrequencySlider();
        InitializeAudioSources();
        UpdateDisplay();
    }

    private void SetupButtons()
    {
        for (int i = 0; i < frequencyButtons.Length; i++)
        {
            int index = i; // Capture the loop variable
            if (frequencyButtons[i] != null)
                frequencyButtons[i].onClick.AddListener(() => PlayFrequency(index));
        }
        
        if (submitSequenceButton != null)
            submitSequenceButton.onClick.AddListener(CheckSequence);
            
        if (clearSequenceButton != null)
            clearSequenceButton.onClick.AddListener(ClearSequence);
    }

    private void SetupFrequencySlider()
    {
        if (frequencySlider != null)
        {
            frequencySlider.minValue = 0f;
            frequencySlider.maxValue = 1f;
            frequencySlider.value = 0.5f;
            frequencySlider.onValueChanged.AddListener(OnFrequencyChanged);
        }
    }

    private void InitializeAudioSources()
    {
        for (int i = 0; i < frequencySources.Length && i < targetFrequencies.Length; i++)
        {
            if (frequencySources[i] != null)
            {
                // Set up basic sine wave properties
                frequencySources[i].volume = 0.5f;
                frequencySources[i].pitch = targetFrequencies[i] / 440f; // Relative to A4
            }
        }
    }

    private void PlayFrequency(int index)
    {
        if (index >= frequencySources.Length || frequencySources[index] == null)
            return;

        // Check if current frequency matches the target frequency for this button
        float targetFreq = targetFrequencies[index];
        bool frequencyMatches = Mathf.Abs(currentFrequency - targetFreq) <= frequencyTolerance;

        if (frequencyMatches)
        {
            // Play the tone and add to sequence
            frequencySources[index].pitch = currentFrequency / 440f;
            frequencySources[index].Play();
            
            playerSequence.Add(index);
            UpdateWaveformIndicator(index, true);
            
            GiveFeedback($"Tone {index + 1} activated. Frequency matched!");
        }
        else
        {
            // Play a discordant tone to indicate mismatch
            frequencySources[index].pitch = (currentFrequency + 50f) / 440f; // Slightly off
            frequencySources[index].Play();
            
            UpdateWaveformIndicator(index, false);
            GiveFeedback($"Frequency mismatch. Adjust to {targetFreq:F0} Hz for Tone {index + 1}");
        }

        UpdateSequenceDisplay();
    }

    private void OnFrequencyChanged(float value)
    {
        // Map slider value to frequency range (200Hz to 1200Hz)
        currentFrequency = Mathf.Lerp(200f, 1200f, value);
        
        if (frequencyDisplay != null)
            frequencyDisplay.text = $"{currentFrequency:F0} Hz";

        // Update visual feedback for frequency matching
        UpdateFrequencyMatchIndicator();
    }

    private void UpdateFrequencyMatchIndicator()
    {
        // Check if current frequency is close to any target frequency
        isFrequencyMatched = false;
        for (int i = 0; i < targetFrequencies.Length; i++)
        {
            if (Mathf.Abs(currentFrequency - targetFrequencies[i]) <= frequencyTolerance)
            {
                isFrequencyMatched = true;
                break;
            }
        }

        // Update frequency display color based on match
        if (frequencyDisplay != null)
        {
            frequencyDisplay.color = isFrequencyMatched ? Color.green : Color.white;
        }
    }

    private void UpdateWaveformIndicator(int index, bool isMatched)
    {
        if (index < waveformIndicators.Length && waveformIndicators[index] != null)
        {
            waveformIndicators[index].color = isMatched ? Color.green : Color.red;
        }
    }

    private void UpdateSequenceDisplay()
    {
        if (sequenceDisplay != null)
        {
            string display = "Frequency Sequence: ";
            for (int i = 0; i < playerSequence.Count; i++)
            {
                display += $"T{playerSequence[i] + 1}";
                if (i < playerSequence.Count - 1)
                    display += " → ";
            }
            
            if (playerSequence.Count == 0)
                display += "None";
                
            sequenceDisplay.text = display;
        }
    }

    private void ClearSequence()
    {
        playerSequence.Clear();
        UpdateSequenceDisplay();
        
        // Reset waveform indicators
        for (int i = 0; i < waveformIndicators.Length; i++)
        {
            if (waveformIndicators[i] != null)
                waveformIndicators[i].color = Color.gray;
        }
        
        GiveFeedback("Sequence cleared. Start over.");
    }

    private void CheckSequence()
    {
        if (playerSequence.Count != correctSequence.Length)
        {
            GiveFeedback($"Sequence incomplete. Need {correctSequence.Length} tones, have {playerSequence.Count}.");
            return;
        }

        bool isCorrect = true;
        for (int i = 0; i < playerSequence.Count; i++)
        {
            if (playerSequence[i] != correctSequence[i])
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            GiveFeedback($"Perfect harmonic resonance achieved! Your escape number is: {randomNumber}");
            CompletePuzzle();
            
            // Notify the escape code manager
            EscapeCodeManager escapeManager = FindFirstObjectByType<EscapeCodeManager>();
            if (escapeManager != null)
                escapeManager.OnPuzzleCompleted(randomNumber);
        }
        else
        {
            GiveFeedback("Harmonic sequence incorrect. Listen carefully to the resonance patterns.");
            ClearSequence();
        }
    }

    public override void ResetPuzzle()
    {
        base.ResetPuzzle();
        ClearSequence();
        
        if (frequencySlider != null)
            frequencySlider.value = 0.5f;
            
        currentFrequency = 440f;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (frequencyDisplay != null)
            frequencyDisplay.text = $"{currentFrequency:F0} Hz";
            
        UpdateSequenceDisplay();
        UpdateFrequencyMatchIndicator();
    }

    public override void ShowPuzzle()
    {
        base.ShowPuzzle();
        
        // Give initial instruction
        GiveFeedback("Adjust frequency and activate tones in the correct harmonic sequence.");
    }
}