using UnityEngine;

namespace Firearms.Muzzle
{
    public class Muzzle : MonoBehaviour
    {
        public ParticleSystem[] particleSystems;
        
        public void UpdateColor(Color color)
        {
            foreach (var p in particleSystems)
            {
                var main = p.main;
                main.startColor = color;
            }

            var m = particleSystems[0].main;
            m.stopAction = ParticleSystemStopAction.Callback;
        }
        
        private void OnParticleSystemStopped()
        {
            FireMuzzleObjectPool.Instance.RemoveObj(gameObject);
        }
    }
}