using Player.Context;
using UnityEngine;

namespace Player.Module
{
    public class PlayerIkModule : IModule
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
        }

        public void OnUpdate()
        {
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
            
            _target.position = Vector3.SmoothDamp(
                _target.position, 
                _context.CameraController.rayEnd, 
                ref _velocity, 
                0.03f
            );
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
        }

        public void OnFixedUpdate()
        {
            
        }
    }
}