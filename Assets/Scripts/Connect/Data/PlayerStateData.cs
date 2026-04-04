using UnityEngine;

namespace Connect.Data
{
    [System.Serializable]
    public class PlayerStateData
    {
        public int playerId;
        public bool isGround, isJump, isShoot, isReload, isSprint, isThrow, isCrouch, isSliding;
        public Vector2 inputMove;
        public Vector2 playerMove;
        public Vector3 position;
        public Vector3 aimPosition;
        public Quaternion rotation;
        public float curHp;
        public float maxHp;
        public float curShield;
        public float maxShield;
    }
}