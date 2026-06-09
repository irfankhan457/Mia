using System.Collections.Generic;
using UnityEngine;

namespace MiaFantasyRun.Gameplay
{
    public sealed class TrackSegmentSpawner : MonoBehaviour
    {
        [SerializeField] private RunnerGameManager gameManager;
        [SerializeField] private List<ObjectPool> segmentPools = new();
        [SerializeField] private float segmentLength = 30f;
        [SerializeField] private int activeSegments = 7;

        private float nextSpawnZ;
        private readonly Queue<GameObject> spawned = new();

        private void Start()
        {
            for (var i = 0; i < activeSegments; i++)
            {
                Spawn();
            }
        }

        private void Update()
        {
            if (gameManager.State.DistanceMeters + activeSegments * segmentLength > nextSpawnZ)
            {
                Spawn();
            }
        }

        private void Spawn()
        {
            if (segmentPools.Count == 0)
            {
                return;
            }

            var pool = segmentPools[Random.Range(0, segmentPools.Count)];
            var segment = pool.Get(new Vector3(0f, 0f, nextSpawnZ), Quaternion.identity);
            spawned.Enqueue(segment);
            nextSpawnZ += segmentLength;

            if (spawned.Count > activeSegments + 1)
            {
                var old = spawned.Dequeue();
                old.SetActive(false);
            }
        }
    }
}
