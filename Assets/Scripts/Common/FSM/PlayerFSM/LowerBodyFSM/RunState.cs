using UnityEngine;

namespace Common.FSM.PlayerFSM.LowerBodyFSM
{
    public class RunState : AbstractState<PlayerManager>
    {
        private float _rotationY;
        float _rotateY;
        public RunState(PlayerManager manager) : base(manager)
        {
            Conditions.Add(new IdleCondition(MyManager,"Idle"));
            Conditions.Add(new ShootWalkCondition(MyManager, "ShootWalk"));
        }

        public override void OnEnterState()
        {
            MyManager.speed = 4f;
            MyManager.PlayAnimController.LeftHandIk = 1;
            MyManager.PlayAnimController.RightHandIk = 0;
            MyManager.gunCube.SetParent(MyManager.hand);
            MyManager.gunCube.localPosition = new Vector3(0.00016f, 0.00203f, 0.00078f);
            MyManager.gunCube.localEulerAngles = new Vector3(-90, 180, 0);
            MyManager.cameraController.shootCinema.gameObject.SetActive(false);
            MyManager.stateName = "奔跑";
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