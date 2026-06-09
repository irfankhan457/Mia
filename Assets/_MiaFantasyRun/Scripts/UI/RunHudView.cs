using UnityEngine;
using UnityEngine.UI;

namespace MiaFantasyRun.UI
{
    public sealed class RunHudView : UIView
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Text coinText;
        [SerializeField] private Text gemText;
        [SerializeField] private Text distanceText;

        public void Bind(int score, int coins, int gems, float distanceMeters)
        {
            scoreText.text = score.ToString("N0");
            coinText.text = coins.ToString("N0");
            gemText.text = gems.ToString("N0");
            distanceText.text = $"{distanceMeters:N0} m";
        }
    }
}
