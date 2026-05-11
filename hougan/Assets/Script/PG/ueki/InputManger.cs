using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private InputActions inputActions;
    private ThrowController throwController;
    private ChargeSystem chargeSystem;
    private DirectionSystem directionSystem;
    private TimingSystem timingSystem;

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

        inputActions = new InputActions();

        chargeSystem =
            FindObjectOfType<ChargeSystem>();

        directionSystem =
            FindObjectOfType<DirectionSystem>();

        timingSystem =
            FindObjectOfType<TimingSystem>();

        throwController = FindObjectOfType<ThrowController>();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.ChargeButton.performed += OnCharge;
        inputActions.Player.Timing_Circle.performed += OnTimingCross;
    }

    private void OnDisable()
    {
        inputActions.Player.ChargeButton.performed -= OnCharge;
        inputActions.Player.Timing_Circle.performed -= OnTimingCross;

        inputActions.Disable();
    }

    private void OnCharge(InputAction.CallbackContext ctx)
    {
            // Chargeフェーズ以外無効
            if (GameManager.Instance.currentPhase
                != GamePhase.Charge)
                return;

            // 連打加算
            chargeSystem.AddCharge();
    }

    private void OnTimingCross(InputAction.CallbackContext ctx)
    {
        Debug.Log("投擲！");
    }
}