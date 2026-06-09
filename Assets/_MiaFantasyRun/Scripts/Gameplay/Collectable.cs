using UnityEngine;

namespace MiaFantasyRun.Gameplay
{
    public enum CollectableType
    {
        Coin,
        Gem,
        Key,
        EventToken
    }

    public sealed class Collectable : MonoBehaviour
    {
        [SerializeField] private CollectableType type;
        [SerializeField] private int amount = 1;
        [SerializeField] private GameObject pickupVfx;

        public void Configure(CollectableType collectableType, int collectableAmount)
        {
            type = collectableType;
            amount = collectableAmount;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerController _))
            {
                return;
            }

            var manager = FindFirstObjectByType<RunnerGameManager>();
            if (manager == null || manager.IsGameOver)
            {
                return;
            }

            var powerUp = other.GetComponent<PowerUpController>();
            if (type == CollectableType.Gem)
            {
                manager.CollectGem(amount);
            }
            else if (type == CollectableType.Key || type == CollectableType.EventToken)
            {
                powerUp?.ActivateShield(6f);
            }
            else
            {
                var multiplier = powerUp != null ? powerUp.CoinMultiplier : 1f;
                manager.CollectCoin(Mathf.RoundToInt(amount * multiplier));
            }

            if (pickupVfx != null)
            {
                Instantiate(pickupVfx, transform.position, Quaternion.identity);
            }

            gameObject.SetActive(false);
        }
    }
}
