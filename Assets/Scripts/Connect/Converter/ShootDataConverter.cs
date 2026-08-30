using Connect.Data;
using Connect.ShootDTO;
using Google.FlatBuffers;

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
            ShootDataFlat.AddPosX(_bulletBuilder, (short)(data.position.x * 100));
            ShootDataFlat.AddPosY(_bulletBuilder, (short)(data.position.y * 100));
            ShootDataFlat.AddPosZ(_bulletBuilder, (short)(data.position.z * 100));
            ShootDataFlat.AddDirX(_bulletBuilder, (sbyte)(data.forward.x * 127));
            ShootDataFlat.AddDirY(_bulletBuilder, (sbyte)(data.forward.y * 127));
            ShootDataFlat.AddDirZ(_bulletBuilder, (sbyte)(data.forward.z * 127));
            ShootDataFlat.AddElement(_bulletBuilder, (byte)data.element);
            ShootDataFlat.AddSpeed(_bulletBuilder, (ushort)(data.speed * 10));

            var offset = ShootDataFlat.EndShootDataFlat(_bulletBuilder);
            _bulletBuilder.Finish(offset.Value);
            return _bulletBuilder.SizedByteArray();
        }
    }
}