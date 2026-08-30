using BaseClass;
using Connect;
using Connect.Converter;
using Connect.DTO;
using Connect.Protocol;
using Firearms.Bullet;
using GameFSM.LevelFSM.PlayerFSM;
using mEnum;
using mInterface;
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
                
                // 子弹射线检测
                if (Physics.Raycast(lastPosition, bullet.moveNormalize, out RaycastHit hit, distance, combinedMask))
                {
                    bullet.Transform.position = hit.point;
                    
                    var hitBox = hit.collider.GetComponent<HitBox>();
                    bool handled = false;

                    // 击中带有 hitbox 物体
                    if (hitBox && hitBox.Owner != null)
                    {
                        handled = TryHandlePlayerHit(bullet, hitBox);
                    }
                    if (!handled)
                    {
                        handled = TryHandleTargetHit(bullet, hit, hitBox);
                    }
                    if(!handled)
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

        // 命中标靶
        private bool TryHandleTargetHit(Bullet bullet, RaycastHit hitInfo, HitBox hitBox)
        {
            // 不是自己发射的子弹
            if (bullet.shooterId != NetworkService.Bridge.GetMyId()) return false;

            // 获取靶子接口
            var hittable = hitInfo.collider.GetComponentInParent<IHittable>();
            if (hittable == null) return false;
            
            HitInfo info = new HitInfo
            {
                Point = hitInfo.point,
                Normal = hitInfo.normal,
                Part = hitBox != null ? hitBox.part : HitPart.Body,
                Damage = (hitBox != null && hitBox.part == HitPart.Head) ? (int)bullet.headHit : (int)bullet.baseHit,
                Element = bullet.element,
                ElementMeasure = bullet.elementMeasure
            };

            hittable.OnHit(info);
            return true;
        }

        // 命中玩家
        private bool TryHandlePlayerHit(Bullet bullet, HitBox hitBox)
        {
            // 击中自己
            if (hitBox.Owner.EntityId == bullet.shooterId) return false;
            
            if (hitBox.Owner is PlayerManager pm)
            {
                pm.HpModule.TakeDamage();
            }
            
            // 不是自己发射的子弹
            if (bullet.shooterId != NetworkService.Bridge.GetMyId()) return false;
            
            // 单机模式
            if (!NetworkService.Instance.isSinglePlayer) return false;

            
            
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
                                
            NetworkService.Bridge.SendToServer(new Message(MessageType.C2S_PlayerHit, HitDataConverter.ToBinary(hitDto)));
            return true;
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