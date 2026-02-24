using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace mController
{
    public class BodyController : MonoBehaviour
    {
        public int hp;
        [SerializeField] private int maxHp;
        [SerializeField] private Image hpUI;

        public HashSet<Transform> objs;
        public float h;
        public float radius;
        private Vector3 _bottom;
        private void Start()
        {
            maxHp = 1000;
            hpUI = transform.Find("Canvas/Hp").GetComponent<Image>();
            objs = new HashSet<Transform>();
        }

        public void LoseHp(int hit)
        {
            hp -= hit;
            if (hp < 0) hp = 0;
            hpUI.fillAmount = 1f * hp / maxHp;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_bottom, radius);
        }
    }
}