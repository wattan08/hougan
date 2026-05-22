using UnityEngine;

public class ThrowController : MonoBehaviour
{
    [Header("砲丸Prefab")]
    public GameObject shotBallPrefab;

    [Header("生成位置")]
    public Transform spawnPoint;

    [Header("基礎パワー")]
    public float basePower = 50f;

    [Header("最大投擲角度")]
    public float maxThrowAngle = 70f;

    [Header("最小投擲角度")]
    public float minThrowAngle = 10f;

    /// <summary>
    /// 投擲（Rigidbodyなし版）
    /// </summary>
    public void ThrowBall(float finalPower)
    {
        //==============================
        // Nullチェック
        //==============================
        if (shotBallPrefab == null)
        {
            Debug.LogError("Prefab未設定");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint未設定");
            return;
        }

        //==============================
        // 生成
        //==============================
        GameObject ball =
            Instantiate(
                shotBallPrefab,
                spawnPoint.position,
                Quaternion.identity);

        //==============================
        // 投擲角度
        //==============================
        float gauge =
            GameManager.Instance
            .directionSystem
            .gaugeValue;

        float throwAngle =
            Mathf.Lerp(
                minThrowAngle,
                maxThrowAngle,
                gauge);

        //==============================
        // 方向計算
        //==============================
        float rad = throwAngle * Mathf.Deg2Rad;

        Vector3 forward =
            spawnPoint.forward * Mathf.Cos(rad);

        Vector3 upward =
            Vector3.up * Mathf.Sin(rad);

        Vector3 direction = 
            (forward + upward).normalized;

        //==============================
        // Rigidbodyなし移動処理
        //==============================

        // 速度情報を保持して動かす
        ball.AddComponent<SimpleProjectile>()
            .Init(direction, finalPower);

        //==============================
        // Debug
        //==============================
        Debug.Log($"角度 : {throwAngle}");
        Debug.Log($"方向 : {direction}");
        Debug.Log($"パワー : {finalPower}");
    }
}