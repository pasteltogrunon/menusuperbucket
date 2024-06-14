using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public static bool CinematicInputsLocked;

    public static bool JumpPressed { get => (Input.GetKeyDown(Instance.JumpKey) || Input.GetKeyDown(Instance.JumpButton)) && !CinematicInputsLocked; }
    public static bool JumpHolded { get => (Input.GetKey(Instance.JumpKey) || Input.GetKey(Instance.JumpButton)) && !CinematicInputsLocked; }
    public static bool JumpUp { get => (Input.GetKeyUp(Instance.JumpKey) || Input.GetKeyUp(Instance.JumpButton)) && !CinematicInputsLocked; }
    public static bool Right { get => (Instance.RightAxis()) && !CinematicInputsLocked; }
    public static bool Left { get => (Instance.LeftAxis()) && !CinematicInputsLocked; }
    public static bool Dash { get => (Input.GetKeyDown(Instance.DashKey) || Input.GetKeyDown(Instance.DashButton)) && !CinematicInputsLocked; }
    public static bool Attack { get => (Input.GetKeyDown(Instance.AttackKey) || Input.GetKeyDown(Instance.AttackButton)) && !CinematicInputsLocked; }
    public static bool StrongAttack { get => (Input.GetKeyDown(Instance.StrongAttackKey) || Input.GetKeyDown(Instance.StrongAttackButton)) && !CinematicInputsLocked; }
    public static bool Throw { get => (Input.GetKeyDown(Instance.ThrowKey) || Input.GetKeyDown(Instance.ThrowButton)) && !CinematicInputsLocked; }
    public static bool Grapple { get => (Input.GetKeyDown(Instance.GrappleKey) || Input.GetKeyDown(Instance.GrappleButton)) && !CinematicInputsLocked; }
    public static bool Interact { get => (Input.GetKeyDown(Instance.InteractKey) || Input.GetKeyDown(Instance.InteractButton)) && !CinematicInputsLocked; }
    public static bool Platform { get => (Input.GetKeyDown(Instance.PlatformKey) || Input.GetKeyDown(Instance.PlatformButton)) && !CinematicInputsLocked; }
    public static bool EnterLine { get => Input.GetKeyDown(Instance.EnterLineKey) || Input.GetKeyDown(Instance.EnterLineButton); }
    public static bool Pause { get => Input.GetKeyDown(Instance.PauseMenuKey); }

    public static float Deadzone { get => Instance.DeadzoneValue; }
    public static float HorizontalAxis { get => CinematicInputsLocked || Mathf.Abs(Input.GetAxisRaw("Horizontal")) < Deadzone ? 0 : Input.GetAxisRaw("Horizontal"); }
    public static float VerticalAxis { get => CinematicInputsLocked || Mathf.Abs(Input.GetAxisRaw("Vertical")) < Deadzone ? 0 : Input.GetAxisRaw("Vertical"); }


    [HideInInspector] public static Vector2 mousePosition;

    /* Perdon por cambiar los controles del teclao uwu,
    a mi me gustan mas los nuevos para poder usar las dos manos comodamente,
    pero por si quieres tirar patras, te dejo los antiwos uwuw

    [SerializeField] KeyCode JumpKey = KeyCode.Space;
    [SerializeField] KeyCode RightKey = KeyCode.D;
    [SerializeField] KeyCode LeftKey = KeyCode.A;
    [SerializeField] KeyCode DashKey = KeyCode.LeftShift;
    [SerializeField] KeyCode AttackKey = KeyCode.Q;
    [SerializeField] KeyCode StrongAttackKey = KeyCode.W;
    [SerializeField] KeyCode ThrowKey = KeyCode.R;
    [SerializeField] KeyCode GrappleKey = KeyCode.F;
    [SerializeField] KeyCode InteractKey = KeyCode.E;
    [SerializeField] KeyCode EnterLineKey = KeyCode.Return;
    */

    [SerializeField] KeyCode JumpKey = KeyCode.Space;
    [SerializeField] KeyCode DashKey = KeyCode.LeftShift;
    [SerializeField] KeyCode AttackKey = KeyCode.J;
    [SerializeField] KeyCode StrongAttackKey = KeyCode.K;
    [SerializeField] KeyCode ThrowKey = KeyCode.H;
    [SerializeField] KeyCode GrappleKey = KeyCode.L;
    [SerializeField] KeyCode InteractKey = KeyCode.F;
    [SerializeField] KeyCode EnterLineKey = KeyCode.Return;
    [SerializeField] KeyCode PlatformKey = KeyCode.U;
    [SerializeField] KeyCode PauseMenuKey = KeyCode.Escape;

    [SerializeField] KeyCode JumpButton = KeyCode.JoystickButton0;
    [SerializeField] KeyCode DashButton = KeyCode.JoystickButton1;
    [SerializeField] KeyCode AttackButton = KeyCode.JoystickButton2;
    [SerializeField] KeyCode StrongAttackButton = KeyCode.JoystickButton3;
    [SerializeField] KeyCode ThrowButton = KeyCode.JoystickButton4;
    [SerializeField] KeyCode GrappleButton = KeyCode.JoystickButton5;
    [SerializeField] KeyCode InteractButton = KeyCode.JoystickButton9;
    [SerializeField] KeyCode EnterLineButton = KeyCode.JoystickButton6;
    [SerializeField] KeyCode PlatformButton = KeyCode.JoystickButton10;
    [Range(0.0f, 1.0f)] [SerializeField] float DeadzoneValue = 0.1f;

    private bool RightAxis()
    {
        return Input.GetAxisRaw("Horizontal") > DeadzoneValue;
    }

    private bool LeftAxis()
    {
        return Input.GetAxisRaw("Horizontal") < -DeadzoneValue;
    }

    public Camera mainCamera;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {

    }

    public static Vector2 getAimDirection(Vector3 playerPosition)
    {
        mousePosition = Input.mousePosition;
        Vector2 screenPosition = Instance.mainCamera.WorldToScreenPoint(playerPosition);
        return (mousePosition - screenPosition).normalized;
    }
}
