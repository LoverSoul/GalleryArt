using UnityEngine;
namespace GA.Decorations
{
    public class RaysRotation : MonoBehaviour
    {
        [SerializeField] private float speed = 30f;

        private void Update()
        {
            transform.Rotate(0f, 0f, speed * Time.deltaTime);
        }
    }
}
