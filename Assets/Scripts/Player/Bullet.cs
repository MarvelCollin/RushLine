using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float maxDistance = 20f;
    
    private Vector3 startPosition;
    private SpriteRenderer sr;

    void Start()
    {
        startPosition = transform.position;
        
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            sr = gameObject.AddComponent<SpriteRenderer>();
        }
        
        if (sr.sprite == null)
        {
            sr.sprite = CreateBulletSprite();
        }
        
        sr.color = new Color(1f, 0.8f, 0.2f, 1f);
        sr.sortingOrder = 10;
        
        CircleCollider2D col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.2f;
        
        gameObject.tag = "Bullet";
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
        {
            Destroy(gameObject);
            return;
        }

        transform.Translate(Vector3.right * speed * Time.deltaTime);
        
        float distance = Vector3.Distance(startPosition, transform.position);
        if (distance > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    Sprite CreateBulletSprite()
    {
        int size = 16;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        int centerX = size / 2;
        int centerY = size / 2;
        float radius = size / 2f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                if (dist <= radius)
                {
                    pixels[y * size + x] = Color.white;
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    public static void SpawnBullet(Vector3 position)
    {
        GameObject bullet = new GameObject("Bullet");
        bullet.transform.position = position + new Vector3(0.5f, 0, 0);
        bullet.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        bullet.AddComponent<Bullet>();
    }
}
