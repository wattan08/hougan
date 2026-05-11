using UnityEngine;

public class DirectionSystem : MonoBehaviour
{
    [Header("ゲージ移動速度")]
    public float moveSpeed = 1.5f;

    [Header("現在ゲージ値")]
    [Range(0f, 1f)]
    public float gaugeValue;

    [Header("方向精度")]
    [Range(0f, 1f)]
    public float directionAccuracy;

    private bool isRunning = false;

    private void Update()
    {
        if (!isRunning)
            return;

        // 0～1を往復
        gaugeValue =
            Mathf.PingPong(
                Time.time * moveSpeed,
                1f);
    }

    /// <summary>
    /// Direction開始
    /// </summary>
    public void StartDirection()
    {
        isRunning = true;

        Debug.Log("方向決定開始");
    }

    /// <summary>
    /// ×ボタン押下
    /// </summary>
    public void ConfirmDirection()
    {
        if (!isRunning)
            return;

        isRunning = false;

        // 中央(0.5)との距離
        float distanceFromCenter =
            Mathf.Abs(gaugeValue - 0.5f);

        // 0～1へ変換
        directionAccuracy =
            1f - (distanceFromCenter * 2f);

        directionAccuracy =
            Mathf.Clamp01(directionAccuracy);

        Debug.Log(
            $"方向精度 : {directionAccuracy}");

        // GameManagerへ保存
        GameManager.Instance.directionAccuracy =
            directionAccuracy;

        // 次フェーズへ
        GameManager.Instance.StartTimingPhase();
    }
}