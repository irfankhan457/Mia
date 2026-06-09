using UnityEngine;

namespace MiaFantasyRun.Gameplay
{
    public sealed class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new(0f, 4.15f, -7.2f);
        [SerializeField] private Vector3 lookOffset = new(0f, 1.25f, 7.5f);
        [SerializeField] private float smoothSpeed = 9f;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothSpeed * Time.deltaTime);
            transform.LookAt(target.position + lookOffset);
        }
    }
}
