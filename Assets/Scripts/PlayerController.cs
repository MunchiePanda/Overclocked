using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Base setup")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    [Header("Interaction setup")]
    public float maxInteractionDistance = 3f;
    public LayerMask interactableLayer;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    [SerializeField]
    private float cameraYOffset = 0.4f;
    private Camera playerCamera;
    private Alteruna.Avatar _avatar;
    private IInteractable currentInteractable;
    private IInteractable lastInteractable;



    void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();

        if (!_avatar.IsMe)
            return;

        characterController = GetComponent<CharacterController>();
        playerCamera = Camera.main;
        playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
        playerCamera.transform.SetParent(transform);
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!_avatar.IsMe)
            return;

        bool isRunning = false;

        // Press Left Shift to run
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // We are grounded, so recalculate move direction based on axis
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove && playerCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        // Interaction logic
        HandleInteraction();
    }

    private void HandleInteraction()
    {
        // Reset current interactable
        currentInteractable = null;

        // Raycast to detect interactable objects
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(ray.origin, ray.direction * maxInteractionDistance, Color.red);
        Debug.Log($"Raycast origin: {ray.origin}, direction: {ray.direction}");

        if (Physics.Raycast(ray, out RaycastHit hit, maxInteractionDistance, interactableLayer))
        {
            Debug.Log($"Raycast hit: {hit.collider.name} at distance {hit.distance}");
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                Debug.Log($"Found interactable: {hit.collider.name}");
                currentInteractable = interactable;
                currentInteractable.OnHover();
            }
            else
            {
                Debug.Log($"Hit {hit.collider.name}, but it is not interactable.");
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
        }

        // If no interactable is detected, notify the last one to exit hover state
        if (currentInteractable == null && lastInteractable != null)
        {
            Debug.Log($"No interactable detected. Exiting hover state for {lastInteractable}");
            lastInteractable.OnHoverExit();
            lastInteractable = null;
        }

        // If a new interactable is detected, update lastInteractable
        if (currentInteractable != null && lastInteractable != currentInteractable)
        {
            if (lastInteractable != null)
            {
                Debug.Log($"New interactable detected. Exiting hover state for {lastInteractable}");
                lastInteractable.OnHoverExit();
            }
            lastInteractable = currentInteractable;
        }

        // Handle interaction input
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            Debug.Log($"Interacting with {currentInteractable}");
            currentInteractable.OnInteract();
        }
    }
}
