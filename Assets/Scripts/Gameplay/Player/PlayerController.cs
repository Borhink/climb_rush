using UnityEngine;
using ClimbRush.Input;
using ClimbRush.Gameplay.World;

namespace ClimbRush.Gameplay.Player
{
    /// <summary>
    /// Main player controller - handles body, hands, and climbing mechanics.
    /// Simple drag-and-drop hand system for mobile climbing.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Body")]
        [SerializeField] private Rigidbody2D bodyRb;
        
        [Header("Hands")]
        [SerializeField] private Rigidbody2D leftHand;
        [SerializeField] private Rigidbody2D rightHand;
        [SerializeField] private float handMoveSpeed = 15f;
        
        [Header("Hand Visuals")]
        [SerializeField] private SpriteRenderer leftHandVisual;
        [SerializeField] private SpriteRenderer rightHandVisual;
        
        [Header("References")]
        [SerializeField] private InputHandler inputHandler;
        [SerializeField] private LayerMask holdLayer;
        
        [Header("Settings")]
        [SerializeField] private float gripSearchRadius = 1f;
        [SerializeField] private float bodyHandDistance = 1.5f;

        private enum Hand { Left, Right }
        private Hand activeHand = Hand.Left;
        
        private bool leftGripping;
        private bool rightGripping;
        private ClimbHold leftHold;
        private ClimbHold rightHold;
        private FixedJoint2D leftGripJoint;
        private FixedJoint2D rightGripJoint;
        private DistanceJoint2D leftBodyJoint;
        private DistanceJoint2D rightBodyJoint;

        private Vector2 dragTarget;

        public Rigidbody2D Body => bodyRb;
        public Rigidbody2D LeftHand => leftHand;
        public Rigidbody2D RightHand => rightHand;

        private void Awake()
        {
            CreateBodyJoints();
        }

        private void Start()
        {
            if (inputHandler == null)
                inputHandler = GetComponent<InputHandler>();
            
            inputHandler.OnDragStart += OnDragStart;
            inputHandler.OnDragUpdate += OnDragUpdate;
            inputHandler.OnDragEnd += OnDragEnd;
        }

        private void OnDestroy()
        {
            if (inputHandler != null)
            {
                inputHandler.OnDragStart -= OnDragStart;
                inputHandler.OnDragUpdate -= OnDragUpdate;
                inputHandler.OnDragEnd -= OnDragEnd;
            }
        }

        private void CreateBodyJoints()
        {
            // Left hand to body
            leftBodyJoint = gameObject.AddComponent<DistanceJoint2D>();
            leftBodyJoint.connectedBody = leftHand;
            leftBodyJoint.distance = bodyHandDistance;
            leftBodyJoint.frequency = 8f;
            leftBodyJoint.dampingRatio = 0.5f;
            leftBodyJoint.maximumForce = 100f;

            // Right hand to body
            rightBodyJoint = gameObject.AddComponent<DistanceJoint2D>();
            rightBodyJoint.connectedBody = rightHand;
            rightBodyJoint.distance = bodyHandDistance;
            rightBodyJoint.frequency = 8f;
            rightBodyJoint.dampingRatio = 0.5f;
            rightBodyJoint.maximumForce = 100f;
        }

        private void OnDragStart(Vector2 position)
        {
            dragTarget = position;
            
            // Release grip on active hand if gripping
            if (activeHand == Hand.Left && leftGripping)
                ReleaseGrip(Hand.Left);
            else if (activeHand == Hand.Right && rightGripping)
                ReleaseGrip(Hand.Right);
        }

        private void OnDragUpdate(Vector2 position)
        {
            dragTarget = position;
        }

        private void OnDragEnd(Vector2 position)
        {
            // Try to grip with active hand
            bool gripped = TryGrip(activeHand, position);
            
            // If grip succeeds, release opposite hand
            if (gripped)
            {
                Hand opposite = activeHand == Hand.Left ? Hand.Right : Hand.Left;
                if (opposite == Hand.Left && leftGripping)
                    ReleaseGrip(Hand.Left);
                else if (opposite == Hand.Right && rightGripping)
                    ReleaseGrip(Hand.Right);
            }
            
            // Switch active hand
            activeHand = activeHand == Hand.Left ? Hand.Right : Hand.Left;
        }

        private bool TryGrip(Hand hand, Vector2 position)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(position, gripSearchRadius, holdLayer);
            
            foreach (var hit in hits)
            {
                ClimbHold hold = hit.GetComponent<ClimbHold>();
                if (hold != null && hold.CanGrip())
                {
                    if (hand == Hand.Left)
                    {
                        leftGripJoint = leftHand.gameObject.AddComponent<FixedJoint2D>();
                        leftGripJoint.connectedBody = hold.Rb;
                        leftGripJoint.breakForce = 100f;
                        leftHold = hold;
                        leftHold.SetOccupied(true);
                        leftGripping = true;
                        UpdateHandVisual(Hand.Left, true);
                    }
                    else
                    {
                        rightGripJoint = rightHand.gameObject.AddComponent<FixedJoint2D>();
                        rightGripJoint.connectedBody = hold.Rb;
                        rightGripJoint.breakForce = 100f;
                        rightHold = hold;
                        rightHold.SetOccupied(true);
                        rightGripping = true;
                        UpdateHandVisual(Hand.Right, true);
                    }
                    return true;
                }
            }
            return false;
        }

        private void ReleaseGrip(Hand hand)
        {
            if (hand == Hand.Left && leftGripping)
            {
                if (leftGripJoint != null) { Destroy(leftGripJoint); leftGripJoint = null; }
                if (leftHold != null) { leftHold.SetOccupied(false); leftHold = null; }
                leftGripping = false;
                UpdateHandVisual(Hand.Left, false);
            }
            else if (hand == Hand.Right && rightGripping)
            {
                if (rightGripJoint != null) { Destroy(rightGripJoint); rightGripJoint = null; }
                if (rightHold != null) { rightHold.SetOccupied(false); rightHold = null; }
                rightGripping = false;
                UpdateHandVisual(Hand.Right, false);
            }
        }

        private void FixedUpdate()
        {
            // Move active hand towards drag target
            Rigidbody2D activeRb = activeHand == Hand.Left ? leftHand : rightHand;
            
            if (!IsHandGripping(activeHand))
            {
                Vector2 dir = dragTarget - activeRb.position;
                activeRb.velocity = dir * handMoveSpeed;
            }
        }

        private bool IsHandGripping(Hand hand)
        {
            return hand == Hand.Left ? leftGripping : rightGripping;
        }

        private void UpdateHandVisual(Hand hand, bool gripping)
        {
            SpriteRenderer visual = hand == Hand.Left ? leftHandVisual : rightHandVisual;
            if (visual != null)
                visual.color = gripping ? Color.green : Color.white;
        }
    }
}
