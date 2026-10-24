using BaseClass;
using mCommand;
using mEvent;
using mModel.SettingModel;
using mSystem.InputSystem;
using mSystem.mSettingSystem;
using mSystem.mSettingSystem.EditData;
using mSystem.SoundSystem;
using mView.Button;
using mView.Setting;
using QFramework;
using UnityEngine;

namespace mView
{
    public class SettingViewController : AbstractSingleton<SettingViewController>, IController
    {
        [SerializeField] private BaseButton applyBtn;
        [SerializeField] private BaseButton backBtn;
        [SerializeField] private BaseButton defaultBtn;

        [SerializeField] private SettingMainButton imageSettingBtn;
        [SerializeField] private SettingMainButton voiceSettingBtn;
        [SerializeField] private SettingMainButton inputSettingBtn;
        private SettingMainButton _prevSettingMainBtn;

        [SerializeField] private SettingImageViewController imageViewController;
        [SerializeField] private SettingVoiceViewController voiceViewController;
        [SerializeField] private SettingInputViewController inputViewController;
        [SerializeField] private GameObject prevView;

        [SerializeField] private CanvasGroup group;
        [SerializeField] private GameObject setting;
        [SerializeField] private PauseViewController pause;
        
        private SettingEditSession _settingEdit;
        private GeneralSettingEditData _generalSettingEdit;
        
        public bool Enable => setting.activeSelf || pause.gameObject.activeSelf;
        public GeneralSettingEditData SettingEditData => _generalSettingEdit;
        private IAudioSystem _audioSystem;
        private void Start()
        {
            _audioSystem = this.GetSystem<IAudioSystem>();
            
            applyBtn.OnClick += ApplySetting;
            backBtn.OnClick += CloseSetting;
            defaultBtn.OnClick += ResetDefault;

            imageSettingBtn.OnClick += OpenImageSettingMainViewInternal;
            voiceSettingBtn.OnClick += OpenVoiceSettingMainViewInternal;
            inputSettingBtn.OnClick += OpenInputSettingMainViewInternal;

            pause.InSettingClick += OpenSetting;

            _prevSettingMainBtn = imageSettingBtn;
            
            _settingEdit = new SettingEditSession();
            _generalSettingEdit = new GeneralSettingEditData(this.GetModel<ISettingModel>());
            _settingEdit.AddData(_generalSettingEdit);
            
            inputViewController.Init(_settingEdit, _audioSystem);
            imageViewController.Init(_generalSettingEdit, _audioSystem);
            voiceViewController.Init(_generalSettingEdit);

            this.RegisterEvent<SettingModifyEvent>(_ =>
            {
                if (_settingEdit.CheckModify())
                {
                    applyBtn.enabled = true;
                    group.alpha = 1f;
                }
                else
                {
                    applyBtn.enabled = false;
                    group.alpha = 0.5f;
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            
            this.RegisterEvent<OpenSettingEvent>(_ =>
            {
                if (Enable) CloseSetting();
                else
                {
                    if(GameEnter.Instance.isGame)
                    {
                        OpenPause();
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    }
                    else OpenSetting();
                    this.GetSystem<IInputSystem>().GamePlayEnable = false;
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            
        }
        

        public void CloseSetting()
        {
            if (_settingEdit.CheckModify())
            {
                this.SendCommand(new TextTipCommand("数据被修改，是否应用修改", ApplySetting, AbandonSetting, CloseSetting));
            }
            else
            {
                setting.SetActive(false);
                pause.gameObject.SetActive(false);
                this.GetSystem<IInputSystem>().GamePlayEnable = true;
                if (GameEnter.Instance.isGame)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }

        public void OpenPause()
        {
            pause.gameObject.SetActive(true);
        }

        public void OpenSetting()
        {
            setting.SetActive(true);
        }

        /// <summary>
        /// 应用更新
        /// </summary>
        private void ApplySetting()
        {
            applyBtn.enabled = false;
            group.alpha = 0.5f;
            _settingEdit.ApplyModify();
            this.GetModel<ISettingModel>().Save();
            this.GetSystem<ISettingSystem>().Apply();
            this.GetSystem<IInputSystem>().ReloadKeyBindings();
        }

        private void AbandonSetting()
        {
            _settingEdit.CancelModify();
            this.GetSystem<ISettingSystem>().Apply();
            applyBtn.enabled = false;
            group.alpha = 0.5f;
        }

        private void ResetDefault()
        {
            var settingModel = this.GetModel<ISettingModel>();
            settingModel?.ResetBaseInput();
            settingModel?.ResetCombatInput();

            _generalSettingEdit.ResetToDefault();
            this.SendCommand<SettingDataResetCommand>();
            this.SendCommand<SettingModifyCommand>();
        }

        private void OpenImageSettingMainViewInternal()
        {
            _prevSettingMainBtn?.ExitChose();
            prevView?.SetActive(false);
            _prevSettingMainBtn = imageSettingBtn;
            prevView = imageViewController.gameObject;
            imageViewController.gameObject.SetActive(true);
            _audioSystem.PlaySound(
                "Assets/AssetBundle/Sounds/UI/ESM_Battle_Game_UI_Interface_Menu_Clip_Button_Smooth_Warm_Soft_1_Hybrid_One_Shot 1.wav");
        }

        private void OpenVoiceSettingMainViewInternal()
        {
            _prevSettingMainBtn?.ExitChose();
            prevView?.SetActive(false);
            _prevSettingMainBtn = voiceSettingBtn;
            prevView = voiceViewController.gameObject;
            voiceViewController.gameObject.SetActive(true);
            _audioSystem.PlaySound(
                "Assets/AssetBundle/Sounds/UI/ESM_Battle_Game_UI_Interface_Menu_Clip_Button_Smooth_Warm_Soft_1_Hybrid_One_Shot 1.wav");

        }

        private void OpenInputSettingMainViewInternal()
        {
            _prevSettingMainBtn?.ExitChose();
            prevView?.SetActive(false);
            _prevSettingMainBtn = inputSettingBtn;
            prevView = inputViewController.gameObject;
            inputViewController.gameObject.SetActive(true);
            _audioSystem.PlaySound(
                "Assets/AssetBundle/Sounds/UI/ESM_Battle_Game_UI_Interface_Menu_Clip_Button_Smooth_Warm_Soft_1_Hybrid_One_Shot 1.wav");

        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}
