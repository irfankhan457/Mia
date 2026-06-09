using UnityEngine;

namespace MiaFantasyRun.Gameplay
{
    public sealed class Obstacle : MonoBehaviour
    {
        [SerializeField] private bool lethal = true;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerController player))
            {
                return;
            }

            player.Hit();
            var powerUp = other.GetComponent<PowerUpController>();
            if (powerUp != null && powerUp.ConsumeShield())
            {
                gameObject.SetActive(false);
                return;
            }

            if (lethal)
            {
                var manager = FindFirstObjectByType<RunnerGameManager>();
                manager?.GameOver();
            }
        }
    }
}
