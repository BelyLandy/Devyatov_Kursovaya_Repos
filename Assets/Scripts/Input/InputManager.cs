using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using System.Collections;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Input-startup settings")]
    [Tooltip("Задержка (сек) перед тем, как включить все действия ввода")]
    [SerializeField] private float startupDelay = 0f;

    private string controlsScheme;
    public PlayerControls playerInput;

    private InputAction move;
    private InputAction punch;
    private InputAction defend;
    private InputAction grab;
    private InputAction jump;

    #region MonoBehaviour lifecycle
    private void Awake()
    {
        playerInput   = new PlayerControls();
        controlsScheme = playerInput.ToString();

        if (Instance == null) Instance = this;
        else
        {
            Debug.LogWarning("Multiple InputManagers found in this scene, there can be only one.");
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;

        move   = playerInput.Player.Move;
        punch  = playerInput.Player.Punch;
        defend = playerInput.Player.Defend;
        grab   = playerInput.Player.Grab;
        jump   = playerInput.Player.Jump;


        if (startupDelay > 0f)
            StartCoroutine(EnableInputDelayed());
        else
            EnableInput();
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
        DisableInput();
    }
    #endregion

    #region Input activation helpers
    private IEnumerator EnableInputDelayed()
    {
        yield return new WaitForSeconds(startupDelay);
        EnableInput();
    }

    private void EnableInput()
    {
        move.Enable();
        punch.Enable();
        defend.Enable();
        grab.Enable();
        jump.Enable();
    }

    private void DisableInput()
    {
        move.Disable();
        punch.Disable();
        defend.Disable();
        grab.Disable();
        jump.Disable();
    }
    #endregion

    #region Public query methods
    public static bool PunchKeyDown()  => Instance.punch.WasPressedThisFrame();
    public static bool DefendKeyDown() => Instance.defend.IsPressed();
    public static bool GrabKeyDown()   => Instance.grab.WasPressedThisFrame();
    public static bool JumpKeyDown()   => Instance.jump.WasPressedThisFrame();
    public static Vector2 GetInputVector() => Instance.move.ReadValue<Vector2>();
    public static bool JoypadDirInputDetected() =>
        Instance.move.ReadValue<Vector2>() != Vector2.zero;
    #endregion

    #region Device change handler
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added)
        {
            InputUser.PerformPairingWithDevice(device,
                                               InputUser.all[0],
                                               InputUserPairingOptions.ForceNoPlatformUserAccountSelection);
        }
    }
    #endregion
}
