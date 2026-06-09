using UnityEngine;

namespace MiaFantasyRun.Services
{
    public sealed class GooglePlayGamesManager : MonoBehaviour
    {
        [SerializeField] private string highScoreLeaderboardId = "CgkI_replace_me_high_score";
        [SerializeField] private string distanceLeaderboardId = "CgkI_replace_me_distance";

        public void SignIn()
        {
            Debug.Log("Google Play Games sign-in requested. Install and configure the official plugin before release.");
        }

        public void SubmitScore(int score)
        {
            Debug.Log($"Submit high score {score} to {highScoreLeaderboardId}");
        }

        public void SubmitDistance(int meters)
        {
            Debug.Log($"Submit distance {meters} to {distanceLeaderboardId}");
        }
    }
}
