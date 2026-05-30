using UnityEngine;
using UnityEngine.UI;

public class DirectionSystem : MonoBehaviour
{
    [Header("ゲージ移動速度")]
    public float moveSpeed = 1.5f;

    [Header("速度表示")]
    public Slider speedSlider;

    [Header("現在ゲージ値")]
    [Range(0f, 1f)]
    public float gaugeValue;

    [Header("方向精度")]
    [Range(0f, 1f)]
    public float directionAccuracy;

    private bool isRunning = false;

    private void Start()
    {
        if (speedSlider != null)
        {
            speedSlider.minValue = 0f;
            speedSlider.maxValue = 1f; // 想定最大速度
        }
    }

    private void Update()
    {
        if (!isRunning)
            return;

        gaugeValue = Mathf.PingPong(
            Time.time * moveSpeed,
            1f);

        if (speedSlider != null)
        {
            speedSlider.value = gaugeValue;
        }
    }

    public void StartDirection()
    {
        isRunning = true;

        Debug.Log("方向決定開始");
    }

    public void ConfirmDirection()
    {
        if (!isRunning)
            return;

        isRunning = false;

        float distanceFromCenter =
            Mathf.Abs(gaugeValue - 0.5f);

        directionAccuracy =
            1f - (distanceFromCenter * 2f);

        directionAccuracy =
            Mathf.Clamp01(directionAccuracy);

        Debug.Log($"方向精度 : {directionAccuracy}");

        GameManager.Instance.directionAccuracy =
            directionAccuracy;

        GameManager.Instance.StartTimingPhase();
    }
}