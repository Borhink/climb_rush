using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static InputHandler Instance { get; private set; }

    private InputSystem_Actions input;

    public Vector2 PointerScreenPosition { get; private set; }
    public bool IsPressed { get; private set; }
    public bool Clicked { get; private set; }
    public bool Released { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        input = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        input.Enable();

        input.Player.Tracking.performed += OnTracking;
        input.Player.Clicking.started += OnClickStarted;
        input.Player.Clicking.canceled += OnClickCanceled;
    }

    private void OnDisable()
    {
        input.Player.Tracking.performed -= OnTracking;
        input.Player.Clicking.started -= OnClickStarted;
        input.Player.Clicking.canceled -= OnClickCanceled;

        input.Disable();
    }

    private void OnTracking(InputAction.CallbackContext ctx)
    {
        PointerScreenPosition = ctx.ReadValue<Vector2>();
    }

    private void OnClickStarted(InputAction.CallbackContext ctx)
    {
        IsPressed = true;
        Clicked = true;
    }

    private void OnClickCanceled(InputAction.CallbackContext ctx)
    {
        IsPressed = false;
        Released = true;
    }

    private void LateUpdate()
    {
        Clicked = false;
        Released = false;
    }
}
