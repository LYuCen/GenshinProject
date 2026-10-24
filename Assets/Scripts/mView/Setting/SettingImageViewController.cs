using mSystem.mSettingSystem.EditData;
using mSystem.SoundSystem;
using mView.Button;
using QFramework;
using UnityEngine;

namespace mView.Setting
{
    public class SettingImageViewController : MonoBehaviour
    {
        [SerializeField] private SettingSecondButton baseBtn;
        [SerializeField] private SettingSecondButton highBtn;

        [SerializeField] private SettingScrollView baseSetting;
        [SerializeField] private SettingScrollView highSetting;

        private IAudioSystem _audioSystem;
        private void Start()
        {
            baseBtn.OnClick += OpenBaseSetting;
            highBtn.OnClick += OpenHighSetting;
        }

        public void Init(GeneralSettingEditData editData, IAudioSystem audioSystem)
        {
            _audioSystem = audioSystem;
            baseSetting.Init(editData);
            highSetting.Init(editData);
        }

        private void OpenBaseSetting()
        {
            baseSetting.gameObject.SetActive(true);
            highSetting.gameObject.SetActive(false);
            highBtn.ExitChose();
            _audioSystem.PlaySound(
                "Assets/AssetBundle/Sounds/UI/ESM_Battle_Game_UI_Interface_Menu_Clip_Button_Smooth_Warm_Soft_1_Hybrid_One_Shot 1.wav");

        }

        private void OpenHighSetting()
        {
            baseSetting.gameObject.SetActive(false);
            highSetting.gameObject.SetActive(true);
            baseBtn.ExitChose();
            _audioSystem.PlaySound(
                "Assets/AssetBundle/Sounds/UI/ESM_Battle_Game_UI_Interface_Menu_Clip_Button_Smooth_Warm_Soft_1_Hybrid_One_Shot 1.wav");

            
        }
    }
}