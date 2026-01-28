using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GA.GameScreen
{
    public class GalleryVirtualizedList : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private ScrollRect scroll;
        [SerializeField] private RectTransform content;
        [SerializeField] private GridLayoutGroup grid;

        [Header("Pool")]
        [SerializeField] private ArtObjectView prefab;
        [SerializeField] private int poolSize = 12;

        public event Action<IReadOnlyCollection<ArtObjectView>> VisibleViewsChanged;


        private readonly List<int> dataIndices = new();
        private readonly Dictionary<int, ArtObjectView> activeViews = new();
        private readonly Queue<ArtObjectView> freeViews = new();

        private int columnCount;
        private float itemHeight;

        private void Awake()
        {
            columnCount = grid.constraintCount;
            itemHeight = grid.cellSize.y + grid.spacing.y;

            for (int i = 0; i < poolSize; i++)
            {
                var view = Instantiate(prefab, content);
                view.gameObject.SetActive(false);
                freeViews.Enqueue(view);
            }

            scroll.onValueChanged.AddListener(_ =>
            {
                ClampScroll();
                UpdateVisible();
            });
        }

        // ---------- DATA ----------

        public void SetFilter(Func<int, bool> predicate)
        {
            dataIndices.Clear();
            activeViews.Clear();

            for (int i = 1; i <= 66; i++)
                if (predicate(i))
                    dataIndices.Add(i);

            UpdateContentHeight();
            ClampScroll();
            UpdateVisible(true);
        }

        private void UpdateContentHeight()
        {
            int rows = Mathf.CeilToInt((float)dataIndices.Count / columnCount);

            float height =
                grid.padding.top +
                rows * grid.cellSize.y +
                Mathf.Max(0, rows - 1) * grid.spacing.y +
                grid.padding.bottom;

            content.sizeDelta = new Vector2(content.sizeDelta.x, height);
        }

        // ---------- SCROLL ----------

        private void ClampScroll()
        {
            float viewportHeight = scroll.viewport.rect.height;
            float contentHeight = content.sizeDelta.y;

            float maxY = Mathf.Max(0f, contentHeight - viewportHeight);
            Vector2 pos = content.anchoredPosition;
            pos.y = Mathf.Clamp(pos.y, 0f, maxY);
            content.anchoredPosition = pos;
        }

        // ---------- VIRTUALIZATION ----------

        private void UpdateVisible(bool force = false)
        {
            if (dataIndices.Count == 0)
                return;

            float scrollTop = content.anchoredPosition.y;
            float scrollBottom = scrollTop + scroll.viewport.rect.height;

            int firstVisibleRow = Mathf.FloorToInt(scrollTop / itemHeight);
            int lastVisibleRow = Mathf.CeilToInt(scrollBottom / itemHeight);

            int firstIndex = firstVisibleRow * columnCount;
            int lastIndex = Mathf.Min(
                dataIndices.Count - 1,
                lastVisibleRow * columnCount + (columnCount - 1)
            );

            var stillVisible = new HashSet<int>();

            for (int i = firstIndex; i <= lastIndex; i++)
            {
                if (i < 0 || i >= dataIndices.Count)
                    continue;

                int dataIndex = dataIndices[i];
                stillVisible.Add(dataIndex);

                if (activeViews.ContainsKey(dataIndex))
                    continue;

                if (freeViews.Count == 0)
                    break;

                var view = freeViews.Dequeue();
                activeViews[dataIndex] = view;

                PositionView(view, i);
                view.gameObject.SetActive(true);
                view.Init(dataIndex);
            }

            // убрать вышедшие из viewport
            var toRemove = new List<int>();

            foreach (var kv in activeViews)
            {
                if (!stillVisible.Contains(kv.Key))
                {
                    kv.Value.gameObject.SetActive(false);
                    freeViews.Enqueue(kv.Value);
                    toRemove.Add(kv.Key);
                }
            }

            foreach (var idx in toRemove)
                activeViews.Remove(idx);

            VisibleViewsChanged?.Invoke(activeViews.Values);
        }

        private void PositionView(ArtObjectView view, int linearIndex)
        {
            int row = linearIndex / columnCount;
            int col = linearIndex % columnCount;

            float totalWidth =
                columnCount * grid.cellSize.x +
                (columnCount - 1) * grid.spacing.x;

            float startX =
                grid.padding.left +
                (content.rect.width - totalWidth) * 0.5f;

            float x = startX + col * (grid.cellSize.x + grid.spacing.x);
            float y = -grid.padding.top - row * itemHeight;

            ((RectTransform)view.transform).anchoredPosition = new Vector2(x, y);
        }

        public void SetColumnCount(int count)
        {
            grid.constraintCount = count;
            columnCount = count;

            UpdateContentHeight();
            ClampScroll();
            UpdateVisible(true);
        }
    }
}
