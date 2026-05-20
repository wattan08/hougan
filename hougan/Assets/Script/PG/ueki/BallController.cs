using UnityEngine;

public class BallController : MonoBehaviour
{
    private bool isLanded = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (isLanded) return;

        if (GameManager.Instance == null) return;

        // ★ここは削除 or 緩和（重要）
        // if (GameManager.Instance.currentThrow <= 0) return;

        // ★地面だけ判定するのが安全
        if (!collision.collider.CompareTag("Ground"))
            return;

        isLanded = true;

        GameManager.Instance.OnBallLanded(transform.position);
    }
}