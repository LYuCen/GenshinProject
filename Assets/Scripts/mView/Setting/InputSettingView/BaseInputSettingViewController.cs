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
    public class BaseInputSettingViewController : MonoBehaviour, IController
    {
        [SerializeField] private SetKeyButton moveForwardBtn;
        [SerializeField] private SetKeyButton moveBackBtn;
        [SerializeField] private SetKeyButton moveLeftBtn;
        [SerializeField] private SetKeyButton moveRightBtn;
        [SerializeField] private SetKeyButton sprintBtn;
        [SerializeField] private SetKeyButton crouchBtn;

        [SerializeField] private string actionName;
        [SerializeField] private SetKeyButton setBtn;
        [SerializeField] private bool waiting;

        private SettingEditSession _settingEdit;
        private BaseInputSettingEditData _data;

        private void Start()
        {
            moveForwardBtn.OnClick += OnResetButton;
            moveBackBtn.OnClick += OnResetButton;
            moveLeftBtn.OnClick += OnResetButton;
            moveRightBtn.OnClick += OnResetButton;
            sprintBtn.OnClick += OnResetButton;
            crouchBtn.OnClick += OnResetButton;
        }
        
        public void Init(SettingEditSession settingEdit)
        {
            _settingEdit = settingEdit;
            _data = new BaseInputSettingEditData(this.GetModel<ISettingModel>());
            _settingEdit.AddData(_data);
            RefreshUI();
        }

        private void RefreshUI()
        {
            var model = this.GetModel<ISettingModel>();
            SetText(moveForwardBtn, model.GetKey(moveForwardBtn.actionName, model.BaseConfig));
            SetText(moveBackBtn,    model.GetKey(moveBackBtn.actionName,    model.BaseConfig));
            SetText(moveLeftBtn,    model.GetKey(moveLeftBtn.actionName,    model.BaseConfig));
            SetText(moveRightBtn,   model.GetKey(moveRightBtn.actionName,   model.BaseConfig));
            SetText(sprintBtn,      model.GetKey(sprintBtn.actionName,      model.BaseConfig));
            SetText(crouchBtn,      model.GetKey(crouchBtn.actionName,      model.BaseConfig));
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