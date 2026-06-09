using UnityEngine;

namespace MiaFantasyRun.Gameplay
{
    public sealed class RunnerDustTrail : MonoBehaviour
    {
        private ParticleSystem dust;
        private RunnerGameManager manager;

        private void Awake()
        {
            manager = FindFirstObjectByType<RunnerGameManager>();
            dust = new GameObject("Running Foot Dust").AddComponent<ParticleSystem>();
            dust.transform.SetParent(transform, false);
            dust.transform.localPosition = new Vector3(0f, 0.08f, -0.42f);

            var main = dust.main;
            main.startColor = new Color(0.9f, 0.82f, 0.68f, 0.55f);
            main.startLifetime = 0.42f;
            main.startSpeed = 0.42f;
            main.startSize = 0.055f;
            main.maxParticles = 28;

            var emission = dust.emission;
            emission.rateOverTime = 24f;

            var shape = dust.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 20f;
            shape.radius = 0.24f;
        }

        private void Update()
        {
            if (dust == null)
            {
                return;
            }

            var shouldPlay = manager == null || !manager.IsGameOver;
            if (shouldPlay && !dust.isPlaying)
            {
                dust.Play();
            }
            else if (!shouldPlay && dust.isPlaying)
            {
                dust.Stop();
            }
        }
    }
}
