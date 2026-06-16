using System;
using System.Collections.Generic;
using mModel;
using mSystem.BuffSystem.ElementBuff;
using UnityEngine;

namespace mView.Buff
{
    public class BuffView : MonoBehaviour
    {
        [SerializeField] private Transform buffIconParent;
        [SerializeField] private Transform elementIconParent;
        
        [SerializeField] private ElementBuffController controller;
        private ElementUI[] _elementIcon = new ElementUI[8];
        private Dictionary<Type, BuffUI> _activeBuffs = new();

        private void Start()
        {
            controller.RemoveBuff += HideBuffBar;
        }

        void Update()
        {
            for (int i = 1; i <= 7; i++)
            {
                if (controller.Elements[i].Enable)
                {
                    if (!_elementIcon[i]) ShowElementIcon(i);
                    if (_elementIcon[i])
                        _elementIcon[i].SetProgress(controller.Elements[i].GetVal(Time.time) / controller.Elements[i].Capacity);
                }
                else if(_elementIcon[i])
                {
                    HideElementIcon(i);
                }
            }

            foreach (var kv in controller._statusEffects)
            {
                if (!_activeBuffs.ContainsKey(kv.Key))
                {
                    _activeBuffs.Add(kv.Key, ShowBuffBar(kv.Key));
                }

                _activeBuffs[kv.Key].SetProgress(controller.GetBuffProgress(kv.Key));
            }
        }
        
        void ShowElementIcon(int element)
        {
            if (!ElementUIObjectPool.Instance) return;

            if (!ElementModel.Instance) return;
            
            GameObject obj = ElementUIObjectPool.Instance.GetObj();
            if (!obj) return;

            ElementUI ui = obj.GetComponent<ElementUI>();
            if (!ui) return;

            ui.SetIcon(ElementModel.Instance.elementUI[element]);
            ui.SetProgress(controller.Elements[element].GetVal(Time.time) / controller.Elements[element].Capacity);
            obj.transform.SetParent(elementIconParent, false);
            obj.SetActive(true);
            _elementIcon[element] = ui;
        }

        void HideElementIcon(int element)    
        {
            Debug.Log($"Element元素移除{element}");
            if (_elementIcon[element])
            {
                GameObject obj = _elementIcon[element].gameObject;
                _elementIcon[element] = null;
                obj.transform.SetParent(null); 
                ElementUIObjectPool.Instance.RemoveObj(obj);
            }
        }
        
        private BuffUI ShowBuffBar(Type type)
        {
            GameObject obj = BuffUIObjectPool.Instance.GetObj();
            BuffUI ui = obj.GetComponent<BuffUI>();
            Sprite sprite = controller.GetBuffSprite(type);
            ui.SetIcon(sprite);
            float progress = controller.GetBuffProgress(type);
            ui.SetProgress(progress >= 0 ? progress : 1f);
            obj.transform.SetParent(buffIconParent, false);
            obj.SetActive(true);
            return ui;
        }

        void HideBuffBar(Type type)
        {
            if (_activeBuffs.TryGetValue(type, out var ui))
            {
                GameObject obj = ui.gameObject;
                _activeBuffs.Remove(type);
                obj.transform.SetParent(null);
                BuffUIObjectPool.Instance.RemoveObj(obj.gameObject);
            }
        }
        
        private void OnDestroy()
        {
            controller.RemoveBuff -= HideBuffBar;
        }
    }
}