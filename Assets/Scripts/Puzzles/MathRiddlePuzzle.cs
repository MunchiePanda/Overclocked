using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Math riddle puzzle where the player needs to solve a math problem
/// </summary>
public class MathRiddlePuzzle : BasePuzzle
{
    [Header("Math Riddle Settings")]
    [Tooltip("The math problem to solve")]
    public string mathProblem = "2 + 2 * 2 = ?";

    [Tooltip("The correct answer to the math problem")]
    public string correctAnswer = "6";

    [Tooltip("Input field for the player's answer")]
    public TMP_InputField answerInputField;

    [Tooltip("Button to submit the answer")]
    public Button submitButton;

    [Header("UI Feedback")]
    [Tooltip("Color for correct answer feedback")]
    public Color correctAnswerColor = Color.green;

    [Tooltip("Color for incorrect answer feedback")]
    public Color incorrectAnswerColor = Color.red;

    private string playerAnswer = "";

    /// <summary>
    /// Initialize the puzzle
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        // Set up the puzzle description
        if (descriptionText != null)
        {
            descriptionText.text = "Solve the math problem:\n\n" + mathProblem;
        }

        // Set up the input field
        if (answerInputField != null)
        {
            answerInputField.onValueChanged.AddListener(OnAnswerChanged);
            answerInputField.onSubmit.AddListener(OnAnswerSubmitted);
        }

        // Set up the submit button
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnAnswerSubmitted);
        }

        Debug.Log("Math Riddle Puzzle initialized");
    }

    /// <summary>
    /// Called when the player changes their answer
    /// </summary>
    /// <param name="newAnswer">The new answer</param>
    private void OnAnswerChanged(string newAnswer)
    {
        playerAnswer = newAnswer;
        Debug.Log("Player entered answer: " + playerAnswer);
    }

    /// <summary>
    /// Called when the player submits their answer
    /// </summary>
    /// <param name="answer">The submitted answer</param>
    private void OnAnswerSubmitted(string answer)
    {
        playerAnswer = answer;
        CheckAnswer();
    }

    /// <summary>
    /// Overload for button click
    /// </summary>
    private void OnAnswerSubmitted()
    {
        CheckAnswer();
    }

    /// <summary>
    /// Check if the player's answer is correct
    /// </summary>
    private void CheckAnswer()
    {
        if (string.IsNullOrEmpty(playerAnswer))
        {
            GiveFeedback("Please enter an answer");
            return;
        }

        if (playerAnswer.Trim().Equals(correctAnswer, System.StringComparison.OrdinalIgnoreCase))
        {
            // Correct answer
            if (feedbackText != null)
            {
                feedbackText.text = "Correct! Puzzle solved!";
                feedbackText.color = correctAnswerColor;
            }

            CompletePuzzle();
        }
        else
        {
            // Incorrect answer
            if (feedbackText != null)
            {
                feedbackText.text = "Incorrect. Try again!";
                feedbackText.color = incorrectAnswerColor;
            }
        }
    }

    /// <summary>
    /// Reset the puzzle to its initial state
    /// </summary>
    public override void ResetPuzzle()
    {
        base.ResetPuzzle();

        if (answerInputField != null)
        {
            answerInputField.text = "";
        }

        playerAnswer = "";

        if (feedbackText != null)
        {
            feedbackText.text = "";
            feedbackText.color = Color.white;
        }
    }

    /// <summary>
    /// Show the puzzle UI
    /// </summary>
    public override void ShowPuzzle()
    {
        base.ShowPuzzle();

        if (answerInputField != null)
        {
            answerInputField.ActivateInputField();
            answerInputField.Select();
        }
    }
}
