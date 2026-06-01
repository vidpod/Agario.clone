using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public Camera cam;
    public float speed = 2f;
    public float speedMultiplierPerScale = 0.1f;

    private SizeManager sizeManager;
    private InputAction splitAction;
    private List<SizeManager> mySplitCells = new List<SizeManager>();

    private void Start()
    {
        sizeManager = GetComponent<SizeManager>();
        if (sizeManager != null)
        {
            sizeManager.ownerMovement = this;
            mySplitCells.Add(sizeManager);
        }

        splitAction = new InputAction("Split", binding: "<Keyboard>/space");
        splitAction.performed += OnSplitPerformed;
        splitAction.Enable();

        Debug.Log("[MOVEMENT] Initialized. Split action enabled.");
    }

    private void Update()
    {
        if (cam == null || Mouse.current == null) return;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mousePos = new Vector3(mouseScreen.x, mouseScreen.y, 5f);
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);

        // Move only the main player cell.
        Vector3 newPosition = Vector3.MoveTowards(transform.position, worldPos, speed * Time.deltaTime);
        newPosition.z = transform.position.z;
        transform.position = newPosition;

        mySplitCells.RemoveAll(cell => cell == null);
    }

    private void OnSplitPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("[MOVEMENT] Split key pressed!");
        if (sizeManager != null)
        {
            sizeManager.SplitCell();
        }
        else
        {
            Debug.LogError("[ERROR] No SizeManager found!");
        }
    }

    public void RegisterSplitCell(SizeManager newCell)
    {
        if (newCell == null) return;
        if (!mySplitCells.Contains(newCell))
        {
            mySplitCells.Add(newCell);
            Debug.Log($"[MOVEMENT] Split cell registered. Total cells: {mySplitCells.Count}");
        }
    }

    public void UnregisterSplitCell(SizeManager cell)
    {
        if (cell == null) return;
        if (mySplitCells.Remove(cell))
        {
            Debug.Log($"[MOVEMENT] Split cell unregistered. Total cells: {mySplitCells.Count}");
        }
    }

    public List<SizeManager> GetAllCells()
    {
        mySplitCells.RemoveAll(cell => cell == null);
        return mySplitCells;
    }

    public float GetTotalMass()
    {
        float totalMass = 0f;
        foreach (SizeManager cell in mySplitCells)
        {
            if (cell != null) totalMass += cell.currentScale;
        }
        return totalMass;
    }

    private void OnDestroy()
    {
        if (splitAction != null)
        {
            splitAction.performed -= OnSplitPerformed;
            splitAction.Disable();
            splitAction.Dispose();
            splitAction = null;
        }
    }
}