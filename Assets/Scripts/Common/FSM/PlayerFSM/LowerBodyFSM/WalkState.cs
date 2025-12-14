using UnityEngine;

namespace Common.FSM.PlayerFSM.LowerBodyFSM
{
    public class WalkState : AbstractState<PlayerManager>
    {
        private float _rotationY;
        private WalkCondition _condition;
        
        public WalkState(PlayerManager manager) : base(manager)
        {
            _condition = new WalkCondition(MyManager, "Walk");
            Conditions.Add(new IdleCondition(MyManager,"Idle"));
            Conditions.Add(new JumpCondition(MyManager,"Jump"));
            Conditions.Add(new ShootCondition(MyManager, "Shoot"));
            Conditions.Add(new ShootWalkCondition(MyManager,"ShootWalk"));
            Conditions.Add(new RunCondition(MyManager, "Run"));
        }

        public override void OnEnterState()
        {
            MyManager.speed = 1.6f;
            MyManager.PlayAnimController.LeftHandIk = 1;
            MyManager.PlayAnimController.RightHandIk = 0;
            MyManager.gunCube.SetParent(MyManager.hand);
            MyManager.gunCube.localPosition = new Vector3(0.00016f, 0.00203f, 0.00078f);
            MyManager.gunCube.localEulerAngles = new Vector3(-90, 180, 0);
            MyManager.cameraController.shootCinema.gameObject.SetActive(false);
            MyManager.stateName = "移动";
        }

        public override void OnUpdateState()
        {
            float rotateY = MyManager.cameraController.Direction[((int)MyManager.inputMove.x, (int)MyManager.inputMove.y)] -
                            MyManager.cameraController.GetAngle();
            _rotationY = MyManager.transform.eulerAngles.y + rotateY;
            float newY = Mathf.LerpAngle(MyManager.transform.eulerAngles.y, _rotationY, Time.deltaTime * 10f);
            MyManager.transform.eulerAngles = new Vector3(0, newY, 0);

            Vector3 right = new Vector3(MyManager.playerFront.z, 0, -MyManager.playerFront.x);
            Vector3 move = MyManager.playerFront * MyManager.inputMove.y + right * MyManager.inputMove.x;
            MyManager.characterController.Move(move.normalized * (MyManager.speed * Time.deltaTime));
        }

        public override void OnExitState()
        {
        }
    }
}