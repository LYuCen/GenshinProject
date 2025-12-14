using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace mController
{
    public class BodyController : MonoBehaviour
    {
        public int hp;
        public int maxHp;
        [SerializeField] private Image hpUI;

        public HashSet<Transform> objs;
        public float h;
        public float radius;
        private Vector3 _bottom;
        private void Start()
        {
            hpUI = transform.Find("Canvas/Hp").GetComponent<Image>();
            objs = new HashSet<Transform>();
        }

        public void LoseHp(int hit)
        {
            hp -= hit;
            if (hp < 0) hp = maxHp;
            hpUI.fillAmount = 1f * hp / maxHp;
        }

        private void Update()
        {
            IsGrounded();
        }

        void IsGrounded()
        {
            _bottom = transform.position + Vector3.down * h;

            Collider[] hits = Physics.OverlapSphere(_bottom, radius);
            objs.Clear();
            foreach (var col in hits)
            {
                if (col.transform.root == transform) continue; // 排除自己
                if (col.gameObject.CompareTag("Plane") || col.gameObject.CompareTag("Wall")) continue;
                objs.Add(col.transform.root);
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_bottom, radius);
        }
    }
}