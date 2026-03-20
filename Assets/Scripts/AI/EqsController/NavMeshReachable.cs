using System.Collections.Generic;
using AI.VisualRequest.VisualRequest;
using mSystem.EQSSystem;
using mSystem.EQSSystem.EqsContext;
using mSystem.EQSSystem.EqsGenerator;
using mSystem.EQSSystem.EqsTest;
using UnityEngine;

namespace AI.EqsController
{
    public class NavMeshReachable : MonoBehaviour
    {
        public Transform target;
        private IEqsSystem _eqsSystem;
        private GridEqsGenerator _eqsGenerator;
        private List<IEqsTest> _eqsTests = new List<IEqsTest>();

        private IReadOnlyList<EqsPoint> _points;
        private BaseContext _context;
        
        
        [Header("网格长度")] public float Length = 10f;      // 网格长度
        [Header("网格宽度")] public float Width = 10f;       // 网格宽度
        [Header("高度偏移量")] public float HeightOffset = 0f; // 高度偏移量
        [Header("点间距")] public float SpaceBetween = 1f; // 点间距
        [Header("投射高度")] public float RayOriginHeight = 10f; // 投射高度
        [Header("半径")] public float Radius = 2f;
        [Header("发现掩体")] public bool findCover; // 找到掩体
        public EqsPoint BestPoint;
        public DistanceToPositionTest DistanceToPositionTest;
        public bool enable;

        private MixVisualRequest _mvr;
        private List<PointVisualRequest> _list = new List<PointVisualRequest>();

        private InCircleTest _inCircleTest;

        private void Awake()
        {
            _eqsGenerator = new GridEqsGenerator();
            _eqsGenerator.Length = Length;
            _eqsGenerator.Width = Width;
            _eqsGenerator.HeightOffset = HeightOffset;
            _eqsGenerator.SpaceBetween = SpaceBetween;
            _eqsGenerator.RayOriginHeight = RayOriginHeight;

            _context = new BaseContext(transform, target);

            _inCircleTest = new InCircleTest(transform.position, 2f, 1, true);
            _eqsTests.Add(new NavMeshReachableTest());
            _eqsTests.Add(new SightTargetTest(2, true, LayerMask.GetMask("Wall"), true));
            _eqsTests.Add(_inCircleTest);
            DistanceToPositionTest = new DistanceToPositionTest(target.position, 1f);
            _eqsTests.Add(DistanceToPositionTest);
            _eqsSystem = new EqsSystem(_eqsGenerator, _eqsTests);
            _eqsSystem.RunQuery(_context);
            _points = _eqsSystem.Points;

            
            
            _mvr = new MixVisualRequest();
            for (int i = 0; i < _points.Count; i++)
            {
                PointVisualRequest pvr = new PointVisualRequest();
                pvr.PointColor = _points[i].IsValid ? Color.green : Color.red;
                pvr.PointPos = _points[i].Position;
                pvr.Text = _points[i].Score.ToString("F2");
                _list.Add(pvr);
                _mvr.AddRequest(pvr);
            }

            EqsPoint best = _eqsSystem.GetBestPoint();
            for (int i = 0; i < _points.Count; i++)
            {
                if(_points[i] == best) _list[i].PointColor = Color.cyan;
            }
            
            
            VisualizerManager.Instance.Register(_mvr);
        }

        private void Update()
        {
            _eqsSystem.RunQuery(_context);
            
            _mvr.Enable = enable;
            if (!enable) return;
            
            _inCircleTest.Radius = Radius;
            _eqsGenerator.Length = Length;
            _eqsGenerator.Width = Width;
            _eqsGenerator.HeightOffset = HeightOffset;
            _eqsGenerator.SpaceBetween = SpaceBetween;
            _eqsGenerator.RayOriginHeight = RayOriginHeight;
            _points = _eqsSystem.Points;
            DistanceToPositionTest.TargetPosition = target.position;
            
            if (_points.Count == _list.Count)
            {
                for (int i = 0; i < _points.Count; i++)
                {
                    _list[i].PointColor = _points[i].IsValid ? Color.green : Color.red;
                    _list[i].PointPos = _points[i].Position;
                    _list[i].Text = _points[i].Score.ToString("F2");
                }
            }
            else
            {
                _mvr.ClearRequest();
                for (int i = 0; i < _points.Count; i++)
                {
                    PointVisualRequest pvr = new PointVisualRequest();
                    pvr.PointColor = _points[i].IsValid ? Color.green : Color.red;
                    pvr.PointPos = _points[i].Position;
                    pvr.Text = _points[i].Score.ToString("F2");
                    _list.Add(pvr);
                    _mvr.AddRequest(pvr);
                }
            }
            BestPoint = _eqsSystem.GetBestPoint();
            for (int i = 0; i < _points.Count && BestPoint != null; i++)
            {
                if(_points[i] == BestPoint) _list[i].PointColor = Color.cyan;
            }
        }

        public void SetEqsSystemTest(List<IEqsTest> tests)
        {
            if (tests == null)
            {
                _eqsSystem.SetTestList(_eqsTests);
            }
            else _eqsSystem.SetTestList(tests);
        }

        public IReadOnlyList<EqsPoint> GetPoints()
        {
            return _points;
        }
    }
}