using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static float FinalScore;

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

    [Header("システム")]
    public ChargeSystem chargeSystem;
    public DirectionSystem directionSystem;
    public TimingSystem timingSystem;
    public ThrowController throwController;
    public WeatherSystem weatherSystem;

    [HideInInspector] public float chargePower;
    [HideInInspector] public float directionAccuracy;
    [HideInInspector] public float timingAccuracy;

    // ★投ごとのスコア保存
    private float[] throwScores;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // 配列初期化
        throwScores = new float[maxThrow];
    }

    private void Start()
    {
        UpdateScoreUI();
        StartChargePhase();
    }

    //==================================================
    // Charge
    //==================================================
    public void StartChargePhase()
    {
        weatherSystem.DecideWeather();

        currentPhase = GamePhase.Charge;

        Debug.Log($"----- {currentThrow}投目 -----");

        chargeSystem.StartCharge();
    }

    //==================================================
    // Direction
    //==================================================
    public void StartDirectionPhase()
    {
        currentPhase = GamePhase.Direction;
        directionSystem.StartDirection();
    }

    //==================================================
    // Timing
    //==================================================
    public void StartTimingPhase()
    {
        currentPhase = GamePhase.Timing;
        timingSystem.StartTiming();
    }

    //==================================================
    // Throw
    //==================================================
    public void StartThrowPhase()
    {
        currentPhase = GamePhase.Throw;

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
        Vector3 start = new Vector3(
            throwStartPoint.position.x, 0f, throwStartPoint.position.z);

        Vector3 end = new Vector3(
            landingPoint.x, 0f, landingPoint.z);

        float distance = Vector3.Distance(start, end);

        float finalScore = weatherSystem.ApplyScore(distance);

        totalScore += finalScore;

        //==================================================
        // ★安全にスコア保存（ここが修正ポイント）
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
        // フェーズ分岐
        //==================================================
        if (currentThrow <= maxThrow)
        {
            StartChargePhase();
            UpdateScoreUI();
        }
        else
        {
            currentPhase = GamePhase.Result;

            FinalScore = totalScore; // ★追加

            Debug.Log("ゲーム終了");
            Debug.Log($"最終スコア : {totalScore:F2}m");

            SceneManager.LoadScene("result Scene");
        }
    }

    //==================================================
    // 右上UI表示
    //==================================================
    void UpdateScoreUI()
    {
        if (scoreUI == null) return;

        string text = "";

        // 2投目以降：1投目表示
        if (currentThrow >= 2)
        {
            text += $"1投目: {throwScores[0]:F1}\n";
        }

        // 3投目以降：2投目＋合計表示
        if (currentThrow >= 3)
        {
            text += $"2投目: {throwScores[1]:F1}\n";
            text += $"合計: {totalScore:F1}";
        }

        scoreUI.text = text;
    }
}