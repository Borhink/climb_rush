using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private HandController leftHand;
    [SerializeField] private HandController rightHand;

    private HandController draggedHand;
    private HandController grippedHand;

    private void Start()
    {
        draggedHand = leftHand;
        grippedHand = rightHand;
        grippedHand.BeginGrip();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        Vector2 worldPos = mainCamera.ScreenToWorldPoint(InputHandler.Instance.PointerScreenPosition);

        if (InputHandler.Instance.Clicked)
        {
            draggedHand.BeginDrag(worldPos);
            grippedHand.BeginGrip();
        }

        if (InputHandler.Instance.IsPressed)
            draggedHand.UpdateDrag(worldPos);

        if (InputHandler.Instance.Released)
        {
            draggedHand.EndDrag();
            grippedHand.EndGrip();
            SwitchHand();
        }
    }

    private void SwitchHand()
    {
        draggedHand = draggedHand == leftHand ? rightHand : leftHand;
        grippedHand = grippedHand == leftHand ? rightHand : leftHand;
    }
}