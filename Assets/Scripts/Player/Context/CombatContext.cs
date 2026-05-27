using Connect.Data;
using mController;
using mSystem.InputSystem.Context;
using Player.AnimTree;
using UnityEngine;

namespace Player.Context
{
    public class CombatContext
    {
        public CameraController CameraController;
        public InputContext InputContext;
        public PlayerAnimController AnimController;
        public PlayerStateData StateData;
        public Transform GunCube;
        public bool IsLocal = true;
    }
}