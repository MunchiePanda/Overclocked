using UnityEngine;

public interface IInteractable
{
    // Called when the player looks at the object
    void OnHover();

    // Called when the player stops looking at the object
    void OnHoverExit();

    // Called when the player interacts with the object (e.g., presses E)
    void OnInteract();
}
