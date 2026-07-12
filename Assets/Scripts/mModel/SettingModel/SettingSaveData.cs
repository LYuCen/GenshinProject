using System;
using System.Collections.Generic;

namespace mModel.SettingModel
{
    [Serializable]
    public class SettingSaveData
    {
        public List<SettingItemSave> items = new();
    }

    [Serializable]
    public class SettingItemSave
    {
        public string id;
        public int value;
    }
}