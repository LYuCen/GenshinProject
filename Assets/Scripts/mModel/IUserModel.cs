using System.IO;
using QFramework;
using UnityEngine;

namespace mModel
{
    public class UserInfo
    {
        public int Id;              // 玩家id
        public string Name;         // 玩家昵称
        public string Signature;    // 玩家签名
    }
    
    public interface IUserModel : IModel
    {
        UserInfo UserInfo { get; }
        void UpdateName(string str);
        void UpdateSignature(string str);
    }

    public class UserModel : AbstractModel, IUserModel
    {
        private static string SavePath => Path.Combine(Application.persistentDataPath, "UserInfo.json");
        private UserInfo _info;
        public UserInfo UserInfo => _info;

        public void UpdateName(string str)
        {
            _info.Name = str;
            SaveUserInfo();
        }

        public void UpdateSignature(string str)
        {
            _info.Signature = str;
            SaveUserInfo();
        }
        
        protected override void OnInit()
        {
            _info = new UserInfo();
            if (!LoadUserInfo())
            {
                _info.Name = "默认昵称";
                _info.Signature = "系统原装签名，送给每一位小可爱~";
            }
        }
        
        private bool LoadUserInfo()
        {
            if (!File.Exists(SavePath)) return false;
            try
            {
                string json = File.ReadAllText(SavePath);
                _info = JsonUtility.FromJson<UserInfo>(json);
                if (_info == null)
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
        
        private void SaveUserInfo()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SavePath)!);
            string json = JsonUtility.ToJson(_info, true);
            File.WriteAllText(SavePath, json);
            Debug.Log("用户数据已存储到 " + SavePath);
        }
    }
}