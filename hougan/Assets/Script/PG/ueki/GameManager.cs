using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // =========================
    // Singleton
    // =========================

    public static GameManager Instance;

    public InputManager InputManager;

    // =========================
    // Score
    // =========================

    public static float FinalScore;

    public static List<float> ScoreHistory =
        new List<float>();

    private const string SCORE_KEY =
        "ScoreHistory";

    // =========================
    // Phase
    // =========================

    [Header("現在フェーズ")]
    public GamePhase currentPhase;

    // =========================
    // Throw
    // =========================

    public int currentThrow = 1;

    public int maxThrow = 3;

    public float totalScore;

    private float[] throwScores;

    // =========================
    // UI
    // =========================

    [Header("UI")]
    public TMP_Text scoreUI;
    public TMP_Text weatherUI;

    // Directionゲージ用PanelまたはSlider
    public GameObject directionSlider;

    // =========================
    // Systems
    // =========================

    [Header("Systems")]
    public ChargeSystem chargeSystem;

    public DirectionSystem directionSystem;

    public TimingSystem timingSystem;

    public ThrowController throwController;

    public WeatherSystem weatherSystem;

    public RouletteController rouletteController;

    // =========================
    // Camera
    // =========================

    [Header("Cameras")]
    public Camera mainCamera;

    public Camera gameCamera;

    public BallFollowCamera ballFollowCamera;

    // =========================
    // Runtime
    // =========================

    [Header("Runtime")]
    public Transform currentBall;

    [HideInInspector]
    public float chargePower;

    [HideInInspector]
    public float directionAccuracy;

    [HideInInspector]
    public float timingAccuracy;

    // =========================
    // Awake
    // =========================

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        throwScores = new float[maxThrow];

        LoadScores();
    }

    // =========================
    // Start
    // =========================

    private void Start()
    {
        SetCamera(CameraMode.Main);

        UpdateScoreUI();

        UpdateWeatherUI();

        UpdatePhaseUI();

        StartChargePhase();
    }

    // =========================
    // Charge Phase
    // =========================

    public void StartChargePhase()
    {
        currentPhase = GamePhase.Charge;

        UpdatePhaseUI();

        SetCamera(CameraMode.Main);

        StartCoroutine(WeatherRouletteSequence());
    }

    // =========================
    // Roulette
    // =========================

    IEnumerator WeatherRouletteSequence()
    {
        // rouletteController未設定対策
        if (rouletteController == null)
        {
            Debug.LogWarning(
                "RouletteControllerが未設定です");

            weatherSystem.DecideWeather();

            UpdateWeatherUI();

            chargeSystem?.StartCharge();

            yield break;
        }

        // ルーレット終了待ち
        yield return StartCoroutine(
            rouletteController.SpinRoulette()
        );

        // 結果反映
        switch (rouletteController.Result)
        {
            case 0:

                weatherSystem.currentWeather =
                    WeatherType.Sunny;

                break;

            case 1:

                weatherSystem.currentWeather =
                    WeatherType.Rain;

                break;

            case 2:

                weatherSystem.currentWeather =
                    WeatherType.Storm;

                break;
        }

        UpdateWeatherUI();

        // 次フェーズ
        if (chargeSystem != null)
        {
            chargeSystem.StartCharge();
        }
    }

    // =========================
    // Direction Phase
    // =========================

    public void StartDirectionPhase()
    {
        currentPhase = GamePhase.Direction;

        UpdatePhaseUI();

        SetCamera(CameraMode.Main);

        if (directionSystem != null)
        {
            directionSystem.StartDirection();
        }
    }

    // =========================
    // Timing Phase
    // =========================

    public void StartTimingPhase()
    {
        currentPhase = GamePhase.Timing;

        UpdatePhaseUI();

        SetCamera(CameraMode.Main);

        if (timingSystem != null)
        {
            timingSystem.StartTiming();
        }
    }

    // =========================
    // Throw Phase
    // =========================

    public void StartThrowPhase()
    {
        StartCoroutine(ThrowSequence());
    }

    IEnumerator ThrowSequence()
    {
        SetCamera(CameraMode.Game);

        yield return new WaitForSeconds(0.2f);

        currentPhase =
            GamePhase.Throw;

        UpdatePhaseUI();

        // 投擲パワー
        float finalPower =
            throwController.basePower * 3f *
            chargePower * 3f *
            directionAccuracy * 2f +
            (timingAccuracy * 2f);

        // ボール生成
        GameObject ball =
            Instantiate(
                throwController.shotBallPrefab,
                throwController.spawnPoint.position,
                Quaternion.identity);

        currentBall = ball.transform;

        InputManager.Instance.AnimationMove();

        // Projectile
        SimpleProjectile projectile =
            ball.AddComponent<SimpleProjectile>();

        projectile.Init(
            throwController.spawnPoint.forward,
            finalPower);



        currentPhase =
            GamePhase.WaitingLanding;

        UpdatePhaseUI();

        yield return new WaitForSeconds(0.1f);

        // Follow Camera
        if (ballFollowCamera != null)
        {
            ballFollowCamera.target =
                currentBall;
        }

        SetCamera(CameraMode.BallFollow);
    }

    // =========================
    // Landing
    // =========================

    public void OnBallLanded(
        Vector3 landingPoint)
    {
        SetCamera(CameraMode.Main);

        // Follow解除
        if (ballFollowCamera != null)
        {
            ballFollowCamera.target =
                null;
        }

        currentBall = null;

        Vector3 start =
            new Vector3(
                throwController.spawnPoint.position.x,
                0,
                throwController.spawnPoint.position.z);

        Vector3 end =
            new Vector3(
                landingPoint.x,
                0,
                landingPoint.z);

        float distance =
            Vector3.Distance(start, end);

        float finalScore =
            distance;

        if (weatherSystem != null)
        {
            finalScore =
                weatherSystem.ApplyScore(
                    distance);
        }

        totalScore += finalScore;

        int index =
            currentThrow - 1;

        if (index >= 0 &&
            index < throwScores.Length)
        {
            throwScores[index] =
                finalScore;
        }

        UpdateScoreUI();

        currentThrow++;

        // 次投
        if (currentThrow <= maxThrow)
        {
            StartChargePhase();
        }
        else
        {
            FinalScore = totalScore;

            SaveScore(totalScore);

            SceneManager.LoadScene(
                "result Scene");
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

        // BallFollow側のCamera取得
        Camera followCam = null;

        if (ballFollowCamera != null)
        {
            followCam =
                ballFollowCamera
                .GetComponent<Camera>();
        }

        // 全OFF
        if (mainCamera != null)
        {
            mainCamera.enabled = false;
        }

        if (gameCamera != null)
        {
            gameCamera.enabled = false;
        }

        if (followCam != null)
        {
            followCam.enabled = false;
        }

        // 必要だけON
        switch (mode)
        {
            case CameraMode.Main:

                if (mainCamera != null)
                {
                    mainCamera.enabled = true;
                }

                break;

            case CameraMode.Game:

                if (gameCamera != null)
                {
                    gameCamera.enabled = true;
                }

                break;

            case CameraMode.BallFollow:

                if (followCam != null)
                {
                    followCam.enabled = true;
                }

                break;
        }
    }

    // =========================
    // UI
    // =========================

    void UpdateScoreUI()
    {
        if (scoreUI == null)
            return;

        string text = "";

        for (int i = 0;
             i < maxThrow;
             i++)
        {
            if (throwScores[i] > 0)
            {
                text +=
                    $"{i + 1}投目 : " +
                    $"{throwScores[i]:F1}\n";
            }
        }

        text +=
            $"\n合計 : {totalScore:F1}";

        scoreUI.text = text;
    }

    void UpdateWeatherUI()
    {
        if (weatherUI == null ||
            weatherSystem == null)
        {
            return;
        }

        string name =
            weatherSystem.currentWeather switch
            {
                WeatherType.Sunny => "晴れ",
                WeatherType.Rain => "雨",
                WeatherType.Storm => "嵐",
                _ => "不明"
            };

        weatherUI.text =
            $"天候 : {name}";
    }

    void UpdatePhaseUI()
    {
        if (directionSlider != null)
        {
            directionSlider.SetActive(
                currentPhase == GamePhase.Direction
            );
        }
    }

    // =========================
    // Save
    // =========================

    public void SaveScore(float score)
    {
        ScoreHistory.Add(score);

        ScoreHistory =
            ScoreHistory
            .OrderByDescending(x => x)
            .Take(3)
            .ToList();

        PlayerPrefs.SetString(
            SCORE_KEY,
            string.Join(",", ScoreHistory));

        PlayerPrefs.Save();
    }

    void LoadScores()
    {
        ScoreHistory.Clear();

        if (!PlayerPrefs.HasKey(
            SCORE_KEY))
        {
            return;
        }

        foreach (string s in
                 PlayerPrefs
                 .GetString(SCORE_KEY)
                 .Split(','))
        {
            if (float.TryParse(
                s,
                out float value))
            {
                ScoreHistory.Add(value);
            }
        }

        ScoreHistory =
            ScoreHistory
            .OrderByDescending(x => x)
            .ToList();
    }
}