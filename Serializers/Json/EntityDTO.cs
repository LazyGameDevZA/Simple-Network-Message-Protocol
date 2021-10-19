namespace Serializers.Json
{
    internal class EntityDTO
    {
        public byte ID { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int VelocityX { get; set; }
        public int VelocityY { get; set; }
        public uint PackedColour { get; set; }
    }
}