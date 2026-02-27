using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class HandController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private bool isDragging;
    private Vector2 targetPosition;
    private Vector2 lastPointerPosition;

    [SerializeField] private float dragForce = 300f;
    [SerializeField] private float maxVelocity = 10f;
    private DistanceJoint2D joint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        joint = GetComponent<DistanceJoint2D>();
        joint.maxDistanceOnly = false;
    }

    public void BeginDrag(Vector2 startPosition)
    {
        isDragging = true;
        lastPointerPosition = startPosition;
    }

    public void UpdateDrag(Vector2 newPosition)
    {
        targetPosition = newPosition;
    }

    public void EndDrag()
    {
        isDragging = false;
    }

    public void BeginGrip()
    {
        sprite.color = Color.brown;
        rb.bodyType = RigidbodyType2D.Static;
    }

    public void EndGrip()
    {
        sprite.color = Color.white;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void FixedUpdate()
{
    if (!isDragging) return;

    Vector2 pointerDelta = (targetPosition - lastPointerPosition).normalized;
    Vector2 force = pointerDelta * dragForce;
    rb.AddForce(force, ForceMode2D.Force);

    if (rb.linearVelocity.magnitude > maxVelocity)
        rb.linearVelocity = rb.linearVelocity.normalized * maxVelocity;

    lastPointerPosition = targetPosition;
}
}