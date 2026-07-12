using System.Collections.Generic;
using System.IO;
using AssetBundleFramework.Core.Resource;
using mSystem.mSettingSystem.ItemData;
using QFramework;
using UnityEngine;

namespace mModel.SettingModel
{
    public interface ISettingModel : IModel
    {
        UserInputSettings BaseConfig { get; }
        UserInputSettings CombatConfig { get; }
        public SettingSaveData SettingSaveData { get; }

        public KeyCode GetKey(string actionName, UserInputSettings config);
        void ResetBaseInput();
        void ResetCombatInput();
        void UpdateBaseConfig(UserInputSettings config);
        void UpdateCombatConfig(UserInputSettings config);
        void Save();
    }
    
    public class SettingModel : AbstractModel, ISettingModel
    {
        public UserInputSettings BaseConfig => _baseConfig;
        public UserInputSettings CombatConfig => _combatConfig;
        public SettingSaveData SettingSaveData => _saveData;

        private UserInputSettings _baseConfig;
        private UserInputSettings _combatConfig;
        private SettingSaveData _saveData;
        private static string BaseSavePath =>
            Path.Combine(Application.persistentDataPath, "BaseInputSettings.json");
        private static string CombatSavePath =>
            Path.Combine(Application.persistentDataPath, "CombatInputSettings.json");
        private static string SettingConfig =>
            Path.Combine(Application.persistentDataPath, "setting.json");
        
        protected override void OnInit()
        {
            _baseConfig = new UserInputSettings();
            _combatConfig = new UserInputSettings();
            _saveData = new SettingSaveData();
            
            if (!LoadBaseFromFile()) ResetBaseInput();
            if (!LoadCombatFromFile()) ResetCombatInput();
            var loaded = LoadSettingSaveFormFile();
            if (loaded != null) _saveData = loaded;
            else ResetSettingSave();
        }

        /// <summary>
        /// 从本地文件加载基础配置
        /// </summary>
        private bool LoadBaseFromFile()
        {
            if (!File.Exists(BaseSavePath)) return false;
            try
            {
                string json = File.ReadAllText(BaseSavePath);
                _baseConfig = JsonUtility.FromJson<UserInputSettings>(json);
                if (_baseConfig == null)
                {
                    Debug.LogWarning("基础配置文件解析失败，将使用默认配置");
                    return false;
                }
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"读取基础配置文件失败: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 从本地文件加载战斗配置
        /// </summary>
        private bool LoadCombatFromFile()
        {
            if (!File.Exists(CombatSavePath)) return false;
            try
            {
                string json = File.ReadAllText(CombatSavePath);
                _combatConfig = JsonUtility.FromJson<UserInputSettings>(json);
                if (_combatConfig == null)
                {
                    Debug.LogWarning("战斗配置文件解析失败，将使用默认配置");
                    return false;
                }
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"读取战斗配置文件失败: {e.Message}");
                return false;
            }
        }

        private SettingSaveData LoadSettingSaveFormFile()
        {
            if (!File.Exists(SettingConfig)) return null;
            try
            {
                string json = File.ReadAllText(SettingConfig);
                var saveData = JsonUtility.FromJson<SettingSaveData>(json);
                if (saveData == null) return null;

                // 对比 AssetBundle 配置，合并新增项
                MergeNewItemsFromConfig(saveData);
                return saveData;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"读取设置配置文件失败: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 比对 AssetBundle 中的 SettingConfigSo，将本地存档中不存在的新项用默认值补入
        /// </summary>
        private void MergeNewItemsFromConfig(SettingSaveData saveData)
        {
            var ir = ResourceManager.instance.Load(
                "Assets/AssetBundle/ScriptableObject/Setting/SettingItem/SettingConfigSo.asset", false);
            if (ir == null) return;
            var config = ir.GetAsset<SettingConfigSo>();
            if (!config || config.items == null) return;

            var existingIds = new HashSet<string>();
            foreach (var item in saveData.items)
                existingIds.Add(item.id);

            bool hasNewItem = false;
            foreach (var k in config.items)
            {
                if (!existingIds.Contains(k.id))
                {
                    saveData.items.Add(new SettingItemSave
                    {
                        id = k.id,
                        value = k.defaultValue
                    });
                    hasNewItem = true;
                }
            }

            if (hasNewItem)
            {
                Debug.Log("检测到新增设置项，已使用默认值更新本地配置");
                SaveSettingSave();
            }
        }
        
        public KeyCode GetKey(string actionName, UserInputSettings config)
        {
            if (config == null) return KeyCode.None;
            var entry = config.keyBindings.Find(k => k.actionName == actionName);
            if (entry == null) return KeyCode.None;
            return entry.key;
        }

        public void ResetBaseInput()
        {
            var ir = ResourceManager.instance.Load(
                "Assets/AssetBundle/ScriptableObject/Setting/BaseInputSetting.asset", false);
            var baseConfig = ir.GetAsset<InputSettingConfig>();
            
            _baseConfig.mouseSensitivity = baseConfig.defaultMouseSensitivity;
            _baseConfig.keyBindings = new List<KeyBindingEntry>();
            foreach (var k in baseConfig.defaultKeyBindings)
            {
                _baseConfig.keyBindings.Add(new KeyBindingEntry
                {
                    actionName = k.actionName,
                    key = k.defaultKey
                });
            }
        }

        public void ResetCombatInput()
        {
            var ir2 = ResourceManager.instance.Load(
                "Assets/AssetBundle/ScriptableObject/Setting/CombatInputSetting.asset", false);
            var combatConfig = ir2.GetAsset<InputSettingConfig>();
            
            _combatConfig.mouseSensitivity = combatConfig.defaultMouseSensitivity;
            _combatConfig.keyBindings = new List<KeyBindingEntry>();
            foreach (var k in combatConfig.defaultKeyBindings)
            {
                _combatConfig.keyBindings.Add(new KeyBindingEntry
                {
                    actionName = k.actionName,
                    key = k.defaultKey
                });
            }
        }

        public void ResetSettingSave()
        {
            var ir2 = ResourceManager.instance.Load(
                "Assets/AssetBundle/ScriptableObject/Setting/SettingItem/SettingConfigSo.asset", false);
            var config = ir2.GetAsset<SettingConfigSo>();
            
            foreach (var k in config.items)
            {
                _saveData.items.Add(new SettingItemSave
                {
                    id = k.id,
                    value = k.defaultValue
                });
            }
        }
        
        public void UpdateBaseConfig(UserInputSettings config)
        {
            _baseConfig.CopyFrom(config);
        }

        public void UpdateCombatConfig(UserInputSettings config)
        {
            _combatConfig.CopyFrom(config);
        }

        public void Save()
        {
            Debug.Log("按键存储位置：" + Application.persistentDataPath);
            SaveBase();
            SaveCombat();
            SaveSettingSave();
        }
        
        private void SaveBase()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(BaseSavePath)!);
            string json = JsonUtility.ToJson(_baseConfig, true);
            File.WriteAllText(BaseSavePath, json);
        }

        private void SaveCombat()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(CombatSavePath)!);
            string json = JsonUtility.ToJson(_combatConfig, true);
            File.WriteAllText(CombatSavePath, json);
        }
        
        private void SaveSettingSave()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingConfig)!);
            string json = JsonUtility.ToJson(_saveData, true);
            File.WriteAllText(SettingConfig, json);    
        }
    }
}