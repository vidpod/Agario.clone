using UnityEngine;
using System.Collections.Generic;

public class SizeManager : MonoBehaviour
{
    public float scaleSpeed = 5f;
    public float currentScale = 1f, growthPerFood = 0.5f;
    public float combatGrowthFactor = 0.2f;

    [Header("Split Settings")]
    public float minSizeToSplit = 2f;
    public float splitScaleReduction = 0.8f;
    public float splitForce = 15f;

    [Header("Merge Settings")]
    public float mergeDistance = 2.5f;
    public float mergeDelay = 0.5f;
    private float timeSinceSplit = 0f;

    [Header("Cell Ownership")]
    public string ownerTag = "Player";
    public Movement ownerMovement;

    private List<SizeManager> linkedCells = new List<SizeManager>();
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        timeSinceSplit = 0f;

        // Ensure physics is properly configured
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {
            currentScale += 0.1f / currentScale;
            GameManager.instance.currentFood--;
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SizeManager otherCharacter = collision.gameObject.GetComponent<SizeManager>();

        if (otherCharacter != null)
        {
            // Check if both cells belong to same owner (merge logic)
            if (ownerMovement != null && otherCharacter.ownerMovement == ownerMovement)
            {
                TryMerge(otherCharacter);
                return;
            }

            // Combat logic (eating other cells)
            if (this.currentScale > otherCharacter.currentScale)
            {
             
                currentScale += otherCharacter.currentScale * combatGrowthFactor;

                Debug.Log($"[USPEH] {gameObject.name} je pojedel {collision.gameObject.name}. Nova ciljna velikost: {currentScale}");

                Destroy(collision.gameObject);
            }
        }
    }

    void Update()
    {
        timeSinceSplit += Time.deltaTime;
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(currentScale, currentScale, 1), Time.deltaTime * scaleSpeed);

        if (timeSinceSplit >= mergeDelay && ownerMovement != null)
        {
            CheckForMergeNearby();
        }
    }

    /// <summary>
    /// Splits the cell into two cells with reduced size
    /// </summary>
    public void SplitCell()
    {
        if (currentScale < minSizeToSplit)
        {
            Debug.Log("Cell too small to split!");
            return;
        }

        currentScale *= splitScaleReduction;

        GameObject newCell = Instantiate(gameObject, transform.position, Quaternion.identity);

        // Configure the new cell
        SizeManager newSizeManager = newCell.GetComponent<SizeManager>();
        newSizeManager.currentScale = currentScale;
        newSizeManager.ownerMovement = ownerMovement;
        newSizeManager.ownerTag = ownerTag;
        newSizeManager.timeSinceSplit = 0f;

        // Ensure Rigidbody is properly configured
        Rigidbody2D newRb = newCell.GetComponent<Rigidbody2D>();
        if (newRb != null)
        {
            newRb.freezeRotation = true;
            newRb.constraints = RigidbodyConstraints2D.FreezeRotation;

            Vector2 splitDirection = Random.insideUnitCircle.normalized;
            newRb.linearVelocity = splitDirection * splitForce;
        }

        // Disable Movement component on split cell (only main cell controls all)
        Movement newMovement = newCell.GetComponent<Movement>();
        if (newMovement != null)
        {
            newMovement.enabled = false;
        }

        // Register new cell with owner
        if (ownerMovement != null)
        {
            ownerMovement.RegisterSplitCell(newSizeManager);
        }

        Debug.Log($"Cell split! New scale: {currentScale}. Split cells registered: {ownerMovement?.GetAllCells().Count}");
    }

    /// <summary>
    /// Checks for nearby cells owned by same player and merges if conditions met
    /// </summary>
    private void CheckForMergeNearby()
    {
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, mergeDistance);

        foreach (Collider2D collider in nearbyColliders)
        {
            if (collider.gameObject == gameObject)
                continue;

            SizeManager otherSize = collider.GetComponent<SizeManager>();
            if (otherSize != null && otherSize.ownerMovement == ownerMovement && otherSize.timeSinceSplit >= mergeDelay)
            {
                TryMerge(otherSize);
                break;
            }
        }
    }

    /// <summary>
    /// Merges two cells belonging to the same owner
    /// </summary>
    private void TryMerge(SizeManager otherCell)
    {
        if (otherCell == null || otherCell.gameObject == gameObject)
            return;

        // Combine scales
        currentScale += otherCell.currentScale;
        timeSinceSplit = 0f;

        Debug.Log($"Cells merged! New scale: {currentScale}");

        // Remove from Movement's tracking before destroying
        if (ownerMovement != null)
        {
            ownerMovement.UnregisterSplitCell(otherCell);
        }

        Destroy(otherCell.gameObject);
    }

    public void RegisterLinkedCell(SizeManager cell)
    {
        if (!linkedCells.Contains(cell))
            linkedCells.Add(cell);
    }

    public List<SizeManager> GetLinkedCells()
    {
        return linkedCells;
    }

    public void UnregisterLinkedCell(SizeManager cell)
    {
        linkedCells.Remove(cell);
    }
}