using UnityEngine;

public sealed class CameraFollower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 4f, -7f);
    [SerializeField] private float followSharpness = 20f;

    public void SetTarget(Transform player)
    {
        target = player;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = new Vector3(offset.x, offset.y, target.position.z + offset.z);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, 1f - Mathf.Exp(-followSharpness * Time.deltaTime));
        transform.rotation = Quaternion.Euler(20f, 0f, 0f);
    }
}