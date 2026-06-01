using UnityEngine;

public class ChargeSystem : MonoBehaviour
{
    [Header("チャージ制限時間")]
    public float chargeTime = 3f;

    [Header("最大連打数")]
    public int maxPressCount = 100;

    [Header("現在時間")]
    public float currentTime;

    [Header("現在連打数")]
    public int pressCount;

    [Header("現在チャージ量")]
    [Range(0f, 1f)]
    public float chargePower;

    private bool isCharging = false;

    private void Update()
    {
        if (!isCharging)
            return;

        // 時間減少
        currentTime -= Time.deltaTime;

        // 時間終了
        if (currentTime <= 0f)
        {
            EndCharge();
        }
    }

    /// <summary>
    /// チャージ開始
    /// </summary>
    public void StartCharge()
    {
        isCharging = true;

        currentTime = chargeTime;

        pressCount = 0;

        chargePower = 0f;

        Debug.Log("チャージ開始");
    }

    /// <summary>
    /// 連打追加
    /// </summary>
    public void AddCharge()
    {
        if (!isCharging)
            return;

        pressCount++;

        // リアルタイムチャージ量
        chargePower =
            Mathf.Clamp01(
                (float)pressCount / maxPressCount);

        Debug.Log(
            $"連打数 : {pressCount}  "
            + $"Power : {chargePower}");
    }
    /// <summary>
    /// チャージ終了
    /// </summary>
    private void EndCharge()
    {
        isCharging = false;

        Debug.Log(
            $"最終チャージ : {chargePower}");

        GameManager.Instance.chargePower =
            chargePower;

        // 修正
        GameManager.Instance.StartCoroutine(
     GameManager.Instance.DelayedDirectionPhase());
    }
}
