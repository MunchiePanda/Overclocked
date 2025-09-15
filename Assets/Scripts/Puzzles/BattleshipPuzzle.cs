using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Battleship-style puzzle on a 9x9 grid
/// </summary>
public class BattleshipPuzzle : BasePuzzle
{
    [Header("Battleship Puzzle Settings")]
    [Tooltip("Size of the grid (9x9)")]
    public int gridSize = 9;

    [Tooltip("Number of ships to find")]
    public int shipCount = 3;

    [Tooltip("Prefabs for grid cells")]
    public GameObject gridCellPrefab;

    [Tooltip("Parent transform for grid cells")]
    public Transform gridParent;

    [Tooltip("Color for empty cells")]
    public Color emptyCellColor = Color.white;

    [Tooltip("Color for ship cells")]
    public Color shipCellColor = Color.blue;

    [Tooltip("Color for hit cells")]
    public Color hitCellColor = Color.red;

    [Tooltip("Color for miss cells")]
    public Color missCellColor = Color.gray;

    [Tooltip("Button to submit guesses")]
    public Button submitButton;

    // Internal variables
    private List<Vector2Int> shipPositions = new List<Vector2Int>();
    private List<Vector2Int> guessedPositions = new List<Vector2Int>();
    private GameObject[,] gridCells;
    private int shipsFound = 0;

    /// <summary>
    /// Initialize the puzzle
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        // Initialize the grid
        gridCells = new GameObject[gridSize, gridSize];

        // Generate ship positions
        GenerateShipPositions();

        // Create the grid UI
        CreateGridUI();

        // Set up the submit button
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(CheckCompletion);
        }

        Debug.Log("Battleship Puzzle initialized with " + shipCount + " ships");
    }

    /// <summary>
    /// Generate random positions for the ships
    /// </summary>
    private void GenerateShipPositions()
    {
        shipPositions.Clear();

        for (int i = 0; i < shipCount; i++)
        {
            Vector2Int position;
            do
            {
                position = new Vector2Int(
                    Random.Range(0, gridSize),
                    Random.Range(0, gridSize)
                );
            } while (shipPositions.Contains(position));

            shipPositions.Add(position);
            Debug.Log("Ship " + (i+1) + " at position: " + position);
        }
    }

    /// <summary>
    /// Create the grid UI
    /// </summary>
    private void CreateGridUI()
    {
        if (gridCellPrefab == null || gridParent == null)
        {
            Debug.LogError("Grid cell prefab or parent transform not assigned!");
            return;
        }

        // Clear existing grid
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        // Create new grid
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                GameObject cell = Instantiate(gridCellPrefab, gridParent);
                cell.name = "Cell_" + row + "_" + col;

                // Position the cell
                RectTransform rt = cell.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchoredPosition = new Vector2(
                        col * 50 - (gridSize * 50) / 2 + 25,
                        -row * 50 + (gridSize * 50) / 2 - 25
                    );
                }

                // Set up the cell
                Image cellImage = cell.GetComponent<Image>();
                if (cellImage != null)
                {
                    cellImage.color = emptyCellColor;
                }

                // Add button component for interaction
                Button cellButton = cell.GetComponent<Button>();
                if (cellButton == null)
                {
                    cellButton = cell.AddComponent<Button>();
                }

                int cellRow = row;
                int cellCol = col;
                cellButton.onClick.AddListener(() => OnCellClicked(cellRow, cellCol));

                gridCells[row, col] = cell;
            }
        }
    }

    /// <summary>
    /// Called when a cell is clicked
    /// </summary>
    /// <param name="row">Row of the clicked cell</param>
    /// <param name="col">Column of the clicked cell</param>
    private void OnCellClicked(int row, int col)
    {
        Vector2Int position = new Vector2Int(col, row);

        // Check if this position has already been guessed
        if (guessedPositions.Contains(position))
        {
            GiveFeedback("You've already guessed this position!");
            return;
        }

        // Add to guessed positions
        guessedPositions.Add(position);

        // Check if it's a hit
        if (shipPositions.Contains(position))
        {
            // Hit a ship
            shipsFound++;
            UpdateCellColor(row, col, hitCellColor);
            GiveFeedback("Hit! " + shipsFound + "/" + shipCount + " ships found.");

            // Check if all ships are found
            if (shipsFound >= shipCount)
            {
                CompletePuzzle();
            }
        }
        else
        {
            // Miss
            UpdateCellColor(row, col, missCellColor);
            GiveFeedback("Miss! Keep trying.");
        }
    }

    /// <summary>
    /// Update the color of a grid cell
    /// </summary>
    /// <param name="row">Row of the cell</param>
    /// <param name="col">Column of the cell</param>
    /// <param name="color">Color to set</param>
    private void UpdateCellColor(int row, int col, Color color)
    {
        if (row >= 0 && row < gridSize && col >= 0 && col < gridSize)
        {
            GameObject cell = gridCells[row, col];
            if (cell != null)
            {
                Image cellImage = cell.GetComponent<Image>();
                if (cellImage != null)
                {
                    cellImage.color = color;
                }
            }
        }
    }

    /// <summary>
    /// Check if the puzzle is completed
    /// </summary>
    private void CheckCompletion()
    {
        if (shipsFound >= shipCount)
        {
            CompletePuzzle();
        }
        else
        {
            GiveFeedback("You haven't found all the ships yet! Found " + shipsFound + "/" + shipCount);
        }
    }

    /// <summary>
    /// Reset the puzzle to its initial state
    /// </summary>
    public override void ResetPuzzle()
    {
        base.ResetPuzzle();

        // Reset ships found
        shipsFound = 0;

        // Clear guessed positions
        guessedPositions.Clear();

        // Generate new ship positions
        GenerateShipPositions();

        // Reset grid colors
        if (gridCells != null)
        {
            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    UpdateCellColor(row, col, emptyCellColor);
                }
            }
        }
    }

    /// <summary>
    /// Show the puzzle UI
    /// </summary>
    public override void ShowPuzzle()
    {
        base.ShowPuzzle();

        // Reset the puzzle when shown
        ResetPuzzle();
    }
}
