using mSystem.mSettingSystem.EditData;
using mView.Button;
using QFramework;
using UnityEngine;

namespace mView.Setting
{
    public class SettingImageViewController : MonoBehaviour, IController
    {
        [SerializeField] private SettingSecondButton baseBtn;
        [SerializeField] private SettingSecondButton highBtn;

        [SerializeField] private SettingScrollView baseSetting;
        [SerializeField] private SettingScrollView highSetting;
        private void Start()
        {
            baseBtn.OnClick += OpenBaseSetting;
            highBtn.OnClick += OpenHighSetting;
        }

        public void Init(GeneralSettingEditData editData)
        {
            baseSetting.Init(editData);
            highSetting.Init(editData);
        }

        private void OpenBaseSetting()
        {
            baseSetting.gameObject.SetActive(true);
            highSetting.gameObject.SetActive(false);
            highBtn.ExitChose();
        }

        private void OpenHighSetting()
        {
            baseSetting.gameObject.SetActive(false);
            highSetting.gameObject.SetActive(true);
            baseBtn.ExitChose();
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}