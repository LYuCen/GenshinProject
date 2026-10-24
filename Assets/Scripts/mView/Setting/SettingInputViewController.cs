using System;
using mSystem.mSettingSystem;
using mSystem.SoundSystem;
using mView.Button;
using mView.Setting.InputSettingView;
using UnityEngine;

namespace mView.Setting
{
    public class SettingInputViewController : MonoBehaviour
    {
        [SerializeField] private SettingSecondButton baseBtn;
        [SerializeField] private SettingSecondButton combatBtn;

        [SerializeField] private BaseInputSettingViewController baseSetting;
        [SerializeField] private CombatInputSettingViewController combatSetting;

        private Action _pendingSwitch;
        private IAudioSystem _audioSystem;

        private void Start()
        {
            baseBtn.OnClick += OpenBaseSettingInternal;
            combatBtn.OnClick += OpenCombatSettingInternal;
        }

        public void Init(SettingEditSession settingEdit, IAudioSystem audioSystem)
        {
            _audioSystem = audioSystem;
            baseSetting.Init(settingEdit);
            combatSetting.Init(settingEdit);
        }

        private void OpenBaseSettingInternal()
        {
            combatSetting.gameObject.SetActive(false);
            combatBtn.ExitChose();
            baseSetting.gameObject.SetActive(true);
            _audioSystem.PlaySound(
                "Assets/AssetBundle/Sounds/UI/ESM_Battle_Game_UI_Interface_Menu_Clip_Button_Smooth_Warm_Soft_1_Hybrid_One_Shot 1.wav");

        }

        private void OpenCombatSettingInternal()
        {
            baseSetting.gameObject.SetActive(false);
            baseBtn.ExitChose();
            combatSetting.gameObject.SetActive(true);
            _audioSystem.PlaySound(
                "Assets/AssetBundle/Sounds/UI/ESM_Battle_Game_UI_Interface_Menu_Clip_Button_Smooth_Warm_Soft_1_Hybrid_One_Shot 1.wav");

        }
    }
}