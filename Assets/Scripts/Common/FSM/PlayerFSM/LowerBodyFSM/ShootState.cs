using UnityEngine;

namespace Common.FSM.PlayerFSM.LowerBodyFSM
{
    public class ShootState : AbstractState<PlayerManager>
    {
        public ShootState(PlayerManager manager) : base(manager)
        {
            Conditions.Add(new IdleCondition(MyManager,"Idle"));
            Conditions.Add(new WalkCondition(MyManager,"Walk"));
            Conditions.Add(new ShootWalkCondition(MyManager,"ShootWalk"));
        }

        public override void OnEnterState()
        {
            MyManager.speed = 0.4f;
            MyManager.PlayAnimController.LeftHandIk = 1;
            MyManager.PlayAnimController.RightHandIk = 1;
            if (!MyManager.isReload)
            {
                MyManager.gunCube.SetParent(MyManager.gun);
                MyManager.gunCube.localPosition = new Vector3(0.5f, 0f, 4f);
                MyManager.gunCube.localEulerAngles = Vector3.zero;
                MyManager.rightHandIk.position = MyManager.gunGrip.position;
            }
            MyManager.cameraController.shootCinema.gameObject.SetActive(true);
            MyManager.stateName = "据枪待机";
        }

        public override void OnUpdateState()
        {
            Vector3 shootPos = MyManager.cameraController.rayEnd - MyManager.transform.position;
            shootPos.y = 0;
            shootPos.Normalize();
            MyManager.transform.forward = Vector3.Lerp(MyManager.transform.forward, shootPos,1f);
        }

        public override void OnExitState()
        {
            MyManager.PlayAnimController.RightHandIk = 0;
        }
    }
}