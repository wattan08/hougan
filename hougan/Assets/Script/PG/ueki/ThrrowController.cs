using UnityEngine;

public class ThrowController : MonoBehaviour
{
    [Header("砲丸Prefab")]
    public GameObject shotBallPrefab;

    [Header("生成位置")]
    public Transform spawnPoint;

    [Header("基礎パワー")]
    public float basePower = 30f;

    [Header("最大投擲角度")]
    public float maxThrowAngle = 70f;

    [Header("最小投擲角度")]
    public float minThrowAngle = 10f;

    /// <summary>
    /// 投擲
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
        // 砲丸生成
        //==============================

        GameObject ball =
            Instantiate(
                shotBallPrefab,
                spawnPoint.position,
                Quaternion.identity);

        //==============================
        // Rigidbody取得
        //==============================

        Rigidbody rb =
            ball.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbodyなし");
            return;
        }

        //==============================
        // 投擲角度
        //==============================

        float gauge =
            GameManager.Instance
            .directionSystem
            .gaugeValue;

        // 10～70度
        float throwAngle =
            Mathf.Lerp(
                10f,
                70f,
                gauge);

        //==============================
        // 方向計算
        //==============================

        float rad =
            throwAngle * Mathf.Deg2Rad;

        // 前方向
        Vector3 forward =
            spawnPoint.forward
            * Mathf.Cos(rad);

        // 上方向
        Vector3 upward =
            Vector3.up
            * Mathf.Sin(rad);

        // 合成
        Vector3 direction =
            (forward + upward)
            .normalized;

        //==============================
        // 力を加える
        //==============================

        rb.AddForce(
            direction * finalPower,
            ForceMode.Impulse);

        //==============================
        // Debug
        //==============================

        Debug.Log(
            $"角度 : {throwAngle}");

        Debug.Log(
            $"方向 : {direction}");

        Debug.Log(
            $"パワー : {finalPower}");
    }

}