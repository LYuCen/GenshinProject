using Connect.InputData;
using Google.FlatBuffers;

namespace Connect.Converter
{
    public static class InputDataConverter
    {
        private static FlatBufferBuilder _builder = new FlatBufferBuilder(256);

        public struct InputSample
        {
            public uint Tick;
            public byte InputX;
            public byte InputY;
            public short Yaw;
            public short Pitch;
            public ushort HoldButton;
            public uint ServerTick;
        }

        public static byte[] Pack(InputSample sample)
        {
            _builder.Clear();

            var root = InputDataFlat.CreateInputDataFlat(_builder,
                sample.Tick, sample.InputX, sample.InputY, sample.Yaw, sample.Pitch,
                sample.HoldButton, sample.ServerTick);
            _builder.Finish(root.Value);
            return _builder.SizedByteArray();
        }

        public static InputSample Unpack(byte[] data)
        {
            var bb = new ByteBuffer(data);
            var flat = InputDataFlat.GetRootAsInputDataFlat(bb);

            return new InputSample
            {
                Tick = flat.Tick,
                InputX = flat.InputX,
                InputY = flat.InputY,
                Yaw = flat.Yaw,
                Pitch = flat.Pitch,
                HoldButton = flat.HoldButton,
                ServerTick = flat.ServerTick
            };
        }
    }
}