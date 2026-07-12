using System;
using mCommand;
using mModel.SettingModel;
using mSystem.mSettingSystem;
using mSystem.mSettingSystem.EditData;
using mView.Button;
using QFramework;
using UnityEngine;

namespace mView.Setting.InputSettingView
{
    public class CombatInputSettingViewController : MonoBehaviour, IController
    {
        [SerializeField] private SetKeyButton shoot;
        [SerializeField] private SetKeyButton openShot;
        [SerializeField] private SetKeyButton shoulderShot;
        [SerializeField] private SetKeyButton weapon1;
        [SerializeField] private SetKeyButton weapon2;
        [SerializeField] private SetKeyButton reload;
        [SerializeField] private SetKeyButton grenade;

        [SerializeField] private string actionName;
        [SerializeField] private SetKeyButton setBtn;
        [SerializeField] private bool waiting;
        
        private SettingEditSession _settingEdit;
        private CombatInputSettingEditData _data;

        private void Start()
        {
            shoot.OnClick += OnResetButton;
            openShot.OnClick += OnResetButton;
            shoulderShot.OnClick += OnResetButton;
            weapon1.OnClick += OnResetButton;
            weapon2.OnClick += OnResetButton;
            reload.OnClick += OnResetButton;
            grenade.OnClick += OnResetButton;
        }
        
        public void Init(SettingEditSession settingEdit)
        {
            _settingEdit = settingEdit;
            _data = new CombatInputSettingEditData(this.GetModel<ISettingModel>());
            _settingEdit.AddData(_data);
            RefreshUI();
        }

        private void RefreshUI()
        {
            var model = this.GetModel<ISettingModel>();
            SetText(shoot,       model.GetKey(shoot.actionName,       model.CombatConfig));
            SetText(openShot,    model.GetKey(openShot.actionName,    model.CombatConfig));
            SetText(shoulderShot,model.GetKey(shoulderShot.actionName,model.CombatConfig));
            SetText(weapon1,     model.GetKey(weapon1.actionName,     model.CombatConfig));
            SetText(weapon2,     model.GetKey(weapon2.actionName,     model.CombatConfig));
            SetText(reload,      model.GetKey(reload.actionName,      model.CombatConfig));
            SetText(grenade,     model.GetKey(grenade.actionName,     model.CombatConfig));
        }

        private static void SetText(SetKeyButton btn, KeyCode key)
        {
            btn.SetText(key.ToString());
        }

        private void Update()
        {
            if (!waiting) return;

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    if (_data.SetKey(actionName, key))
                    {
                        this.SendCommand(new SettingModifyCommand());
                        setBtn.SetText(key.ToString());
                        setBtn.ExitChose();
                    }
                    else
                    {
                        Debug.LogWarning($"改键失败{key}");
                    }
                    waiting = false;
                    break;
                }
            }
        }

        private void OnResetButton(string str, SetKeyButton btn)
        {
            waiting = true;
            actionName = str;
            setBtn = btn;
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}