using MiaFantasyRun.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace MiaFantasyRun.UI
{
    public sealed class PrototypeHud : MonoBehaviour
    {
        [SerializeField] private RunnerGameManager manager;
        [SerializeField] private PowerUpController powerUp;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text coinsText;
        [SerializeField] private Text gemsText;
        [SerializeField] private Text distanceText;
        [SerializeField] private Text statusText;

        private void Start()
        {
            manager ??= FindFirstObjectByType<RunnerGameManager>();
            powerUp ??= FindFirstObjectByType<PowerUpController>();
        }

        private void Update()
        {
            if (manager == null)
            {
                return;
            }

            var state = manager.State;
            scoreText.text = $"Score: {state.Score:N0}";
            coinsText.text = $"Coins: {state.Coins:N0}";
            gemsText.text = $"Gems: {state.Gems:N0}";
            distanceText.text = $"{state.DistanceMeters:N0} m";

            if (manager.IsGameOver)
            {
                statusText.text = "Game Over";
            }
            else if (powerUp != null && powerUp.HasShield)
            {
                statusText.text = "";
            }
            else
            {
                statusText.text = "";
            }
        }
    }
}
