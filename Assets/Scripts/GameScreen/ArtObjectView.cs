using UnityEngine;
using UnityEngine.UI;
using System;

namespace GA.GameScreen
{
    public class ArtObjectView : MonoBehaviour
    {
        public int Index { get; private set; }
        public bool IsLoaded { get; private set; }
        public bool IsVisible { get; private set; }
        public bool IsPremium => Index % 4 == 0;
        public Sprite Sprite => image.sprite;

        public event Action<ArtObjectView> Clicked;

        [SerializeField] private Image image;
        [SerializeField] private GameObject premiumBadge;
        [SerializeField] private GameObject loadingObject;
        [SerializeField] private Button button;

        private Coroutine loadRoutine;

        public void Init(int index)
        {
            Index = index;
            IsLoaded = false;

            image.sprite = null;
            premiumBadge.SetActive(IsPremium);
            loadingObject.SetActive(false);

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => Clicked?.Invoke(this));

            Debug.Log($"[VIEW] INIT index={Index}");
        }

        public void SetVisible(bool visible)
        {
            if (IsVisible == visible)
                return;

            IsVisible = visible;

            Debug.Log($"[VIEW] {(visible ? "ENTER" : "EXIT")} viewport index={Index}");

            if (!visible)
                CancelLoading();
        }

        public void StartLoading(ImageWebLoader loader)
        {
            if (IsLoaded || loadRoutine != null)
                return;

            Debug.Log($"[VIEW] START loading index={Index}");

            loadRoutine = StartCoroutine(
                loader.Load(
                    Index,
                    OnLoaded,
                    () =>
                    {
                        loadingObject.SetActive(true);
                        Debug.Log($"[VIEW] SHOW loading index={Index}");
                    })
            );
        }


        private void OnLoaded(Sprite sprite)
        {
            if (!IsVisible)
            {
                Debug.Log($"[VIEW] DROP loaded sprite (not visible) index={Index}");
                return;
            }

            IsLoaded = true;
            image.sprite = sprite;
            loadingObject.SetActive(false);
            loadRoutine = null;

            Debug.Log($"[VIEW] LOADED index={Index}");
        }

        private void CancelLoading()
        {
            if (loadRoutine != null)
            {
                Debug.Log($"[VIEW] CANCEL loading index={Index}");
                StopCoroutine(loadRoutine);
                loadRoutine = null;
                loadingObject.SetActive(false);
            }
        }
    }
}
