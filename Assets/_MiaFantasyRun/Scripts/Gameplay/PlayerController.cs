using System.Collections;
using MiaFantasyRun.Data;
using UnityEngine;

namespace MiaFantasyRun.Gameplay
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private SwipeInput input;
        [SerializeField] private CharacterDefinition character;
        [SerializeField] private Animator animator;
        [SerializeField] private RunnerVisualAnimator visualAnimator;
        [SerializeField] private float laneWidth = 2.5f;
        [SerializeField] private float laneChangeSpeed = 15f;
        [SerializeField] private float jumpForce = 8.5f;
        [SerializeField] private float gravity = -28f;
        [SerializeField] private float slideDuration = 0.75f;
        [SerializeField] private float hoverGravityScale = 0.25f;
        [SerializeField] private float forwardSpeed = 8f;

        private CharacterController controller;
        private int currentLane;
        private int jumpCount;
        private float verticalVelocity;
        private bool sliding;
        private bool hovering;
        private Vector3 startPosition;
        private RunnerGameManager gameManager;

        private static readonly int RunHash = Animator.StringToHash("Run");
        private static readonly int JumpHash = Animator.StringToHash("Jump");
        private static readonly int SlideHash = Animator.StringToHash("Slide");
        private static readonly int HitHash = Animator.StringToHash("Hit");

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            visualAnimator ??= GetComponentInChildren<RunnerVisualAnimator>();
            startPosition = transform.position;
            gameManager = FindFirstObjectByType<RunnerGameManager>();
        }

        private void OnEnable()
        {
            if (input == null)
            {
                input = FindFirstObjectByType<SwipeInput>();
            }

            if (input == null)
            {
                enabled = false;
                return;
            }

            input.SwipeLeft += MoveLeft;
            input.SwipeRight += MoveRight;
            input.SwipeUp += Jump;
            input.SwipeDown += Slide;
            input.HoldChanged += SetHover;
            if (animator != null)
            {
                animator.SetTrigger(RunHash);
            }

            visualAnimator?.PlayRun();
        }

        private void OnDisable()
        {
            if (input == null)
            {
                return;
            }

            input.SwipeLeft -= MoveLeft;
            input.SwipeRight -= MoveRight;
            input.SwipeUp -= Jump;
            input.SwipeDown -= Slide;
            input.HoldChanged -= SetHover;
        }

        private void Update()
        {
            if (gameManager != null && gameManager.IsGameOver)
            {
                return;
            }

            ApplyMovement();
        }

        private void ApplyMovement()
        {
            var targetX = currentLane * laneWidth;
            var horizontal = Mathf.MoveTowards(transform.position.x, targetX, laneChangeSpeed * Time.deltaTime) - transform.position.x;

            if (controller.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -1f;
                jumpCount = 0;
            }

            var gravityScale = hovering && !controller.isGrounded ? hoverGravityScale : 1f;
            verticalVelocity += gravity * gravityScale * Time.deltaTime;

            var motion = new Vector3(horizontal, verticalVelocity * Time.deltaTime, forwardSpeed * Time.deltaTime);
            controller.Move(motion);
        }

        private void MoveLeft() => currentLane = Mathf.Max(currentLane - 1, -1);
        private void MoveRight() => currentLane = Mathf.Min(currentLane + 1, 1);

        private void Jump()
        {
            if (jumpCount >= 2 || sliding)
            {
                return;
            }

            jumpCount++;
            verticalVelocity = jumpForce;
            if (animator != null)
            {
                animator.SetTrigger(JumpHash);
            }

            visualAnimator?.PlayJump();
        }

        private void Slide()
        {
            if (!sliding && controller.isGrounded)
            {
                StartCoroutine(SlideRoutine());
            }
        }

        private IEnumerator SlideRoutine()
        {
            sliding = true;
            if (animator != null)
            {
                animator.SetTrigger(SlideHash);
            }
            visualAnimator?.PlaySlide();
            var originalHeight = controller.height;
            controller.height = originalHeight * 0.5f;
            yield return new WaitForSeconds(slideDuration);
            controller.height = originalHeight;
            sliding = false;
        }

        private void SetHover(bool isHovering)
        {
            hovering = isHovering && character != null && character.CanHover;
        }

        public void Hit()
        {
            if (animator != null)
            {
                animator.SetTrigger(HitHash);
            }

            visualAnimator?.PlayHit();
        }

        public void ResetRunner()
        {
            currentLane = 0;
            jumpCount = 0;
            verticalVelocity = 0f;
            sliding = false;
            hovering = false;
            if (controller == null)
            {
                controller = GetComponent<CharacterController>();
            }

            controller.enabled = false;
            transform.position = startPosition;
            controller.enabled = true;
            visualAnimator?.PlayRun();
        }
    }
}
