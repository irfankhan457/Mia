using UnityEngine;

namespace MiaFantasyRun.Gameplay
{
    public sealed class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new(0f, 4.15f, -7.2f);
        [SerializeField] private Vector3 lookOffset = new(0f, 1.25f, 7.5f);
        [SerializeField] private float smoothSpeed = 9f;
        [SerializeField] private float swayAmount = 0.16f;
        [SerializeField] private float bounceAmount = 0.08f;
        [SerializeField] private float motionSpeed = 5.2f;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            var sway = Mathf.Sin(Time.time * motionSpeed) * swayAmount;
            var bounce = Mathf.Abs(Mathf.Sin(Time.time * motionSpeed * 0.82f)) * bounceAmount;
            var dynamicOffset = offset + new Vector3(sway, bounce, 0f);
            transform.position = Vector3.Lerp(transform.position, target.position + dynamicOffset, smoothSpeed * Time.deltaTime);
            transform.LookAt(target.position + lookOffset + new Vector3(sway * 0.25f, 0f, 0f));
        }
    }
}
