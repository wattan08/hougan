using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("追従対象")]
    public Transform target;

    [Header("オフセット")]
    public Vector3 offset = new Vector3(0, 5, -10);

    [Header("追従スピード")]
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;

        transform.LookAt(target);
    }
}