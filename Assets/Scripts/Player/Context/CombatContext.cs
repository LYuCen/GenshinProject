using mController;
using mSystem.InputSystem.Context;
using UnityEngine;

namespace Player.Context
{
    public class CombatContext
    {
        public FirearmController FirearmController;
        public CameraController CameraController;
        public InputContext InputContext;

        public Transform GunCube;
        public Transform Weapon;
    }
}