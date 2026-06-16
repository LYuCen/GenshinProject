using UnityEngine;

namespace mView.BulletChose
{
    public class AnnulusView : MonoBehaviour
    {
        public int offset = 1;
        public int size = 2;
        public int radius = 200;
        public RectTransform[] icons = new RectTransform[100];
        public AnnulusItem[] items = new AnnulusItem[100];
        public GameObject prefab;
        public GameObject icon;

        public int innerRadius = 100; // 内圆半径，在内圆中视为取消
        private int _lastIndex;


        public void Init(int anSize, int anOffset)
        {
            for (int i = anSize; i < 100; i++)
            {
                if (icons[i] == null && items[i] == null) break;
                if (icons[i] != null) icons[i].gameObject.SetActive(false);
                if (items[i] != null) items[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < anSize; i++)
            {
                if (!items[i])
                {
                    Transform trans = Instantiate(prefab).transform;
                    trans.SetParent(transform, false);
                    items[i] = trans.GetComponent<AnnulusItem>();
                }
                items[i].gameObject.SetActive(true);
                float fillAmount = 1f / anSize - anOffset / 360f;
                items[i].SetFillAmount(fillAmount);
                items[i].SetRotationZ(-360f / anSize * i);
            }
            
            for (int i = 0; i < anSize; i++)
            {
                if (icons[i] == null)
                {
                    Transform trans = Instantiate(icon).transform;
                    trans.SetParent(transform);
                    icons[i] = trans.GetComponent<RectTransform>();
                }
                icons[i].gameObject.SetActive(true);
                float angleRad = -360f / anSize * (i - anSize / 2f + 0.5f) * Mathf.Deg2Rad;
                float x = radius * Mathf.Cos(angleRad);
                float y = radius * Mathf.Sin(angleRad);
                Vector2 itemPosition = new Vector2(x, y);
                icons[i].anchoredPosition = itemPosition;
                icons[i].transform.SetAsLastSibling();
            }

            size = anSize;
            offset = anOffset;
        }

        /// <summary>
        /// 通过鼠标位置确认扇形索引， -1 为取消选择
        /// </summary>
        public int GetIndexByMouse(Vector2 mousePos)
        {
            Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 dir = mousePos - center;
            float dist = dir.sqrMagnitude;

            if (dist < innerRadius * innerRadius) return -1;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            float adjusted = (180f - angle + 360f) % 360f;
            float segAngle = 360f / size;        
            return (int)(adjusted / segAngle) % size;
        }

        public void SetHighLight(int index)
        {
            if (index == _lastIndex) return;
            
            // 取消原有高亮
            if (_lastIndex >= 0 && _lastIndex < size)
            {
                items[_lastIndex].SetHighLight(false);
            }

            if (index >= 0 && index < size)
            {
                items[index].SetHighLight(true);
            }

            _lastIndex = index;
        }

        public void ShowWheel()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            gameObject.SetActive(true);
        }

        public void HideWheel()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            SetHighLight(-1);
            gameObject.SetActive(false);
        }
    }
}