using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GA.GameScreen
{
    public class GalleryLoader : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField] private RectTransform content;
        [SerializeField] private ScrollRect scroll;
        [SerializeField] private ArtObjectView prefab;
        [SerializeField] private int viewCount = 66;

        [Header("Grid Settings")]
        [SerializeField] private GridLayoutGroup grid;
        [SerializeField] private Vector2 androidCellSize = new(640f, 640f);
        [SerializeField] private Vector2 iosCellSize = new(420f, 420f);
        [SerializeField] private bool forceIOSLayoutInEditor;


        private readonly List<ArtObjectView> items = new();

        public event Action<IReadOnlyList<ArtObjectView>> ViewportChanged;

        private void Awake()
        {
            if (grid == null)
                grid = content.GetComponent<GridLayoutGroup>();

            scroll.onValueChanged.AddListener(_ => CheckViewport());
        }

        public void Build(Func<int, bool> filter)
        {
            ApplyPlatformGrid();

            foreach (Transform c in content)
                Destroy(c.gameObject);

            items.Clear();

            for (int i = 1; i <= viewCount; i++)
            {
                if (!filter(i))
                    continue;

                var view = Instantiate(prefab, content);
                view.Init(i);
                items.Add(view);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            CheckViewport();
        }

        private void ApplyPlatformGrid()
        {
#if UNITY_IOS
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 3;
            grid.cellSize = iosCellSize;
#elif UNITY_ANDROID
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 2;
            grid.cellSize = androidCellSize;
#else
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 2;
            grid.cellSize = androidCellSize;
#endif
#if UNITY_EDITOR
            if (forceIOSLayoutInEditor)
            {
                grid.constraintCount = 3;
                grid.cellSize = iosCellSize;
                return;
            }
#endif

        }

        private void CheckViewport()
        {
            var visible = new List<ArtObjectView>();
            Rect viewportRect = GetWorldRect(scroll.viewport);

            foreach (var view in items)
            {
                Rect viewRect = GetWorldRect((RectTransform)view.transform);
                bool isVisible = viewportRect.Overlaps(viewRect);

                view.SetVisible(isVisible);

                if (isVisible)
                    visible.Add(view);
            }

            ViewportChanged?.Invoke(visible);
        }

        private Rect GetWorldRect(RectTransform rt)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            return new Rect(corners[0], corners[2] - corners[0]);
        }
    }
}
