using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Physical stacking puzzle where the player needs to stack objects in a specific order
/// </summary>
public class StackingPuzzle : BasePuzzle
{
    [Header("Stacking Puzzle Settings")]
    [Tooltip("Prefabs for the stackable objects")]
    public List<GameObject> stackableObjectPrefabs;

    [Tooltip("Transform where objects should be stacked")]
    public Transform stackTarget;

    [Tooltip("Distance threshold for correct stacking")]
    public float stackDistanceThreshold = 0.2f;

    [Tooltip("Angle threshold for correct stacking")]
    public float stackAngleThreshold = 15f;

    [Tooltip("Parent transform for spawned objects")]
    public Transform objectsParent;

    [Tooltip("Button to check the solution")]
    public Button checkSolutionButton;

    [Tooltip("Button to reset the puzzle")]
    public Button resetButton;

    [Header("Stacking Order")]
    [Tooltip("The correct order of objects (by prefab index)")]
    public List<int> correctOrder;

    // Internal variables
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<int> currentOrder = new List<int>();
    private bool isDragging = false;
    private GameObject draggedObject;
    private Vector3 dragOffset;

    /// <summary>
    /// Initialize the puzzle
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        // Set up buttons
        if (checkSolutionButton != null)
        {
            checkSolutionButton.onClick.AddListener(CheckSolution);
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetPuzzle);
        }

        Debug.Log("Stacking Puzzle initialized");
    }

    /// <summary>
    /// Show the puzzle UI
    /// </summary>
    public override void ShowPuzzle()
    {
        base.ShowPuzzle();

        // Spawn the stackable objects
        SpawnObjects();
    }

    /// <summary>
    /// Spawn the stackable objects
    /// </summary>
    private void SpawnObjects()
    {
        // Clear existing objects
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedObjects.Clear();
        currentOrder.Clear();

        // Spawn new objects
        for (int i = 0; i < stackableObjectPrefabs.Count; i++)
        {
            if (stackableObjectPrefabs[i] != null)
            {
                // Calculate position for the object
                Vector3 spawnPosition = new Vector3(
                    i * 1.5f - (stackableObjectPrefabs.Count * 1.5f) / 2,
                    0.5f,
                    0
                );

                // Spawn the object
                GameObject obj = Instantiate(
                    stackableObjectPrefabs[i],
                    spawnPosition,
                    Quaternion.identity,
                    objectsParent
                );

                // Add components for dragging
                if (obj.GetComponent<Collider>() == null)
                {
                    obj.AddComponent<BoxCollider>();
                }

                StackableObject stackable = obj.AddComponent<StackableObject>();
                stackable.puzzle = this;
                stackable.prefabIndex = i;

                spawnedObjects.Add(obj);
            }
        }
    }

    /// <summary>
    /// Start dragging an object
    /// </summary>
    /// <param name="obj">The object being dragged</param>
    public void StartDragging(GameObject obj)
    {
        if (isDragging) return;

        isDragging = true;
        draggedObject = obj;

        // Calculate offset from mouse to object center
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)
        );
        dragOffset = obj.transform.position - mousePosition;

        Debug.Log("Started dragging object: " + obj.name);
    }

    /// <summary>
    /// Update dragging position
    /// </summary>
    public void UpdateDragging()
    {
        if (!isDragging || draggedObject == null) return;

        // Calculate new position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)
        );
        Vector3 newPosition = mousePosition + dragOffset;

        // Update object position
        draggedObject.transform.position = new Vector3(
            newPosition.x,
            draggedObject.transform.position.y,
            newPosition.z
        );
    }

    /// <summary>
    /// Stop dragging an object
    /// </summary>
    public void StopDragging()
    {
        if (!isDragging) return;

        isDragging = false;

        // Check if the object is close to the stack target
        if (draggedObject != null)
        {
            float distance = Vector3.Distance(
                draggedObject.transform.position,
                stackTarget.position
            );

            if (distance < stackDistanceThreshold)
            {
                // Snap to stack position
                StackObject(draggedObject);
            }

            Debug.Log("Stopped dragging object: " + draggedObject.name);
            draggedObject = null;
        }
    }

    /// <summary>
    /// Stack an object on the target
    /// </summary>
    /// <param name="obj">The object to stack</param>
    private void StackObject(GameObject obj)
    {
        StackableObject stackable = obj.GetComponent<StackableObject>();
        if (stackable == null) return;

        // Calculate the position for this object in the stack
        int stackIndex = currentOrder.Count;
        Vector3 stackPosition = stackTarget.position + Vector3.up * (stackIndex + 1) * 0.5f;

        // Move the object to the stack position
        obj.transform.position = stackPosition;

        // Add to current order
        currentOrder.Add(stackable.prefabIndex);

        Debug.Log("Stacked object " + stackable.prefabIndex + " at position " + stackIndex);
    }

    /// <summary>
    /// Check if the current stack matches the correct order
    /// </summary>
    private void CheckSolution()
    {
        if (currentOrder.Count != correctOrder.Count)
        {
            GiveFeedback("You haven't stacked all the objects yet!");
            return;
        }

        bool isCorrect = true;
        for (int i = 0; i < currentOrder.Count; i++)
        {
            if (currentOrder[i] != correctOrder[i])
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            GiveFeedback("Correct! Puzzle solved!");
            CompletePuzzle();
        }
        else
        {
            GiveFeedback("Incorrect order. Try again!");
        }
    }

    /// <summary>
    /// Reset the puzzle to its initial state
    /// </summary>
    public override void ResetPuzzle()
    {
        base.ResetPuzzle();

        // Clear the stack
        currentOrder.Clear();

        // Respawn objects
        SpawnObjects();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if (isDragging)
        {
            UpdateDragging();

            // Check for mouse release
            if (Input.GetMouseButtonUp(0))
            {
                StopDragging();
            }
        }
    }
}

/// <summary>
/// Component added to stackable objects to handle dragging
/// </summary>
public class StackableObject : MonoBehaviour
{
    public StackingPuzzle puzzle;
    public int prefabIndex;

    private void OnMouseDown()
    {
        if (puzzle != null)
        {
            puzzle.StartDragging(gameObject);
        }
    }

    private void OnMouseDrag()
    {
        // Handled in the puzzle's Update method
    }

    private void OnMouseUp()
    {
        // Handled in the puzzle's Update method
    }
}
