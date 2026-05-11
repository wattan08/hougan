using UnityEngine;

public enum TimingButtonType
{
    Cross,
    Circle,
    Triangle,
    Square,
    L,
    R,
    L2,
    R2
}
public class TimingSystem : MonoBehaviour
{
    [Header("バー移動速度")]
    public float moveSpeed = 2f;

    [Header("現在バー位置")]
    [Range(0f, 1f)]
    public float gaugeValue;

    [Header("タイミング精度")]
    [Range(0f, 1f)]
    public float timingAccuracy;

    [Header("現在指定ボタン")]
    public TimingButtonType currentButton;

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
    /// Timing開始
    /// </summary>
    public void StartTiming()
    {
        isRunning = true;

        // ランダムボタン決定
        currentButton =
            (TimingButtonType)
            Random.Range(
                0,
                System.Enum.GetValues(
                    typeof(TimingButtonType)).Length);

        Debug.Log(
            $"指定ボタン : {currentButton}");
    }

    /// <summary>
    /// ボタン押下
    /// </summary>
    public void PressButton(
        TimingButtonType pressedButton)
    {
        if (!isRunning)
            return;

        isRunning = false;

        // 正しいボタンか
        if (pressedButton != currentButton)
        {
            timingAccuracy = 0f;

            Debug.Log("ボタンミス");
        }
        else
        {
            // 中央との差
            float distanceFromCenter =
                Mathf.Abs(gaugeValue - 0.5f);

            // 0～1へ変換
            timingAccuracy =
                1f - (distanceFromCenter * 2f);

            timingAccuracy =
                Mathf.Clamp01(timingAccuracy);

            Debug.Log(
                $"タイミング精度 : {timingAccuracy}");
        }

        // 保存
        GameManager.Instance.timingAccuracy =
            timingAccuracy;

        // 投擲フェーズ
        GameManager.Instance.StartThrowPhase();
    }
}
