using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private InputActions inputActions;

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
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.ChargeButton.performed += OnCharge;

        inputActions.Player.Timing_Cross.performed += OnCross;
    }

    private void OnDisable()
    {
        inputActions.Player.ChargeButton.performed -= OnCharge;

        inputActions.Player.Timing_Cross.performed -= OnCross;

        inputActions.Disable();
    }

    //==================================================
    // Charge
    //==================================================

    private void OnCharge(InputAction.CallbackContext ctx)
    {
        if (GameManager.Instance.currentPhase
            != GamePhase.Charge)
            return;

        chargeSystem.AddCharge();
    }

    //==================================================
    // Cross
    //==================================================

    private void OnCross(InputAction.CallbackContext ctx)
    {
        Debug.Log(
            $"現在フェーズ : "
            + $"{GameManager.Instance.currentPhase}");

        //==============================
        // Direction
        //==============================

        if (GameManager.Instance.currentPhase
            == GamePhase.Direction)
        {
            directionSystem.ConfirmDirection();

            return;
        }

        //==============================
        // Timing
        //==============================

        if (GameManager.Instance.currentPhase
            == GamePhase.Timing)
        {
            timingSystem.PressButton(
                TimingButtonType.Cross);
        }
    }
}