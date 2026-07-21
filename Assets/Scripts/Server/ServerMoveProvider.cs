using Connect.Data;
using mEnum.Flags;
using mEnum.Query;
using mInterface;

namespace Server
{
    public class ServerMoveProvider : IMoveQueryProvider
    {
        private PlayerStateData _state;
        private PlayerMoveQuery _query;

        public ServerMoveProvider(PlayerStateData state)
        {
            _state = state;
        }

        public PlayerMoveQuery GetQuery()
        {
            _query.SetState(0);
            PlayerMoveFlags flags = PlayerMoveFlags.None;

            if (_state.isGround)  flags |= PlayerMoveFlags.Ground;
            if (_state.isJump)    flags |= PlayerMoveFlags.Jump;
            if (_state.isSprint)  flags |= PlayerMoveFlags.Sprint;
            if (_state.isCrouch)  flags |= PlayerMoveFlags.Crouch;
            if (_state.isSliding) flags |= PlayerMoveFlags.Sliding;

            float x = _state.inputMove.x;
            float y = _state.inputMove.y;
            float t = 0.1f;

            if (y > t)
            {
                if (x > t)       flags |= PlayerMoveFlags.ForwardRight;
                else if (x < -t) flags |= PlayerMoveFlags.ForwardLeft;
                else             flags |= PlayerMoveFlags.Forward;
            }
            else if (y < -t)
            {
                if (x > t)       flags |= PlayerMoveFlags.BackRight;
                else if (x < -t) flags |= PlayerMoveFlags.BackLeft;
                else             flags |= PlayerMoveFlags.Back;
            }
            else
            {
                if (x > t)       flags |= PlayerMoveFlags.Right;
                else if (x < -t) flags |= PlayerMoveFlags.Left;
            }

            if (((int)flags & 0xFF) != 0) flags |= PlayerMoveFlags.Move;

            _query.SetState((int)flags);
            return _query;
        }
    }
}
