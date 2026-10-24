using AssetBundleFramework.Core.Resource;
using mModel;
using mModel.SettingModel;
using mSystem.InputSystem;
using mSystem.mSettingSystem;
using mSystem.SceneSystem;
using mSystem.SoundSystem;
using QFramework;

public class GenshinArchitecture : Architecture<GenshinArchitecture>
{
    protected override void Init()
    {
        

        // model
        this.RegisterModel<BagModel>(new BagModel());
        this.RegisterModel<ISettingModel>(new SettingModel());
        this.RegisterModel<IKeyModel>(new KeyModel());
        this.RegisterModel<IUserModel>(new UserModel());

        // system
        this.RegisterSystem<IInputSystem>(new InputSystem());
        this.RegisterSystem<ISettingSystem>(new SettingSystem());
        this.RegisterSystem<ISceneSystem>(new SceneSystem());
        this.RegisterSystem<IAudioSystem>(new AudioSystem());
    }
}