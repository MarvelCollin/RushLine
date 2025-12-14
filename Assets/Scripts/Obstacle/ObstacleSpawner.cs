using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Cave Settings")]
    public float gapHeight = 3.5f;
    public float ceilingThickness = 8f;
    public float floorThickness = 8f;

    [Header("Smooth Wave Settings")]
    public float maxWaveY = 2f;
    public float minWaveY = -2f;
    public float waveSmoothing = 2f;
    public float targetChangeInterval = 3f;

    [Header("Spawn Settings")]
    public float segmentWidth = 1f;
    public float spawnXPosition = 12f;
    public float destroyXPosition = -12f;

    [Header("Movement Settings")]
    public float baseScrollSpeed = 4f;
    public float maxScrollSpeed = 8f;

    [Header("Colors")]
    public Color ceilingColor = new Color(0.35f, 0.25f, 0.2f);
    public Color floorColor = new Color(0.5f, 0.4f, 0.3f);
    public Color spikeColor = new Color(0.8f, 0.2f, 0.2f);

    [Header("Spike Settings")]
    public float spikeChance = 0.15f;
    public float spikeHeight = 0.5f;
    public float spikeWidth = 0.4f;
    public float minSpikesGap = 3f;

    private List<CaveSegment> caveSegments = new List<CaveSegment>();
    private float lastSpikeX = -100f;
    private Sprite sharedSprite;
    private Sprite triangleSprite;
    
    private float currentWaveY = 0f;
    private float waveVelocity = 0f;
    private float targetWaveY = 0f;
    private float targetTimer = 0f;
    private float lastSpawnX = 0f;

    void Start()
    {
        sharedSprite = CreateSquareSprite();
        triangleSprite = CreateTriangleSprite();
        currentWaveY = 0f;
        waveVelocity = 0f;
        targetWaveY = 0f;
        targetTimer = 0f;
        lastSpawnX = spawnXPosition;
        FillInitialCave();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;

        UpdateWaveTarget();
        
        currentWaveY = Mathf.SmoothDamp(currentWaveY, targetWaveY, ref waveVelocity, waveSmoothing);

        float currentSpeed = GetCurrentSpeed();

        for (int i = caveSegments.Count - 1; i >= 0; i--)
        {
            CaveSegment seg = caveSegments[i];
            
            seg.xPosition -= currentSpeed * Time.deltaTime;

            if (seg.floor != null)
            {
                Vector3 floorPos = seg.floor.transform.position;
                seg.floor.transform.position = new Vector3(seg.xPosition, floorPos.y, 0f);
            }

            if (seg.ceiling != null)
            {
                Vector3 ceilPos = seg.ceiling.transform.position;
                seg.ceiling.transform.position = new Vector3(seg.xPosition, ceilPos.y, 0f);
            }

            if (seg.xPosition < destroyXPosition)
            {
                if (seg.floor != null) Destroy(seg.floor);
                if (seg.ceiling != null) Destroy(seg.ceiling);
                caveSegments.RemoveAt(i);
            }
        }

        lastSpawnX -= currentSpeed * Time.deltaTime;
        
        while (lastSpawnX < spawnXPosition)
        {
            lastSpawnX += segmentWidth;
            SpawnCaveSegment(lastSpawnX);
        }
    }

    void UpdateWaveTarget()
    {
        targetTimer += Time.deltaTime;
        
        if (targetTimer >= targetChangeInterval)
        {
            targetTimer = 0f;
            
            float range = maxWaveY - minWaveY;
            float randomOffset = Random.Range(-range * 0.4f, range * 0.4f);
            targetWaveY = currentWaveY + randomOffset;
            targetWaveY = Mathf.Clamp(targetWaveY, minWaveY, maxWaveY);
        }
    }

    void FillInitialCave()
    {
        float overlap = segmentWidth * 0.5f;
        for (float x = destroyXPosition - segmentWidth; x <= spawnXPosition + segmentWidth; x += segmentWidth - overlap)
        {
            CreateSegmentAt(x, 0f);
        }
        lastSpawnX = spawnXPosition;
    }

    void SpawnCaveSegment(float xPos)
    {
        CreateSegmentAt(xPos, currentWaveY);
    }

    void CreateSegmentAt(float xPos, float waveY)
    {
        CaveSegment segment = new CaveSegment();
        segment.xPosition = xPos;

        float floorTopY = waveY - (gapHeight / 2f);
        float ceilingBottomY = waveY + (gapHeight / 2f);

        GameObject floor = new GameObject("Floor");
        float floorCenterY = floorTopY - (floorThickness / 2f);
        floor.transform.position = new Vector3(xPos, floorCenterY, 0f);
        floor.transform.localScale = new Vector3(segmentWidth * 1.5f, floorThickness, 1f);
        
        SpriteRenderer floorSR = floor.AddComponent<SpriteRenderer>();
        floorSR.sprite = sharedSprite;
        floorSR.color = floorColor;
        floorSR.sortingOrder = 1;
        
        BoxCollider2D floorCol = floor.AddComponent<BoxCollider2D>();
        floor.tag = "Ground";
        
        segment.floor = floor;

        GameObject ceiling = new GameObject("Ceiling");
        float ceilingCenterY = ceilingBottomY + (ceilingThickness / 2f);
        ceiling.transform.position = new Vector3(xPos, ceilingCenterY, 0f);
        ceiling.transform.localScale = new Vector3(segmentWidth * 1.5f, ceilingThickness, 1f);
        
        SpriteRenderer ceilingSR = ceiling.AddComponent<SpriteRenderer>();
        ceilingSR.sprite = sharedSprite;
        ceilingSR.color = ceilingColor;
        ceilingSR.sortingOrder = 1;
        
        BoxCollider2D ceilingCol = ceiling.AddComponent<BoxCollider2D>();
        ceiling.tag = "Ceiling";
        
        segment.ceiling = ceiling;

        if (xPos > 5f && xPos - lastSpikeX >= minSpikesGap && Random.value < spikeChance)
        {
            bool onFloor = Random.value > 0.5f;
            CreateSpike(xPos, waveY, onFloor, segment);
            lastSpikeX = xPos;
        }

        caveSegments.Add(segment);
    }

    float GetCurrentSpeed()
    {
        float difficulty = 1f;
        if (GameManager.Instance != null)
        {
            difficulty = GameManager.Instance.GetDifficulty();
        }
        float adjustedSpeed = baseScrollSpeed * difficulty;
        return Mathf.Min(adjustedSpeed, maxScrollSpeed);
    }

    void CreateSpike(float xPos, float waveY, bool onFloor, CaveSegment segment)
    {
        GameObject spike = new GameObject("Spike");
        
        float floorTopY = waveY - (gapHeight / 2f);
        float ceilingBottomY = waveY + (gapHeight / 2f);
        
        float spikeY;
        float scaleY;
        if (onFloor)
        {
            spikeY = floorTopY + (spikeHeight / 2f);
            scaleY = spikeHeight;
        }
        else
        {
            spikeY = ceilingBottomY - (spikeHeight / 2f);
            scaleY = -spikeHeight;
        }
        
        spike.transform.position = new Vector3(xPos, spikeY, 0f);
        spike.transform.localScale = new Vector3(spikeWidth, scaleY, 1f);
        
        SpriteRenderer spikeSR = spike.AddComponent<SpriteRenderer>();
        spikeSR.sprite = triangleSprite;
        spikeSR.color = spikeColor;
        spikeSR.sortingOrder = 2;
        
        PolygonCollider2D spikeCol = spike.AddComponent<PolygonCollider2D>();
        spikeCol.isTrigger = true;
        spike.tag = "Obstacle";
        
        spike.transform.SetParent(segment.floor.transform);
    }

    Sprite CreateSquareSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }

    Sprite CreateTriangleSprite()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        
        for (int y = 0; y < size; y++)
        {
            float progress = (float)y / size;
            int halfWidth = Mathf.RoundToInt((1f - progress) * size / 2f);
            int centerX = size / 2;
            
            for (int x = centerX - halfWidth; x <= centerX + halfWidth; x++)
            {
                if (x >= 0 && x < size)
                {
                    pixels[y * size + x] = Color.white;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}

public class CaveSegment
{
    public float xPosition;
    public GameObject floor;
    public GameObject ceiling;
}
