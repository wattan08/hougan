using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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

    [Header("システム")]
    public ChargeSystem chargeSystem;
    public DirectionSystem directionSystem;
    public TimingSystem timingSystem;
    public ThrowController throwController;
    public WeatherSystem weatherSystem;

    // ===== 各フェーズ結果 =====

    [HideInInspector]
    public float chargePower;

    [HideInInspector]
    public float directionAccuracy;

    [HideInInspector]
    public float timingAccuracy;

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
        }
    }

    private void Start()
    {
        StartChargePhase();
    }

    //==================================================
    // Charge Phase
    //==================================================

    public void StartChargePhase()
    {
      
            //==============================
            // 天候決定
            //==============================

            weatherSystem.DecideWeather();

            //==============================
            // Charge開始
            //==============================

            currentPhase = GamePhase.Charge;

            Debug.Log(
                $"----- {currentThrow}投目 -----");

            Debug.Log("チャージ開始");

            chargeSystem.StartCharge();
      
    }
    //==================================================
    //Direction Phase
    //==================================================
    public void StartDirectionPhase()
    {
        currentPhase = GamePhase.Direction;

        Debug.Log("方向決定開始");

        directionSystem.StartDirection();
    }
    //==================================================
    // Timing Phase
    //==================================================

    public void StartTimingPhase()
    {
        currentPhase = GamePhase.Timing;

        Debug.Log("タイミング開始");

        timingSystem.StartTiming();
    }
    //==================================================
    // Throw Phase
    //==================================================

    public void StartThrowPhase()
    {
        currentPhase = GamePhase.Throw;

        Debug.Log("投擲開始");

        // 最終パワー計算
        float finalPower =
      throwController.basePower
      * chargePower
      * directionAccuracy
      + (timingAccuracy * 2f);

        Debug.Log($"最終パワー : {finalPower}");

        // 投擲
        throwController.ThrowBall(finalPower);
        Debug.Log("投擲");

        // 着地待機
        currentPhase = GamePhase.WaitingLanding;
    }

    //==================================================
    // Landing
    //==================================================

    public void OnBallLanded(Vector3 landingPoint)
    {
        // XZ平面距離
        Vector3 start =
            new Vector3(
                throwStartPoint.position.x,
                0f,
                throwStartPoint.position.z);

        Vector3 end =
            new Vector3(
                landingPoint.x,
                0f,
                landingPoint.z);

        // 飛距離計測
        float distance =
            Vector3.Distance(start, end);

        // スコア加算
        float finalScore =
    weatherSystem.ApplyScore(
        distance);

        totalScore += finalScore;

        //==============================
        // Debug表示
        //==============================

        Debug.Log(
            $"今回の飛距離 : {distance:F2}m");

        Debug.Log(
            $"現在合計スコア : {totalScore:F2}m");

        //==============================
        // 次投擲
        //==============================

        currentThrow++;

        // 3投未満
        if (currentThrow <= maxThrow)
        {
            StartChargePhase();
        }
        else
        {
            currentPhase = GamePhase.Result;

            Debug.Log("ゲーム終了");

            Debug.Log(
                $"最終スコア : {totalScore:F2}m");
        }
    }
}