using BaseClass;
using Connect;
using Connect.Converter;
using Connect.DTO;
using Firearms.Bullet;
using mEnum;
using Player;
using UnityEngine;

namespace mSystem.BulletSystem
{
    public class BulletComponentSystem : ComponentSystem<Bullet>
    {
        // 提前缓存 LayerMask 提高性能
        private int _hitboxMask;
        private int _wallMask;
        private int _planeMask;

        public override void Init()
        {
            _hitboxMask = 1 << LayerMask.NameToLayer("Hitbox");
            _wallMask = 1 << LayerMask.NameToLayer("Wall");
            _planeMask = 1 << LayerMask.NameToLayer("Plane");
        }

        protected override void UpdateComponent()
        {
            // 合并 Mask：子弹既能打中人，也能打中墙
            int combinedMask = _hitboxMask | _wallMask | _planeMask;

            for(int i = Queue.Count; i > 0; i--)
            {
                Bullet bullet = Queue.Dequeue();
                if (bullet.MoveTime < 0)
                {
                    BulletGameObjectPool.Instance.RemoveObj(bullet.Transform.gameObject);
                    continue;
                }

                Vector3 move = bullet.moveNormalize * (bullet.speed * Time.fixedDeltaTime);
                Vector3 lastPosition = bullet.Transform.position;
                Vector3 newPosition = lastPosition + move;
                float distance = move.magnitude;

                // 核心修改：使用 combinedMask 进行射线检测
                if (Physics.Raycast(lastPosition, bullet.moveNormalize, out RaycastHit hit, distance, combinedMask))
                {
                    bullet.Transform.position = hit.point;

                    // 1. 尝试获取 HitBox 脚本
                    var hitBox = hit.collider.GetComponent<HitBox>();

                    // 2. 如果打中的是带 HitBox 的实体 (玩家或 AI)
                    if (hitBox && hitBox.Owner != null)
                    {
                        // 只有自己发射的子弹才上报请求
                        if (bullet.shooterId == NetworkService.Bridge.GetMyId())
                        {
                            // 检查是否打中自己（如果起点在体内可能发生）
                            if (hitBox.Owner.EntityId != bullet.shooterId)
                            {
                                HitDto hitDto = new HitDto
                                {
                                    attackerId = bullet.shooterId,
                                    victimId = hitBox.Owner.EntityId, // 从接口或基类获取 ID
                                    // 根据 HitBox 记录的部位计算伤害
                                    damage = (hitBox.part == HitPart.Head) ? (int)bullet.headHit : (int)bullet.baseHit,
                                    hitType = (int)hitBox.part, 
                                    element = bullet.element,
                                    elementMeasure = bullet.elementMeasure
                                };
                                
                                Debug.Log($"玩家{hitDto.victimId} 被 玩家{hitDto.attackerId} 击中 {hitBox.part}");
                                
                                NetworkService.Bridge.SendToServer(new Message("DamageConfirm", HitDataConverter.ToBinary(hitDto)));
                            }
                        }
                    }
                    else
                    {
                        // 3. 如果没打中 HitBox 但射线停了，说明打中了环境（墙壁、地面）
                        // 在这里可以生成弹孔特效或火花
                        Debug.Log("子弹击中环境: " + hit.collider.name);
                    }
                    
                    // 命中任何物体（人或墙）子弹都消失
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
        private BulletComponentSystem _bulletCs = new BulletComponentSystem();

        private void Start()
        {
            _bulletCs.Init();
        }

        public void EnQueue(Bullet component)
        {
            _bulletCs.EnQueue(component);
        }

        private void FixedUpdate()
        {
            _bulletCs.FixedUpdate();
        }
    }
}