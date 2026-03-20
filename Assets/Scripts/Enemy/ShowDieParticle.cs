using System;
using UnityEngine;

namespace Enemy
{
    public class ShowDieParticle : MonoBehaviour
    {
        private float _disableTime = 0.5f;
        private bool _isEnd = false;
        public Action<int> Action;

        private void FixedUpdate()
        {
            _disableTime -= Time.fixedDeltaTime;
            if (!_isEnd && _disableTime < 0)
            {
                Action?.Invoke(0);
                _isEnd = true;
            }
        }
    }
}