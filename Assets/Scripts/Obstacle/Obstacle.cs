using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [HideInInspector]
    public float moveSpeed = 6f;
    
    public float destroyXPosition = -15f;

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver())
        {
            return;
        }

        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        if (transform.position.x < destroyXPosition)
        {
            Destroy(gameObject);
        }
    }
}
