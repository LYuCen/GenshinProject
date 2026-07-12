using System;
using mView.Button;
using UnityEngine;
using UnityEngine.EventSystems;

namespace mView.Setting
{
    public class SettingItemViewController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject backGround;
        [SerializeField] private GameObject enterImg;
        [SerializeField] private SettingTextButton btn;
        [SerializeField] private int itemIndex;     // 设置栏下标
        [SerializeField] private int choseIndex;    // 选择下标
        [SerializeField] private int lenght;           // 数组长度

        public Action<int, int> OnEnter;

        private void Start()
        {
            btn.OnClick += OnClick;
        }

        public void Init(int itemInd, int choseInd, int len)
        {
            itemIndex = itemInd;
            choseIndex = choseInd;
            lenght = len;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            enterImg.SetActive(true);
            OnEnter?.Invoke(itemIndex, choseIndex);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            enterImg.SetActive(false);
        }

        private void OnClick(int i)
        {
            choseIndex = (choseIndex + i + lenght) % lenght;
            OnEnter?.Invoke(itemIndex, choseIndex);
        }
    }
}