using System;
using System.Collections.Generic;
using UnityEngine;

namespace mModel.SettingModel
{
    [Serializable]
    public class UserInputSettings
    {
        public float mouseSensitivity = 1f;
        public List<KeyBindingEntry> keyBindings = new();
        
        public UserInputSettings Clone()
        {
            var copy = new UserInputSettings();

            copy.mouseSensitivity = mouseSensitivity;

            copy.keyBindings = new List<KeyBindingEntry>();

            foreach (var item in keyBindings)
            {
                copy.keyBindings.Add(item.Clone());
            }

            return copy;
        }
        
        public void CopyFrom(UserInputSettings other)
        {
            mouseSensitivity = other.mouseSensitivity;

            keyBindings.Clear();

            foreach (var item in other.keyBindings)
            {
                keyBindings.Add(item.Clone());
            }
        }
    }

    [Serializable]
    public class KeyBindingEntry
    {
        public string actionName;
        public KeyCode key;
        
        public KeyBindingEntry Clone()
        {
            return new KeyBindingEntry
            {
                actionName = actionName,
                key = key
            };
        }
    }
}