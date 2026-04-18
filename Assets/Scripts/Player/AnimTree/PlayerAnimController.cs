using GameFSM.FSM.PlayerFSM;
using Player.AnimPro;
using UnityEngine.Playables;

namespace Player.AnimTree
{
    public class PlayerAnimController
    {
        private PlayerAnimTreeBuild _animBuild;
        public PlayerAnimSlotTreeRoot Root; 
        public PlayerAnimSlot Slot;

        public PlayerAnimController(PlayerManager manager, AnimGraphData defaultTree)
        {
            Slot = new PlayerAnimSlot();
            manager.AnimController = this; 
            _animBuild = new PlayerAnimTreeBuild(manager, defaultTree, Slot);
            Root = _animBuild.RootNode;
        }
        
        public PlayableGraph Graph => _animBuild.Graph;
        public void Update() => _animBuild.Update();
        public void Destroy() => _animBuild.DestroyGraph();
    }
}