using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace GA.Decorations {
    public class GarlandBlink : MonoBehaviour
    {
        [SerializeField] private Image[] bulbs; // строго 6
        [SerializeField] private float stepDelay = 0.25f;

        private int patternIndex;

        private List<int[]> patterns;

        private void Awake()
        {
            BuildPatterns();
        }

        private void OnEnable()
        {
            StartCoroutine(BlinkRoutine());
        }

        private IEnumerator BlinkRoutine()
        {
            while (true)
            {
                DisableAll();

                int[] pattern = patterns[patternIndex];
                for (int i = 0; i < pattern.Length; i++)
                    bulbs[pattern[i]].enabled = true;

                patternIndex++;
                if (patternIndex >= patterns.Count)
                    patternIndex = 0;

                yield return new WaitForSeconds(stepDelay);
            }
        }

        private void DisableAll()
        {
            for (int i = 0; i < bulbs.Length; i++)
                bulbs[i].enabled = false;
        }

        private void BuildPatterns()
        {
            patterns = new List<int[]>
        {
            // одиночные
            new[] { 0 },
            new[] { 1 },
            new[] { 2 },
            new[] { 3 },
            new[] { 4 },
            new[] { 5 },

            // парные (через одну)
            new[] { 0, 2 },
            new[] { 1, 3 },
            new[] { 2, 4 },
            new[] { 3, 5 },

            // симметрия
            new[] { 0, 5 },
            new[] { 1, 4 },
            new[] { 2, 3 },

            // тройки
            new[] { 0, 2, 4 },
            new[] { 1, 3, 5 },

            // волна слева направо
            new[] { 0, 1 },
            new[] { 1, 2 },
            new[] { 2, 3 },
            new[] { 3, 4 },
            new[] { 4, 5 },

            // волна обратно
            new[] { 5, 4 },
            new[] { 4, 3 },
            new[] { 3, 2 },
            new[] { 2, 1 },
            new[] { 1, 0 },

            // вспышки
            new[] { 0, 1, 2 },
            new[] { 3, 4, 5 },
            new[] { 0, 1, 2, 3, 4, 5 }
        };
        }
    }
}
