#if UNITY_EDITOR
using System.IO;
using MiaFantasyRun.Data;
using UnityEditor;
using UnityEngine;

namespace MiaFantasyRun.Editor
{
    public static class CharacterDataGenerator
    {
        private const string CharacterDataFolder = "Assets/_MiaFantasyRun/ScriptableObjects/Characters";
        private const string CharacterPrefabFolder = "Assets/_MiaFantasyRun/Prefabs/Characters";

        private static readonly (string name, int cost, Color color)[] Characters =
        {
            ("Mia", 0, new Color(0.04f, 0.68f, 0.82f)),
            ("Emma", 1200, new Color(1f, 0.48f, 0.62f)),
            ("Aurora", 2500, new Color(0.72f, 0.42f, 1f)),
            ("Luna", 1800, new Color(0.24f, 0.36f, 0.9f)),
            ("Olivia", 2000, new Color(0.18f, 0.76f, 0.42f)),
            ("Sophia", 2200, new Color(1f, 0.72f, 0.18f))
        };

        [MenuItem("Mia Fantasy Run/Generate Character Data")]
        public static void Generate()
        {
            Directory.CreateDirectory(CharacterDataFolder);
            Directory.CreateDirectory(CharacterPrefabFolder);

            foreach (var character in Characters)
            {
                var prefab = CreateOrUpdatePrefab(character.name, character.color);
                CreateOrUpdateData(character.name, character.cost, prefab);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static GameObject CreateOrUpdatePrefab(string characterName, Color color)
        {
            var prefabPath = $"{CharacterPrefabFolder}/{characterName}.prefab";
            var root = new GameObject(characterName);
            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = new Vector3(0f, 0.9f, 0f);
            body.transform.localScale = new Vector3(0.7f, 0.9f, 0.45f);
            body.GetComponent<Renderer>().sharedMaterial = CreateMaterial($"{characterName} Outfit", color);
            Object.DestroyImmediate(body.GetComponent<Collider>());

            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.SetParent(root.transform, false);
            head.transform.localPosition = new Vector3(0f, 1.75f, 0f);
            head.transform.localScale = Vector3.one * 0.55f;
            head.GetComponent<Renderer>().sharedMaterial = CreateMaterial($"{characterName} Skin", new Color(1f, 0.74f, 0.58f));
            Object.DestroyImmediate(head.GetComponent<Collider>());

            var hair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hair.name = "Hair";
            hair.transform.SetParent(root.transform, false);
            hair.transform.localPosition = new Vector3(0f, 1.96f, -0.04f);
            hair.transform.localScale = new Vector3(0.62f, 0.35f, 0.58f);
            hair.GetComponent<Renderer>().sharedMaterial = CreateMaterial($"{characterName} Hair", new Color(0.23f, 0.12f, 0.07f));
            Object.DestroyImmediate(hair.GetComponent<Collider>());

            var savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            Object.DestroyImmediate(root);
            return savedPrefab;
        }

        private static void CreateOrUpdateData(string characterName, int unlockCost, GameObject prefab)
        {
            var path = $"{CharacterDataFolder}/{characterName}.asset";
            var data = AssetDatabase.LoadAssetAtPath<CharacterData>(path);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<CharacterData>();
                AssetDatabase.CreateAsset(data, path);
            }

            data.characterName = characterName;
            data.unlockCost = unlockCost;
            data.prefab = prefab;
            EditorUtility.SetDirty(data);
        }

        private static Material CreateMaterial(string name, Color color)
        {
            var material = new Material(Shader.Find("Standard") ?? Shader.Find("Diffuse"))
            {
                name = name,
                color = color
            };
            return material;
        }
    }
}
#endif
