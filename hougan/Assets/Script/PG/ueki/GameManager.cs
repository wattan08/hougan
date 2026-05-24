using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static float FinalScore;
    public static List<float> ScoreHistory = new List<float>();

    private const string SCORE_KEY = "ScoreHistory";

    [Header("現在フェーズ")]
    public GamePhase currentPhase;

    public int currentThrow = 1;
    public int maxThrow = 3;

    public float totalScore;

    [Header("UI")]
    public TMP_Text scoreUI;
    public TMP_Text weatherUI;

    [Header("Systems")]
    public ChargeSystem chargeSystem;
    public DirectionSystem directionSystem;
    public TimingSystem timingSystem;
    public ThrowController throwController;
    public WeatherSystem weatherSystem;

    [Header("Cameras")]
    public Camera mainCamera;
    public Camera gameCamera;
    public BallFollowCamera ballFollowCamera;

    [Header("Runtime")]
    public Transform currentBall;

    [HideInInspector] public float chargePower;
    [HideInInspector] public float directionAccuracy;
    [HideInInspector] public float timingAccuracy;

    private float[] throwScores;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        throwScores = new float[maxThrow];
        LoadScores();
    }

    private void Start()
    {
        SetCamera(CameraMode.Main);

        UpdateScoreUI();
        UpdateWeatherUI();

        StartChargePhase();
    }

    // =========================
    // Phase
    // =========================

    public void StartChargePhase()
    {
        SetCamera(CameraMode.Main);

        currentPhase = GamePhase.Charge;

        weatherSystem?.DecideWeather();
        UpdateWeatherUI();

        chargeSystem?.StartCharge();
    }

    public void StartDirectionPhase()
    {
        currentPhase = GamePhase.Direction;
        directionSystem?.StartDirection();
    }

    public void StartTimingPhase()
    {
        currentPhase = GamePhase.Timing;
        timingSystem?.StartTiming();
    }

    public void StartThrowPhase()
    {
        StartCoroutine(ThrowSequence());
    }

    IEnumerator ThrowSequence()
    {
        SetCamera(CameraMode.Game);

        yield return new WaitForSeconds(0.3f);

        currentPhase = GamePhase.Throw;

        float finalPower =
            throwController.basePower * 2 *
            chargePower *
            directionAccuracy +
            (timingAccuracy * 2f);

        // ボール生成
        GameObject ball = Instantiate(
            throwController.shotBallPrefab,
            throwController.spawnPoint.position,
            Quaternion.identity);

        currentBall = ball.transform;

        // 投擲
        ball.AddComponent<SimpleProjectile>()
            .Init(throwController.spawnPoint.forward, finalPower);

        currentPhase = GamePhase.WaitingLanding;

        yield return new WaitForSeconds(0.1f);

        // ★ここが重要：BallFollowに正しく接続
        ballFollowCamera.target = currentBall;

        SetCamera(CameraMode.BallFollow);
    }

    // =========================
    // Landing
    // =========================

    public void OnBallLanded(Vector3 landingPoint)
    {
        SetCamera(CameraMode.Main);

        currentBall = null;
        ballFollowCamera.target = null;

        Vector3 start = new Vector3(
            throwController.spawnPoint.position.x,
            0,
            throwController.spawnPoint.position.z);

        Vector3 end = new Vector3(
            landingPoint.x, 0,
            landingPoint.z);

        float distance = Vector3.Distance(start, end);

        float finalScore =
            weatherSystem != null
            ? weatherSystem.ApplyScore(distance)
            : distance;

        totalScore += finalScore;

        int index = currentThrow - 1;

        if (index >= 0 && index < throwScores.Length)
            throwScores[index] = finalScore;

        UpdateScoreUI();

        currentThrow++;

        if (currentThrow <= maxThrow)
        {
            StartChargePhase();
        }
        else
        {
            FinalScore = totalScore;
            SaveScore(totalScore);
            SceneManager.LoadScene("result Scene");
        }
    }

    // =========================
    // Camera
    // =========================

    public enum CameraMode
    {
        Main,
        Game,
        BallFollow
    }

    public CameraMode currentCameraMode;

    void SetCamera(CameraMode mode)
    {
        currentCameraMode = mode;

        if (mainCamera) mainCamera.enabled = false;
        if (gameCamera) gameCamera.enabled = false;
        if (ballFollowCamera) ballFollowCamera.enabled = false;

        switch (mode)
        {
            case CameraMode.Main:
                if (mainCamera) mainCamera.enabled = true;
                break;

            case CameraMode.Game:
                if (gameCamera) gameCamera.enabled = true;
                break;

            case CameraMode.BallFollow:
                if (ballFollowCamera) ballFollowCamera.enabled = true;
                break;
        }
    }

    // =========================
    // UI
    // =========================

    void UpdateScoreUI()
    {
        if (!scoreUI) return;

        string text = "";

        for (int i = 0; i < maxThrow; i++)
        {
            if (throwScores[i] > 0)
                text += $"{i + 1}投目 : {throwScores[i]:F1}\n";
        }

        text += $"\n合計 : {totalScore:F1}";
        scoreUI.text = text;
    }

    void UpdateWeatherUI()
    {
        if (!weatherUI || !weatherSystem) return;

        string name = weatherSystem.currentWeather switch
        {
            WeatherType.Sunny => "晴れ",
            WeatherType.Rain => "雨",
            WeatherType.Storm => "嵐",
            _ => "不明"
        };

        weatherUI.text = $"天候 : {name}";
    }

    // =========================
    // Save
    // =========================

    public void SaveScore(float score)
    {
        ScoreHistory.Add(score);
        ScoreHistory = ScoreHistory.OrderByDescending(x => x).Take(3).ToList();

        PlayerPrefs.SetString(SCORE_KEY, string.Join(",", ScoreHistory));
        PlayerPrefs.Save();
    }

    void LoadScores()
    {
        ScoreHistory.Clear();

        if (!PlayerPrefs.HasKey(SCORE_KEY)) return;

        foreach (var s in PlayerPrefs.GetString(SCORE_KEY).Split(','))
        {
            if (float.TryParse(s, out float v))
                ScoreHistory.Add(v);
        }

        ScoreHistory = ScoreHistory.OrderByDescending(x => x).ToList();
    }
}