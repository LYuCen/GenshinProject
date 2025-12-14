using UnityEngine;

namespace Common.FSM.PlayerFSM.LowerBodyFSM
{
    public class ShootWalkState : AbstractState<PlayerManager>
    {
        private Vector2 _inputMove;
        public ShootWalkState(PlayerManager manager) : base(manager)
        {
            Conditions.Add(new WalkCondition(MyManager,"Walk"));
            Conditions.Add(new ShootCondition(MyManager,"Shoot"));
            Conditions.Add(new RunCondition(MyManager,"Run"));
        }

        public override void OnEnterState()  
        {
            MyManager.speed = 1.6f;
            MyManager.PlayAnimController.LeftHandIk = 1;
            MyManager.PlayAnimController.RightHandIk = 1;
            _inputMove = MyManager.inputMove;
            if (!MyManager.isReload)
            {
                MyManager.gunCube.SetParent(MyManager.gun);
                MyManager.gunCube.localPosition = new Vector3(0.5f, 0f, 4f);
                MyManager.gunCube.localEulerAngles = Vector3.zero;
                MyManager.rightHandIk.position = MyManager.gunGrip.position;
            }
            MyManager.cameraController.shootCinema.gameObject.SetActive(true);
            MyManager.stateName = "据枪移动";
        }

        public override void OnUpdateState()
        {
            MyManager.PlayAnimController.LeftHandIk = 1;
            MyManager.PlayAnimController.RightHandIk = 1;
            if (_inputMove != MyManager.inputMove && MyManager.inputMove != Vector2.zero)
            {
                _inputMove = MyManager.inputMove;
            }
            Vector3 shootPos = MyManager.cameraController.rayEnd - MyManager.transform.position;
            shootPos.y = 0;
            shootPos.Normalize();
            MyManager.transform.forward = Vector3.Lerp(MyManager.transform.forward, shootPos,1f);
            Vector3 right = new Vector3(MyManager.playerFront.z, 0, -MyManager.playerFront.x);
            Vector3 move = MyManager.playerFront * MyManager.inputMove.y + right * MyManager.inputMove.x;
            MyManager.characterController.Move(move.normalized * (MyManager.speed * Time.deltaTime));
        }

        public override void OnExitState()
        {
            MyManager.PlayAnimController.RightHandIk = 0;
        }
    }
}