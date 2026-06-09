using System.Collections.Generic;
using MiaFantasyRun.Data;
using UnityEngine;

namespace MiaFantasyRun.Services
{
    public sealed class MissionManager : MonoBehaviour
    {
        [SerializeField] private List<MissionDefinition> missions = new();

        private readonly Dictionary<string, int> progress = new();

        public void AddProgress(MissionMetric metric, int amount)
        {
            foreach (var mission in missions)
            {
                if (mission.Metric != metric)
                {
                    continue;
                }

                progress.TryGetValue(mission.Id, out var value);
                progress[mission.Id] = Mathf.Min(value + amount, mission.TargetValue);
            }
        }

        public bool IsComplete(MissionDefinition mission)
        {
            return progress.TryGetValue(mission.Id, out var value) && value >= mission.TargetValue;
        }
    }
}
