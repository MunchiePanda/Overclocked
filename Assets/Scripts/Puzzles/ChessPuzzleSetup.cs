using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Setup helper for creating chess puzzle UI
/// </summary>
public class ChessPuzzleSetup : MonoBehaviour
{
    [Header("🏗️ Auto Setup")]
    [Tooltip("Click to automatically create chess puzzle UI")]
    public bool createChessPuzzleUI = false;
    
    [Header("🎯 Configuration")]
    [Tooltip("Canvas to create the chess UI in")]
    public Canvas targetCanvas;
    
    [Tooltip("Square size in pixels")]
    public float squareSize = 60f;
    
    [Tooltip("Board border padding")]
    public float boardPadding = 20f;

    private void OnValidate()
    {
        if (createChessPuzzleUI)
        {
            CreateChessPuzzleUI();
            createChessPuzzleUI = false;
        }
    }

    [ContextMenu("🏗️ Create Chess Puzzle UI")]
    public void CreateChessPuzzleUI()
    {
        Debug.Log("🏗️ Creating chess puzzle UI...");

        // Find or create target canvas
        if (targetCanvas == null)
        {
            targetCanvas = FindFirstObjectByType<Canvas>();
            if (targetCanvas == null)
            {
                GameObject canvasObj = new GameObject("ChessCanvas");
                targetCanvas = canvasObj.AddComponent<Canvas>();
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
                
                targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                targetCanvas.sortingOrder = 50;
            }
        }

        // Create main chess panel
        GameObject chessPanel = CreateChessPanel();
        
        // Create chess board
        GameObject chessBoard = CreateChessBoard(chessPanel);
        
        // Create UI controls
        CreateChessControls(chessPanel);
        
        // Add ChessPuzzle component
        ChessPuzzle chessPuzzle = GetComponent<ChessPuzzle>();
        if (chessPuzzle == null)
        {
            chessPuzzle = gameObject.AddComponent<ChessPuzzle>();
        }
        
        // Auto-assign references
        AssignChessPuzzleReferences(chessPuzzle, chessPanel, chessBoard);
        
        Debug.Log("✅ Chess puzzle UI created successfully!");
    }

    private GameObject CreateChessPanel()
    {
        GameObject panel = new GameObject("ChessPuzzlePanel");
        panel.transform.SetParent(targetCanvas.transform, false);
        
        // Add RectTransform and Image
        RectTransform rect = panel.AddComponent<RectTransform>();
        Image image = panel.AddComponent<Image>();
        
        // Configure panel
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        image.color = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Semi-transparent dark background
        
        // Start inactive
        panel.SetActive(false);
        
        return panel;
    }

    private GameObject CreateChessBoard(GameObject parent)
    {
        GameObject boardContainer = new GameObject("ChessBoard");
        boardContainer.transform.SetParent(parent.transform, false);
        
        RectTransform containerRect = boardContainer.AddComponent<RectTransform>();
        
        // Center the board
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = new Vector2(-100f, 50f); // Offset left for controls
        
        float boardSize = squareSize * 8 + boardPadding * 2;
        containerRect.sizeDelta = new Vector2(boardSize, boardSize);
        
        // Add background
        Image boardBg = boardContainer.AddComponent<Image>();
        boardBg.color = new Color(0.3f, 0.2f, 0.1f, 1f); // Brown board border
        
        // Create grid layout for squares
        GameObject squaresParent = new GameObject("Squares");
        squaresParent.transform.SetParent(boardContainer.transform, false);
        
        RectTransform squaresRect = squaresParent.AddComponent<RectTransform>();
        squaresRect.anchorMin = Vector2.zero;
        squaresRect.anchorMax = Vector2.one;
        squaresRect.offsetMin = new Vector2(boardPadding, boardPadding);
        squaresRect.offsetMax = new Vector2(-boardPadding, -boardPadding);
        
        GridLayoutGroup grid = squaresParent.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(squareSize, squareSize);
        grid.spacing = Vector2.zero;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 8;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.childAlignment = TextAnchor.MiddleCenter;
        
        return squaresParent;
    }

    private void CreateChessControls(GameObject parent)
    {
        GameObject controlsPanel = new GameObject("ChessControls");
        controlsPanel.transform.SetParent(parent.transform, false);
        
        RectTransform controlsRect = controlsPanel.AddComponent<RectTransform>();
        controlsRect.anchorMin = new Vector2(0.7f, 0.3f);
        controlsRect.anchorMax = new Vector2(0.95f, 0.8f);
        controlsRect.offsetMin = Vector2.zero;
        controlsRect.offsetMax = Vector2.zero;
        
        // Create vertical layout
        VerticalLayoutGroup layout = controlsPanel.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 20f;
        layout.padding = new RectOffset(20, 20, 20, 20);
        layout.childControlHeight = false;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        
        // Title
        CreateTextElement("ChessPuzzleTitle", "Chess Puzzle", controlsPanel, 24);
        
        // Move counter
        CreateTextElement("MoveCounter", "Moves: 0", controlsPanel, 18);
        
        // Target moves
        CreateTextElement("TargetMoves", "Target: 3 moves", controlsPanel, 16);
        
        // Position display
        CreateTextElement("PositionDisplay", "Position: Play on", controlsPanel, 16);
        
        // Buttons
        CreateButton("ResetButton", "Reset Puzzle", controlsPanel);
        CreateButton("HintButton", "Show Hint", controlsPanel);
        CreateButton("CloseButton", "Close", controlsPanel);
        
        // Instructions
        GameObject instructions = CreateTextElement("Instructions", 
            "Find checkmate in the given number of moves. Click pieces to select, then click destination.", 
            controlsPanel, 14);
        
        TMP_Text instructText = instructions.GetComponent<TMP_Text>();
        if (instructText != null)
        {
            instructText.color = Color.gray;
        }
    }

    private GameObject CreateTextElement(string name, string text, GameObject parent, int fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);
        
        TMP_Text textComponent = textObj.AddComponent<TMP_Text>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = Color.white;
        textComponent.alignment = TextAlignmentOptions.Center;
        
        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, fontSize + 10);
        
        return textObj;
    }

    private GameObject CreateButton(string name, string text, GameObject parent)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent.transform, false);
        
        // Add Image component
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.3f, 0.5f, 1f);
        
        // Add Button component
        Button button = buttonObj.AddComponent<Button>();
        
        // Set button size
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(0, 40);
        
        // Create text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        TMP_Text buttonText = textObj.AddComponent<TMP_Text>();
        buttonText.text = text;
        buttonText.fontSize = 16;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return buttonObj;
    }

    private void AssignChessPuzzleReferences(ChessPuzzle chessPuzzle, GameObject panel, GameObject board)
    {
        // Assign board reference
        chessPuzzle.chessBoardParent = board.transform;
        
        // Find and assign UI elements
        chessPuzzle.moveCountDisplay = FindTextComponent(panel, "MoveCounter");
        chessPuzzle.targetMovesDisplay = FindTextComponent(panel, "TargetMoves");
        chessPuzzle.positionDisplay = FindTextComponent(panel, "PositionDisplay");
        
        chessPuzzle.resetButton = FindButtonComponent(panel, "ResetButton");
        chessPuzzle.hintButton = FindButtonComponent(panel, "HintButton");
        
        // Create square prefab
        CreateSquarePrefab(chessPuzzle);
        
        Debug.Log("✅ Chess puzzle references assigned");
    }

    private TMP_Text FindTextComponent(GameObject parent, string name)
    {
        Transform found = parent.transform.Find(name);
        return found != null ? found.GetComponent<TMP_Text>() : null;
    }

    private Button FindButtonComponent(GameObject parent, string name)
    {
        Transform found = parent.transform.Find(name);
        return found != null ? found.GetComponent<Button>() : null;
    }

    private void CreateSquarePrefab(ChessPuzzle chessPuzzle)
    {
        GameObject squarePrefab = new GameObject("ChessSquarePrefab");
        
        // Add Image for background
        Image bgImage = squarePrefab.AddComponent<Image>();
        bgImage.color = Color.white;
        
        // Add Button for interaction
        Button button = squarePrefab.AddComponent<Button>();
        
        // Add ChessSquare component
        ChessSquare chessSquare = squarePrefab.AddComponent<ChessSquare>();
        chessSquare.backgroundImage = bgImage;
        chessSquare.button = button;
        
        // Create piece image child
        GameObject pieceObj = new GameObject("PieceImage");
        pieceObj.transform.SetParent(squarePrefab.transform, false);
        
        Image pieceImage = pieceObj.AddComponent<Image>();
        RectTransform pieceRect = pieceObj.GetComponent<RectTransform>();
        pieceRect.anchorMin = Vector2.zero;
        pieceRect.anchorMax = Vector2.one;
        pieceRect.offsetMin = Vector2.zero;
        pieceRect.offsetMax = Vector2.zero;
        
        chessSquare.pieceImage = pieceImage;
        pieceObj.SetActive(false);
        
        // Convert to prefab (in runtime, this will just be the template)
        chessPuzzle.squarePrefab = squarePrefab;
        
        // Hide the prefab (it's just a template)
        squarePrefab.SetActive(false);
        
        Debug.Log("✅ Square prefab created");
    }
}