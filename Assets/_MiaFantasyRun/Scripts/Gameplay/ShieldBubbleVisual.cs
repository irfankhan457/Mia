using UnityEngine;

namespace MiaFantasyRun.Gameplay
{
    [RequireComponent(typeof(PowerUpController))]
    public sealed class ShieldBubbleVisual : MonoBehaviour
    {
        [SerializeField] private Sprite shieldIconSprite;

        private PowerUpController powerUp;
        private Transform shieldIcon;
        private TextMesh timerText;
        private TextMesh timerShadow;
        private Light glow;
        private ParticleSystem pinkParticles;
        private ParticleSystem blueSparks;
        private TrailRenderer trail;

        private void Awake()
        {
            powerUp = GetComponent<PowerUpController>();
            CreateTimer();
            CreateParticles();
            SetActive(false);
        }

        private void Update()
        {
            var active = powerUp != null && powerUp.HasShield;
            SetActive(active);
            if (!active)
            {
                return;
            }

            var seconds = Mathf.CeilToInt(powerUp.RemainingShieldSeconds);
            var value = $"{seconds}s";
            timerText.text = value;
            timerShadow.text = value;

            if (shieldIcon != null)
            {
                shieldIcon.localPosition = new Vector3(0f, 2.9f + Mathf.Sin(Time.time * 3.2f) * 0.04f, -0.28f);
                shieldIcon.localRotation = Quaternion.Euler(18f, Mathf.Sin(Time.time * 2.6f) * 8f, 0f);
            }

            var y = 2.42f + Mathf.Sin(Time.time * 3.2f) * 0.02f;
            timerText.transform.localPosition = new Vector3(0f, y, -0.26f);
            timerShadow.transform.localPosition = new Vector3(0.018f, y - 0.018f, -0.265f);

            if (glow != null)
            {
                glow.intensity = 0.65f + Mathf.Sin(Time.time * 6f) * 0.22f;
            }
        }

        private void CreateTimer()
        {
            if (shieldIconSprite != null)
            {
                var iconObject = new GameObject("Small Shield Status Icon");
                iconObject.transform.SetParent(transform, false);
                iconObject.transform.localPosition = new Vector3(0f, 2.9f, -0.28f);
                iconObject.transform.localRotation = Quaternion.Euler(18f, 0f, 0f);
                iconObject.transform.localScale = Vector3.one * 0.26f;
                var renderer = iconObject.AddComponent<SpriteRenderer>();
                renderer.sprite = shieldIconSprite;
                renderer.sortingOrder = 22;
                shieldIcon = iconObject.transform;
            }

            timerShadow = CreateText("Shield Countdown Shadow", new Color(0f, 0f, 0f, 0.85f), 0.095f);
            timerText = CreateText("Shield Countdown", Color.white, 0.095f);

            glow = gameObject.AddComponent<Light>();
            glow.type = LightType.Point;
            glow.color = new Color(0.26f, 0.85f, 1f);
            glow.range = 2.3f;
            glow.intensity = 0.65f;
        }

        private TextMesh CreateText(string name, Color color, float characterSize)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(transform, false);
            textObject.transform.localPosition = new Vector3(0f, 2.42f, -0.26f);
            textObject.transform.localRotation = Quaternion.Euler(18f, 0f, 0f);

            var text = textObject.AddComponent<TextMesh>();
            text.text = "";
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.fontSize = 38;
            text.fontStyle = FontStyle.Bold;
            text.characterSize = characterSize;
            text.color = color;
            return text;
        }

        private void CreateParticles()
        {
            pinkParticles = CreateParticleSystem("Shield Pink Particles", new Color(1f, 0.1f, 0.72f, 0.85f), 15f, 0.045f, 0.62f);
            blueSparks = CreateParticleSystem("Shield Blue Sparks", new Color(0.22f, 0.85f, 1f, 0.9f), 12f, 0.035f, 0.72f);

            var trailObject = new GameObject("Shield Subtle Trail");
            trailObject.transform.SetParent(transform, false);
            trailObject.transform.localPosition = new Vector3(0f, 1.1f, -0.3f);
            trail = trailObject.AddComponent<TrailRenderer>();
            trail.time = 0.28f;
            trail.startWidth = 0.12f;
            trail.endWidth = 0.01f;
            trail.material = new Material(Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Color"))
            {
                color = new Color(0.2f, 0.85f, 1f, 0.42f)
            };
        }

        private ParticleSystem CreateParticleSystem(string name, Color color, float rate, float size, float radius)
        {
            var particleObject = new GameObject(name);
            particleObject.transform.SetParent(transform, false);
            particleObject.transform.localPosition = new Vector3(0f, 1.1f, -0.1f);

            var particles = particleObject.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.startColor = color;
            main.startLifetime = 0.58f;
            main.startSpeed = 0.46f;
            main.startSize = size;
            main.maxParticles = 36;

            var emission = particles.emission;
            emission.rateOverTime = rate;

            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = radius;

            return particles;
        }

        private void SetActive(bool active)
        {
            if (timerText != null)
            {
                timerText.gameObject.SetActive(active);
            }

            if (shieldIcon != null)
            {
                shieldIcon.gameObject.SetActive(active);
            }

            if (timerShadow != null)
            {
                timerShadow.gameObject.SetActive(active);
            }

            if (glow != null)
            {
                glow.enabled = active;
            }

            SetParticleActive(pinkParticles, active);
            SetParticleActive(blueSparks, active);

            if (trail != null)
            {
                trail.emitting = active;
                trail.enabled = active;
            }
        }

        private static void SetParticleActive(ParticleSystem particles, bool active)
        {
            if (particles == null)
            {
                return;
            }

            if (active && !particles.isPlaying)
            {
                particles.Play();
            }
            else if (!active && particles.isPlaying)
            {
                particles.Stop();
            }

            particles.gameObject.SetActive(active);
        }
    }
}
