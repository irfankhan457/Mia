using UnityEngine;

namespace MiaFantasyRun.Gameplay
{
    public sealed class CollectableSpinner : MonoBehaviour
    {
        [SerializeField] private float degreesPerSecond = 160f;

        private void Update()
        {
            transform.Rotate(0f, degreesPerSecond * Time.deltaTime, 0f, Space.World);
        }
    }
}
