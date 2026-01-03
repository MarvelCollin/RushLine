using UnityEngine;

public class Diamond : MonoBehaviour
{
    [Header("Animation Settings")]
    public float bobSpeed = 3f;
    public float bobHeight = 0.15f;
    public float rotateSpeed = 100f;
    public float scaleSpeed = 2f;
    public float scaleAmount = 0.1f;

    [Header("Collect Animation")]
    public float flySpeed = 15f;
    public float flyScaleSpeed = 3f;

    [Header("Magnet Settings")]
    public float magnetRange = 5f;
    public float magnetSpeed = 8f;

    private Vector3 startPosition;
    private float timeOffset;
    private Vector3 baseScale;
    private bool isCollected = false;
    private Vector3 targetScreenPos;
    private SpriteRenderer sr;
    private Collider2D col;
    private Transform playerTransform;

    void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0f, Mathf.PI * 2f);
        baseScale = transform.localScale;
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (isCollected)
        {
            FlyToUI();
            return;
        }

        if (PowerUpManager.Instance != null && PowerUpManager.Instance.IsMagnetActive() && playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            
            if (distanceToPlayer < magnetRange)
            {
                Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
                transform.position += directionToPlayer * magnetSpeed * Time.deltaTime;
                startPosition = transform.position;
                return;
            }
        }

        float bobOffset = Mathf.Sin((Time.time + timeOffset) * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPosition.x, startPosition.y + bobOffset, startPosition.z);

        float scaleOffset = 1f + Mathf.Sin((Time.time + timeOffset) * scaleSpeed) * scaleAmount;
        transform.localScale = baseScale * scaleOffset;
    }

    void FlyToUI()
    {
        if (UIManager.Instance != null && Camera.main != null)
        {
            Vector3 panelWorldPos = UIManager.Instance.GetDiamondPanelWorldPosition();
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 targetScreenPos = new Vector3(Screen.width - 100f, Screen.height - 40f, screenPos.z);
            Vector3 targetWorld = Camera.main.ScreenToWorldPoint(targetScreenPos);
            targetWorld.z = 0;
            
            transform.position = Vector3.MoveTowards(transform.position, targetWorld, flySpeed * Time.deltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, flyScaleSpeed * Time.deltaTime);
            
            float dist = Vector3.Distance(transform.position, targetWorld);
            if (dist < 0.3f || transform.localScale.x < 0.05f)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddDiamond();
                }
                Destroy(gameObject);
            }
        }
        else
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddDiamond();
            }
            Destroy(gameObject);
        }
    }

    public void SetWorldPosition(Vector3 pos)
    {
        startPosition = pos;
        transform.position = pos;
    }

    public void UpdateStartPosition(float newX)
    {
        startPosition.x = newX;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            if (col != null) col.enabled = false;
        }
    }
}
