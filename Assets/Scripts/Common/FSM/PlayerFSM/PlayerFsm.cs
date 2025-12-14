using Common.FSM.PlayerFSM.LowerBodyFSM;
using Common.FSM.PlayerFSM.UpperBodyFSM;

namespace Common.FSM.PlayerFSM
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