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
        }

        public static byte[] Pack(uint sequence, InputSample[] samples)
        {
            _builder.Clear();

            var offsets = new Offset<InputDataFlat>[samples.Length];
            for (int i = 0; i < samples.Length; i++)
            {
                var s = samples[i];
                offsets[i] =
                    InputDataFlat.CreateInputDataFlat(_builder, s.Tick, s.InputX, s.InputY, s.Yaw, s.Pitch, s.HoldButton);
            }

            var cmdVector = InputPacket.CreateCommandVector(_builder, offsets);
            InputPacket.StartInputPacket(_builder);
            InputPacket.AddSequence(_builder, sequence);
            InputPacket.AddCommand(_builder, cmdVector);
            var root = InputPacket.EndInputPacket(_builder);
            _builder.Finish(root.Value);
            return _builder.SizedByteArray();
        }

        public static void Unpack(byte[] data, out uint sequence, out InputSample[] samples)
        {
            var bb = new ByteBuffer(data);
            var packet = InputPacket.GetRootAsInputPacket(bb);
            sequence = packet.Sequence;

            int count = packet.CommandLength;
            samples = new InputSample[count];
            for (int i = 0; i < count; i++)
            {
                var cmd = packet.Command(i).Value;
                samples[i] = new InputSample
                {
                    Tick = cmd.Tick,
                    InputX = cmd.InputX,
                    InputY = cmd.InputY,
                    Yaw = cmd.Yaw,
                    Pitch = cmd.Pitch,
                    HoldButton = cmd.HoldButton
                };
            }
        }
    }
}