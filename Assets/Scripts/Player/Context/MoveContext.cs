using mController;
using mSystem.InputSystem.Context;
using Player.Module;
using UnityEngine;

namespace Player.Context
{
    public class MoveContext
    {
        public CameraController CameraController;
        public CharacterController CharacterController;
        public Transform PlayerTransform;
        public PlayerCombatModule CombatModule;
        public bool IsLocal = true;
    }
}