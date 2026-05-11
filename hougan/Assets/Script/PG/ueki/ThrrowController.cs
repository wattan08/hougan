using UnityEngine;

public class ThrowController : MonoBehaviour
{
    [Header("砲丸Prefab")]
    public GameObject shotBallPrefab;

    [Header("生成位置")]
    public Transform spawnPoint;

    [Header("基礎パワー")]
    public float basePower = 30f;

    [Header("上方向補正")]
    public float upwardPower = 5f;

    /// <summary>
    /// 投擲
    /// </summary>
    public void ThrowBall(float finalPower)
    {
        // 砲丸生成
        GameObject ball =
            Instantiate(
                shotBallPrefab,
                spawnPoint.position,
                Quaternion.identity);

        // Rigidbody取得
        Rigidbody rb =
            ball.GetComponent<Rigidbody>();

        // 投擲方向
        Vector3 direction =
            transform.forward
            + Vector3.up * 0.2f;

        direction.Normalize();

        // 力を加える
        rb.AddForce(
            direction * finalPower,
            ForceMode.Impulse);

        Debug.Log(
            $"投擲パワー : {finalPower}");
    }
}