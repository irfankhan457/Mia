using System.Collections;
using MiaFantasyRun.Data;
using UnityEngine;

namespace MiaFantasyRun.Gameplay
{
    public sealed class PowerUpController : MonoBehaviour
    {
        public bool HasShield { get; private set; }
        public float CoinMultiplier { get; private set; } = 1f;
        public float GemMultiplier { get; private set; } = 1f;
        public float RemainingShieldSeconds { get; private set; }

        private Coroutine activeRoutine;

        public void Activate(PowerUpDefinition powerUp)
        {
            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
            }

            activeRoutine = StartCoroutine(ApplyRoutine(powerUp));
        }

        public void ActivateShield(float seconds)
        {
            if (activeRoutine != null)
            {
                StopCoroutine(activeRoutine);
            }

            activeRoutine = StartCoroutine(ShieldRoutine(seconds));
        }

        public bool ConsumeShield()
        {
            if (!HasShield)
            {
                return false;
            }

            HasShield = false;
            RemainingShieldSeconds = 0f;
            return true;
        }

        private IEnumerator ApplyRoutine(PowerUpDefinition powerUp)
        {
            switch (powerUp.Type)
            {
                case PowerUpType.Shield:
                case PowerUpType.Invincibility:
                    HasShield = true;
                    break;
                case PowerUpType.CoinMultiplier:
                case PowerUpType.MegaCoinRush:
                    CoinMultiplier = powerUp.Multiplier;
                    break;
                case PowerUpType.GemMultiplier:
                    GemMultiplier = powerUp.Multiplier;
                    break;
            }

            yield return new WaitForSeconds(powerUp.DurationSeconds);

            HasShield = false;
            CoinMultiplier = 1f;
            GemMultiplier = 1f;
            RemainingShieldSeconds = 0f;
            activeRoutine = null;
        }

        private IEnumerator ShieldRoutine(float seconds)
        {
            HasShield = true;
            RemainingShieldSeconds = seconds;
            while (RemainingShieldSeconds > 0f)
            {
                RemainingShieldSeconds -= Time.deltaTime;
                yield return null;
            }

            HasShield = false;
            RemainingShieldSeconds = 0f;
            activeRoutine = null;
        }
    }
}
