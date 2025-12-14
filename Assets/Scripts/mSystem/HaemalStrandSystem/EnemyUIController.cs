using UnityEngine;
using UnityEngine.UI;

namespace mSystem.HaemalStrandSystem
{
    public class EnemyUIController : MonoBehaviour
    {
        public Transform body;
        public Image hp;
        public Vector3 offset;

        private Camera _camera;

        private void Start()
        {
            offset = new Vector3(0, 2, 0);
            _camera = Camera.main;
        }

        private void LateUpdate()
        {
            if (!body) return;

            Vector3 screenPos = _camera.WorldToScreenPoint(body.position + offset);
            Debug.Log(screenPos);
            if (screenPos.z > 0)
            {
                transform.position = screenPos;
            }
            else
            {
                transform.position = new Vector3(-9999, -9999, 0); // 藏到屏幕外
            }
        }
    }
}