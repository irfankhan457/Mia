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
        [SerializeField] private GameObject gameOverPanel;

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
            scoreText.text = $"{state.Score:N0}\nSCORE";
            coinsText.text = $"{state.Coins:N0}";
            gemsText.text = $"{state.Gems:N0}";
            distanceText.text = $"{state.DistanceMeters:N0} m";

            if (manager.IsGameOver)
            {
                statusText.text = "";
            }
            else if (powerUp != null && powerUp.HasShield)
            {
                statusText.text = "";
            }
            else
            {
                statusText.text = "";
            }

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(manager.IsGameOver);
            }
        }
    }
}
