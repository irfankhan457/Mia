using System.Collections.Generic;
using UnityEngine;

namespace MiaFantasyRun.Gameplay
{
    public sealed class PrototypeWorldSpawner : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private float spawnAheadDistance = 120f;
        [SerializeField] private float cleanupBehindDistance = 35f;
        [SerializeField] private float segmentLength = 18f;
        [SerializeField] private Material roadMaterial;
        [SerializeField] private Material coinMaterial;
        [SerializeField] private Material gemMaterial;
        [SerializeField] private Material shieldMaterial;
        [SerializeField] private Material obstacleMaterial;
        [SerializeField] private Material decorationMaterial;
        [SerializeField] private Material laneMaterial;
        [SerializeField] private Sprite shieldPickupSprite;

        private readonly List<GameObject> spawned = new();
        private readonly Dictionary<string, Material> materialCache = new();
        private Material sidewalkMaterial;
        private Material grassMaterial;
        private Material buildingBlueMaterial;
        private Material buildingCoralMaterial;
        private Material buildingYellowMaterial;
        private Material shadowMaterial;
        private float nextSegmentZ;
        private int segmentIndex;

        private static readonly CountryTheme[] Countries =
        {
            new("INDIA", 0f, new Color(0.95f, 0.45f, 0.12f), new Color(0.1f, 0.5f, 0.25f), new Color(1f, 0.72f, 0.16f), new Color(0.9f, 0.18f, 0.22f), new Color(0.08f, 0.42f, 0.72f), "Rickshaw", "Incredible India"),
            new("USA", 5000f, new Color(0.12f, 0.22f, 0.35f), new Color(0.95f, 0.74f, 0.08f), new Color(0.18f, 0.45f, 0.95f), new Color(0.9f, 0.12f, 0.12f), new Color(0.05f, 0.08f, 0.14f), "Taxi", "New York Lights"),
            new("JAPAN", 15000f, new Color(0.72f, 0.18f, 0.35f), new Color(1f, 0.68f, 0.82f), new Color(0.9f, 0.08f, 0.12f), new Color(0.1f, 0.12f, 0.18f), new Color(0.45f, 0.05f, 0.38f), "Delivery Bike", "Tokyo Sakura"),
            new("FRANCE", 30000f, new Color(0.36f, 0.42f, 0.55f), new Color(0.86f, 0.82f, 0.68f), new Color(0.18f, 0.28f, 0.72f), new Color(0.95f, 0.95f, 0.92f), new Color(0.78f, 0.18f, 0.24f), "Cafe Barrier", "Paris Dreams"),
            new("EGYPT", 50000f, new Color(0.78f, 0.54f, 0.18f), new Color(0.92f, 0.78f, 0.45f), new Color(0.96f, 0.72f, 0.22f), new Color(0.62f, 0.38f, 0.14f), new Color(0.25f, 0.18f, 0.1f), "Stone Block", "Pyramid Trail"),
            new("DUBAI", 80000f, new Color(0.72f, 0.78f, 0.82f), new Color(0.94f, 0.82f, 0.45f), new Color(0.12f, 0.72f, 0.9f), new Color(0.95f, 0.95f, 1f), new Color(0.04f, 0.18f, 0.35f), "Luxury Barrier", "Dubai Skyline"),
            new("CHINA", 110000f, new Color(0.72f, 0.08f, 0.08f), new Color(0.96f, 0.74f, 0.12f), new Color(0.8f, 0.04f, 0.06f), new Color(0.12f, 0.34f, 0.18f), new Color(0.92f, 0.58f, 0.12f), "Lantern Stand", "Great Wall Run"),
            new("SWITZERLAND", 150000f, new Color(0.68f, 0.84f, 0.95f), new Color(0.93f, 0.97f, 1f), new Color(0.2f, 0.55f, 0.32f), new Color(0.48f, 0.28f, 0.12f), new Color(0.85f, 0.08f, 0.1f), "Snow Log", "Alps Adventure"),
            new("BRAZIL", 200000f, new Color(0.08f, 0.55f, 0.24f), new Color(0.98f, 0.82f, 0.08f), new Color(0.08f, 0.3f, 0.72f), new Color(0.95f, 0.22f, 0.46f), new Color(0.16f, 0.7f, 0.36f), "Carnival Drum", "Rio Carnival"),
            new("SPACE WORLD", 300000f, new Color(0.05f, 0.04f, 0.16f), new Color(0.18f, 0.9f, 1f), new Color(0.72f, 0.2f, 1f), new Color(0.12f, 0.12f, 0.24f), new Color(0.9f, 0.92f, 1f), "Asteroid Crate", "Future Colony")
        };

        private readonly struct CountryTheme
        {
            public CountryTheme(string name, float unlockMeters, Color road, Color accent, Color decoration, Color buildingA, Color buildingB, string obstacleName, string posterText)
            {
                Name = name;
                UnlockMeters = unlockMeters;
                Road = road;
                Accent = accent;
                Decoration = decoration;
                BuildingA = buildingA;
                BuildingB = buildingB;
                ObstacleName = obstacleName;
                PosterText = posterText;
            }

            public string Name { get; }
            public float UnlockMeters { get; }
            public Color Road { get; }
            public Color Accent { get; }
            public Color Decoration { get; }
            public Color BuildingA { get; }
            public Color BuildingB { get; }
            public string ObstacleName { get; }
            public string PosterText { get; }
        }

        private void Awake()
        {
            roadMaterial = CreateRuntimeMaterial("Road Runtime", new Color(0.06f, 0.58f, 0.9f));
            coinMaterial = CreateRuntimeMaterial("Coin Runtime", new Color(1f, 0.82f, 0.08f));
            gemMaterial = CreateRuntimeMaterial("Gem Runtime", new Color(0.05f, 0.82f, 1f));
            shieldMaterial = CreateRuntimeMaterial("Shield Runtime", new Color(0.18f, 0.95f, 0.42f));
            obstacleMaterial = CreateRuntimeMaterial("Obstacle Runtime", new Color(0.78f, 0.24f, 0.52f));
            decorationMaterial = CreateRuntimeMaterial("Decoration Runtime", new Color(0.46f, 0.26f, 0.96f));
            laneMaterial = CreateRuntimeMaterial("Lane Runtime", new Color(1f, 1f, 1f));
            sidewalkMaterial = CreateRuntimeMaterial("Sidewalk Runtime", new Color(0.86f, 0.92f, 0.96f));
            grassMaterial = CreateRuntimeMaterial("Grass Runtime", new Color(0.24f, 0.76f, 0.34f));
            buildingBlueMaterial = CreateRuntimeMaterial("Blue Building Runtime", new Color(0.16f, 0.52f, 0.95f));
            buildingCoralMaterial = CreateRuntimeMaterial("Coral Building Runtime", new Color(1f, 0.36f, 0.34f));
            buildingYellowMaterial = CreateRuntimeMaterial("Yellow Building Runtime", new Color(1f, 0.77f, 0.2f));
            shadowMaterial = CreateRuntimeMaterial("Soft Shadow Runtime", new Color(0.02f, 0.04f, 0.05f, 0.36f));
            SetMetallic(coinMaterial, 1f, 0.72f);
            SetMetallic(gemMaterial, 0.45f, 0.9f);
        }

        private void Start()
        {
            player ??= FindFirstObjectByType<PlayerController>()?.transform;
            while (nextSegmentZ < spawnAheadDistance)
            {
                SpawnSegment();
            }
        }

        private void Update()
        {
            if (player == null)
            {
                return;
            }

            while (nextSegmentZ < player.position.z + spawnAheadDistance)
            {
                SpawnSegment();
            }

            for (var i = spawned.Count - 1; i >= 0; i--)
            {
                if (spawned[i] == null || spawned[i].transform.position.z < player.position.z - cleanupBehindDistance)
                {
                    if (spawned[i] != null)
                    {
                        Destroy(spawned[i]);
                    }

                    spawned.RemoveAt(i);
                }
            }
        }

        private void SpawnSegment()
        {
            var theme = GetThemeForDistance(nextSegmentZ);
            var root = new GameObject($"Segment {segmentIndex:000}");
            root.transform.position = new Vector3(0f, 0f, nextSegmentZ);
            spawned.Add(root);

            var road = GameObject.CreatePrimitive(PrimitiveType.Cube);
            road.name = $"{theme.Name} Road";
            road.transform.SetParent(root.transform);
            road.transform.localPosition = new Vector3(0f, -0.55f, segmentLength * 0.5f);
            road.transform.localScale = new Vector3(8.25f, 0.22f, segmentLength);
            road.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Road Material", theme.Road);

            CreateLaneMarkers(root.transform);
            CreateRoadDetails(root.transform, theme);
            CreateSidewalks(root.transform, theme);
            CreateDecorations(root.transform, theme);
            CreateCityScenery(root.transform, theme);
            CreateCountryGateIfNeeded(root.transform, theme, nextSegmentZ);
            CreateCollectables(root.transform);
            CreateObstacle(root.transform, theme);

            nextSegmentZ += segmentLength;
            segmentIndex++;
        }

        private static CountryTheme GetThemeForDistance(float distance)
        {
            var selected = Countries[0];
            for (var i = 0; i < Countries.Length; i++)
            {
                if (distance >= Countries[i].UnlockMeters)
                {
                    selected = Countries[i];
                }
            }

            return selected;
        }

        private static bool StartsCountry(float segmentStart, float segmentLengthValue, CountryTheme theme)
        {
            return theme.UnlockMeters >= segmentStart && theme.UnlockMeters < segmentStart + segmentLengthValue;
        }

        private void CreateLaneMarkers(Transform root)
        {
            for (var laneEdge = -1; laneEdge <= 1; laneEdge += 2)
            {
                var rail = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rail.name = laneEdge < 0 ? "Left Guard Rail" : "Right Guard Rail";
                rail.transform.SetParent(root.transform);
                rail.transform.localPosition = new Vector3(laneEdge * 4.2f, -0.08f, segmentLength * 0.5f);
                rail.transform.localScale = new Vector3(0.26f, 0.32f, segmentLength);
                rail.GetComponent<Renderer>().sharedMaterial = laneMaterial;
            }

            for (var laneDivider = -1; laneDivider <= 1; laneDivider += 2)
            {
                var divider = GameObject.CreatePrimitive(PrimitiveType.Cube);
                divider.name = laneDivider < 0 ? "Left Lane Divider" : "Right Lane Divider";
                divider.transform.SetParent(root.transform);
                divider.transform.localPosition = new Vector3(laneDivider * 1.25f, -0.34f, segmentLength * 0.5f);
                divider.transform.localScale = new Vector3(0.07f, 0.1f, segmentLength * 0.88f);
                divider.GetComponent<Renderer>().sharedMaterial = laneMaterial;
            }
        }

        private void CreateRoadDetails(Transform root, CountryTheme theme)
        {
            var crackMaterial = GetMaterial($"{theme.Name} Road Crack", Color.Lerp(theme.Road, Color.black, 0.45f));
            for (var i = 0; i < 3; i++)
            {
                var crack = GameObject.CreatePrimitive(PrimitiveType.Cube);
                crack.name = "Road Crack Decal";
                crack.transform.SetParent(root);
                crack.transform.localPosition = new Vector3(Random.Range(-2.7f, 2.7f), -0.425f, 3.5f + i * 4.8f);
                crack.transform.localRotation = Quaternion.Euler(0f, Random.Range(-18f, 18f), 0f);
                crack.transform.localScale = new Vector3(Random.Range(0.55f, 1.25f), 0.025f, 0.045f);
                crack.GetComponent<Renderer>().sharedMaterial = crackMaterial;
                Destroy(crack.GetComponent<Collider>());
            }

            for (var i = 0; i < 2; i++)
            {
                var reflection = GameObject.CreatePrimitive(PrimitiveType.Cube);
                reflection.name = "Soft Road Reflection";
                reflection.transform.SetParent(root);
                reflection.transform.localPosition = new Vector3(Random.Range(-2f, 2f), -0.415f, 4f + i * 7f);
                reflection.transform.localScale = new Vector3(1.2f, 0.018f, 0.08f);
                reflection.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Road Reflection", new Color(1f, 1f, 1f, 0.18f));
                Destroy(reflection.GetComponent<Collider>());
            }
        }

        private void CreateSidewalks(Transform root, CountryTheme theme)
        {
            for (var side = -1; side <= 1; side += 2)
            {
                var sidewalk = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sidewalk.name = side < 0 ? "Left Sidewalk" : "Right Sidewalk";
                sidewalk.transform.SetParent(root);
                sidewalk.transform.localPosition = new Vector3(side * 5.6f, -0.58f, segmentLength * 0.5f);
                sidewalk.transform.localScale = new Vector3(2.5f, 0.16f, segmentLength);
                sidewalk.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Sidewalk", Color.Lerp(Color.white, theme.Accent, 0.22f));

                var verge = GameObject.CreatePrimitive(PrimitiveType.Cube);
                verge.name = side < 0 ? "Left Green Verge" : "Right Green Verge";
                verge.transform.SetParent(root);
                verge.transform.localPosition = new Vector3(side * 8.1f, -0.62f, segmentLength * 0.5f);
                verge.transform.localScale = new Vector3(2.5f, 0.12f, segmentLength);
                verge.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Verge", Color.Lerp(theme.Accent, Color.black, 0.12f));

                CreateCurb(root, side, theme);
                CreateStreetLamp(root, side, theme, 3.6f);
                CreateFence(root, side, theme, 9f);
                CreateBenchOrBush(root, side, theme, 13.2f);
            }
        }

        private void CreateCurb(Transform root, int side, CountryTheme theme)
        {
            var curb = GameObject.CreatePrimitive(PrimitiveType.Cube);
            curb.name = side < 0 ? "Left Painted Curb" : "Right Painted Curb";
            curb.transform.SetParent(root);
            curb.transform.localPosition = new Vector3(side * 4.72f, -0.32f, segmentLength * 0.5f);
            curb.transform.localScale = new Vector3(0.18f, 0.18f, segmentLength);
            curb.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Curb", Color.Lerp(Color.white, theme.Accent, 0.35f));
        }

        private void CreateStreetLamp(Transform root, int side, CountryTheme theme, float z)
        {
            if (segmentIndex % 2 != 0)
            {
                return;
            }

            var lamp = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lamp.name = "Street Lamp";
            lamp.transform.SetParent(root);
            lamp.transform.localPosition = new Vector3(side * 6.15f, 1.25f, z);
            lamp.transform.localScale = new Vector3(0.08f, 2.2f, 0.08f);
            lamp.GetComponent<Renderer>().sharedMaterial = laneMaterial;

            var lightCap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            lightCap.name = "Lamp Glow";
            lightCap.transform.SetParent(root);
            lightCap.transform.localPosition = new Vector3(side * 6.15f, 2.48f, z);
            lightCap.transform.localScale = Vector3.one * 0.28f;
            lightCap.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Lamp Glow", new Color(1f, 0.88f, 0.45f));
            var light = lightCap.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 3.3f;
            light.intensity = 0.55f;
            light.color = new Color(1f, 0.86f, 0.55f);
        }

        private void CreateFence(Transform root, int side, CountryTheme theme, float z)
        {
            var rail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rail.name = "Decorative Fence";
            rail.transform.SetParent(root);
            rail.transform.localPosition = new Vector3(side * 7f, 0.55f, z);
            rail.transform.localScale = new Vector3(0.12f, 0.55f, 2.8f);
            rail.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Fence", theme.BuildingB);
        }

        private void CreateBenchOrBush(Transform root, int side, CountryTheme theme, float z)
        {
            if (segmentIndex % 3 == 0)
            {
                var bench = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bench.name = "Roadside Bench";
                bench.transform.SetParent(root);
                bench.transform.localPosition = new Vector3(side * 6.45f, 0.32f, z);
                bench.transform.localScale = new Vector3(0.18f, 0.22f, 1.15f);
                bench.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Bench", theme.BuildingA);
            }
            else
            {
                var bush = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                bush.name = "Rounded Bush";
                bush.transform.SetParent(root);
                bush.transform.localPosition = new Vector3(side * 6.55f, 0.2f, z);
                bush.transform.localScale = new Vector3(0.72f, 0.38f, 0.72f);
                bush.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Bush", Color.Lerp(theme.Accent, Color.green, 0.35f));
            }
        }

        private void CreateCollectables(Transform root)
        {
            var pattern = segmentIndex % 5;
            for (var i = 0; i < 4; i++)
            {
                var lane = pattern switch
                {
                    0 => 0,
                    1 => i % 2 == 0 ? -1 : 1,
                    2 => i - 1 < 0 ? -1 : 1,
                    3 => i % 3 - 1,
                    _ => Random.Range(-1, 2)
                };

                CreateCoin(root, lane, 3f + i * 3.2f);
            }

            if (segmentIndex % 4 == 2)
            {
                CreateGem(root, Random.Range(-1, 2), 14f);
            }

            if (segmentIndex % 7 == 3)
            {
                CreateShield(root, Random.Range(-1, 2), 9f);
            }
        }

        private void CreateCoin(Transform root, int lane, float z)
        {
            var coin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            coin.name = "Coin";
            coin.transform.SetParent(root);
            coin.transform.localPosition = new Vector3(lane * 2.5f, 1f, z);
            coin.transform.localScale = new Vector3(0.62f, 0.16f, 0.62f);
            coin.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            coin.GetComponent<Renderer>().sharedMaterial = coinMaterial;
            coin.GetComponent<Collider>().isTrigger = true;
            coin.AddComponent<CollectableSpinner>();
            coin.AddComponent<Collectable>();

            CreateCoinMLogo(coin.transform);

            var glow = coin.AddComponent<Light>();
            glow.type = LightType.Point;
            glow.color = new Color(1f, 0.78f, 0.18f);
            glow.range = 2.2f;
            glow.intensity = 0.45f;
            CreateSparkleParticles(coin.transform, new Color(1f, 0.86f, 0.22f));
        }

        private void CreateGem(Transform root, int lane, float z)
        {
            var gem = new GameObject("Diamond Gem");
            gem.name = "Gem";
            gem.transform.SetParent(root);
            gem.transform.localPosition = new Vector3(lane * 2.5f, 1.25f, z);
            gem.transform.localScale = Vector3.one * 0.72f;
            var meshFilter = gem.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = CreateDiamondMesh();
            gem.AddComponent<MeshRenderer>().sharedMaterial = gemMaterial;
            var collider = gem.AddComponent<SphereCollider>();
            collider.radius = 0.62f;
            collider.isTrigger = true;
            gem.AddComponent<CollectableSpinner>();
            gem.AddComponent<Collectable>().Configure(CollectableType.Gem, 1);
            CreateSparkleParticles(gem.transform, new Color(0.35f, 0.95f, 1f));
        }

        private void CreateShield(Transform root, int lane, float z)
        {
            var shield = new GameObject("Shield Pickup");
            shield.name = "Shield Power-Up";
            shield.transform.SetParent(root);
            shield.transform.localPosition = new Vector3(lane * 2.5f, 1.2f, z);
            shield.transform.localScale = Vector3.one * 1.18f;

            if (shieldPickupSprite != null)
            {
                var renderer = shield.AddComponent<SpriteRenderer>();
                renderer.sprite = shieldPickupSprite;
                renderer.sortingOrder = 12;
                renderer.sharedMaterial = new Material(Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Transparent"))
                {
                    name = "Magic Shield Sprite Material"
                };
            }
            else
            {
                shield.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                var meshFilter = shield.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = CreateShieldMesh();
                shield.AddComponent<MeshRenderer>().sharedMaterial = shieldMaterial;
            }

            var collider = shield.AddComponent<SphereCollider>();
            collider.radius = 0.88f;
            collider.isTrigger = true;
            shield.AddComponent<CollectableSpinner>();
            shield.AddComponent<Collectable>().Configure(CollectableType.Key, 1);
            CreateSparkleParticles(shield.transform, new Color(0.25f, 0.85f, 1f));

            var aura = shield.AddComponent<Light>();
            aura.type = LightType.Point;
            aura.color = new Color(0.18f, 0.78f, 1f);
            aura.range = 2.7f;
            aura.intensity = 0.7f;

            if (shieldPickupSprite == null)
            {
                var crest = GameObject.CreatePrimitive(PrimitiveType.Cube);
                crest.name = "Shield Crest";
                crest.transform.SetParent(shield.transform, false);
                crest.transform.localPosition = new Vector3(0f, 0.1f, -0.035f);
                crest.transform.localScale = new Vector3(0.16f, 0.7f, 0.04f);
                crest.GetComponent<Renderer>().sharedMaterial = laneMaterial;
                Destroy(crest.GetComponent<Collider>());
            }
        }

        private void CreateCoinMLogo(Transform coin)
        {
            var logoMaterial = CreateRuntimeMaterial("Coin M Logo Runtime", new Color(1f, 0.96f, 0.36f));
            for (var i = 0; i < 4; i++)
            {
                var stroke = GameObject.CreatePrimitive(PrimitiveType.Cube);
                stroke.name = "Embossed M Logo";
                stroke.transform.SetParent(coin, false);
                stroke.transform.localPosition = new Vector3(-0.18f + i * 0.12f, 0.54f, i is 1 or 2 ? 0.04f : 0f);
                stroke.transform.localRotation = Quaternion.Euler(90f, 0f, i is 1 ? -28f : i is 2 ? 28f : 0f);
                stroke.transform.localScale = new Vector3(0.05f, 0.22f, 0.035f);
                stroke.GetComponent<Renderer>().sharedMaterial = logoMaterial;
                Destroy(stroke.GetComponent<Collider>());
            }
        }

        private void CreateObstacle(Transform root, CountryTheme theme)
        {
            if (segmentIndex < 2)
            {
                return;
            }

            var obstacleCount = segmentIndex % 6 == 0 ? 2 : 1;
            var usedLane = 99;
            for (var i = 0; i < obstacleCount; i++)
            {
                var lane = Random.Range(-1, 2);
                if (lane == usedLane)
                {
                    lane = Mathf.Clamp(lane + 1, -1, 1);
                }

                usedLane = lane;
                CreateObstacleCart(root, lane, 11f + i * 4f, theme);
            }
        }

        private void CreateObstacleCart(Transform root, int lane, float z, CountryTheme theme)
        {
            var obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacle.name = $"{theme.ObstacleName} Obstacle";
            obstacle.transform.SetParent(root);
            obstacle.transform.localPosition = new Vector3(lane * 2.5f, 0.55f, z);
            obstacle.transform.localScale = new Vector3(1.35f, 1.05f, 0.85f);
            obstacle.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Obstacle", theme.Decoration);
            obstacle.GetComponent<Collider>().isTrigger = true;
            obstacle.AddComponent<Obstacle>();

            var top = GameObject.CreatePrimitive(PrimitiveType.Cube);
            top.name = "Obstacle Canopy";
            top.transform.SetParent(obstacle.transform, false);
            top.transform.localPosition = new Vector3(0f, 0.72f, 0f);
            top.transform.localScale = new Vector3(1.12f, 0.18f, 1.05f);
            top.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Obstacle Top", theme.Accent);
            Destroy(top.GetComponent<Collider>());

            for (var side = -1; side <= 1; side += 2)
            {
                var wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                wheel.name = side < 0 ? "Left Cart Wheel" : "Right Cart Wheel";
                wheel.transform.SetParent(obstacle.transform, false);
                wheel.transform.localPosition = new Vector3(side * 0.46f, -0.54f, -0.18f);
                wheel.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                wheel.transform.localScale = new Vector3(0.24f, 0.08f, 0.24f);
                wheel.GetComponent<Renderer>().sharedMaterial = shadowMaterial;
                Destroy(wheel.GetComponent<Collider>());
            }
        }

        private void CreateDecorations(Transform root, CountryTheme theme)
        {
            for (var side = -1; side <= 1; side += 2)
            {
                var crystal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                crystal.name = side < 0 ? "Left Crystal" : "Right Crystal";
                crystal.transform.SetParent(root);
                crystal.transform.localPosition = new Vector3(side * 4.95f, 0.45f, Random.Range(3f, segmentLength - 3f));
                crystal.transform.localScale = new Vector3(0.22f, Random.Range(0.65f, 1.25f), 0.22f);
                crystal.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Road Decor", theme.Decoration);

                if (segmentIndex % 2 == 0)
                {
                    CreatePoster(root, side, theme, 5.5f + (segmentIndex % 3) * 3f);
                }
            }
        }

        private void CreateCityScenery(Transform root, CountryTheme theme)
        {
            for (var side = -1; side <= 1; side += 2)
            {
                var building = GameObject.CreatePrimitive(PrimitiveType.Cube);
                building.name = side < 0 ? $"Left {theme.Name} Landmark" : $"Right {theme.Name} Landmark";
                building.transform.SetParent(root);
                var height = 1.6f + (segmentIndex % 4) * 0.45f;
                building.transform.localPosition = new Vector3(side * 9.7f, height * 0.5f - 0.55f, 4.5f + (segmentIndex % 3) * 3.5f);
                building.transform.localScale = new Vector3(1.8f, height, 2.4f);
                building.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Building {(segmentIndex + side) % 2}", side < 0 ? theme.BuildingA : theme.BuildingB);

                var roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
                roof.name = "Bright Roof";
                roof.transform.SetParent(building.transform, false);
                roof.transform.localPosition = new Vector3(0f, 0.56f, 0f);
                roof.transform.localScale = new Vector3(1.18f, 0.16f, 1.18f);
                roof.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Roof", theme.Decoration);
                Destroy(roof.GetComponent<Collider>());

                CreateCountrySpecificProp(root, side, theme, 11.5f);
            }
        }

        private void CreateCountryGateIfNeeded(Transform root, CountryTheme theme, float segmentStart)
        {
            if (!StartsCountry(segmentStart, segmentLength, theme))
            {
                return;
            }

            var localZ = Mathf.Clamp(theme.UnlockMeters - segmentStart + 4f, 3f, segmentLength - 3f);
            var gate = new GameObject($"{theme.Name} Entrance Gate");
            gate.transform.SetParent(root);
            gate.transform.localPosition = new Vector3(0f, 0f, localZ);

            var pillarMaterial = GetMaterial($"{theme.Name} Gate Pillars", theme.BuildingA);
            var accentMaterial = GetMaterial($"{theme.Name} Gate Accent", theme.Accent);

            for (var side = -1; side <= 1; side += 2)
            {
                var pillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pillar.name = side < 0 ? "Left Gate Pillar" : "Right Gate Pillar";
                pillar.transform.SetParent(gate.transform, false);
                pillar.transform.localPosition = new Vector3(side * 4.75f, 1.65f, 0f);
                pillar.transform.localScale = new Vector3(0.55f, 3.3f, 0.55f);
                pillar.GetComponent<Renderer>().sharedMaterial = pillarMaterial;

                CreateFlag(gate.transform, side, theme);
            }

            var beam = GameObject.CreatePrimitive(PrimitiveType.Cube);
            beam.name = "Gate Arch Beam";
            beam.transform.SetParent(gate.transform, false);
            beam.transform.localPosition = new Vector3(0f, 3.35f, 0f);
            beam.transform.localScale = new Vector3(9.9f, 0.55f, 0.55f);
            beam.GetComponent<Renderer>().sharedMaterial = accentMaterial;

            var textObject = new GameObject($"WELCOME TO {theme.Name}");
            textObject.transform.SetParent(gate.transform, false);
            textObject.transform.localPosition = new Vector3(0f, 3.45f, -0.36f);
            textObject.transform.localRotation = Quaternion.Euler(12f, 0f, 0f);
            var text = textObject.AddComponent<TextMesh>();
            text.text = $"WELCOME TO {theme.Name}";
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.fontSize = theme.Name.Length > 10 ? 48 : 58;
            text.characterSize = 0.11f;
            text.color = Color.white;

            var particles = new GameObject("Country Entry Celebration");
            particles.transform.SetParent(gate.transform, false);
            particles.transform.localPosition = new Vector3(0f, 2.8f, -0.4f);
            var particleSystem = particles.AddComponent<ParticleSystem>();
            var main = particleSystem.main;
            main.startColor = theme.Accent;
            main.startLifetime = 1.1f;
            main.startSpeed = 2.4f;
            main.startSize = 0.08f;
            main.maxParticles = 60;
            var emission = particleSystem.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0.1f, 42) });
            var shape = particleSystem.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 35f;
            shape.radius = 2.8f;
        }

        private void CreateFlag(Transform parent, int side, CountryTheme theme)
        {
            var pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = side < 0 ? "Left Flag Pole" : "Right Flag Pole";
            pole.transform.SetParent(parent, false);
            pole.transform.localPosition = new Vector3(side * 5.25f, 2.45f, -0.1f);
            pole.transform.localScale = new Vector3(0.05f, 0.72f, 0.05f);
            pole.GetComponent<Renderer>().sharedMaterial = laneMaterial;

            var flag = GameObject.CreatePrimitive(PrimitiveType.Cube);
            flag.name = $"{theme.Name} Flag";
            flag.transform.SetParent(parent, false);
            flag.transform.localPosition = new Vector3(side * 5.48f, 2.78f, -0.1f);
            flag.transform.localScale = new Vector3(0.42f, 0.28f, 0.035f);
            flag.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Flag", theme.Decoration);
        }

        private void CreatePoster(Transform root, int side, CountryTheme theme, float z)
        {
            var poster = GameObject.CreatePrimitive(PrimitiveType.Cube);
            poster.name = $"{theme.Name} Billboard";
            poster.transform.SetParent(root);
            poster.transform.localPosition = new Vector3(side * 6.75f, 1.35f, z);
            poster.transform.localRotation = Quaternion.Euler(0f, side < 0 ? 18f : -18f, 0f);
            poster.transform.localScale = new Vector3(1.8f, 1.05f, 0.08f);
            poster.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Poster Board", theme.BuildingB);

            var labelObject = new GameObject("Poster Label");
            labelObject.transform.SetParent(poster.transform, false);
            labelObject.transform.localPosition = new Vector3(0f, 0f, -0.08f);
            labelObject.transform.localRotation = Quaternion.identity;
            var text = labelObject.AddComponent<TextMesh>();
            text.text = theme.PosterText;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.fontSize = 34;
            text.characterSize = 0.07f;
            text.color = Color.white;
        }

        private void CreateCountrySpecificProp(Transform root, int side, CountryTheme theme, float z)
        {
            var x = side * 7.9f;
            switch (theme.Name)
            {
                case "INDIA":
                    CreateStackedTower(root, "Temple Tower", x, z, theme);
                    CreateTree(root, "Coconut Tree", side * 6.2f, z - 3f, theme, true);
                    break;
                case "USA":
                    CreateTrafficLight(root, x, z, theme);
                    break;
                case "JAPAN":
                    CreateTree(root, "Cherry Blossom Tree", x, z, theme, false);
                    break;
                case "FRANCE":
                    CreateSimpleTower(root, "Eiffel Inspired Tower", x, z, theme);
                    break;
                case "EGYPT":
                    CreatePyramid(root, x, z, theme);
                    break;
                case "DUBAI":
                    CreateSimpleTower(root, "Burj Inspired Spire", x, z, theme);
                    break;
                case "CHINA":
                    CreateLantern(root, x, z, theme);
                    break;
                case "SWITZERLAND":
                    CreateTree(root, "Pine Tree", x, z, theme, false);
                    break;
                case "BRAZIL":
                    CreateTree(root, "Palm Tree", x, z, theme, true);
                    break;
                default:
                    CreatePlanet(root, x, z, theme);
                    break;
            }
        }

        private void CreateStackedTower(Transform root, string name, float x, float z, CountryTheme theme)
        {
            var basePart = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            basePart.name = name;
            basePart.transform.SetParent(root);
            basePart.transform.localPosition = new Vector3(x, 0.55f, z);
            basePart.transform.localScale = new Vector3(0.7f, 1.1f, 0.7f);
            basePart.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Tower Base", theme.BuildingA);

            var top = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            top.name = $"{name} Crown";
            top.transform.SetParent(root);
            top.transform.localPosition = new Vector3(x, 1.55f, z);
            top.transform.localScale = new Vector3(0.42f, 0.5f, 0.42f);
            top.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Tower Crown", theme.Accent);
        }

        private void CreateSimpleTower(Transform root, string name, float x, float z, CountryTheme theme)
        {
            var tower = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tower.name = name;
            tower.transform.SetParent(root);
            tower.transform.localPosition = new Vector3(x, 1.4f, z);
            tower.transform.localScale = new Vector3(0.5f, 2.8f, 0.5f);
            tower.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Tower", theme.BuildingB);

            var spire = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            spire.name = $"{name} Spire";
            spire.transform.SetParent(root);
            spire.transform.localPosition = new Vector3(x, 3.05f, z);
            spire.transform.localScale = new Vector3(0.13f, 0.65f, 0.13f);
            spire.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Spire", theme.Decoration);
        }

        private void CreatePyramid(Transform root, float x, float z, CountryTheme theme)
        {
            var pyramid = new GameObject("Pyramid Landmark");
            pyramid.transform.SetParent(root);
            pyramid.transform.localPosition = new Vector3(x, 0.15f, z);
            pyramid.AddComponent<MeshFilter>().sharedMesh = CreatePyramidMesh();
            pyramid.AddComponent<MeshRenderer>().sharedMaterial = GetMaterial($"{theme.Name} Pyramid", theme.Accent);
        }

        private void CreateTree(Transform root, string name, float x, float z, CountryTheme theme, bool palm)
        {
            var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = name;
            trunk.transform.SetParent(root);
            trunk.transform.localPosition = new Vector3(x, 0.75f, z);
            trunk.transform.localScale = new Vector3(0.18f, 1.45f, 0.18f);
            trunk.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Tree Trunk", new Color(0.45f, 0.25f, 0.1f));

            var leaves = GameObject.CreatePrimitive(palm ? PrimitiveType.Cube : PrimitiveType.Sphere);
            leaves.name = $"{name} Leaves";
            leaves.transform.SetParent(root);
            leaves.transform.localPosition = new Vector3(x, 1.95f, z);
            leaves.transform.localScale = palm ? new Vector3(1.25f, 0.18f, 0.42f) : new Vector3(0.95f, 0.95f, 0.95f);
            leaves.transform.localRotation = Quaternion.Euler(0f, 35f, 0f);
            leaves.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Leaves", theme.Accent);
        }

        private void CreateTrafficLight(Transform root, float x, float z, CountryTheme theme)
        {
            var pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "Traffic Light Pole";
            pole.transform.SetParent(root);
            pole.transform.localPosition = new Vector3(x, 0.9f, z);
            pole.transform.localScale = new Vector3(0.08f, 1.7f, 0.08f);
            pole.GetComponent<Renderer>().sharedMaterial = laneMaterial;

            var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = "Traffic Light Box";
            box.transform.SetParent(root);
            box.transform.localPosition = new Vector3(x, 2f, z);
            box.transform.localScale = new Vector3(0.35f, 0.75f, 0.25f);
            box.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Traffic Light", theme.BuildingB);
        }

        private void CreateLantern(Transform root, float x, float z, CountryTheme theme)
        {
            var lantern = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            lantern.name = "Red Lantern";
            lantern.transform.SetParent(root);
            lantern.transform.localPosition = new Vector3(x, 1.55f, z);
            lantern.transform.localScale = new Vector3(0.46f, 0.62f, 0.46f);
            lantern.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Lantern", theme.Decoration);
        }

        private void CreatePlanet(Transform root, float x, float z, CountryTheme theme)
        {
            var planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            planet.name = "Floating Planet";
            planet.transform.SetParent(root);
            planet.transform.localPosition = new Vector3(x, 2.35f, z);
            planet.transform.localScale = Vector3.one * 0.85f;
            planet.GetComponent<Renderer>().sharedMaterial = GetMaterial($"{theme.Name} Planet", theme.Decoration);
        }

        private static Material CreateRuntimeMaterial(string name, Color color)
        {
            var shader = Shader.Find("Standard") ?? Shader.Find("Diffuse") ?? Shader.Find("Unlit/Color");
            var material = new Material(shader)
            {
                name = name,
                color = color
            };

            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", color);
            }

            if (color.a < 0.99f)
            {
                material.SetFloat("_Mode", 3f);
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
            }

            return material;
        }

        private Material GetMaterial(string name, Color color)
        {
            if (materialCache.TryGetValue(name, out var cached))
            {
                return cached;
            }

            var material = CreateRuntimeMaterial(name, color);
            materialCache[name] = material;
            return material;
        }

        private static void SetMetallic(Material material, float metallic, float smoothness)
        {
            if (material == null)
            {
                return;
            }

            if (material.HasProperty("_Metallic"))
            {
                material.SetFloat("_Metallic", metallic);
            }

            if (material.HasProperty("_Glossiness"))
            {
                material.SetFloat("_Glossiness", smoothness);
            }
        }

        private static void CreateSparkleParticles(Transform parent, Color color)
        {
            var sparkles = new GameObject("Gem Sparkles");
            sparkles.transform.SetParent(parent, false);
            sparkles.transform.localPosition = Vector3.zero;
            var particles = sparkles.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.startLifetime = 0.65f;
            main.startSpeed = 0.18f;
            main.startSize = 0.045f;
            main.startColor = color;
            main.maxParticles = 18;
            var emission = particles.emission;
            emission.rateOverTime = 10f;
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.48f;
            var renderer = particles.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
        }

        private static Mesh CreateStarMesh()
        {
            var vertices = new List<Vector3> { Vector3.zero };
            for (var i = 0; i < 10; i++)
            {
                var angle = Mathf.Deg2Rad * (90f + i * 36f);
                var radius = i % 2 == 0 ? 0.5f : 0.22f;
                vertices.Add(new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f));
            }

            var triangles = new List<int>();
            for (var i = 1; i <= 10; i++)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i == 10 ? 1 : i + 1);
            }

            var mesh = new Mesh
            {
                name = "Coin Embossed Star Mesh"
            };
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh CreateDiamondMesh()
        {
            var mesh = new Mesh
            {
                name = "Diamond Gem Mesh"
            };
            mesh.vertices = new[]
            {
                new Vector3(0f, 0.72f, 0f),
                new Vector3(0.48f, 0f, 0f),
                new Vector3(0f, 0f, 0.48f),
                new Vector3(-0.48f, 0f, 0f),
                new Vector3(0f, 0f, -0.48f),
                new Vector3(0f, -0.72f, 0f)
            };
            mesh.triangles = new[]
            {
                0, 1, 2,
                0, 2, 3,
                0, 3, 4,
                0, 4, 1,
                5, 2, 1,
                5, 3, 2,
                5, 4, 3,
                5, 1, 4
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh CreatePyramidMesh()
        {
            var mesh = new Mesh
            {
                name = "Low Poly Pyramid Mesh"
            };
            mesh.vertices = new[]
            {
                new Vector3(-1f, 0f, -1f),
                new Vector3(1f, 0f, -1f),
                new Vector3(1f, 0f, 1f),
                new Vector3(-1f, 0f, 1f),
                new Vector3(0f, 1.45f, 0f)
            };
            mesh.triangles = new[]
            {
                0, 1, 4,
                1, 2, 4,
                2, 3, 4,
                3, 0, 4,
                0, 3, 2,
                0, 2, 1
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static Mesh CreateShieldMesh()
        {
            var mesh = new Mesh
            {
                name = "Shield Pickup Mesh"
            };
            mesh.vertices = new[]
            {
                new Vector3(0f, 0.72f, -0.04f),
                new Vector3(0.55f, 0.42f, -0.04f),
                new Vector3(0.44f, -0.22f, -0.04f),
                new Vector3(0f, -0.72f, -0.04f),
                new Vector3(-0.44f, -0.22f, -0.04f),
                new Vector3(-0.55f, 0.42f, -0.04f),
                new Vector3(0f, 0.72f, 0.04f),
                new Vector3(0.55f, 0.42f, 0.04f),
                new Vector3(0.44f, -0.22f, 0.04f),
                new Vector3(0f, -0.72f, 0.04f),
                new Vector3(-0.44f, -0.22f, 0.04f),
                new Vector3(-0.55f, 0.42f, 0.04f)
            };
            mesh.triangles = new[]
            {
                0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5,
                6, 8, 7, 6, 9, 8, 6, 10, 9, 6, 11, 10,
                0, 6, 7, 0, 7, 1,
                1, 7, 8, 1, 8, 2,
                2, 8, 9, 2, 9, 3,
                3, 9, 10, 3, 10, 4,
                4, 10, 11, 4, 11, 5,
                5, 11, 6, 5, 6, 0
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

    }
}
