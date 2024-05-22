using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;



    [HideInInspector] public static bool JumpPressed;
    [HideInInspector] public static bool JumpHolded;
    [HideInInspector] public static bool Right;
    [HideInInspector] public static bool Left;
    [HideInInspector] public static bool Dash;
    [HideInInspector] public static bool Attack;
    [HideInInspector] public static bool Throw;
    [HideInInspector] public static bool Grapple;

    [HideInInspector] public static Vector2 mousePosition;

    [SerializeField] KeyCode JumpKey = KeyCode.Space;
    [SerializeField] KeyCode RightKey = KeyCode.D;
    [SerializeField] KeyCode LeftKey = KeyCode.A;
    [SerializeField] KeyCode DashKey = KeyCode.LeftShift;
    [SerializeField] KeyCode AttackKey = KeyCode.Q;
    [SerializeField] KeyCode ThrowKey = KeyCode.R;
    [SerializeField] KeyCode GrappleKey = KeyCode.F;

    public Camera mainCamera;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        JumpPressed = Input.GetKeyDown(JumpKey);
        JumpHolded = Input.GetKey(JumpKey);
        Right = Input.GetKey(RightKey);
        Left = Input.GetKey(LeftKey);
        Dash = Input.GetKeyDown(DashKey);
        Attack = Input.GetKeyDown(AttackKey);
        Throw = Input.GetKeyDown(ThrowKey);
        Grapple = Input.GetKeyDown(GrappleKey);

        mousePosition = Input.mousePosition;
    }

    public static Vector2 getAimDirection(Vector3 playerPosition)
    {
        Vector2 screenPosition = Instance.mainCamera.WorldToScreenPoint(playerPosition);
        return (mousePosition - screenPosition).normalized;
    }
}
