using System.Collections.Generic;
using UnityEngine;

namespace mSystem.mSettingSystem.ItemData
{
    [CreateAssetMenu(menuName = "Setting/Setting Config")]
    public class SettingConfigSo : ScriptableObject
    {
        public List<SettingItemData> items;
    }
}