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

    private Vector3 startPosition;
    private float timeOffset;
    private Vector3 baseScale;
    private bool isCollected = false;
    private Vector3 targetScreenPos;
    private SpriteRenderer sr;
    private Collider2D col;

    void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0f, Mathf.PI * 2f);
        baseScale = transform.localScale;
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (isCollected)
        {
            FlyToUI();
            return;
        }

        float bobOffset = Mathf.Sin((Time.time + timeOffset) * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPosition.x, startPosition.y + bobOffset, startPosition.z);

        float scaleOffset = 1f + Mathf.Sin((Time.time + timeOffset) * scaleSpeed) * scaleAmount;
        transform.localScale = baseScale * scaleOffset;
    }

    void FlyToUI()
    {
        if (UIManager.Instance != null)
        {
            Vector3 targetWorld = Camera.main.ScreenToWorldPoint(UIManager.Instance.GetDiamondPanelWorldPosition());
            targetWorld.z = 0;
            
            transform.position = Vector3.MoveTowards(transform.position, targetWorld, flySpeed * Time.deltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, flyScaleSpeed * Time.deltaTime);
            
            float dist = Vector3.Distance(transform.position, targetWorld);
            if (dist < 0.5f || transform.localScale.x < 0.1f)
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
