using UnityEngine;

namespace ClimbRush.Gameplay.World
{
    /// <summary>
    /// A climbable hold that hands can grab.
    /// </summary>
    public class ClimbHold : MonoBehaviour
    {
        [SerializeField] private float gripRadius = 0.3f;
        [SerializeField] private bool isOccupied;
        
        [SerializeField] private SpriteRenderer visual;
        [SerializeField] private Color availableColor = Color.white;
        [SerializeField] private Color occupiedColor = Color.gray;

        public float GripRadius => gripRadius;
        public Rigidbody2D Rb { get; private set; }
        public bool IsOccupied => isOccupied;

        private void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            UpdateVisual();
        }

        public bool CanGrip() => !isOccupied;

        public void SetOccupied(bool occupied)
        {
            isOccupied = occupied;
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (visual != null)
                visual.color = isOccupied ? occupiedColor : availableColor;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, gripRadius);
        }
    }
}
