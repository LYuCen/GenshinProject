using mEvent;
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
        public PlayerIkModule(PlayerIkContext context)
        {
            _context = context;
        }
        
        public void Init()
        {
            _target = new GameObject("Aim").transform;
            _context.AimIK.solver.target = _target;
            _context.BodyBipedIK.solver.leftHandEffector.target = _hand;

            if (_context.ListenEvents)
            {
                this.RegisterEvent<BoltEvent>(_ =>
                    {
                        _context.AimIK.solver.IKPositionWeight = 0;
                        _aimIkWeight = 0;
                    })
                    .UnRegisterWhenGameObjectDestroyed(_context.BodyBipedIK.transform);
                this.RegisterEvent<BoltEndEvent>(_ =>
                    {
                        _aimIkWeight = 1;
                    })
                    .UnRegisterWhenGameObjectDestroyed(_context.BodyBipedIK.transform);
                this.RegisterEvent<OnReloadEvent>(_ =>
                    {
                        _context.AimIK.solver.IKPositionWeight = 0;
                        _aimIkWeight = 0;
                        _context.BodyBipedIK.solver.IKPositionWeight = 0;
                        _handIkWeight = 0;
                    })
                    .UnRegisterWhenGameObjectDestroyed(_context.BodyBipedIK.transform);
            }
        }

        public void OnUpdate()
        {
            if (_context.IsLocal)
            {
                Transform camera = _context.CameraController.transform;
                Vector3 aimDir = (_context.CameraController.rayEnd - camera.position).normalized;
                _target.position = camera.position + aimDir * 100f;
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

        public void SetAimIk(float weight)
        {
            _aimIkWeight = weight;
        }

        public void SetHandIk(float weight)
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