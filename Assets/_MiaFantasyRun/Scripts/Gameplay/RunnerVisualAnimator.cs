using UnityEngine;

namespace MiaFantasyRun.Gameplay
{
    public sealed class RunnerVisualAnimator : MonoBehaviour
    {
        [SerializeField] private Transform leftArm;
        [SerializeField] private Transform rightArm;
        [SerializeField] private Transform leftLeg;
        [SerializeField] private Transform rightLeg;
        [SerializeField] private Transform cape;
        [SerializeField] private Transform ponytail;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Texture2D runSheet;
        [SerializeField] private int runSheetColumns = 4;
        [SerializeField] private int runSheetRows = 2;
        [SerializeField] private float spritePixelsPerUnit = 430f;
        [SerializeField] private float frameRate = 12f;
        [SerializeField] private float strideSpeed = 9.5f;
        [SerializeField] private float limbSwingDegrees = 34f;
        [SerializeField] private float capeSwingDegrees = 7f;
        [SerializeField] private float bodyBounce = 0.045f;

        private Vector3 startPosition;
        private Sprite[] runFrames;
        private int currentFrame = -1;
        private float overrideUntil;
        private VisualState state = VisualState.Run;

        private enum VisualState
        {
            Idle,
            Run,
            Jump,
            Slide,
            Hit
        }

        private void Awake()
        {
            startPosition = transform.localPosition;
            BuildRunFrames();
        }

        private void Update()
        {
            if (Time.time > overrideUntil && state != VisualState.Run && state != VisualState.Idle)
            {
                state = VisualState.Run;
            }

            var speed = state == VisualState.Idle ? strideSpeed * 0.28f : strideSpeed;
            var stride = Mathf.Sin(Time.time * speed);
            var oppositeStride = -stride;
            var bounce = state == VisualState.Run ? Mathf.Abs(stride) * bodyBounce : Mathf.Sin(Time.time * 1.9f) * bodyBounce * 0.35f;
            transform.localPosition = startPosition + new Vector3(0f, bounce, 0f);

            UpdateSpriteFrame();

            if (state == VisualState.Jump)
            {
                SwingLimb(leftArm, -0.55f);
                SwingLimb(rightArm, -0.55f);
                SwingLimb(leftLeg, 0.28f);
                SwingLimb(rightLeg, 0.28f);
            }
            else if (state == VisualState.Slide)
            {
                transform.localRotation = Quaternion.Euler(14f, 0f, 0f);
                SwingLimb(leftArm, 0.7f);
                SwingLimb(rightArm, -0.7f);
                SwingLimb(leftLeg, -0.35f);
                SwingLimb(rightLeg, 0.35f);
            }
            else if (state == VisualState.Hit)
            {
                transform.localRotation = Quaternion.Euler(-8f, 0f, 8f);
                SwingLimb(leftArm, -1f);
                SwingLimb(rightArm, 1f);
                SwingLimb(leftLeg, 0.15f);
                SwingLimb(rightLeg, -0.15f);
            }
            else
            {
                transform.localRotation = Quaternion.identity;
                var swingScale = state == VisualState.Idle ? 0.12f : 1f;
                SwingLimb(leftArm, stride * swingScale);
                SwingLimb(rightArm, oppositeStride * swingScale);
                SwingLimb(leftLeg, oppositeStride * swingScale);
                SwingLimb(rightLeg, stride * swingScale);
            }

            if (cape != null)
            {
                cape.localRotation = Quaternion.Euler(8f + Mathf.Abs(stride) * capeSwingDegrees, 0f, 0f);
            }

            if (ponytail != null)
            {
                ponytail.localRotation = Quaternion.Euler(14f + Mathf.Abs(stride) * 8f, stride * 9f, -16f + stride * 7f);
            }
        }

        private void BuildRunFrames()
        {
            if (spriteRenderer == null || runSheet == null || runSheetColumns <= 0 || runSheetRows <= 0)
            {
                return;
            }

            var frameWidth = runSheet.width / runSheetColumns;
            var frameHeight = runSheet.height / runSheetRows;
            runFrames = new Sprite[runSheetColumns * runSheetRows];
            var index = 0;
            for (var row = 0; row < runSheetRows; row++)
            {
                for (var column = 0; column < runSheetColumns; column++)
                {
                    var rect = new Rect(column * frameWidth, (runSheetRows - 1 - row) * frameHeight, frameWidth, frameHeight);
                    runFrames[index] = Sprite.Create(runSheet, rect, new Vector2(0.5f, 0.08f), spritePixelsPerUnit);
                    index++;
                }
            }

            if (runFrames.Length > 0)
            {
                spriteRenderer.sprite = runFrames[0];
            }
        }

        private void UpdateSpriteFrame()
        {
            if (spriteRenderer == null || runFrames == null || runFrames.Length == 0)
            {
                return;
            }

            var frame = state == VisualState.Idle
                ? 5
                : Mathf.FloorToInt(Time.time * frameRate) % runFrames.Length;

            if (state == VisualState.Jump)
            {
                frame = 2;
            }
            else if (state == VisualState.Slide)
            {
                frame = 6;
            }
            else if (state == VisualState.Hit)
            {
                frame = 0;
            }

            frame = Mathf.Clamp(frame, 0, runFrames.Length - 1);
            if (frame != currentFrame)
            {
                spriteRenderer.sprite = runFrames[frame];
                currentFrame = frame;
            }

            spriteRenderer.transform.localRotation = state switch
            {
                VisualState.Slide => Quaternion.Euler(10f, 0f, 0f),
                VisualState.Hit => Quaternion.Euler(-6f, 0f, 8f),
                _ => Quaternion.identity
            };
        }

        private void SwingLimb(Transform limb, float stride)
        {
            if (limb == null)
            {
                return;
            }

            limb.localRotation = Quaternion.Euler(stride * limbSwingDegrees, 0f, 0f);
        }

        public void PlayIdle() => state = VisualState.Idle;

        public void PlayRun() => state = VisualState.Run;

        public void PlayJump()
        {
            state = VisualState.Jump;
            overrideUntil = Time.time + 0.38f;
        }

        public void PlaySlide()
        {
            state = VisualState.Slide;
            overrideUntil = Time.time + 0.55f;
        }

        public void PlayHit()
        {
            state = VisualState.Hit;
            overrideUntil = Time.time + 0.45f;
        }
    }
}
