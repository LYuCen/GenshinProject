using System.Collections.Generic;
using UnityEngine;

namespace mSystem.mSettingSystem.ItemData
{
    [System.Serializable]
    public class SettingItemData
    {
        [Tooltip("唯一ID，用于存档和系统识别")]
        public string id;

        [Tooltip("显示在UI上的名称")]
        public string name;

        [Tooltip("设置所属分类")]
        public string category;

        [Tooltip("控件类型：滑动条/下拉框/开关")]
        public SettingControlType type;

        [Tooltip("滑动条最小值")]
        public int min;

        [Tooltip("滑动条最大值")]
        public int max;

        [Tooltip("下拉选项列表")]
        public List<string> options;

        [Tooltip("初始数据")]
        public int defaultValue;
        
        [Tooltip("当前数据")]
        public int value;

        [Tooltip("设置简要介绍")] 
        public string introduce;
    }
    
    public enum SettingControlType
    {
        Slider,
        Dropdown,
        Toggle
    }
}