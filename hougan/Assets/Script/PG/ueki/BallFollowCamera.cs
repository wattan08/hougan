using UnityEngine;

public class BallFollowCamera : MonoBehaviour
{
    [Header("追従対象")]
    public Transform target;

    [Header("オフセット")]
    public Vector3 offset = new Vector3(0, 5, -10);

    [Header("追従速度")]
    public float smooth = 8f;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smooth * Time.deltaTime
        );

        transform.LookAt(target);
    }

    // =========================
    // 外部から安全に設定する用
    // =========================
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ClearTarget()
    {
        target = null;
    }
}