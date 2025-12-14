using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public float yOffset = 2f;
    public float zoomMultiplier = 1.5f;

    private Vector3 initialPosition;
    private Camera cam;
    private float initialOrthographicSize;

    void Start()
    {
        initialPosition = transform.position;
        cam = GetComponent<Camera>();
        if (cam != null && cam.orthographic)
        {
            initialOrthographicSize = cam.orthographicSize;
            cam.orthographicSize = initialOrthographicSize / zoomMultiplier;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        float targetY = target.position.y + yOffset;
        float smoothY = Mathf.Lerp(transform.position.y, targetY, smoothSpeed * Time.deltaTime);

        transform.position = new Vector3(initialPosition.x, smoothY, initialPosition.z);
    }
}
