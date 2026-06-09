using System;
using UnityEngine;

namespace MiaFantasyRun.Services
{
    public sealed class RewardManager : MonoBehaviour
    {
        [SerializeField] private int[] dailyCoins = { 250, 0, 0, 0, 1000, 0, 2500 };
        [SerializeField] private int[] dailyGems = { 0, 10, 0, 0, 0, 50, 100 };

        public bool TryClaimDailyReward(SaveData saveData, out int coins, out int gems)
        {
            var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            if (saveData.LastDailyRewardDate == today)
            {
                coins = 0;
                gems = 0;
                return false;
            }

            var index = saveData.DailyRewardDay % 7;
            coins = dailyCoins[index];
            gems = dailyGems[index];
            saveData.Coins += coins;
            saveData.Gems += gems;
            saveData.DailyRewardDay = (index + 1) % 7;
            saveData.LastDailyRewardDate = today;
            return true;
        }
    }
}
