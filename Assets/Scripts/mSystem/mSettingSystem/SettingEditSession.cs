using System;
using System.Collections.Generic;

namespace mSystem.mSettingSystem
{
    /// <summary>
    /// 设置数据
    /// </summary>
    public class SettingEditSession
    {
        private Dictionary<Type, SettingEditData> _datas = new();

        public void AddData(SettingEditData data)
        {
            _datas.TryAdd(data.GetType(), data);
        }

        public void ApplyModify()
        {
            foreach (var kv in _datas)
            {
                kv.Value.Apply();
            }
        }

        public void CancelModify()
        {
            foreach (var kv in _datas)
            {
                kv.Value.Cancel();
            }
        }

        public bool CheckModify()
        {
            foreach (var kv in _datas)
            {
                if (kv.Value.IsModify) return true;
            }

            return false;
        }
    }
}