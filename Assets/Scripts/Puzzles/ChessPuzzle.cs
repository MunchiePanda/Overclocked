using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Chess puzzle where player must win in a specific number of moves
/// </summary>
public class ChessPuzzle : BasePuzzle
{
    [Header("Chess Board UI")]
    [Tooltip("Parent object for the chess board grid")]
    public Transform chessBoardParent;
    
    [Tooltip("Prefab for chess board squares")]
    public GameObject squarePrefab;
    
    [Tooltip("Chess piece sprites")]
    public ChessPieceSprites pieceSprites;
    
    [Tooltip("Display showing current move count")]
    public TMP_Text moveCountDisplay;
    
    [Tooltip("Display showing target moves")]
    public TMP_Text targetMovesDisplay;
    
    [Tooltip("Display showing the current position evaluation")]
    public TMP_Text positionDisplay;
    
    [Tooltip("Button to reset the puzzle")]
    public Button resetButton;
    
    [Tooltip("Button to show hint")]
    public Button hintButton;

    [Header("Puzzle Configuration")]
    [Tooltip("Maximum moves allowed to solve")]
    public int maxMoves = 3;
    
    [Tooltip("Predefined chess position (FEN notation)")]
    public string startingPosition = "r1bqk2r/pppp1ppp/2n2n2/2b1p3/2B1P3/3P1N2/PPP2PPP/RNBQK2R w KQkq - 0 4";
    
    [Tooltip("Solution moves in algebraic notation")]
    public string[] solutionMoves = {"Qd5", "Nf6+", "Qxf7#"};

    private ChessPosition currentPosition;
    private List<ChessMove> playerMoves = new List<ChessMove>();
    private ChessSquare[,] chessBoard = new ChessSquare[8, 8];
    private ChessSquare selectedSquare;
    private List<ChessSquare> validMoveSquares = new List<ChessSquare>();
    private int randomNumber;

    [System.Serializable]
    public class ChessPieceSprites
    {
        [Header("White Pieces")]
        public Sprite whiteKing;
        public Sprite whiteQueen;
        public Sprite whiteRook;
        public Sprite whiteBishop;
        public Sprite whiteKnight;
        public Sprite whitePawn;
        
        [Header("Black Pieces")]
        public Sprite blackKing;
        public Sprite blackQueen;
        public Sprite blackRook;
        public Sprite blackBishop;
        public Sprite blackKnight;
        public Sprite blackPawn;
    }

    public override void Initialize()
    {
        base.Initialize();
        randomNumber = Random.Range(1000, 9999);
        
        SetupChessBoard();
        LoadPosition(startingPosition);
        SetupButtons();
        UpdateDisplay();
    }

    private void SetupChessBoard()
    {
        if (chessBoardParent == null || squarePrefab == null)
        {
            Debug.LogError("Chess board setup missing components!");
            return;
        }

        // Clear existing board
        foreach (Transform child in chessBoardParent)
        {
            DestroyImmediate(child.gameObject);
        }

        // Create 8x8 grid
        for (int rank = 0; rank < 8; rank++)
        {
            for (int file = 0; file < 8; file++)
            {
                GameObject square = Instantiate(squarePrefab, chessBoardParent);
                ChessSquare chessSquare = square.GetComponent<ChessSquare>();
                
                if (chessSquare == null)
                    chessSquare = square.AddComponent<ChessSquare>();

                chessSquare.Initialize(file, rank, this);
                chessBoard[file, rank] = chessSquare;

                // Set square colors
                bool isLight = (file + rank) % 2 == 0;
                chessSquare.SetSquareColor(isLight ? Color.white : new Color(0.4f, 0.4f, 0.4f));
            }
        }
    }

    private void SetupButtons()
    {
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetPuzzle);
            
        if (hintButton != null)
            hintButton.onClick.AddListener(ShowHint);
    }

    private void LoadPosition(string fen)
    {
        currentPosition = new ChessPosition(fen);
        
        // Clear all pieces
        for (int file = 0; file < 8; file++)
        {
            for (int rank = 0; rank < 8; rank++)
            {
                chessBoard[file, rank].SetPiece(ChessPieceType.None, false);
            }
        }

        // Place pieces according to FEN
        PlacePiecesFromPosition();
    }

    private void PlacePiecesFromPosition()
    {
        if (currentPosition == null) return;

        for (int file = 0; file < 8; file++)
        {
            for (int rank = 0; rank < 8; rank++)
            {
                ChessPieceType piece = currentPosition.GetPieceAt(file, rank);
                bool isWhite = currentPosition.IsPieceWhite(file, rank);
                
                if (piece != ChessPieceType.None)
                {
                    chessBoard[file, rank].SetPiece(piece, isWhite);
                    chessBoard[file, rank].SetPieceSprite(GetSpriteForPiece(piece, isWhite));
                }
            }
        }
    }

    private Sprite GetSpriteForPiece(ChessPieceType piece, bool isWhite)
    {
        if (pieceSprites == null) return null;

        return piece switch
        {
            ChessPieceType.King => isWhite ? pieceSprites.whiteKing : pieceSprites.blackKing,
            ChessPieceType.Queen => isWhite ? pieceSprites.whiteQueen : pieceSprites.blackQueen,
            ChessPieceType.Rook => isWhite ? pieceSprites.whiteRook : pieceSprites.blackRook,
            ChessPieceType.Bishop => isWhite ? pieceSprites.whiteBishop : pieceSprites.blackBishop,
            ChessPieceType.Knight => isWhite ? pieceSprites.whiteKnight : pieceSprites.blackKnight,
            ChessPieceType.Pawn => isWhite ? pieceSprites.whitePawn : pieceSprites.blackPawn,
            _ => null
        };
    }

    public void OnSquareClicked(ChessSquare square)
    {
        if (selectedSquare == null)
        {
            // Select piece if it belongs to the current player
            if (square.HasPiece() && square.IsWhitePiece() == currentPosition.IsWhiteToMove())
            {
                SelectSquare(square);
            }
        }
        else
        {
            // Try to make a move
            if (validMoveSquares.Contains(square))
            {
                MakeMove(selectedSquare, square);
            }
            
            DeselectSquare();
        }
    }

    private void SelectSquare(ChessSquare square)
    {
        selectedSquare = square;
        square.SetSelected(true);
        
        // Highlight valid moves
        validMoveSquares = GetValidMoves(square);
        foreach (ChessSquare moveSquare in validMoveSquares)
        {
            moveSquare.SetValidMove(true);
        }
    }

    private void DeselectSquare()
    {
        if (selectedSquare != null)
        {
            selectedSquare.SetSelected(false);
            selectedSquare = null;
        }
        
        // Clear move highlights
        foreach (ChessSquare square in validMoveSquares)
        {
            square.SetValidMove(false);
        }
        validMoveSquares.Clear();
    }

    private List<ChessSquare> GetValidMoves(ChessSquare fromSquare)
    {
        List<ChessSquare> moves = new List<ChessSquare>();
        
        // Simplified move generation - implement based on piece type
        ChessPieceType piece = fromSquare.GetPieceType();
        bool isWhite = fromSquare.IsWhitePiece();
        
        // Add basic move validation here
        // For now, return adjacent squares as example
        int file = fromSquare.File;
        int rank = fromSquare.Rank;
        
        for (int df = -1; df <= 1; df++)
        {
            for (int dr = -1; dr <= 1; dr++)
            {
                if (df == 0 && dr == 0) continue;
                
                int newFile = file + df;
                int newRank = rank + dr;
                
                if (newFile >= 0 && newFile < 8 && newRank >= 0 && newRank < 8)
                {
                    ChessSquare targetSquare = chessBoard[newFile, newRank];
                    if (!targetSquare.HasPiece() || targetSquare.IsWhitePiece() != isWhite)
                    {
                        moves.Add(targetSquare);
                    }
                }
            }
        }
        
        return moves;
    }

    private void MakeMove(ChessSquare from, ChessSquare to)
    {
        // Create move object
        ChessMove move = new ChessMove(from.File, from.Rank, to.File, to.Rank);
        playerMoves.Add(move);
        
        // Move the piece
        ChessPieceType piece = from.GetPieceType();
        bool isWhite = from.IsWhitePiece();
        Sprite pieceSprite = from.GetPieceSprite();
        
        from.SetPiece(ChessPieceType.None, false);
        to.SetPiece(piece, isWhite);
        to.SetPieceSprite(pieceSprite);
        
        // Update position
        currentPosition.MakeMove(move);
        
        // Check for puzzle completion
        CheckPuzzleCompletion();
        
        UpdateDisplay();
    }

    private void CheckPuzzleCompletion()
    {
        if (playerMoves.Count > maxMoves)
        {
            GiveFeedback($"Too many moves! Maximum allowed: {maxMoves}. Resetting puzzle.");
            ResetPuzzle();
            return;
        }

        // Check if player found the solution
        if (IsCheckmate() && playerMoves.Count <= maxMoves)
        {
            GiveFeedback($"Brilliant! Checkmate in {playerMoves.Count} moves! Your escape code is: {randomNumber}");
            CompletePuzzle();
            
            // Notify escape code manager
            EscapeCodeManager escapeManager = FindObjectOfType<EscapeCodeManager>();
            if (escapeManager != null)
                escapeManager.OnPuzzleCompleted(randomNumber);
        }
        else if (playerMoves.Count == maxMoves)
        {
            if (!IsCheckmate())
            {
                GiveFeedback("No checkmate achieved. Try a different approach!");
                ResetPuzzle();
            }
        }
    }

    private bool IsCheckmate()
    {
        // Simplified checkmate detection
        // In a real implementation, you'd check if the king is in check 
        // and has no legal moves
        return currentPosition.IsCheckmate();
    }

    private void ShowHint()
    {
        if (playerMoves.Count < solutionMoves.Length)
        {
            string hint = solutionMoves[playerMoves.Count];
            GiveFeedback($"Hint: Consider the move {hint}");
        }
        else
        {
            GiveFeedback("No more hints available!");
        }
    }

    public override void ResetPuzzle()
    {
        base.ResetPuzzle();
        
        playerMoves.Clear();
        DeselectSquare();
        LoadPosition(startingPosition);
        UpdateDisplay();
        
        GiveFeedback($"Find checkmate in {maxMoves} moves or fewer!");
    }

    private void UpdateDisplay()
    {
        if (moveCountDisplay != null)
            moveCountDisplay.text = $"Moves: {playerMoves.Count}";
            
        if (targetMovesDisplay != null)
            targetMovesDisplay.text = $"Target: {maxMoves} moves";
            
        if (positionDisplay != null)
        {
            string evaluation = currentPosition.IsInCheck() ? "Check!" : "Play on";
            positionDisplay.text = $"Position: {evaluation}";
        }
    }

    public override void ShowPuzzle()
    {
        base.ShowPuzzle();
        GiveFeedback($"Solve this chess position with checkmate in {maxMoves} moves or fewer!");
    }
}

// Supporting classes for chess logic
[System.Serializable]
public class ChessMove
{
    public int fromFile, fromRank, toFile, toRank;
    
    public ChessMove(int fromFile, int fromRank, int toFile, int toRank)
    {
        this.fromFile = fromFile;
        this.fromRank = fromRank;
        this.toFile = toFile;
        this.toRank = toRank;
    }
}

public enum ChessPieceType
{
    None, King, Queen, Rook, Bishop, Knight, Pawn
}

public class ChessPosition
{
    private ChessPieceType[,] board = new ChessPieceType[8, 8];
    private bool[,] isWhite = new bool[8, 8];
    private bool whiteToMove = true;
    
    public ChessPosition(string fen)
    {
        LoadFromFEN(fen);
    }
    
    private void LoadFromFEN(string fen)
    {
        // Simplified FEN loading - implement full FEN parsing as needed
        string[] parts = fen.Split(' ');
        string position = parts[0];
        whiteToMove = parts[1] == "w";
        
        // Parse position (basic implementation)
        // For demo purposes, start with empty board
        ClearBoard();
    }
    
    private void ClearBoard()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                board[i, j] = ChessPieceType.None;
                isWhite[i, j] = false;
            }
        }
    }
    
    public ChessPieceType GetPieceAt(int file, int rank)
    {
        return board[file, rank];
    }
    
    public bool IsPieceWhite(int file, int rank)
    {
        return isWhite[file, rank];
    }
    
    public bool IsWhiteToMove()
    {
        return whiteToMove;
    }
    
    public void MakeMove(ChessMove move)
    {
        board[move.toFile, move.toRank] = board[move.fromFile, move.fromRank];
        isWhite[move.toFile, move.toRank] = isWhite[move.fromFile, move.fromRank];
        
        board[move.fromFile, move.fromRank] = ChessPieceType.None;
        isWhite[move.fromFile, move.fromRank] = false;
        
        whiteToMove = !whiteToMove;
    }
    
    public bool IsInCheck()
    {
        // Simplified check detection
        return false;
    }
    
    public bool IsCheckmate()
    {
        // Simplified checkmate detection
        return false;
    }
}