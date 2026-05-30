using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    private Vector3 velocity;

    public float gravity = -9.81f;
    public bool isActive = true;

    [Header("見た目用モデル")]
    public Transform visualModel;

    [Header("見た目速度倍率")]
    [Range(0.1f, 5f)]
    public float visualSpeedMultiplier = 1f;

    public SimpleProjectile Init(Vector3 direction, float power)
    {
        velocity = direction.normalized * power;
        return this;
    }

    void Update()
    {
        if (!isActive) return;

        // ★本体（スコア用・絶対変えない）
        transform.position += velocity * Time.deltaTime;

        velocity.y += gravity * Time.deltaTime;

        // ★見た目だけ別速度
        if (visualModel != null)
        {
            visualModel.position += velocity * Time.deltaTime * visualSpeedMultiplier;
        }

        // 地面判定（本体基準）
        if (transform.position.y <= 0f)
        {
            transform.position = new Vector3(
                transform.position.x,
                0f,
                transform.position.z);

            isActive = false;

            GameManager.Instance.OnBallLanded(transform.position);
        }
    }
}