using System.Collections.Generic;
using mSystem.EQSSystem.EqsContext;
using mSystem.EQSSystem.EqsGenerator;
using mSystem.EQSSystem.EqsTest;

namespace mSystem.EQSSystem
{
    public class EqsSystem : IEqsSystem
    {
        private readonly IEqsGenerator _generator;
        private readonly List<IEqsTest> _tests;
        private readonly List<EqsPoint> _points = new();

        public IReadOnlyList<EqsPoint> Points => _points;

        public EqsSystem(IEqsGenerator generator, List<IEqsTest> tests)
        {
            _generator = generator;
            _tests = tests;
        }

        public void RunQuery(IEqsContext context)
        {
            _points.Clear();
            _points.AddRange(_generator.Generate(context));

            for (int i = 0; i < _tests.Count; i++)
            {
                _tests[i].Execute(_points, context);
            }
        }

        public EqsPoint GetBestPoint()
        {
            EqsPoint best = null;

            for (int i = 0; i < _points.Count; i++)
            {
                var p = _points[i];

                if (!p.IsValid) continue;

                if (best == null || p.Score > best.Score)
                    best = p;
            }

            return best;
        }
    }
}