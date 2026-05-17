using mModel;
using QFramework;

public class GenshinArchitecture : Architecture<GenshinArchitecture>
{
    protected override void Init()
    {
        this.RegisterModel(new BagModel());
    }
}