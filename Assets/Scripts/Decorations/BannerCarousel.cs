using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace GA.Decorations
{
    public class BannerCarousel : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Movement")]
        [SerializeField] private RectTransform content;
        [SerializeField] private float moveDuration = 0.6f;
        [SerializeField] private float autoScrollDelay = 5f;
        [SerializeField] private float dragThreshold = 80f;
        [SerializeField] private AnimationCurve moveCurve;

        [Header("Dots")]
        [SerializeField] private Image[] dots;
        [SerializeField] private Sprite activeDot;
        [SerializeField] private Sprite inactiveDot;

        private readonly float[] positions =
        {
            -720f,   // banner 0
            -2796f,  // banner 1
            -5214f,  // banner 2
            -7435f   // copy of banner 0
        };

        private int index;
        private bool isMoving;
        private bool dragHandled;

        private Vector2 dragStart;
        private Coroutine autoScrollCoroutine;

        private void OnEnable()
        {
            index = 0;
            content.anchoredPosition = new Vector2(positions[0], 0f);
            UpdateDots();
            StartAutoScroll();
        }

        private void OnDisable()
        {
            StopAutoScroll();
        }

        // -------------------- Drag --------------------

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isMoving) return;

            dragHandled = false;
            dragStart = eventData.position;

            StopAutoScroll();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isMoving || dragHandled) return;

            float deltaX = eventData.position.x - dragStart.x;

            if (Mathf.Abs(deltaX) < dragThreshold)
                return;

            dragHandled = true;

            if (deltaX < 0f)
                ScrollForward();
            else
                ScrollBackward();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            StartAutoScroll();
        }

        // -------------------- Auto scroll --------------------

        private void StartAutoScroll()
        {
            StopAutoScroll();
            autoScrollCoroutine = StartCoroutine(AutoScrollRoutine());
        }

        private void StopAutoScroll()
        {
            if (autoScrollCoroutine != null)
            {
                StopCoroutine(autoScrollCoroutine);
                autoScrollCoroutine = null;
            }
        }

        private IEnumerator AutoScrollRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(autoScrollDelay);
                ScrollForward();
            }
        }

        // -------------------- Navigation --------------------

        private void ScrollForward()
        {
            if (isMoving) return;

            index++;
            StartCoroutine(MoveToIndex());
        }

        private void ScrollBackward()
        {
            if (isMoving) return;

            if (index == 0)
            {
                index = positions.Length - 1;
                content.anchoredPosition = new Vector2(positions[index], 0f);
            }

            index--;
            StartCoroutine(MoveToIndex());
        }

        private IEnumerator MoveToIndex()
        {
            isMoving = true;

            float startX = content.anchoredPosition.x;
            float targetX = positions[index];

            float time = 0f;
            while (time < 1f)
            {
                time += Time.deltaTime / moveDuration;
                float x = Mathf.Lerp(startX, targetX, moveCurve.Evaluate(time));
                content.anchoredPosition = new Vector2(x, 0f);
                yield return null;
            }

            content.anchoredPosition = new Vector2(targetX, 0f);

            if (index == positions.Length - 1)
            {
                index = 0;
                content.anchoredPosition = new Vector2(positions[0], 0f);
            }

            UpdateDots();
            isMoving = false;
        }

        // -------------------- UI --------------------

        private void UpdateDots()
        {
            int dotIndex = index;
            if (dotIndex >= dots.Length)
                dotIndex = 0;

            for (int i = 0; i < dots.Length; i++)
                dots[i].sprite = i == dotIndex ? activeDot : inactiveDot;
        }
    }
}
