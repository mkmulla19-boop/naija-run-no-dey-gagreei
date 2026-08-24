using UnityEngine;

public sealed class CameraFollow : MonoBehaviour
{
    [Header("Target to Follow")]
    public Transform playerTransform;

    [Header("Camera Distance Offset")]
    public Vector3 offset = new Vector3(0f, 3.5f, -6f);

    [Header("Follow Smoothness")]
    public float smoothSpeed = 10f;

    private void LateUpdate()
    {
        if (playerTransform == null)
            return;

        Vector3 targetPosition = new Vector3(
            offset.x,
            offset.y,
            playerTransform.position.z + offset.z
        );

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}