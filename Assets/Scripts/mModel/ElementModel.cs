using BaseClass;
using UnityEngine;

namespace mModel
{
    public class ElementModel : AbstractSingleton<ElementModel>
    {
        public Material[] materials;
        public Color[] colors;
        public Sprite[] elementUI;
    }
}