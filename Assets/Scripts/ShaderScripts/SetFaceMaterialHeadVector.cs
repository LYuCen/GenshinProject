using UnityEngine;

namespace ShaderScripts
{
    /// <summary>
    /// 设置面部材质方向向量
    /// </summary>
    [ExecuteInEditMode] // 在编辑器中执行
    public class SetFaceMaterialHeadVector : MonoBehaviour
    {
        private static readonly int HeadForward = Shader.PropertyToID("_HeadForward");
        private static readonly int HeadRight = Shader.PropertyToID("_HeadRight");
        private static readonly int HeadUp = Shader.PropertyToID("_HeadUp");
        public Transform head;
        public Transform headForward;
        public Transform headRight;
        public Transform headUp;
        public Material faceMaterial;

        private void Update()
        {
            Vector3 headf = Vector3.Normalize(headForward.position - head.position);
            Vector3 headr = Vector3.Normalize(headRight.position - head.position);
            Vector3 headu = Vector3.Normalize(headUp.position - head.position);

            faceMaterial.SetVector(HeadForward, headf);
            faceMaterial.SetVector(HeadRight, headr);
            faceMaterial.SetVector(HeadUp, headu);
        }
    }
}