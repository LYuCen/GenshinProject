using System.Collections.Generic;
using mModel.ScriptableConfig;
using mWeapon.ComponentData;
using Player.AnimPro;

namespace mWeapon.Component
{
    public class AnimComponent
    {
        public AnimGraphData AnimGraphData => _data.animGraphData;
        public List<TagClipPair> AnimMappings => _data.animMappings;

        private AnimComponentData _data;
        
        public AnimComponent(AnimComponentData data)
        {
            _data = data;
        }
    }
}