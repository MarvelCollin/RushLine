using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public int maxJumps = 2;

    [Header("Player Scale")]
    public float playerScale = 2f;

    [Header("Position Lock")]
    public float lockedXPosition = -3f;
    public float fallDeathY = -10f;

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private bool isDead;
    private bool isGrounded;
    private int groundContactCount = 0;
    private int jumpCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        isDead = false;
        transform.localScale = new Vector3(playerScale, playerScale, 1f);
        lockedXPosition = transform.position.x;
    }

    void Update()
    {
        if (isDead) return;

        if (transform.position.y < fallDeathY)
        {
            Die();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (jumpCount < maxJumps)
            {
                Jump();
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;
        
        Vector3 pos = transform.position;
        pos.x = lockedXPosition;
        transform.position = pos;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpCount++;
        isGrounded = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Die();
            return;
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    groundContactCount++;
                    isGrounded = true;
                    jumpCount = 0;
                    break;
                }
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    isGrounded = true;
                    return;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundContactCount--;
            if (groundContactCount <= 0)
            {
                groundContactCount = 0;
                isGrounded = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}
