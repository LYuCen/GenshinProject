using UnityEngine;

namespace Common.FSM.PlayerFSM.UpperBodyFSM
{
    public class ReloadState : AbstractState<PlayerManager>
    {
        private bool _aimIkEnable;
        public ReloadState(PlayerManager manager) : base(manager)
        {
            Conditions.Add(new NullCondition(MyManager,"Null"));
        }

        public override void OnEnterState()
        {
            MyManager.upperName = "换弹";
            MyManager.gunCube.SetParent(MyManager.hand);
            MyManager.gunCube.localPosition = new Vector3(0.000118f, 0.001919f, 0.001026f);
            MyManager.gunCube.localEulerAngles = new Vector3(283.742096f, 323.333099f, 229.175308f);
        }

        public override void OnUpdateState()
        {
        }

        public override void OnExitState()
        {
        }
    }
}