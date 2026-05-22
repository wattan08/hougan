using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //==================================================
    // リザルト用
    //==================================================
    public static float FinalScore;

    // スコア履歴
    public static List<float> ScoreHistory = new List<float>();

    // PlayerPrefs保存キー
    private const string SCORE_KEY = "ScoreHistory";

    [Header("現在フェーズ")]
    public GamePhase currentPhase;

    [Header("現在投数")]
    public int currentThrow = 1;

    [Header("最大投数")]
    public int maxThrow = 3;

    [Header("合計スコア")]
    public float totalScore;

    [Header("投擲開始位置")]
    public Transform throwStartPoint;

    [Header("右上スコアUI")]
    public TMP_Text scoreUI;

    [Header("左上天候UI")]
    public TMP_Text weatherUI;

    [Header("システム")]
    public ChargeSystem chargeSystem;
    public DirectionSystem directionSystem;
    public TimingSystem timingSystem;
    public ThrowController throwController;
    public WeatherSystem weatherSystem;

    [HideInInspector] public float chargePower;
    [HideInInspector] public float directionAccuracy;
    [HideInInspector] public float timingAccuracy;

    //==================================================
    // 投ごとのスコア保存
    //==================================================
    private float[] throwScores;

    //==================================================
    // Awake
    //==================================================
    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // maxThrow最低保証
        if (maxThrow <= 0)
        {
            maxThrow = 3;
        }

        // 配列初期化
        throwScores = new float[maxThrow];

        // スコア読込
        LoadScores();
    }

    //==================================================
    // Start
    //==================================================
    private void Start()
    {
        UpdateScoreUI();
        UpdateWeatherUI();

        StartChargePhase();
    }

    //==================================================
    // Charge
    //==================================================
    public void StartChargePhase()
    {
        currentPhase = GamePhase.Charge;

        if (weatherSystem != null)
        {
            weatherSystem.DecideWeather();

            // 天候UI更新
            UpdateWeatherUI();
        }

        Debug.Log($"----- {currentThrow}投目 -----");

        if (chargeSystem != null)
        {
            chargeSystem.StartCharge();
        }
        else
        {
            Debug.LogError("ChargeSystem が設定されていません");
        }
    }

    //==================================================
    // Direction
    //==================================================
    public void StartDirectionPhase()
    {
        currentPhase = GamePhase.Direction;

        if (directionSystem != null)
        {
            directionSystem.StartDirection();
        }
        else
        {
            Debug.LogError("DirectionSystem が設定されていません");
        }
    }

    //==================================================
    // Timing
    //==================================================
    public void StartTimingPhase()
    {
        currentPhase = GamePhase.Timing;

        if (timingSystem != null)
        {
            timingSystem.StartTiming();
        }
        else
        {
            Debug.LogError("TimingSystem が設定されていません");
        }
    }

    //==================================================
    // Throw
    //==================================================
    public void StartThrowPhase()
    {
        currentPhase = GamePhase.Throw;

        if (throwController == null)
        {
            Debug.LogError("ThrowController が設定されていません");
            return;
        }

        float finalPower =
            throwController.basePower *
            chargePower *
            directionAccuracy +
            (timingAccuracy * 2f);

        throwController.ThrowBall(finalPower);

        currentPhase = GamePhase.WaitingLanding;
    }

    //==================================================
    // Landing
    //==================================================
    public void OnBallLanded(Vector3 landingPoint)
    {
        if (throwStartPoint == null)
        {
            Debug.LogError("throwStartPoint が設定されていません");
            return;
        }

        Vector3 start = new Vector3(
            throwStartPoint.position.x,
            0f,
            throwStartPoint.position.z);

        Vector3 end = new Vector3(
            landingPoint.x,
            0f,
            landingPoint.z);

        float distance = Vector3.Distance(start, end);

        //==================================================
        // 天候補正
        //==================================================
        float finalScore = distance;

        if (weatherSystem != null)
        {
            finalScore = weatherSystem.ApplyScore(distance);
        }

        totalScore += finalScore;

        //==================================================
        // 投スコア保存
        //==================================================
        int index = currentThrow - 1;

        if (index >= 0 && index < throwScores.Length)
        {
            throwScores[index] = finalScore;
        }
        else
        {
            Debug.LogError("throwScores 範囲外アクセス");
        }

        Debug.Log($"今回の飛距離 : {distance:F2}m");
        Debug.Log($"今回スコア : {finalScore:F2}m");

        UpdateScoreUI();

        // 次投へ
        currentThrow++;

        //==================================================
        // 次フェーズ
        //==================================================
        if (currentThrow <= maxThrow)
        {
            StartChargePhase();
        }
        else
        {
            currentPhase = GamePhase.Result;

            // 最終スコア
            FinalScore = totalScore;

            // 保存
            SaveScore(totalScore);

            Debug.Log("ゲーム終了");
            Debug.Log($"最終スコア : {totalScore:F2}m");

            SceneManager.LoadScene("result Scene");
        }
    }

    //==================================================
    // スコア保存
    //==================================================
    public void SaveScore(float score)
    {
        // リスト追加
        ScoreHistory.Add(score);

        // 大きい順
        ScoreHistory = ScoreHistory
            .OrderByDescending(x => x)
            .ToList();

        // 3件だけ残す
        if (ScoreHistory.Count > 3)
        {
            ScoreHistory.RemoveRange(3, ScoreHistory.Count - 3);
        }

        // 保存文字列化
        string saveData = string.Join(",", ScoreHistory);

        PlayerPrefs.SetString(SCORE_KEY, saveData);
        PlayerPrefs.Save();
    }

    //==================================================
    // スコア読込
    //==================================================
    private void LoadScores()
    {
        ScoreHistory.Clear();

        if (!PlayerPrefs.HasKey(SCORE_KEY))
            return;

        string saveData = PlayerPrefs.GetString(SCORE_KEY);

        if (string.IsNullOrEmpty(saveData))
            return;

        string[] scores = saveData.Split(',');

        foreach (string s in scores)
        {
            if (float.TryParse(s, out float value))
            {
                ScoreHistory.Add(value);
            }
        }

        // 念のため並び替え
        ScoreHistory = ScoreHistory
            .OrderByDescending(x => x)
            .ToList();
    }

    //==================================================
    // 右上スコアUI
    //==================================================
    void UpdateScoreUI()
    {
        if (scoreUI == null)
            return;

        string text = "";

        for (int i = 0; i < maxThrow; i++)
        {
            if (throwScores[i] > 0)
            {
                text += $"{i + 1}投目 : {throwScores[i]:F1}\n";
            }
        }

        text += $"\n合計 : {totalScore:F1}";

        scoreUI.text = text;
    }

    //==================================================
    // 左上天候UI
    //==================================================
    void UpdateWeatherUI()
    {
        if (weatherUI == null || weatherSystem == null)
            return;

        string weatherName = "";

        switch (weatherSystem.currentWeather)
        {
            case WeatherType.Sunny:
                weatherName = "晴れ";
                break;

            case WeatherType.Rain:
                weatherName = "雨";
                break;

            case WeatherType.Storm:
                weatherName = "嵐";
                break;

            default:
                weatherName = "不明";
                break;
        }

        weatherUI.text = $"天候 : {weatherName}";
    }
}