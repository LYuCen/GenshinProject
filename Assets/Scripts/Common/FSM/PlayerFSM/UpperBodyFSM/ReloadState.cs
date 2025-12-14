using UnityEngine;

namespace Common.FSM.PlayerFSM.UpperBodyFSM
{
    public class ReloadState : AbstractState<PlayerManager>
    {
        public ReloadState(PlayerManager manager) : base(manager)
        {
            Conditions.Add(new NullCondition(MyManager,"Null"));
        }

        public override void OnEnterState()
        {
            Debug.Log("正在换弹");
            MyManager.upperName = "换弹";
            MyManager.gunCube.SetParent(MyManager.hand);
            MyManager.gunCube.localPosition = new Vector3(0.00016f, 0.00203f, 0.00078f);
            MyManager.gunCube.localEulerAngles = new Vector3(-90, 180, 0);
            MyManager.PlayAnimController.OpenIk = false;
        }

        public override void OnUpdateState()
        {
        }

        public override void OnExitState()
        {
            MyManager.PlayAnimController.OpenIk = true;
            if (MyManager.isShoot)
            {
                MyManager.gunCube.SetParent(MyManager.gun);
                MyManager.gunCube.localPosition = new Vector3(0.5f, 0f, 4f);
                MyManager.gunCube.localEulerAngles = Vector3.zero;
            }
        }
    }
}