using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    [Header("現在天候")]
    public WeatherType currentWeather;

    [Header("現在倍率")]
    public float currentMultiplier = 1f;

    // ❌ 重複していたので削除済み
    // public WeatherType currentWeather;

    //==================================================
    // 天候決定
    //==================================================
    public void DecideWeather()
    {
        int random = Random.Range(0, 3);

        currentWeather = (WeatherType)random;

        ApplyWeatherEffect();

        Debug.Log($"現在天候 : {currentWeather}");
        Debug.Log($"倍率 : {currentMultiplier}");
    }

    //==================================================
    // 効果適用
    //==================================================
    private void ApplyWeatherEffect()
    {
        switch (currentWeather)
        {
            case WeatherType.Sunny:
                currentMultiplier = 1f;
                break;

            case WeatherType.Rain:
                currentMultiplier = 0.5f;
                break;

            case WeatherType.Storm:
                bool lucky = Random.value < 0.5f;
                currentMultiplier = lucky ? 2f : 0.3f;
                break;
        }
    }

    //==================================================
    // スコア補正
    //==================================================
    public float ApplyScore(float baseScore)
    {
        return baseScore * currentMultiplier;
    }
}