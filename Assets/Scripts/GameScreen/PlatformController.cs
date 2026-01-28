using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace GA.GameScreen
{
    public class PlatformController : MonoBehaviour
    {
        [SerializeField] private GalleryLoader gallery;
        [SerializeField] private ImageWebLoader webLoader;
        [SerializeField] private float loadDelay = 0.3f;

        [Header("Buttons")]
        [SerializeField] private ButtonSelect all;
        [SerializeField] private ButtonSelect odd;
        [SerializeField] private ButtonSelect even;

        [Header("Preview")]
        [SerializeField] private GameObject previewMenu;
        [SerializeField] private Image previewImage;

        [Header("Premium")]
        [SerializeField] private GameObject premiumMenu;

        private Coroutine delayedRoutine;

        private void Start()
        {
            Debug.Log("[PLATFORM] START");

            gallery.ViewportChanged += OnViewportChanged;

            all.GetComponent<Button>().onClick.AddListener(() => ApplyFilter(_ => true, all));
            odd.GetComponent<Button>().onClick.AddListener(() => ApplyFilter(i => i % 2 == 1, odd));
            even.GetComponent<Button>().onClick.AddListener(() => ApplyFilter(i => i % 2 == 0, even));

            ApplyFilter(_ => true, all);
        }

        private void OnDestroy()
        {
            gallery.ViewportChanged -= OnViewportChanged;
        }

        private void ApplyFilter(System.Func<int, bool> filter, ButtonSelect selected)
        {
            Debug.Log("[PLATFORM] APPLY FILTER");

            all.Select(false);
            odd.Select(false);
            even.Select(false);
            selected.Select(true);

            gallery.Build(filter);
        }

        private void OnViewportChanged(IReadOnlyList<ArtObjectView> views)
        {
            Debug.Log($"[PLATFORM] VIEWPORT CHANGED visible={views.Count}");

            if (delayedRoutine != null)
                StopCoroutine(delayedRoutine);

            foreach (var v in views)
            {
                v.Clicked -= OnArtClicked;
                v.Clicked += OnArtClicked;
            }

            delayedRoutine = StartCoroutine(DelayedLoad(views));
        }

        private IEnumerator DelayedLoad(IReadOnlyList<ArtObjectView> views)
        {
            Debug.Log($"[PLATFORM] DELAY load {loadDelay}s");

            yield return new WaitForSeconds(loadDelay);

            foreach (var view in views)
            {
                Debug.Log($"[PLATFORM] REQUEST LOAD index={view.Index}");
                view.StartLoading(webLoader);
            }
        }

        private void OnArtClicked(ArtObjectView view)
        {
            Debug.Log($"[PLATFORM] CLICK index={view.Index}");

            if (!view.IsLoaded)
                return;

            if (view.IsPremium)
            {
                premiumMenu.SetActive(true);
                previewMenu.SetActive(false);
                return;
            }

            previewImage.sprite = view.Sprite;
            previewMenu.SetActive(true);
            premiumMenu.SetActive(false);
        }
    }
}
