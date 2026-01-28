using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace GA.GameScreen
{
    public class ImageWebLoader : MonoBehaviour
    {
        [SerializeField]
        private string baseUrl = "https://data.ikppbb.com/test-task-unity-data/pics";

        private readonly Dictionary<int, Sprite> cache = new();

        public IEnumerator Load(int index, System.Action<Sprite> onComplete, System.Action onStarted)
        {
            if (cache.TryGetValue(index, out var cached))
            {
                Debug.Log($"[LOADER] CACHE HIT index={index}");
                onComplete?.Invoke(cached);
                yield break;
            }

            onStarted?.Invoke(); // <<< ÒÎËÜÊÎ çäåñü

            string url = $"{baseUrl}/{index}.jpg";
            Debug.Log($"[LOADER] REQUEST index={index} url={url}");

            using var req = UnityWebRequestTexture.GetTexture(url);
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
                yield break;

            var tex = DownloadHandlerTexture.GetContent(req);

            var sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                100f
            );

            cache[index] = sprite;
            onComplete?.Invoke(sprite);
        }

    }
}
