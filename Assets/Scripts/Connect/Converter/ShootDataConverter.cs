using Connect.Data;
using Connect.ShootDTO;
using Google.FlatBuffers;
using UnityEngine;

namespace Connect.Converter
{
    public static class ShootDataConverter
    {
        // 复用 Builder，避免每开一枪都 new 一个，减少 GC
        private static FlatBufferBuilder _bulletBuilder = new FlatBufferBuilder(512);

        public static byte[] ToDto(ShootData data)
        {
            _bulletBuilder.Clear(); // 清空上次的数据，保留内存池

            // 构建开始
            ShootDataFlat.StartShootDataFlat(_bulletBuilder);
            
            ShootDataFlat.AddShooterId(_bulletBuilder, data.shooterId);

            // Struct 是直接内联的，这种写法是正确的
            ShootDataFlat.AddPosition(_bulletBuilder, 
                Vec3.CreateVec3(_bulletBuilder, data.position.x, data.position.y, data.position.z));
            
            ShootDataFlat.AddForward(_bulletBuilder, 
                Vec3.CreateVec3(_bulletBuilder, data.forward.x, data.forward.y, data.forward.z));
            
            ShootDataFlat.AddSpeed(_bulletBuilder, data.speed);
            ShootDataFlat.AddElement(_bulletBuilder, data.element);
            ShootDataFlat.AddBaseHit(_bulletBuilder, data.baseHit);
            ShootDataFlat.AddHeadHit(_bulletBuilder, data.headHit);
            ShootDataFlat.AddElementMeasure(_bulletBuilder, data.elementMeasure);
            ShootDataFlat.AddMoveTime(_bulletBuilder, data.moveTime);
            ShootDataFlat.AddClientTime(_bulletBuilder, data.clientTime);

            var offset = ShootDataFlat.EndShootDataFlat(_bulletBuilder);
            _bulletBuilder.Finish(offset.Value);

            // 【优化建议】如果是高频射击，建议返回 ArraySegment<byte> 
            // 而不是 SizedByteArray()，因为后者会 new 一个新的 byte[]
            return _bulletBuilder.SizedByteArray();
        }

        public static void FillData(byte[] buffer, ShootData data)
        {
            var bb = new ByteBuffer(buffer);
            var flat = ShootDataFlat.GetRootAsShootDataFlat(bb);
            
            data.shooterId = flat.ShooterId;

            // FlatBuffers 的 Struct 访问会返回一个可空对象，使用 .Value 访问
            if (flat.Position.HasValue)
            {
                var pos = flat.Position.Value;
                data.position = new Vector3(pos.X, pos.Y, pos.Z);
            }

            if (flat.Forward.HasValue)
            {
                var forw = flat.Forward.Value;
                data.forward  = new Vector3(forw.X, forw.Y, forw.Z);
            }
            
            data.speed = flat.Speed;
            data.element = flat.Element;
            data.clientTime = flat.ClientTime;
            data.baseHit = flat.BaseHit;
            data.headHit = flat.HeadHit;
            data.elementMeasure = flat.ElementMeasure;
            data.moveTime = flat.MoveTime;
        }
    }
}