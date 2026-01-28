using UnityEngine;

namespace GA.Decorations
{
    public class ZPingPongRotation : MonoBehaviour
    {
        [SerializeField] private Vector2 zRange = new Vector2(-10f, 10f);
        [SerializeField] private float speed = 1f;
        [SerializeField]
        private AnimationCurve curve =
            AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private float t;
        private float direction = 1f;

        private void Update()
        {
            t += Time.deltaTime * speed * direction;

            if (t >= 1f)
            {
                t = 1f;
                direction = -1f;
            }
            else if (t <= 0f)
            {
                t = 0f;
                direction = 1f;
            }

            float eased = curve.Evaluate(t);
            float z = Mathf.Lerp(zRange.x, zRange.y, eased);

            Vector3 euler = transform.localEulerAngles;
            euler.z = z;
            transform.localEulerAngles = euler;
        }
    }
}