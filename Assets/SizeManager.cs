using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class SizeManager : MonoBehaviour
{
    public float scaleSpeed = 5f;
    public float currentScale = 1f;
    public float growthPerFood = 0.5f;
    public float combatGrowthFactor = 0.2f;

    [Header("Split Settings")]
    public float minSizeToSplit = 2f;
    public float splitScaleReduction = 0.8f;
    public float splitForce = 3600f;
    public float splitTravelMultiplier = 1f;
    public float splitCellDamping = 0.6f;
    public float splitSpawnOffset = 0.6f;
    public float splitReturnDelay = 0.25f;
    public float splitReturnAcceleration = 8f;
    public float splitReturnMaxSpeed = 15f;
    public GameObject splitCellPrefab;

    [Header("Merge Settings")]
    public float mergeDistance = 1.5f;
    public float mergeDelay = 0.5f;

    [Header("Cell Ownership")]
    public string ownerTag = "Player";
    public Movement ownerMovement;
    public bool isMainCell = true;

    private float timeSinceSplit = 0f;
    private float splitTimer = 0f;
    private List<SizeManager> linkedCells = new List<SizeManager>();
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();

        timeSinceSplit = 0f;
        splitTimer = 0f;

        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (splitCellPrefab == null)
        {
            Debug.LogWarning($"[WARNING] {gameObject.name} has no splitCellPrefab assigned!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {
            currentScale += growthPerFood / currentScale;
            GameManager.instance.currentFood--;
            Destroy(other.gameObject);
            return;
        }

        // Bots won't fire OnCollisionEnter2D if collider is a trigger
        SimpleBot bot = other.GetComponent<SimpleBot>();
        if (bot != null)
        {
            float botSize = other.transform.localScale.x;
            if (this.currentScale > botSize)
            {
                currentScale += botSize * combatGrowthFactor;
                Debug.Log($"[BOT EATEN] {gameObject.name} ate {other.gameObject.name}");
                Destroy(other.gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SizeManager otherCharacter = collision.gameObject.GetComponent<SizeManager>();
        if (otherCharacter != null)
        {
            // Merge with own cells
            if (ownerMovement != null && otherCharacter.ownerMovement == ownerMovement && isMainCell && otherCharacter != this)
            {
                TryMerge(otherCharacter);
                return;
            }

            // Combat eat
            if (this.currentScale > otherCharacter.currentScale)
            {
                currentScale += otherCharacter.currentScale * combatGrowthFactor;

                if (collision.gameObject.CompareTag("Player"))
                {
                    Invoke(nameof(LoadGameOver), 1f);
                }
                else
                {
                }

                Destroy(collision.gameObject);
            }
            return;
        }

        // Bot eating
        SimpleBot bot = collision.gameObject.GetComponent<SimpleBot>();
        if (bot != null)
        {
            float botSize = collision.transform.localScale.x;
            if (this.currentScale > botSize)
            {
                currentScale += botSize * combatGrowthFactor;
                Debug.Log($"[BOT EATEN] {gameObject.name} ate {collision.gameObject.name}");
                Destroy(collision.gameObject);
            }
        }
    }

    private void LoadGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    private void Update()
    {
        timeSinceSplit += Time.deltaTime;
        splitTimer += Time.deltaTime;

        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(currentScale, currentScale, 1), Time.deltaTime * scaleSpeed);

        if (!isMainCell && rb != null && ownerMovement != null && splitTimer >= splitReturnDelay)
        {
            Vector2 toOwner = (Vector2)ownerMovement.transform.position - rb.position;
            float distance = toOwner.magnitude;
            if (distance > 0.05f)
            {
                Vector2 desiredVelocity = toOwner.normalized * Mathf.Min(splitReturnMaxSpeed, distance * 3f);
                rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, desiredVelocity, splitReturnAcceleration * Time.deltaTime);
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        if (timeSinceSplit >= mergeDelay && ownerMovement != null)
        {
            CheckForMergeNearby();
        }
    }

    public void SplitCell()
    {
        if (currentScale < minSizeToSplit)
        {
            Debug.Log("Cell too small to split!");
            return;
        }

        if (splitCellPrefab == null)
        {
            Debug.LogError("[ERROR] splitCellPrefab is NOT assigned!");
            return;
        }

        float splitSize = currentScale * 0.5f;

        currentScale = splitSize;

        Vector2 splitDirection = GetCursorDirection();
        Vector3 spawnPosition = transform.position + (Vector3)(splitDirection * splitSpawnOffset);
        GameObject newCell = Instantiate(splitCellPrefab, spawnPosition, Quaternion.identity);

        SpriteRenderer parentSprite = GetComponent<SpriteRenderer>();
        SpriteRenderer newSprite = newCell.GetComponent<SpriteRenderer>();
        if (parentSprite != null && newSprite != null)
        {
            newSprite.sprite = parentSprite.sprite;
            newSprite.color = parentSprite.color;
        }

        SizeManager newSizeManager = newCell.GetComponent<SizeManager>();
        if (newSizeManager == null)
        {
            Destroy(newCell);
            return;
        }

        newSizeManager.currentScale = currentScale;
        newSizeManager.ownerMovement = ownerMovement;
        newSizeManager.ownerTag = ownerTag;
        newSizeManager.isMainCell = false;
        newSizeManager.timeSinceSplit = 0f;
        newSizeManager.splitTimer = 0f;
        newSizeManager.splitCellPrefab = splitCellPrefab;
        newSizeManager.currentScale = splitSize;

        Rigidbody2D newRb = newCell.GetComponent<Rigidbody2D>();
        if (newRb != null)
        {
            newRb.freezeRotation = true;
            newRb.constraints = RigidbodyConstraints2D.FreezeRotation;
            newRb.linearDamping = splitCellDamping;
            newRb.linearVelocity = splitDirection * (splitForce * splitTravelMultiplier);
        }

        Movement newMovement = newCell.GetComponent<Movement>();
        if (newMovement != null)
        {
            newMovement.enabled = false;
        }

        if (ownerMovement != null)
        {
            ownerMovement.RegisterSplitCell(newSizeManager);
        }

        Debug.Log("[SPLIT] Cell split successfully!");
    }

    private Vector2 GetCursorDirection()
    {
        Camera camLocal = ownerMovement != null && ownerMovement.cam != null ? ownerMovement.cam : Camera.main;

        if (camLocal != null && Mouse.current != null)
        {
            Vector2 mouseScreen = Mouse.current.position.ReadValue();
            Vector3 mousePos = new Vector3(mouseScreen.x, mouseScreen.y, 5f);
            Vector3 worldPos = camLocal.ScreenToWorldPoint(mousePos);
            Vector2 direction = ((Vector2)worldPos - (Vector2)transform.position).normalized;
            if (direction != Vector2.zero) return direction;
        }

        return Vector2.right;
    }

    private void CheckForMergeNearby()
    {
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, mergeDistance);
        foreach (Collider2D collider in nearbyColliders)
        {
            if (collider.gameObject == gameObject) continue;

            SizeManager otherSize = collider.GetComponent<SizeManager>();
            if (otherSize != null && otherSize.ownerMovement == ownerMovement && otherSize.timeSinceSplit >= mergeDelay && isMainCell)
            {
                TryMerge(otherSize);
                break;
            }
        }
    }

    private void TryMerge(SizeManager otherCell)
    {
        if (otherCell == null || otherCell.gameObject == gameObject) return;

        currentScale += otherCell.currentScale;
        timeSinceSplit = 0f;

        Debug.Log($"Cells merged! New scale: {currentScale}");

        if (ownerMovement != null)
        {
            ownerMovement.UnregisterSplitCell(otherCell);
        }

        Destroy(otherCell.gameObject);
    }

    public void RegisterLinkedCell(SizeManager cell)
    {
        if (!linkedCells.Contains(cell)) linkedCells.Add(cell);
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