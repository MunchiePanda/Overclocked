using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace Overclocked.Puzzles
{
    public class ChessController : MonoBehaviour
    {
        // UI References (assign in Inspector)
        public TextMeshProUGUI TerminalOutput;
        public TMP_InputField TerminalInput;

        // Game state variables
        private ChessPiece[,] board;
        private PieceColor currentTurn;

        // Enums and Structs (Moved from ChessBoard.cs)
        public enum PieceType
        {
            None,
            Pawn,
            Knight,
            Bishop,
            Rook,
            Queen,
            King
        }

        public enum PieceColor
        {
            None,
            White,
            Black
        }

        public class ChessPiece
        {
            public PieceType Type { get; set; }
            public PieceColor Color { get; set; }

            public ChessPiece(PieceType type, PieceColor color)
            {
                Type = type;
                Color = color;
            }

            public override string ToString()
            {
                if (Type == PieceType.None) return " ";
                string colorChar = (Color == PieceColor.White) ? "W" : "B";
                string typeChar = "";
                switch (Type)
                {
                    case PieceType.Pawn: typeChar = "P"; break;
                    case PieceType.Knight: typeChar = "N"; break;
                    case PieceType.Bishop: typeChar = "B"; break;
                    case PieceType.Rook: typeChar = "R"; break;
                    case PieceType.Queen: typeChar = "Q"; break;
                    case PieceType.King: typeChar = "K"; break;
                }
                return colorChar + typeChar;
            }
        }

        public struct MoveCoordinates
        {
            public int StartRow, StartCol, EndRow, EndCol;
        }

        // --- Public Methods for UI Interaction ---
        void Start()
        {
            InitializeGame();
            if (TerminalInput != null)
            {
                TerminalInput.onEndEdit.AddListener(ProcessPlayerInput);
                TerminalInput.ActivateInputField();
            }
            else
            {
                Debug.LogError("TerminalInput is not assigned in ChessController.");
            }
        }

        public void InitializeGame()
        {
            board = new ChessPiece[8, 8];
            SetupInitialPuzzleBoard();
            currentTurn = PieceColor.White;
            DisplayBoard();
            AppendOutput("\nEnter your move (e.g., e2e4):\n");
        }

        public void ProcessPlayerInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                ClearInputField();
                return;
            }

            AppendOutput("> " + input + "\n");

            if (input.ToLower() == "reset")
            {
                InitializeGame();
                ClearInputField();
                return;
            }

            // if (input.ToLower() == "hint")
            // {
            //     GiveMisleadingHint();
            //     ClearInputField();
            //     return;
            // }

            MoveCoordinates mc = ParseMove(input.ToLower());

            if (mc.StartRow == -1 || mc.EndRow == -1)
            {
                AppendOutput("Invalid move format. Please use algebraic notation (e.g., e2e4).\n");
            }
            else if (IsValidMove(mc, currentTurn))
            {
                ApplyMove(mc);
                DisplayBoard();

                if (IsCheckmate(currentTurn))
                {
                    AppendOutput($"Checkmate! {currentTurn} loses!\n");
                    int randomNumber = Random.Range(1, 10); // Generates a number between 1 (inclusive) and 10 (exclusive)
                    AppendOutput($"Puzzle solved! The number is: {randomNumber}\n");
                    AppendOutput("Type 'reset' to play again.\n");
                }
                else if (IsStalemate(currentTurn))
                {
                    AppendOutput("Stalemate! Game over.\n");
                    AppendOutput("Type 'reset' to play again.\n");
                }
                else if (IsKingInCheck(currentTurn, board))
                {
                    AppendOutput($"{currentTurn} is in check!\n");
                    AppendOutput("Enter your move (e.g., e2e4):\n");
                }
                else
                {
                    AppendOutput("Enter your move (e.g., e2e4):\n");
                }
            }
            else
            {
                AppendOutput("Illegal move. Try again.\n");
                AppendOutput("Enter your move (e.g., e2e4):\n");
            }

            ClearInputField();
        }

        private void ClearInputField()
        {
            if (TerminalInput != null)
            {
                TerminalInput.text = "";
                TerminalInput.ActivateInputField();
            }
        }

        private void AppendOutput(string message)
        {
            if (TerminalOutput != null)
            {
                TerminalOutput.text += message;
                // Optionally scroll to the bottom of the text area
                // terminalOutput.rectTransform.ForceUpdateRectTransforms();
                // terminalOutput.verticalScrollbar.value = 0;
            }
        }

        // Display the board in the terminal output
        private void DisplayBoard()
        {
            if (TerminalOutput == null) return;

            TerminalOutput.text = "";
            AppendOutput("  a b c d e f g h\n");
            AppendOutput(" -----------------\n");
            for (int r = 0; r < 8; r++)
            {
                AppendOutput((8 - r) + "| ");
                for (int c = 0; c < 8; c++)
                {
                    AppendOutput(board[r, c].ToString() + " ");
                }
                AppendOutput("\n"); // Newline for each row
            }
            AppendOutput(" -----------------\n");
            AppendOutput($"{currentTurn} to move.\n");
        }

        private void SetupInitialPuzzleBoard()
        {
            // Set all squares to None initially
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    board[r, c] = new ChessPiece(PieceType.None, PieceColor.None);
                }
            }

            // Set up a simple mate in 1 puzzle for White:
            // White Queen at h7, White King at a1
            // Black King at g8, Black Pawn at h6 (blocking g7 escape)

            board[0, 7] = new ChessPiece(PieceType.Queen, PieceColor.White); // White Queen on h7
            board[7, 0] = new ChessPiece(PieceType.King, PieceColor.White);  // White King on a1 (out of the way)

            board[0, 6] = new ChessPiece(PieceType.King, PieceColor.Black);  // Black King on g8
            board[1, 7] = new ChessPiece(PieceType.Pawn, PieceColor.Black);   // Black Pawn on h6
        }

        // Helper to convert algebraic notation (e.g., "a1") to 0-7 coordinates
        private (int row, int col) ParseCoordinate(string coord)
        {
            if (coord.Length != 2)
            {
                return (-1, -1); // Invalid format
            }
            char fileChar = coord[0];
            char rankChar = coord[1];

            int col = fileChar - 'a'; // 'a' -> 0, 'b' -> 1, etc.
            int row = 8 - (rankChar - '0'); // '1' -> 7, '2' -> 6, etc.

            if (row < 0 || row > 7 || col < 0 || col > 7)
            {
                return (-1, -1); // Out of bounds
            }
            return (row, col);
        }

        // Parses a move string like "e2e4" into coordinates
        public MoveCoordinates ParseMove(string moveString)
        {
            MoveCoordinates mc = new MoveCoordinates { StartRow = -1, StartCol = -1, EndRow = -1, EndCol = -1 };

            if (string.IsNullOrEmpty(moveString) || moveString.Length != 4)
            {
                Debug.LogWarning($"Invalid move string length: {moveString}");
                return mc;
            }

            (int startRow, int startCol) = ParseCoordinate(moveString.Substring(0, 2));
            (int endRow, int endCol) = ParseCoordinate(moveString.Substring(2, 2));

            mc.StartRow = startRow;
            mc.StartCol = startCol;
            mc.EndRow = endRow;
            mc.EndCol = endCol;

            return mc;
        }

        // Basic move validation
        public bool IsValidMove(MoveCoordinates mc, PieceColor playerColor)
        {
            if (mc.StartRow == -1 || mc.EndRow == -1)
            {
                Debug.Log("Invalid move format.");
                return false;
            }

            // Get the piece at the start position
            ChessPiece pieceToMove = board[mc.StartRow, mc.StartCol];

            // Check if there's a piece to move and if it's the current player's turn
            if (pieceToMove.Type == PieceType.None || pieceToMove.Color != playerColor)
            {
                Debug.Log($"No {playerColor} piece at {mc.StartRow},{mc.StartCol} or it's not {playerColor}'s turn.");
                return false;
            }

            // Check if the destination is the same as the start
            if (mc.StartRow == mc.EndRow && mc.StartCol == mc.EndCol)
            {
                Debug.Log("Start and end positions are the same.");
                return false;
            }

            // --- End of common move validations ---

            // Temporarily apply the move to a cloned board to check for self-check
            ChessPiece[,] hypotheticalBoardData = CloneBoardData(this.board);
            
            // Simulate the move on the hypothetical board
            ChessPiece pieceBeingMoved = hypotheticalBoardData[mc.StartRow, mc.StartCol];
            hypotheticalBoardData[mc.EndRow, mc.EndCol] = pieceBeingMoved;
            hypotheticalBoardData[mc.StartRow, mc.StartCol] = new ChessPiece(PieceType.None, PieceColor.None);

            if (IsKingInCheck(playerColor, hypotheticalBoardData))
            {
                Debug.Log($"Move {mc.StartRow}{mc.StartCol}-{mc.EndRow}{mc.EndCol} is illegal: leaves {playerColor} king in check.");
                return false;
            }

            switch (pieceToMove.Type)
            {
                case PieceType.Pawn:
                    return IsValidPawnMove(mc, pieceToMove.Color);
                case PieceType.Knight:
                    return IsValidKnightMove(mc, pieceToMove.Color);
                case PieceType.Bishop:
                    return IsValidBishopMove(mc, pieceToMove.Color);
                case PieceType.Rook:
                    return IsValidRookMove(mc, pieceToMove.Color);
                case PieceType.Queen:
                    return IsValidQueenMove(mc, pieceToMove.Color);
                case PieceType.King:
                    return IsValidKingMove(mc, pieceToMove.Color);
                default:
                    Debug.LogWarning($"Move validation for {pieceToMove.Type} not yet implemented.");
                    return false;
            }
        }

        private bool IsValidPawnMove(MoveCoordinates mc, PieceColor pieceColor)
        {
            int startRow = mc.StartRow;
            int startCol = mc.StartCol;
            int endRow = mc.EndRow;
            int endCol = mc.EndCol;

            int forwardDirection = (pieceColor == PieceColor.White) ? -1 : 1; // White moves up (row decreases), Black moves down (row increases)
            int startPawnRow = (pieceColor == PieceColor.White) ? 6 : 1; // White pawns start at row 6, Black at row 1

            // Single step forward
            if (startCol == endCol && endRow == startRow + forwardDirection)
            {
                // Check if the destination square is empty
                if (board[endRow, endCol].Type == PieceType.None)
                {
                    return true;
                }
            }

            // Double step forward (only from starting position)
            if (startCol == endCol && startRow == startPawnRow && endRow == startRow + (2 * forwardDirection))
            {
                // Check if both squares in front are empty
                if (board[startRow + forwardDirection, startCol].Type == PieceType.None && board[endRow, endCol].Type == PieceType.None)
                {
                    return true;
                }
            }

            // Diagonal capture
            if (Mathf.Abs(startCol - endCol) == 1 && endRow == startRow + forwardDirection)
            {
                // Check if there's an opponent's piece at the destination
                if (board[endRow, endCol].Type != PieceType.None && board[endRow, endCol].Color != pieceColor)
                {
                    return true;
                }
            }

            // TODO: Implement En Passant later if necessary

            return false;
        }

        private bool IsValidKnightMove(MoveCoordinates mc, PieceColor pieceColor)
        {
            int startRow = mc.StartRow;
            int startCol = mc.StartCol;
            int endRow = mc.EndRow;
            int endCol = mc.EndCol;

            // Knight's L-shaped moves: 2 squares in one direction (horz/vert) and 1 square perpendicular
            int rowDiff = Mathf.Abs(startRow - endRow);
            int colDiff = Mathf.Abs(startCol - endCol);

            // Check for L-shape movement (2,1 or 1,2)
            if (!((rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2)))
            {
                return false;
            }

            // Check if destination square contains a friendly piece
            ChessPiece destinationPiece = board[endRow, endCol];
            if (destinationPiece.Type != PieceType.None && destinationPiece.Color == pieceColor)
            {
                return false;
            }

            return true;
        }

        private bool IsPathClearDiagonal(int startRow, int startCol, int endRow, int endCol, ChessPiece[,] targetBoard)
        {
            int rowStep = (endRow > startRow) ? 1 : -1;
            int colStep = (endCol > startCol) ? 1 : -1;

            int currentRow = startRow + rowStep;
            int currentCol = startCol + colStep;

            while (currentRow != endRow || currentCol != endCol)
            {
                if (targetBoard[currentRow, currentCol].Type != PieceType.None)
                {
                    return false; // Obstruction found
                }
                currentRow += rowStep;
                currentCol += colStep;
            }
            return true; // Path is clear
        }

        private bool IsValidBishopMove(MoveCoordinates mc, PieceColor pieceColor)
        {
            int startRow = mc.StartRow;
            int startCol = mc.StartCol;
            int endRow = mc.EndRow;
            int endCol = mc.EndCol;

            int rowDiff = Mathf.Abs(startRow - endRow);
            int colDiff = Mathf.Abs(startCol - endCol);

            // Bishops move diagonally (rowDiff == colDiff)
            if (rowDiff != colDiff || rowDiff == 0)
            {
                return false; // Not a diagonal move or no move
            }

            // Check for obstructions along the diagonal path
            if (!IsPathClearDiagonal(startRow, startCol, endRow, endCol, board))
            {
                return false;
            }

            // Check if destination square contains a friendly piece
            ChessPiece destinationPiece = board[endRow, endCol];
            if (destinationPiece.Type != PieceType.None && destinationPiece.Color == pieceColor)
            {
                return false;
            }

            return true;
        }

        private bool IsPathClearStraight(int startRow, int startCol, int endRow, int endCol, ChessPiece[,] targetBoard)
        {
            // Horizontal move
            if (startRow == endRow)
            {
                int colStep = (endCol > startCol) ? 1 : -1;
                for (int col = startCol + colStep; col != endCol; col += colStep)
                {
                    if (targetBoard[startRow, col].Type != PieceType.None)
                    {
                        return false; // Obstruction found
                    }
                }
            }
            // Vertical move
            else if (startCol == endCol)
            {
                int rowStep = (endRow > startRow) ? 1 : -1;
                for (int row = startRow + rowStep; row != endRow; row += rowStep)
                {
                    if (targetBoard[row, startCol].Type != PieceType.None)
                    {
                        return false; // Obstruction found
                    }
                }
            }
            else
            {
                return false; // Not a straight path
            }
            return true; // Path is clear
        }

        private bool IsValidRookMove(MoveCoordinates mc, PieceColor pieceColor)
        {
            int startRow = mc.StartRow;
            int startCol = mc.StartCol;
            int endRow = mc.EndRow;
            int endCol = mc.EndCol;

            // Rooks move horizontally or vertically
            if (!((startRow == endRow && startCol != endCol) || (startCol == endCol && startRow != endRow)))
            {
                return false; // Not a straight horizontal or vertical move
            }

            // Check for obstructions along the straight path
            if (!IsPathClearStraight(startRow, startCol, endRow, endCol, board))
            {
                return false;
            }

            // Check if destination square contains a friendly piece
            ChessPiece destinationPiece = board[endRow, endCol];
            if (destinationPiece.Type != PieceType.None && destinationPiece.Color == pieceColor)
            {
                return false;
            }

            return true;
        }

        private bool IsValidQueenMove(MoveCoordinates mc, PieceColor pieceColor)
        {
            int startRow = mc.StartRow;
            int startCol = mc.StartCol;
            int endRow = mc.EndRow;
            int endCol = mc.EndCol;

            int rowDiff = Mathf.Abs(startRow - endRow);
            int colDiff = Mathf.Abs(startCol - endCol);

            // Queen moves horizontally, vertically, or diagonally
            bool isStraight = (startRow == endRow && startCol != endCol) || (startCol == endCol && startRow != endRow);
            bool isDiagonal = (rowDiff == colDiff && rowDiff != 0);

            if (!isStraight && !isDiagonal)
            {
                return false; // Not a valid queen move
            }

            // Check for obstructions
            if (isStraight)
            {
                if (!IsPathClearStraight(startRow, startCol, endRow, endCol, board))
                {
                    return false;
                }
            }
            else if (isDiagonal)
            {
                if (!IsPathClearDiagonal(startRow, startCol, endRow, endCol, board))
                {
                    return false;
                }
            }

            // Check if destination square contains a friendly piece
            ChessPiece destinationPiece = board[endRow, endCol];
            if (destinationPiece.Type != PieceType.None && destinationPiece.Color == pieceColor)
            {
                return false;
            }

            return true;
        }

        private bool IsValidKingMove(MoveCoordinates mc, PieceColor pieceColor)
        {
            int startRow = mc.StartRow;
            int startCol = mc.StartCol;
            int endRow = mc.EndRow;
            int endCol = mc.EndCol;

            int rowDiff = Mathf.Abs(startRow - endRow);
            int colDiff = Mathf.Abs(startCol - endCol);

            // King moves one square in any direction
            if (rowDiff > 1 || colDiff > 1 || (rowDiff == 0 && colDiff == 0))
            {
                return false;
            }

            // Check if destination square contains a friendly piece
            ChessPiece destinationPiece = board[endRow, endCol];
            if (destinationPiece.Type != PieceType.None && destinationPiece.Color == pieceColor)
            {
                return false;
            }

            // Ensure king does not move into check
            if (IsSquareAttacked(endRow, endCol, (pieceColor == PieceColor.White) ? PieceColor.Black : PieceColor.White, board))
            {
                Debug.Log($"King cannot move into check at {endRow},{endCol}");
                return false;
            }

            return true;
        }

        // Finds the row and column of the king of the specified color on a given board
        public (int row, int col) FindKingPosition(PieceColor kingColor, ChessPiece[,] targetBoard)
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    ChessPiece piece = targetBoard[r, c];
                    if (piece.Type == PieceType.King && piece.Color == kingColor)
                    {
                        return (r, c);
                    }
                }
            }
            return (-1, -1); // King not found (shouldn't happen in a valid game)
        }

        // Gets all legal moves for a given player's color
        public List<MoveCoordinates> GetAllLegalMoves(PieceColor playerColor)
        {
            List<MoveCoordinates> legalMoves = new List<MoveCoordinates>();

            for (int startRow = 0; startRow < 8; startRow++)
            {
                for (int startCol = 0; startCol < 8; startCol++)
                {
                    ChessPiece piece = board[startRow, startCol];

                    if (piece.Type != PieceType.None && piece.Color == playerColor)
                    {
                        for (int endRow = 0; endRow < 8; endRow++)
                        {
                            for (int endCol = 0; endCol < 8; endCol++)
                            {
                                MoveCoordinates potentialMove = new MoveCoordinates
                                {
                                    StartRow = startRow,
                                    StartCol = startCol,
                                    EndRow = endRow,
                                    EndCol = endCol
                                };

                                // Check if the potential move is valid on the current board (without affecting it)
                                // IsValidMove now takes the playerColor directly.
                                if (IsValidMove(potentialMove, playerColor))
                                {
                                    legalMoves.Add(potentialMove);
                                }
                            }
                        }
                    }
                }
            }
            return legalMoves;
        }

        public bool IsCheckmate(PieceColor playerColor)
        {
            if (!IsKingInCheck(playerColor, board))
            {
                return false; // Not in check, so can't be checkmate
            }

            // If in check, check if there are any legal moves to get out of check
            return GetAllLegalMoves(playerColor).Count == 0;
        }

        public bool IsStalemate(PieceColor playerColor)
        {
            if (IsKingInCheck(playerColor, board))
            {
                return false; // In check, so can't be stalemate
            }

            // If not in check, check if there are any legal moves
            return GetAllLegalMoves(playerColor).Count == 0;
        }

        // Creates a deep copy of the current ChessBoard state for checking hypothetical moves
        private ChessPiece[,] CloneBoardData(ChessPiece[,] originalBoard)
        {
            ChessPiece[,] clonedBoardData = new ChessPiece[8, 8];
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    ChessPiece originalPiece = originalBoard[r, c];
                    clonedBoardData[r, c] = new ChessPiece(originalPiece.Type, originalPiece.Color);
                }
            }
            return clonedBoardData;
        }

        // Checks if the king of the specified color is currently in check on a given board
        public bool IsKingInCheck(PieceColor kingColor, ChessPiece[,] targetBoard)
        {
            (int kingRow, int kingCol) = FindKingPosition(kingColor, targetBoard);
            if (kingRow == -1) return false; // Should not happen

            PieceColor attackingColor = (kingColor == PieceColor.White) ? PieceColor.Black : PieceColor.White;
            return IsSquareAttacked(kingRow, kingCol, attackingColor, targetBoard);
        }

        // Checks if a square is attacked by any piece of the specified color on a given board
        private bool IsSquareAttacked(int row, int col, PieceColor attackingColor, ChessPiece[,] targetBoard)
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    ChessPiece piece = targetBoard[r, c];
                    if (piece.Type != PieceType.None && piece.Color == attackingColor)
                    {
                        if (CanPieceAttackSquare(r, c, row, col, targetBoard))
                        {
                            return true; // Attacked!
                        }
                    }
                }
            }
            return false; // Not attacked
        }

        // Can a piece (at start) attack a target square (end), ignoring current turn and if destination is friendly.
        // Used for check detection. Operates on a given board.
        private bool CanPieceAttackSquare(int startRow, int startCol, int endRow, int endCol, ChessPiece[,] targetBoard)
        {
            ChessPiece piece = targetBoard[startRow, startCol];

            // No piece or attacking an empty square (unless pawn capture)
            if (piece.Type == PieceType.None) return false;

            int rowDiff = endRow - startRow;
            int colDiff = endCol - startCol;

            switch (piece.Type)
            {
                case PieceType.Pawn:
                    // Pawns attack diagonally one square forward
                    int forwardDirection = (piece.Color == PieceColor.White) ? -1 : 1;
                    if (rowDiff == forwardDirection && Mathf.Abs(colDiff) == 1)
                    {
                        return true;
                    }
                    return false;

                case PieceType.Knight:
                    // Knight's L-shaped moves
                    int absRowDiff = Mathf.Abs(rowDiff);
                    int absColDiff = Mathf.Abs(colDiff);
                    return (absRowDiff == 2 && absColDiff == 1) || (absRowDiff == 1 && absColDiff == 2);

                case PieceType.Bishop:
                    // Bishops move diagonally
                    if (Mathf.Abs(rowDiff) == Mathf.Abs(colDiff) && rowDiff != 0)
                    {
                        return IsPathClearDiagonal(startRow, startCol, endRow, endCol, targetBoard); // Check obstructions
                    }
                    return false;

                case PieceType.Rook:
                    // Rooks move horizontally or vertically
                    if ((rowDiff == 0 && colDiff != 0) || (colDiff == 0 && rowDiff != 0))
                    {
                        return IsPathClearStraight(startRow, startCol, endRow, endCol, targetBoard); // Check obstructions
                    }
                    return false;

                case PieceType.Queen:
                    // Queen moves horizontally, vertically, or diagonally
                    bool isStraight = (rowDiff == 0 && colDiff != 0) || (colDiff == 0 && rowDiff != 0);
                    bool isDiagonal = (Mathf.Abs(rowDiff) == Mathf.Abs(colDiff) && rowDiff != 0);
                    if (isStraight && IsPathClearStraight(startRow, startCol, endRow, endCol, targetBoard))
                    {
                        return true;
                    }
                    if (isDiagonal && IsPathClearDiagonal(startRow, startCol, endRow, endCol, targetBoard))
                    {
                        return true;
                    }
                    return false;

                case PieceType.King:
                    // King moves one square in any direction
                    return Mathf.Abs(rowDiff) <= 1 && Mathf.Abs(colDiff) <= 1 && (rowDiff != 0 || colDiff != 0);

                default:
                    return false;
            }
        }

        // Apply a move to the board
        public void ApplyMove(MoveCoordinates mc)
        {
            if (mc.StartRow == -1 || mc.EndRow == -1)
            {
                Debug.LogError($"Cannot apply invalid move: {mc.StartRow}{mc.StartCol}-{mc.EndRow}{mc.EndCol}");
                return;
            }

            // Move the piece
            board[mc.EndRow, mc.EndCol] = board[mc.StartRow, mc.StartCol];
            board[mc.StartRow, mc.StartCol] = new ChessPiece(PieceType.None, PieceColor.None);

            // Toggle turn
            currentTurn = (currentTurn == PieceColor.White) ? PieceColor.Black : PieceColor.White;

            Debug.Log($"Applied move: {mc.StartRow}{mc.StartCol}-{mc.EndRow}{mc.EndCol}. Current turn: {currentTurn}");
        }

        // void GiveMisleadingHint()
        // {
        //     List<MoveCoordinates> legalMoves = GetAllLegalMoves(currentTurn);

        //     if (legalMoves.Count > 0)
        //     {
        //         MoveCoordinates hintMove = legalMoves[Random.Range(0, legalMoves.Count)];

        //         string startFile = ((char)('a' + hintMove.StartCol)).ToString();
        //         string startRank = (8 - hintMove.StartRow).ToString();
        //         string endFile = ((char)('a' + hintMove.EndCol)).ToString();
        //         string endRank = (8 - hintMove.EndRow).ToString();

        //         AppendOutput($"AI Hint: Consider moving {startFile}{startRank} to {endFile}{endRank}. (It might open up new possibilities!)\n");
        //     }
        //     else
        //     {
        //         AppendOutput("AI Hint: I'm calculating... this position is complex. Keep trying!\n");
        //     }
        // }
    }
}
