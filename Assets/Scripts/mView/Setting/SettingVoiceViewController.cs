using mSystem.mSettingSystem.EditData;
using QFramework;
using UnityEngine;

namespace mView.Setting
{
    public class SettingVoiceViewController : MonoBehaviour, IController
    {
        [SerializeField] private SettingScrollView baseSetting;
        
        public void Init(GeneralSettingEditData editData)
        {
            baseSetting.Init(editData);
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}