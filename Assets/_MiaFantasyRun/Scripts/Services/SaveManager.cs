using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace MiaFantasyRun.Services
{
    public sealed class SaveManager : MonoBehaviour, ISaveService
    {
        private const string FileName = "mia_save.dat";
        private const string SaveSecret = "mia-fantasy-run-local-save-v1";
        private string path;
        private SaveData cache;

        public void Initialize()
        {
            path = Path.Combine(Application.persistentDataPath, FileName);
            cache = Load();
        }

        public SaveData Load()
        {
            if (cache != null)
            {
                return cache;
            }

            if (!File.Exists(path))
            {
                cache = new SaveData();
                return cache;
            }

            try
            {
                var envelope = JsonUtility.FromJson<SignedSaveEnvelope>(File.ReadAllText(path));
                if (envelope == null || envelope.Signature != Sign(envelope.Payload))
                {
                    throw new InvalidOperationException("Save signature mismatch.");
                }

                var json = Encoding.UTF8.GetString(Convert.FromBase64String(envelope.Payload));
                cache = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Save load failed, creating a clean save. {exception.Message}");
                cache = new SaveData();
            }

            return cache;
        }

        public void Save(SaveData data)
        {
            cache = data;
            var json = JsonUtility.ToJson(data);
            var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            var envelope = new SignedSaveEnvelope
            {
                Payload = payload,
                Signature = Sign(payload)
            };
            File.WriteAllText(path, JsonUtility.ToJson(envelope));
        }

        public void Delete()
        {
            cache = new SaveData();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private static string Sign(string payload)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(payload + SaveSecret + SystemInfo.deviceUniqueIdentifier);
            return Convert.ToBase64String(sha.ComputeHash(bytes));
        }

        [Serializable]
        private sealed class SignedSaveEnvelope
        {
            public string Payload;
            public string Signature;
        }
    }
}
