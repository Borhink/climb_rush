using UnityEngine;
using System;

namespace ClimbRush.Input
{
    /// <summary>
    /// Simple unified input for mouse and touch.
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        public event Action<Vector2> OnDragStart;
        public event Action<Vector2> OnDragUpdate;
        public event Action<Vector2> OnDragEnd;
        public event Action<Vector2> OnTap;

        [SerializeField] private Camera mainCamera;
        [SerializeField] private float dragThreshold = 0.2f;

        private Vector2 startPosition;
        private Vector2 currentPosition;
        private bool isDragging;
        private bool inputEnabled = true;

        public bool IsDragging => isDragging;
        public Vector2 CurrentPosition => currentPosition;

        private void Awake()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
        }

        public void SetInputEnabled(bool enabled)
        {
            inputEnabled = enabled;
            if (!enabled && isDragging)
            {
                isDragging = false;
            }
        }

        private void Update()
        {
            if (!inputEnabled) return;

#if UNITY_STANDALONE || UNITY_EDITOR
            HandleMouse();
#else
            HandleTouch();
#endif
        }

        private void HandleMouse()
        {
            if (Input.GetMouseButtonDown(0))
            {
                startPosition = GetWorldPos(Input.mousePosition);
                currentPosition = startPosition;
                isDragging = true;
                OnDragStart?.Invoke(currentPosition);
            }
            else if (Input.GetMouseButton(0) && isDragging)
            {
                currentPosition = GetWorldPos(Input.mousePosition);
                OnDragUpdate?.Invoke(currentPosition);
            }
            else if (Input.GetMouseButtonUp(0) && isDragging)
            {
                currentPosition = GetWorldPos(Input.mousePosition);
                
                if (Vector2.Distance(startPosition, currentPosition) < dragThreshold)
                    OnTap?.Invoke(currentPosition);
                else
                    OnDragEnd?.Invoke(currentPosition);
                
                isDragging = false;
            }
        }

        private void HandleTouch()
        {
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                
                switch (t.phase)
                {
                    case TouchPhase.Began:
                        startPosition = GetWorldPos(t.position);
                        currentPosition = startPosition;
                        isDragging = true;
                        OnDragStart?.Invoke(currentPosition);
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if (isDragging)
                        {
                            currentPosition = GetWorldPos(t.position);
                            OnDragUpdate?.Invoke(currentPosition);
                        }
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (isDragging)
                        {
                            currentPosition = GetWorldPos(t.position);
                            
                            if (Vector2.Distance(startPosition, currentPosition) < dragThreshold)
                                OnTap?.Invoke(currentPosition);
                            else
                                OnDragEnd?.Invoke(currentPosition);
                            
                            isDragging = false;
                        }
                        break;
                }
            }
        }

        private Vector2 GetWorldPos(Vector2 screenPos)
        {
            return mainCamera != null ? mainCamera.ScreenToWorldPoint(screenPos) : screenPos;
        }
    }
}
