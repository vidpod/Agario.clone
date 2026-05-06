using UnityEngine;

public class SimpleBot : MonoBehaviour
{
    public float speed = 2f;
    [Range(0, 10)]
    public float smoothness = 1f; // Kako hitro bot menja smer

    private Vector2 targetDirection;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Izbere prvo naključno smer
        ChangeDirection();
        // Vsaki 2 sekundi izbere novo smer, da ne gre samo v loku
        InvokeRepeating("ChangeDirection", 0, 2f);
    }

    void ChangeDirection()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        targetDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    void FixedUpdate()
    {
        // Gladko premikanje (Linear Velocity)
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetDirection * speed, Time.fixedDeltaTime * smoothness);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Če zadene zid, takoj spremeni smer stran od njega
        targetDirection = Vector2.Reflect(targetDirection, collision.contacts[0].normal);
    }
}