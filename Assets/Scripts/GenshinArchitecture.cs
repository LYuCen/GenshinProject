using mSystem;
using mSystem.HaemalStrandSystem;
using mSystem.InputSystem;
using QFramework;

public class GenshinArchitecture : Architecture<GenshinArchitecture>
{
    protected override void Init()
    {
        // Model
        this.RegisterModel<IKeyModel>(new KeyModel());
        
        // System
        this.RegisterSystem<IInputSystem>(new InputSystem());
        this.RegisterSystem<IHaemalSystem>(new HaemalSystem());
    }
}