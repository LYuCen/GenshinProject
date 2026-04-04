using System.Collections.Generic;
using mSystem.EQSSystem.EqsContext;
using mSystem.EQSSystem.EqsGenerator;
using mSystem.EQSSystem.EqsTest;

namespace mSystem.EQSSystem
{
    public class EqsSystem : IEqsSystem
    {
        private readonly IEqsGenerator _generator;
        private readonly List<EqsPoint> _points = new();
        private List<IEqsTest> _tests;

        public IReadOnlyList<EqsPoint> Points => _points;

        private EqsPoint _prevBestPoint;
        private IEqsContext _context;

        public EqsSystem(IEqsGenerator generator, List<IEqsTest> tests)
        {
            _generator = generator;
            _tests = tests;
        }

        public void RunQuery(IEqsContext context)
        {
            _points.Clear();
            _points.AddRange(_generator.Generate(context));
            _context = context;

            for (int i = 0; i < _tests.Count; i++)
            {
                _tests[i].Execute(_points, context);
            }
        }

        public void SetTestList(List<IEqsTest> tests)
        {
            _tests = tests;
        }

        /// <summary>
        /// 获取最优候选点
        /// </summary>
        /// <returns></returns>
        public EqsPoint GetBestPoint()
        {
            EqsPoint best = null;

            if (_prevBestPoint != null)
            {
                List<EqsPoint> l = new List<EqsPoint>();
                l.Add(_prevBestPoint);
                for (int i = 0; i < _tests.Count; i++)
                {
                    _tests[i].Execute(l, _context);
                }

                if (_prevBestPoint.IsValid) return _prevBestPoint;
            }
            

            for (int i = 0; i < _points.Count; i++)
            {
                var p = _points[i];

                if (!p.IsValid) continue;

                if (best == null || p.Score > best.Score)
                    best = p;
            }

            _prevBestPoint = best;
            return best;
        }
    }
}