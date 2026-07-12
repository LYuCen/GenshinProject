using System;
using System.Collections.Generic;
using UnityEngine;

namespace mModel.SettingModel
{
    [CreateAssetMenu(menuName = "Setting/InputSettingsConfig")]
    public class InputSettingConfig : ScriptableObject
    {
        public float defaultMouseSensitivity = 1f;
        public List<KeyBindingDefault> defaultKeyBindings;
    }
    
    [Serializable]
    public class KeyBindingDefault
    {
        public string actionName;
        public KeyCode defaultKey;
    }
}