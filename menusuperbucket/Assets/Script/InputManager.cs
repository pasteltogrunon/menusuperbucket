using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;



    public static bool JumpPressed { get => Input.GetKeyDown(Instance.JumpKey) || Input.GetKeyDown(Instance.JumpButton); }
    public static bool JumpHolded { get => Input.GetKey(Instance.JumpKey) || Input.GetKey(Instance.JumpButton); }
    public static bool JumpUp { get => Input.GetKeyUp(Instance.JumpKey) || Input.GetKeyUp(Instance.JumpButton); }
    public static bool Right { get => Input.GetKey(Instance.RightKey) || Instance.RightAxis(); }
    public static bool Left { get => Input.GetKey(Instance.LeftKey) || Instance.LeftAxis(); }
    public static bool Dash { get => Input.GetKeyDown(Instance.DashKey) || Input.GetKeyDown(Instance.DashButton); }
    public static bool Attack { get => Input.GetKeyDown(Instance.AttackKey) || Input.GetKeyDown(Instance.AttackButton); }
    public static bool StrongAttack { get => Input.GetKeyDown(Instance.StrongAttackKey) || Input.GetKeyDown(Instance.StrongAttackButton); }
    public static bool Throw { get => Input.GetKeyDown(Instance.ThrowKey) || Input.GetKeyDown(Instance.ThrowButton); }
    public static bool Grapple { get => Input.GetKeyDown(Instance.GrappleKey) || Input.GetKeyDown(Instance.GrappleButton); }
    public static bool Interact { get => Input.GetKeyDown(Instance.InteractKey) || Input.GetKeyDown(Instance.InteractButton); }
    public static bool EnterLine { get => Input.GetKeyDown(Instance.EnterLineKey) || Input.GetKeyDown(Instance.EnterLineButton); }
    public static float Deadzone { get => Instance.DeadzoneValue; }

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
    [SerializeField] KeyCode RightKey = KeyCode.D;
    [SerializeField] KeyCode LeftKey = KeyCode.A;
    [SerializeField] KeyCode DashKey = KeyCode.LeftShift;
    [SerializeField] KeyCode AttackKey = KeyCode.J;
    [SerializeField] KeyCode StrongAttackKey = KeyCode.K;
    [SerializeField] KeyCode ThrowKey = KeyCode.H;
    [SerializeField] KeyCode GrappleKey = KeyCode.L;
    [SerializeField] KeyCode InteractKey = KeyCode.F;
    [SerializeField] KeyCode EnterLineKey = KeyCode.Return;

    [SerializeField] KeyCode JumpButton = KeyCode.JoystickButton0;
    [SerializeField] KeyCode DashButton = KeyCode.JoystickButton1;
    [SerializeField] KeyCode AttackButton = KeyCode.JoystickButton2;
    [SerializeField] KeyCode StrongAttackButton = KeyCode.JoystickButton3;
    [SerializeField] KeyCode ThrowButton = KeyCode.JoystickButton4;
    [SerializeField] KeyCode GrappleButton = KeyCode.JoystickButton5;
    [SerializeField] KeyCode InteractButton = KeyCode.JoystickButton9;
    [SerializeField] KeyCode EnterLineButton = KeyCode.JoystickButton6;
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
