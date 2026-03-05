using UnityEngine;

namespace mSystem.EQSSystem.EqsContext
{
    public class BaseContext : IEqsContext
    {
        private Transform _que;
        private Transform _tar;
        public Transform Querier => _que;
        public Transform Target => _tar;

        public BaseContext(Transform querier, Transform target)
        {
            _que = querier;
            _tar = target;
        }
    }
}