using System;
using mSystem.mSettingSystem;
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

        private void Start()
        {
            baseBtn.OnClick += OpenBaseSettingInternal;
            combatBtn.OnClick += OpenCombatSettingInternal;
        }

        public void Init(SettingEditSession settingEdit)
        {
            baseSetting.Init(settingEdit);
            combatSetting.Init(settingEdit);
        }

        private void OpenBaseSettingInternal()
        {
            combatSetting.gameObject.SetActive(false);
            combatBtn.ExitChose();
            baseSetting.gameObject.SetActive(true);
        }

        private void OpenCombatSettingInternal()
        {
            baseSetting.gameObject.SetActive(false);
            baseBtn.ExitChose();
            combatSetting.gameObject.SetActive(true);
        }
    }
}