using UnityEngine;

public class PuzzleButton : MonoBehaviour, IInteractable
{
    [Header("Button Settings")]
    public int buttonId; // Unique identifier for the button in the sequence
    public Material defaultMaterial;
    public Material hoverMaterial;
    public Material pressedMaterial;

    private Renderer buttonRenderer;
    private PowerGridPuzzle puzzleManager;
    private bool isPressed = false;

    void Start()
    {
        buttonRenderer = GetComponent<Renderer>();
        puzzleManager = FindFirstObjectByType<PowerGridPuzzle>();
        SetMaterial(defaultMaterial);
    }

    public void OnHover()
    {
        if (!isPressed)
            SetMaterial(hoverMaterial);
    }

    public void OnHoverExit()
    {
        if (!isPressed)
            SetMaterial(defaultMaterial);
    }

    public void OnInteract()
    {
        if (isPressed || puzzleManager == null)
            return;

        isPressed = true;
        SetMaterial(pressedMaterial);
        Debug.Log($"Button {buttonId} pressed!");
        puzzleManager.OnButtonPressed(buttonId);
    }

    public void ResetButton()
    {
        isPressed = false;
        SetMaterial(defaultMaterial);
    }

    private void SetMaterial(Material material)
    {
        if (buttonRenderer != null && material != null)
            buttonRenderer.material = material;
    }
}
