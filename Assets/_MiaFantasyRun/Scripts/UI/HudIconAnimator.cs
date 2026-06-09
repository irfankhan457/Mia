using UnityEngine;
using UnityEngine.UI;

namespace MiaFantasyRun.UI
{
    public sealed class HudIconAnimator : MonoBehaviour
    {
        [SerializeField] private RectTransform icon;
        [SerializeField] private Image shine;
        [SerializeField] private float rotationDegrees = 5f;
        [SerializeField] private float pulseAmount = 0.06f;
        [SerializeField] private float shinePeriodSeconds = 2f;

        private Vector3 baseScale;
        private RectTransform shineRect;

        private void Awake()
        {
            if (icon == null)
            {
                icon = transform as RectTransform;
            }

            baseScale = icon != null ? icon.localScale : Vector3.one;
            shineRect = shine != null ? shine.rectTransform : null;
        }

        private void Update()
        {
            if (icon == null)
            {
                return;
            }

            var t = Time.unscaledTime;
            icon.localRotation = Quaternion.Euler(0f, Mathf.Sin(t * 2.3f) * rotationDegrees, 0f);
            icon.localScale = baseScale * (1f + Mathf.Sin(t * 4.4f) * pulseAmount);

            if (shineRect == null)
            {
                return;
            }

            var progress = (t % shinePeriodSeconds) / shinePeriodSeconds;
            shine.enabled = progress < 0.55f;
            shineRect.anchoredPosition = new Vector2(Mathf.Lerp(-42f, 42f, progress / 0.55f), 0f);
        }
    }
}
