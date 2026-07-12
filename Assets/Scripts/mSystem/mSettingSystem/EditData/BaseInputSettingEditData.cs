using mModel.SettingModel;
using UnityEngine;

namespace mSystem.mSettingSystem.EditData
{
    public class BaseInputSettingEditData : SettingEditData
    {
        private UserInputSettings _editing;
        private ISettingModel _model;

        public BaseInputSettingEditData(ISettingModel model)
        {
            _model = model;
            _editing = model.BaseConfig.Clone();
        }
        
        public override void Apply()
        {
            if (IsModify)
            {
                _model.UpdateBaseConfig(_editing);
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
            for (int i = 0; i < _model.BaseConfig.keyBindings.Count; i++)
            {
                if(_model.BaseConfig.keyBindings[i].key != _editing.keyBindings[i].key) 
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