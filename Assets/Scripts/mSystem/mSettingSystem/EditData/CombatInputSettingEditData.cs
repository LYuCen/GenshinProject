using mModel.SettingModel;
using UnityEngine;

namespace mSystem.mSettingSystem.EditData
{
    public class CombatInputSettingEditData : SettingEditData
    {
        private UserInputSettings _editing; // 复制数据
        private ISettingModel _model;

        public CombatInputSettingEditData(ISettingModel model)
        {
            _model = model;
            _editing = model.CombatConfig.Clone();
        }
        
        public override void Apply()
        {
            if (IsModify)
            {
                _model.UpdateCombatConfig(_editing);
            }
        }

        public override void Cancel()
        {
            
        }

        public override void Close()
        {
            
        }

        protected override bool Compare()
        {
            for (int i = 0; i < _model.CombatConfig.keyBindings.Count; i++)
            {
                if(_model.CombatConfig.keyBindings[i].key != _editing.keyBindings[i].key) 
                    return true;
            }
            return false;
        }
        
        public bool SetKey(string actionName, KeyCode newKey)
        {
            if (_editing == null) return false;
            var entry = _editing.keyBindings.Find(k => k.actionName == actionName);
            if (entry != null)
            {
                entry.key = newKey;
                return true;
            }
            return false;
        }
    }
}