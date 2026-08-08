using System;
using System.Collections.Generic;
using AssetBundleFramework.Core.Resource;
using mCommand;
using mEvent;
using mModel.SettingModel;
using mSystem.mSettingSystem;
using mSystem.mSettingSystem.EditData;
using mSystem.mSettingSystem.ItemData;
using mView.Setting.Pool;
using mView.Setting.SettingItemView;
using QFramework;
using TMPro;
using UnityEngine;

namespace mView.Setting
{
    public class SettingScrollView : MonoBehaviour, IController
    {
        [SerializeField] private Transform content;
        [SerializeField] private TextMeshProUGUI introduceText;
        [SerializeField] private string id;
        
        private List<GameObject> _activeViews = new();
        private List<Action> _unbindActions = new();
        private GeneralSettingEditData _editData;
        private IUnRegister _resetEventUnRegister;

        public void Init(GeneralSettingEditData editData)
        {
            _editData = editData;
            if (gameObject.activeInHierarchy) LoadItems();
        }

        private void LoadItems()
        {
            if (_editData == null) return;

            var ir2 = ResourceManager.instance.Load(
                "Assets/AssetBundle/ScriptableObject/Setting/SettingItem/SettingConfigSo.asset", false);
            var config = ir2.GetAsset<SettingConfigSo>();

            foreach (var k in config.items)
            {
                if (k.category == id)
                {
                    GetFromPool(k);
                }
            }
        }

        private void GetFromPool(SettingItemData data)
        {
            GameObject obj = data.type switch  
            {
                SettingControlType.Slider   => SettingSliderPool.Instance.GetObj(),
                SettingControlType.Dropdown => SettingDropdownPool.Instance.GetObj(),
                _ => null
            };
            if (obj == null) return;

            obj.transform.SetParent(content, false);       
            _activeViews.Add(obj);

            switch (data.type)
            {
                case SettingControlType.Slider:
                    var sv = obj.GetComponent<SettingSliderView>();
                    sv.Build(data);
                    sv.SetValue(_editData.GetValue(data.id));
                    sv.OnUpdate += () => OnItemChanged(data.id, (int)sv.GetCurrentValue());
                    _unbindActions.Add(() => sv.OnUpdate = null);
                    sv.OnHoverIntroduce = text => introduceText.text = text;
                    break;

                case
                    SettingControlType.Dropdown:
                    var dv = obj.GetComponent<SettingDropdownView>();
                    dv.Build(data);
                    dv.SetIndex(_editData.GetValue(data.id));
                    dv.OnUpdate += () => OnItemChanged(data.id, dv.index);
                    _unbindActions.Add(() => dv.OnUpdate = null);
                    dv.OnHoverIntroduce = text => introduceText.text = text;
                    break;
            }
        }
        
        private void OnEnable()
        {
            LoadItems();
            _resetEventUnRegister = this.RegisterEvent<SettingDataResetEvent>(_ =>
            {
                RecycleAll();
                LoadItems();
            });
        }
        
        private void OnItemChanged(string itemId, int value)
        {
            _editData.SetValue(itemId, value);
            
            this.GetSystem<ISettingSystem>().ApplySingle(itemId, value, _editData.ToSaveData());

            this.SendCommand<SettingModifyCommand>();      
        }
        
        private void RecycleAll()
        {
            for (int i = 0; i < _activeViews.Count; i++)
            {
                _unbindActions[i]?.Invoke();

                var obj = _activeViews[i];
                if (obj == null) continue;

                if (obj.TryGetComponent<SettingSliderView>(out _)) SettingSliderPool.Instance?.RemoveObj(obj);
                else if (obj.TryGetComponent<SettingDropdownView>(out _)) SettingDropdownPool.Instance?.RemoveObj(obj);
            }
            _activeViews.Clear();
            _unbindActions.Clear();
        }

        private void OnDisable()
        {
            _resetEventUnRegister?.UnRegister();
            RecycleAll();
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}