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



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, foodDetectionRadius);
    }
}