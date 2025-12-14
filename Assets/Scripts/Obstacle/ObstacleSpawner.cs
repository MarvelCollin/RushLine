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
    public float smallSpikeHeight = 0.3f;
    public float smallSpikeWidth = 0.2f;
    public float bigSpikeHeight = 0.6f;
    public float bigSpikeWidth = 0.4f;
    public float minSpikesGap = 3f;
    public int minSpikeCount = 1;
    public int maxSpikeCount = 4;

    [Header("Hole Settings")]
    public float holeWidth = 3f;
    public Color holeColor = new Color(0.1f, 0.1f, 0.2f);

    [Header("Diamond Settings")]
    public Sprite diamondSprite;
    public float diamondChance = 0.08f;
    public float diamondScale = 1.5f;
    public float minDiamondGap = 2f;

    private List<CaveSegment> caveSegments = new List<CaveSegment>();
    private List<GameObject> diamonds = new List<GameObject>();
    private float distanceSinceLastSpike = 100f;
    private float distanceSinceLastDiamond = 100f;
    private int holeSegmentsRemaining = 0;
    private Sprite sharedSprite;
    private Sprite triangleSprite;
    
    private float currentWaveY = 0f;
    private float waveVelocity = 0f;
    private float targetWaveY = 0f;
    private float targetTimer = 0f;
    private float lastSpawnX = 0f;
    private bool isInitialFill = false;

    void Start()
    {
        sharedSprite = CreateSquareSprite();
        triangleSprite = CreateTriangleSprite();
        currentWaveY = 0f;
        waveVelocity = 0f;
        targetWaveY = 0f;
        targetTimer = 0f;
        lastSpawnX = spawnXPosition;
        isInitialFill = true;
        FillInitialCave();
        isInitialFill = false;
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

        for (int i = diamonds.Count - 1; i >= 0; i--)
        {
            if (diamonds[i] == null)
            {
                diamonds.RemoveAt(i);
                continue;
            }
            
            Vector3 dPos = diamonds[i].transform.position;
            dPos.x -= currentSpeed * Time.deltaTime;
            diamonds[i].GetComponent<Diamond>().UpdateStartPosition(dPos.x);
            
            if (dPos.x < destroyXPosition)
            {
                Destroy(diamonds[i]);
                diamonds.RemoveAt(i);
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

        bool isHole = holeSegmentsRemaining > 0;
        if (isHole)
        {
            holeSegmentsRemaining--;
        }

        GameObject floor = new GameObject("Floor");
        float floorCenterY = floorTopY - (floorThickness / 2f);
        floor.transform.position = new Vector3(xPos, floorCenterY, 0f);
        floor.transform.localScale = new Vector3(segmentWidth * 1.5f, floorThickness, 1f);
        
        SpriteRenderer floorSR = floor.AddComponent<SpriteRenderer>();
        floorSR.sprite = sharedSprite;
        floorSR.color = isHole ? holeColor : floorColor;
        floorSR.sortingOrder = isHole ? 0 : 1;
        
        if (!isHole)
        {
            BoxCollider2D floorCol = floor.AddComponent<BoxCollider2D>();
            floor.tag = "Ground";
        }
        
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

        if (!isInitialFill)
        {
            distanceSinceLastSpike += segmentWidth;
            
            if (xPos > 5f && distanceSinceLastSpike >= minSpikesGap && Random.value < spikeChance)
            {
                int obstacleType = Random.Range(0, 3);
                
                if (obstacleType == 0)
                {
                    holeSegmentsRemaining = Mathf.RoundToInt(holeWidth / segmentWidth);
                    distanceSinceLastSpike = 0f;
                }
                else if (obstacleType == 1)
                {
                    bool onFloor = Random.value > 0.5f;
                    int spikeCount = Random.Range(minSpikeCount, maxSpikeCount + 1);
                    CreateSpikeGroup(xPos, waveY, onFloor, segment, spikeCount, bigSpikeWidth, bigSpikeHeight);
                    distanceSinceLastSpike = 0f;
                }
                else
                {
                    bool onFloor = Random.value > 0.5f;
                    int spikeCount = Random.Range(minSpikeCount, maxSpikeCount + 1);
                    CreateSpikeGroup(xPos, waveY, onFloor, segment, spikeCount, smallSpikeWidth, smallSpikeHeight);
                    distanceSinceLastSpike = 0f;
                }
            }

            distanceSinceLastDiamond += segmentWidth;
            if (distanceSinceLastDiamond >= minDiamondGap && Random.value < diamondChance && holeSegmentsRemaining <= 0)
            {
                SpawnDiamond(xPos, waveY);
                distanceSinceLastDiamond = 0f;
            }
        }

        caveSegments.Add(segment);
    }

    void SpawnDiamond(float xPos, float waveY)
    {
        GameObject diamond = new GameObject("Diamond");
        
        float floorTopY = waveY - (gapHeight / 2f);
        float diamondY = floorTopY + 0.8f;
        
        diamond.transform.position = new Vector3(xPos, diamondY, 0f);
        diamond.transform.localScale = new Vector3(diamondScale, diamondScale, 1f);
        
        SpriteRenderer sr = diamond.AddComponent<SpriteRenderer>();
        if (diamondSprite != null)
        {
            sr.sprite = diamondSprite;
        }
        else
        {
            sr.sprite = CreateDiamondSprite();
        }
        sr.sortingOrder = 5;
        
        CircleCollider2D col = diamond.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.4f;
        
        Diamond diamondScript = diamond.AddComponent<Diamond>();
        diamondScript.SetWorldPosition(diamond.transform.position);
        
        diamonds.Add(diamond);
    }

    Sprite CreateDiamondSprite()
    {
        int size = 32;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        Color diamondColor = new Color(0.3f, 0.8f, 1f, 1f);
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        
        int centerX = size / 2;
        int centerY = size / 2;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distX = Mathf.Abs(x - centerX) / (float)(size / 2);
                float distY = Mathf.Abs(y - centerY) / (float)(size / 2);
                
                if (distX + distY <= 1f)
                {
                    pixels[y * size + x] = diamondColor;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
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

    void CreateSpikeGroup(float xPos, float waveY, bool onFloor, CaveSegment segment, int count, float width, float height)
    {
        float startX = xPos - ((count - 1) * width * 0.5f);
        for (int i = 0; i < count; i++)
        {
            float spikeX = startX + (i * width);
            CreateSpike(spikeX, waveY, onFloor, segment, width, height);
        }
    }

    void CreateSpike(float xPos, float waveY, bool onFloor, CaveSegment segment, float width, float height)
    {
        GameObject spike = new GameObject("Spike");
        
        float floorTopY = waveY - (gapHeight / 2f);
        float ceilingBottomY = waveY + (gapHeight / 2f);
        
        float spikeY;
        float scaleY;
        if (onFloor)
        {
            spikeY = floorTopY + (height / 2f);
            scaleY = height;
        }
        else
        {
            spikeY = ceilingBottomY - (height / 2f);
            scaleY = -height;
        }
        
        spike.transform.position = new Vector3(xPos, spikeY, 0f);
        spike.transform.localScale = new Vector3(width, scaleY, 1f);
        
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
