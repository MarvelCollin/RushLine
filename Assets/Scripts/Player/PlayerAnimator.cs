using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Run Animation Sprites")]
    public Sprite[] runSprites;
    public float runAnimationSpeed = 0.1f;

    [Header("Jump Sprite")]
    public Sprite jumpSprite;

    [Header("Death Sprite")]
    public Sprite deathSprite;

    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;
    private int currentRunFrame;
    private float runAnimationTimer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GetComponent<PlayerController>();
        currentRunFrame = 0;
        runAnimationTimer = 0f;
    }

    void Update()
    {
        if (playerController == null || spriteRenderer == null) return;

        if (playerController.IsDead())
        {
            if (deathSprite != null)
            {
                spriteRenderer.sprite = deathSprite;
            }
            return;
        }

        if (!playerController.IsGrounded())
        {
            if (jumpSprite != null)
            {
                spriteRenderer.sprite = jumpSprite;
            }
            return;
        }

        PlayRunAnimation();
    }

    void PlayRunAnimation()
    {
        if (runSprites == null || runSprites.Length == 0) return;

        runAnimationTimer += Time.deltaTime;

        if (runAnimationTimer >= runAnimationSpeed)
        {
            runAnimationTimer = 0f;
            currentRunFrame++;

            if (currentRunFrame >= runSprites.Length)
            {
                currentRunFrame = 0;
            }

            spriteRenderer.sprite = runSprites[currentRunFrame];
        }
    }
}
