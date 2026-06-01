using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Movement : MonoBehaviour
{
    public Camera cam;
    public float speed = 2f;
    public float speedMultiplierPerScale = 0.1f;  // Smaller cells move faster

    private SizeManager sizeManager;
    private InputAction splitAction;
    private List<SizeManager> mySplitCells = new List<SizeManager>();

    private void Start()
    {
        sizeManager = GetComponent<SizeManager>();
        sizeManager.ownerMovement = this;
        mySplitCells.Add(sizeManager);

        splitAction = new InputAction("Split", binding: "<Keyboard>/space");
        splitAction.performed += OnSplitPerformed;
        splitAction.Enable();
    }

    void Update()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mousePos = new Vector3(mouseScreen.x, mouseScreen.y, 5f);
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);

        // Move all split cells toward mouse
        foreach (SizeManager cell in mySplitCells)
        {
            if (cell != null)
            {
                // Smaller cells move faster
                float cellSpeed = speed + (speedMultiplierPerScale * (1f / cell.currentScale));

                Vector3 newPosition = Vector3.MoveTowards(
                    cell.transform.position,
                    worldPos,
                    cellSpeed * Time.deltaTime
                );

                newPosition.z = cell.transform.position.z;
                cell.transform.position = newPosition;
            }
        }

        // Clean up destroyed cells
        mySplitCells.RemoveAll(cell => cell == null);
    }

    private void OnSplitPerformed(InputAction.CallbackContext context)
    {
        if (sizeManager != null)
        {
            sizeManager.SplitCell();
        }
    }

    /// <summary>
    /// Register a new split cell
    /// </summary>
    public void RegisterSplitCell(SizeManager newCell)
    {
        if (!mySplitCells.Contains(newCell))
        {
            mySplitCells.Add(newCell);
            Debug.Log($"Split cell registered. Total cells: {mySplitCells.Count}");
        }
    }

    /// <summary>
    /// Unregister a merged/destroyed split cell
    /// </summary>
    public void UnregisterSplitCell(SizeManager cell)
    {
        if (mySplitCells.Contains(cell))
        {
            mySplitCells.Remove(cell);
            Debug.Log($"Split cell unregistered. Total cells: {mySplitCells.Count}");
        }
    }

    /// <summary>
    /// Get all split cells owned by this player
    /// </summary>
    public List<SizeManager> GetAllCells()
    {
        mySplitCells.RemoveAll(cell => cell == null);
        return mySplitCells;
    }

    /// <summary>
    /// Get total mass of all split cells
    /// </summary>
    public float GetTotalMass()
    {
        float totalMass = 0f;
        foreach (SizeManager cell in mySplitCells)
        {
            if (cell != null)
                totalMass += cell.currentScale;
        }
        return totalMass;
    }

    private void OnDestroy()
    {
        splitAction?.Dispose();
    }
}