using UnityEngine;

public class SimpleBot : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 4f;
    public float responseTime = 0.15f;
    public float fleeSpeedMultiplier = 1.4f;

    [Header("AI Behavior")]
    public float detectionRadius = 5f;
    public LayerMask botLayer;

    [Header("Food")]
    public float foodDetectionRadius = 6f;

    [Header("Chase")]
    public float chaseDuration = 1.5f; // sekund lovi po izgubi vida

    private Vector2 targetDirection;
    private Vector2 currentVelocity;
    private Rigidbody2D rb;
    private float wanderTimer;
    private bool isReacting = false;

    private Transform lastKnownPrey;
    private Vector2 lastKnownPreyPosition;
    private float chaseTimer = 0f;
    private bool isChasing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.linearDamping = 0f;
        ChangeWanderDirection();
    }

    void Update()
    {
        if (!isReacting)
        {
            wanderTimer += Time.deltaTime;
            if (wanderTimer >= 2f)
            {
                ChangeWanderDirection();
                wanderTimer = 0f;
            }
        }

        // Chase timer odšteva
        if (isChasing)
        {
            chaseTimer -= Time.deltaTime;
            if (chaseTimer <= 0f)
            {
                isChasing = false;
                lastKnownPrey = null;
            }
        }
    }

    void FixedUpdate()
    {
        Collider2D[] botOverlaps = Physics2D.OverlapCircleAll(transform.position, detectionRadius, botLayer);

        float mySize = transform.localScale.x;

        Transform closestThreat = null;
        Transform closestPrey = null;
        float closestThreatDist = Mathf.Infinity;
        float closestPreyDist = Mathf.Infinity;

        foreach (var col in botOverlaps)
        {
            Debug.Log($"Zaznan: {col.gameObject.name} | tag: {col.tag} | layer: {col.gameObject.layer}");
            if (col.gameObject == this.gameObject) continue;

            float dist = Vector2.Distance(transform.position, col.transform.position);
            float otherSize = col.transform.localScale.x;

            if (otherSize > mySize)
            {
                if (dist < closestThreatDist)
                {
                    closestThreatDist = dist;
                    closestThreat = col.transform;
                }
            }
            else if (otherSize < mySize)
            {
                if (dist < closestPreyDist)
                {
                    closestPreyDist = dist;
                    closestPrey = col.transform;
                }
            }
        }

        // Ko vidimo plen — osvežimo zadnjo znano pozicijo in resetiramo timer
        if (closestPrey != null)
        {
            lastKnownPrey = closestPrey;
            lastKnownPreyPosition = closestPrey.position;
            chaseTimer = chaseDuration;
            isChasing = true;
        }

        // Poišči najbližjo hrano
        Collider2D[] allNearby = Physics2D.OverlapCircleAll(transform.position, foodDetectionRadius);
        Transform closestFood = null;
        float closestFoodDist = Mathf.Infinity;

        foreach (var col in allNearby)
        {
            if (!col.CompareTag("Food")) continue;

            float dist = Vector2.Distance(transform.position, col.transform.position);
            if (dist < closestFoodDist)
            {
                closestFoodDist = dist;
                closestFood = col.transform;
            }
        }

        // --- Prioritete ---
        Vector2 desiredDirection;
        float currentSpeed = speed;

        if (closestThreat != null)
        {
            // 1. BEŽIM
            isReacting = true;
            isChasing = false; // prekinemo lov če grožnja
            desiredDirection = ((Vector2)transform.position - (Vector2)closestThreat.position).normalized;
            currentSpeed = speed * fleeSpeedMultiplier;
        }
        else if (closestFood != null)
        {
            // 2. HRANA
            isReacting = true;
            desiredDirection = ((Vector2)closestFood.position - (Vector2)transform.position).normalized;
            currentSpeed = speed;
        }
        else if (closestPrey != null)
        {
            // 3. LOV — plen v vidnem polju
            isReacting = true;
            desiredDirection = ((Vector2)closestPrey.position - (Vector2)transform.position).normalized;
            currentSpeed = speed;
        }
        else if (isChasing)
        {
            // 4. LOV — plen izgubljen, gremo na zadnjo znano pozicijo
            isReacting = true;
            desiredDirection = (lastKnownPreyPosition - (Vector2)transform.position).normalized;
            currentSpeed = speed;
        }
        else
        {
            // 5. TAVANJE
            if (isReacting)
            {
                isReacting = false;
                ChangeWanderDirection();
            }
            desiredDirection = targetDirection;
            currentSpeed = speed;
        }

        Vector2 targetVelocity = desiredDirection * currentSpeed;
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocity, responseTime);
    }

    void ChangeWanderDirection()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        targetDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, foodDetectionRadius);
    }
}