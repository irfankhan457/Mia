using System;
using UnityEngine;

namespace MiaFantasyRun.Gameplay
{
    public sealed class SwipeInput : MonoBehaviour
    {
        public event Action SwipeLeft;
        public event Action SwipeRight;
        public event Action SwipeUp;
        public event Action SwipeDown;
        public event Action<bool> HoldChanged;

        [SerializeField] private float minSwipeDistance = 70f;
        [SerializeField] private float holdThresholdSeconds = 0.18f;

        private Vector2 startPosition;
        private float pressTime;
        private bool holding;

        private void Update()
        {
            HandleKeyboard();

            if (Input.touchCount > 0)
            {
                HandleTouch(Input.GetTouch(0));
            }
            else if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0) || Input.GetMouseButton(0))
            {
                HandleMouse();
            }
        }

        private void HandleKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                SwipeLeft?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                SwipeRight?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
            {
                SwipeUp?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                SwipeDown?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                HoldChanged?.Invoke(true);
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                HoldChanged?.Invoke(false);
            }
        }

        private void HandleTouch(Touch touch)
        {
            if (touch.phase == TouchPhase.Began)
            {
                BeginPress(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                EndPress(touch.position);
            }
            else
            {
                UpdateHold();
            }
        }

        private void HandleMouse()
        {
            if (Input.GetMouseButtonDown(0))
            {
                BeginPress(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                EndPress(Input.mousePosition);
            }
            else
            {
                UpdateHold();
            }
        }

        private void BeginPress(Vector2 position)
        {
            startPosition = position;
            pressTime = Time.unscaledTime;
            holding = false;
        }

        private void UpdateHold()
        {
            if (!holding && Time.unscaledTime - pressTime >= holdThresholdSeconds)
            {
                holding = true;
                HoldChanged?.Invoke(true);
            }
        }

        private void EndPress(Vector2 endPosition)
        {
            if (holding)
            {
                HoldChanged?.Invoke(false);
                holding = false;
                return;
            }

            var delta = endPosition - startPosition;
            if (delta.magnitude < minSwipeDistance)
            {
                return;
            }

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (delta.x > 0f) SwipeRight?.Invoke();
                else SwipeLeft?.Invoke();
            }
            else
            {
                if (delta.y > 0f) SwipeUp?.Invoke();
                else SwipeDown?.Invoke();
            }
        }

        public void TriggerLeft() => SwipeLeft?.Invoke();

        public void TriggerRight() => SwipeRight?.Invoke();

        public void TriggerJump() => SwipeUp?.Invoke();

        public void TriggerSlide() => SwipeDown?.Invoke();
    }
}
