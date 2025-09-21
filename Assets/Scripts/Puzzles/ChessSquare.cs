using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a single square on the chess board
/// </summary>
public class ChessSquare : MonoBehaviour
{
    [Header("UI Components")]
    public Image backgroundImage;
    public Image pieceImage;
    public Button button;
    
    [Header("Visual States")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color validMoveColor = Color.green;
    
    private int file; // 0-7 (a-h)
    private int rank; // 0-7 (1-8)
    private ChessPieceType currentPiece = ChessPieceType.None;
    private bool isWhitePiece = false;
    private ChessPuzzle chessPuzzle;
    private Color originalColor;
    
    public int File => file;
    public int Rank => rank;

    public void Initialize(int file, int rank, ChessPuzzle puzzle)
    {
        this.file = file;
        this.rank = rank;
        this.chessPuzzle = puzzle;
        
        SetupComponents();
        SetupButton();
    }

    private void SetupComponents()
    {
        // Auto-find components if not assigned
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
            
        if (pieceImage == null)
        {
            // Look for child with piece image
            foreach (Transform child in transform)
            {
                Image childImage = child.GetComponent<Image>();
                if (childImage != null)
                {
                    pieceImage = childImage;
                    break;
                }
            }
            
            // Create piece image if none found
            if (pieceImage == null)
            {
                GameObject pieceObj = new GameObject("PieceImage");
                pieceObj.transform.SetParent(transform);
                pieceImage = pieceObj.AddComponent<Image>();
                
                RectTransform pieceRect = pieceImage.GetComponent<RectTransform>();
                pieceRect.anchorMin = Vector2.zero;
                pieceRect.anchorMax = Vector2.one;
                pieceRect.offsetMin = Vector2.zero;
                pieceRect.offsetMax = Vector2.zero;
                pieceRect.localScale = Vector3.one;
                
                pieceImage.color = Color.white;
            }
        }
        
        if (button == null)
            button = GetComponent<Button>();
            
        if (button == null)
            button = gameObject.AddComponent<Button>();
    }

    private void SetupButton()
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        if (chessPuzzle != null)
        {
            chessPuzzle.OnSquareClicked(this);
        }
    }

    public void SetSquareColor(Color color)
    {
        originalColor = color;
        normalColor = color;
        
        if (backgroundImage != null)
            backgroundImage.color = color;
    }

    public void SetPiece(ChessPieceType piece, bool isWhite)
    {
        currentPiece = piece;
        isWhitePiece = isWhite;
        
        if (pieceImage != null)
        {
            pieceImage.gameObject.SetActive(piece != ChessPieceType.None);
        }
    }

    public void SetPieceSprite(Sprite sprite)
    {
        if (pieceImage != null)
        {
            pieceImage.sprite = sprite;
            pieceImage.gameObject.SetActive(sprite != null);
        }
    }

    public void SetSelected(bool selected)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = selected ? selectedColor : originalColor;
        }
    }

    public void SetValidMove(bool isValidMove)
    {
        if (backgroundImage != null && isValidMove)
        {
            backgroundImage.color = validMoveColor;
        }
        else if (backgroundImage != null && !isValidMove)
        {
            backgroundImage.color = originalColor;
        }
    }

    public bool HasPiece()
    {
        return currentPiece != ChessPieceType.None;
    }

    public ChessPieceType GetPieceType()
    {
        return currentPiece;
    }

    public bool IsWhitePiece()
    {
        return isWhitePiece;
    }

    public Sprite GetPieceSprite()
    {
        return pieceImage != null ? pieceImage.sprite : null;
    }

    public string GetSquareName()
    {
        char fileChar = (char)('a' + file);
        int rankNumber = rank + 1;
        return $"{fileChar}{rankNumber}";
    }

    // Visual feedback methods
    public void HighlightAttack()
    {
        if (backgroundImage != null)
            backgroundImage.color = Color.red;
    }

    public void HighlightDefend()
    {
        if (backgroundImage != null)
            backgroundImage.color = Color.blue;
    }

    public void ClearHighlight()
    {
        if (backgroundImage != null)
            backgroundImage.color = originalColor;
    }
}