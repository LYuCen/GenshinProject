using GameFSM.LevelFSM.LocomotionFSM;
using UnityEngine;

namespace mSystem.InputSystem.Context
{
    public class InputContext
    {
        /// <summary>移动</summary>
        public Vector2 Move;
        /// <summary>主功能键</summary>
        public ButtonState PrimaryAction;
        /// <summary>副功能键</summary>
        public ButtonState SecondaryAction;

        /// <summary>获取主武器</summary>
        public ButtonState GetPrimaryWeapon;
        /// <summary>获取副武器</summary>
        public ButtonState GetSecondaryWeapon;

        /// <summary>换弹</summary>
        public ButtonState Reload;
        /// <summary>冲刺</summary>
        public ButtonState Sprint;
        /// <summary>姿态切换</summary>
        public ButtonState Crouch;
        /// <summary>道具</summary>
        public ButtonState Ability1;
        /// <summary>跳跃</summary>
        public ButtonState Jump;

        /// <summary>鼠标滚轮</summary>
        public float MouseScroll;
    }

    public class FsmContext
    {
        public Locomotion Locomotion;
    }
}