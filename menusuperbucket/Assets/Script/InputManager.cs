using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;



    public static bool JumpPressed { get => Input.GetKeyDown(Instance.JumpKey); }
    public static bool JumpHolded { get => Input.GetKey(Instance.JumpKey); }
    public static bool Right { get => Input.GetKey(Instance.RightKey); }
    public static bool Left { get => Input.GetKey(Instance.LeftKey); }
    public static bool Dash { get => Input.GetKeyDown(Instance.DashKey); }
    public static bool Attack { get => Input.GetKeyDown(Instance.AttackKey); }
    public static bool Throw { get => Input.GetKeyDown(Instance.ThrowKey); }
    public static bool Grapple { get => Input.GetKeyDown(Instance.GrappleKey); }
    public static bool EnterLine { get => Input.GetKeyDown(Instance.EnterLineKey); }

    [HideInInspector] public static Vector2 mousePosition;

    [SerializeField] KeyCode JumpKey = KeyCode.Space;
    [SerializeField] KeyCode RightKey = KeyCode.D;
    [SerializeField] KeyCode LeftKey = KeyCode.A;
    [SerializeField] KeyCode DashKey = KeyCode.LeftShift;
    [SerializeField] KeyCode AttackKey = KeyCode.Q;
    [SerializeField] KeyCode ThrowKey = KeyCode.R;
    [SerializeField] KeyCode GrappleKey = KeyCode.F;
    [SerializeField] KeyCode EnterLineKey = KeyCode.Return;

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
