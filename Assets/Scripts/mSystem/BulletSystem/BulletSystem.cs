using BaseClass;
using Firearms.Bullet;
using mSystem.BuffSystem.ElementBuff;
using mSystem.HurtSystem;
using UnityEngine;

namespace mSystem.BulletSystem
{
    public class BulletComponentSystem : ComponentSystem<Bullet>
    {
        protected override void UpdateComponent()
        {
            for(int i = Queue.Count; i > 0; i--)
            {
                Bullet bullet = Queue.Dequeue();
                if (bullet.MoveTime < 0)
                {
                    BulletGameObjectPool.Instance.RemoveObj(bullet.Transform.gameObject);
                    continue;
                }
                // 本帧位移
                Vector3 move = bullet.moveNormalize * (bullet.speed * Time.fixedDeltaTime);
                Vector3 newPosition = bullet.Transform.position + move;
                Vector3 lastPosition = bullet.Transform.position;

                // 计算位移距离
                float distance = (newPosition - lastPosition).magnitude;

                // 射线检测（从上一帧位置到新位置）
                if (Physics.Raycast(lastPosition, (newPosition - lastPosition).normalized,
                        out RaycastHit hit, distance))
                {
                    bullet.Transform.position = hit.point;
                    Debug.Log("击中：" + hit.collider.name + $" 附加{bullet.elementMeasure}点{(ElementEnum)bullet.element}元素量");
                    if (hit.collider.name is "Body" or "Head")
                    {
                        HurtComponent hurtComponent = new HurtComponent()
                        {
                            Element = bullet.element,
                            ElementMeasure = bullet.elementMeasure,
                            Harm = hit.collider.name == "Body" ? (int)bullet.baseHit : (int)bullet.headHit,
                            HitType = hit.collider.name == "Body" ? 0 : 1,
                            Transform = hit.transform.parent
                        };
                        HurtSystem.HurtSystem.Instance.EnQueue(hurtComponent);
                    }

                    BulletGameObjectPool.Instance.RemoveObj(bullet.Transform.gameObject);
                    continue;
                }

                // 没命中就正常移动
                bullet.Transform.position = newPosition;
                bullet.MoveTime -= Time.fixedDeltaTime;
                Queue.Enqueue(bullet);
            }
        }
    }
    
    public class BulletSystem : AbstractSingleton<BulletSystem>
    {
        private BulletComponentSystem _bulletCS = new BulletComponentSystem();

        public void EnQueue(Bullet component)
        {
            _bulletCS.EnQueue(component);
        }

        private void FixedUpdate()
        {
            _bulletCS.FixedUpdate();
        }
    }
}