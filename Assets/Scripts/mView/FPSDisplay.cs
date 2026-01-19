using TMPro;
using UnityEngine;

namespace mView
{
    public class FPSDisplay : MonoBehaviour
    {
        [Header("UI")]
        public TextMeshProUGUI text;

        [Header("Settings")]
        public float updateInterval = 0.5f;

        private float timeLeft;
        private int frames;
        private float accumFPS;

        // 模拟各类开销统计（粗略估算）
        private float animatorTime;
        private float skinnedMeshTime;
        private float updateLogicTime;
        private float physicsTime;

        void Start()
        {
            if (text == null)
            {
                Debug.LogError("请绑定一个 TextMeshProUGUI 对象！");
                enabled = false;
                return;
            }
            timeLeft = updateInterval;
        }

        void Update()
        {
            float delta = Time.unscaledDeltaTime;

            // 粗略 FPS
            accumFPS += 1f / delta;
            frames++;
            timeLeft -= delta;

            // 模拟各类耗时（可以根据实际情况替换为真实计时）
            animatorTime = EstimateAnimatorTime();
            skinnedMeshTime = EstimateSkinnedMeshTime();
            updateLogicTime = EstimateUpdateLogicTime();
            physicsTime = EstimatePhysicsTime();

            if (timeLeft <= 0f)
            {
                float fps = accumFPS / frames;
                text.text = $"FPS: {fps:0}\n" +
                            $"Animator: {animatorTime:0.0} ms\n" +
                            $"SkinnedMesh: {skinnedMeshTime:0.0} ms\n" +
                            $"Update Logic: {updateLogicTime:0.0} ms\n" +
                            $"Physics: {physicsTime:0.0} ms";

                // 根据 FPS 改颜色
                if (fps >= 60)
                    text.color = Color.green;
                else if (fps >= 30)
                    text.color = Color.yellow;
                else
                    text.color = Color.red;

                timeLeft = updateInterval;
                accumFPS = 0;
                frames = 0;
            }
        }

        #region 模拟耗时函数
        // 这里是粗略估算方法，可以根据你的场景替换成实际计时
        float EstimateAnimatorTime()
        {
            // 例如：每个 Animator 约占 0.5ms
            int animators = FindObjectsOfType<Animator>().Length;
            return animators * 0.5f;
        }

        float EstimateSkinnedMeshTime()
        {
            // 每个 SkinnedMeshRenderer 约占 0.7ms
            int skinned = FindObjectsOfType<SkinnedMeshRenderer>().Length;
            return skinned * 0.7f;
        }

        float EstimateUpdateLogicTime()
        {
            // 假设 Update 占用比例按对象数量计算
            int updates = FindObjectsOfType<MonoBehaviour>().Length;
            return updates * 0.05f;
        }

        float EstimatePhysicsTime()
        {
            // 每个 Rigidbody + Collider 估算 0.02ms
            int rigidbodies = FindObjectsOfType<Rigidbody>().Length;
            int colliders = FindObjectsOfType<Collider>().Length;
            return (rigidbodies + colliders) * 0.02f;
        }
        #endregion
    }
}
