using GameFSM.FSM.PlayerFSM.LowerBodyFSM;
using GameFSM.FSM.PlayerFSM.UpperBodyFSM;

namespace GameFSM.FSM.PlayerFSM
{
    public class PlayerFsm
    {
        private PlayerManager _manager;
        private LowerBodyFsm _lowerBodyFsm;
        private UpperBodyFsm _upperBodyFsm;

        public PlayerFsm(PlayerManager manager)
        {
            _manager = manager;
            Init();
        }

        private void Init()
        {
            _lowerBodyFsm = new LowerBodyFsm(new LowerBodyStateBuilder(_manager));
            _upperBodyFsm = new UpperBodyFsm(new UpperBodyStateBuilder(_manager));
        }

        public void Update()
        {
            _lowerBodyFsm.Update();
            _upperBodyFsm.Update();
        }
    }
}