using Player.Context;
using QFramework;
using UnityEngine;

namespace Player.Module
{
    public class PlayerIkModule : IModule, ICanRegisterEvent
    {
        private PlayerIkContext _context;
        private float _aimIkWeight;
        private float _handIkWeight;
        private Transform _target;
        private Transform _hand;
        private Vector3 _velocity;

        private float _transitionTimer;
        private Vector3 _targetVelocity;
        public PlayerIkModule(PlayerIkContext context)
        {
            _context = context;
        }
        
        public void Init()
        {
            _target = new GameObject("Aim").transform;
            _context.AimIK.solver.target = _target;
            _context.BodyBipedIK.solver.leftHandEffector.target = _hand;
        }

        public void OnUpdate()
        {
            if (_context.IsLocal)
            {
                Transform camera = _context.CameraController.transform;
                Vector3 aimDir = (_context.CameraController.rayEnd - camera.position).normalized;
                float dist = Vector3.Distance(camera.position, _context.CameraController.rayEnd);
                float minDist = 3f;
                //_target.position = camera.position + aimDir * Mathf.Max(dist, minDist);
                _target.position = _context.CameraController.rayEnd;
            }

            _context.AimIK.solver.IKPositionWeight = Mathf.MoveTowards(
                _context.AimIK.solver.IKPositionWeight,
                _aimIkWeight,
                7 * Time.deltaTime
            );

            _context.BodyBipedIK.solver.IKPositionWeight = Mathf.MoveTowards(
                _context.BodyBipedIK.solver.IKPositionWeight,
                _handIkWeight,
                7 * Time.deltaTime
            );
        }

        public void SetAimTargetPosition(Vector3 pos)
        {
            _target.position = pos;
        }

        public void SetAimIk(float weight, bool both = false)
        {
            _aimIkWeight = weight;
        }

        public void SetHandIk(float weight, bool both = false)
        {
            _handIkWeight = weight;
        }

        public void SetHandIk(Transform trans)
        {
            _hand = trans;
            _context.BodyBipedIK.solver.leftHandEffector.target = _hand;
        }

        public void SetMuzzle(Transform muzzle)
        {
            _context.AimIK.solver.transform = muzzle;
        }

        public void OnFixedUpdate()
        {
            
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}