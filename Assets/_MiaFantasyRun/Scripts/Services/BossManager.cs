using UnityEngine;

namespace MiaFantasyRun.Services
{
    public enum BossType
    {
        Dragon,
        GiantGolem,
        IceQueen,
        ShadowBeast,
        MagicKraken
    }

    public sealed class BossManager : MonoBehaviour
    {
        [SerializeField] private float appearEverySeconds = 45f;
        [SerializeField] private float activeSeconds = 18f;

        private float timer;

        public BossType CurrentBoss { get; private set; }
        public bool IsBossActive { get; private set; }

        private void Update()
        {
            timer += Time.deltaTime;
            if (!IsBossActive && timer >= appearEverySeconds)
            {
                StartBoss((BossType)Random.Range(0, 5));
            }
            else if (IsBossActive && timer >= activeSeconds)
            {
                EndBoss();
            }
        }

        public void StartBoss(BossType bossType)
        {
            CurrentBoss = bossType;
            IsBossActive = true;
            timer = 0f;
        }

        public void EndBoss()
        {
            IsBossActive = false;
            timer = 0f;
        }
    }
}
