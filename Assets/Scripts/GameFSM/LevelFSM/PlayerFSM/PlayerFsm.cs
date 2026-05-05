using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.LowerFSMState;
using GameFSM.LevelFSM.PlayerFSM.LevelFSMState.UpperFSMState;

namespace GameFSM.LevelFSM.PlayerFSM
{
    public class PlayerFsm
    {
        private PlayerManager _manager;
        //private LowerBodyFsm _lowerBodyFsm;
        //private UpperBodyFsm _upperBodyFsm;
        private LowerBodyLayer _lowerBodyFsm;
        private UpperBodyLayer _upperBodyFsm;

        public PlayerFsm(PlayerManager manager)
        {
            _manager = manager;
            Init();
        }

        private void Init()
        {
            //_lowerBodyFsm = new LowerBodyFsm(new LowerBodyStateBuilder(_manager));
            //_upperBodyFsm = new UpperBodyFsm(new UpperBodyStateBuilder(_manager));
            _lowerBodyFsm = new LowerBodyLayer(_manager, new LowerBodyLayerCondition(_manager, "Lower"));
            _upperBodyFsm = new UpperBodyLayer(_manager, new UpperBodyLayerCondition(_manager, "Upper"));
            _lowerBodyFsm.Enter();
            _upperBodyFsm.Enter();
        }

        public void Update()
        {
            //_lowerBodyFsm.Update();
            //_upperBodyFsm.Update();
            _lowerBodyFsm.Update();
            _upperBodyFsm.Update();
        }
    }
}