using QFramework;
using UnityEngine;

namespace mSystem.HaemalStrandSystem
{
    public interface IHaemalSystem : ISystem
    {
        void UIUpdate();
    }

    public class HaemalSystem : AbstractSystem, IHaemalSystem
    {
        private GameObject[] _enemy;
        private Camera _camera;
        protected override void OnInit()
        {
            _enemy = GameObject.FindGameObjectsWithTag("BodyInfo");
            _camera = Camera.main;
        }

        public void UIUpdate()
        {
            foreach (var e in _enemy)
            {
                e.transform.LookAt(_camera.transform.position);
            }
        }
    }
}