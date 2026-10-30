using System.Collections.Generic;
using AssetBundleFramework.Core.Resource;
using mModel.SettingModel;
using mSystem.mSettingSystem.ItemData;
using UnityEngine;

namespace mSystem.mSettingSystem.EditData
{
    public class GeneralSettingEditData : SettingEditData
    {
        private ISettingModel _model;
        public List<SettingItemSave> Editing { get; } = new();

        public GeneralSettingEditData(ISettingModel model)
        {
            _model = model;
            foreach (var item in model.SettingSaveData.items)
            {
                Editing.Add(new SettingItemSave { id = item.id, value = item.value });
            }
        }

        public void ResetToDefault()
        {
            var ir = ResourceManager.instance.Load(
                "Assets/AssetBundle/ScriptableObject/Setting/SettingItem/SettingConfigSo.asset", false);
            var config = ir.GetAsset<SettingConfigSo>();

            Editing.Clear();
            foreach (var k in config.items)
            {
                Editing.Add(new SettingItemSave { id = k.id, value = k.defaultValue });
            }
        }

        public void SetValue(string id, int value)
        {
            var item = Editing.Find(i => i.id == id);
            if (item != null) item.value = value;
            else Debug.Log($"未找到id为：{id}的设置");
            
        }

        public int GetValue(string id)
        {
            return Editing.Find(i => i.id == id)?.value ?? 0;
        }

        public override void Apply()
        {
            foreach (var item in Editing)
            {
                var target = _model.SettingSaveData.items.Find(i => i.id == item.id);
                if (target != null) target.value = item.value;
            }
        }

        public override void Cancel()
        {
            Editing.Clear();
            foreach (var item in _model.SettingSaveData.items)
            {
                Editing.Add(new SettingItemSave { id = item.id, value = item.value });
            }
        }
        public override void Close() { }

        public SettingSaveData ToSaveData()
        {
            var data = new SettingSaveData();

            foreach (var item in Editing)
            {
                data.items.Add(new SettingItemSave()
                {
                    id = item.id,
                    value = item.value
                });
            }

            return data;
        }

        protected override bool Compare()
        {
            foreach (var item in Editing)
            {
                var modelItem = _model.SettingSaveData.items.Find(i => i.id == item.id);
                if (modelItem == null || modelItem.value != item.value)
                    return true;
            }
            return false;
        }
    }
}
