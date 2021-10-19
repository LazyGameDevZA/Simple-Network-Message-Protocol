namespace Serializers.BasicBinary
{
    using System;
    using System.IO;
    using System.Linq;
    using BasicGame;
    
    public class GameStateSerializer: IGameStateSerializer
    {
        public byte[] Serialize(BasicGame game)
        {
            using var memoryStream = new MemoryStream();

            foreach (var entity in game.Entities.Where(entity => entity != null))
            {
                memoryStream.WriteByte(entity.ID);

                var positionXBytes = BitConverter.GetBytes(entity.Position.X);
                var positionYBytes = BitConverter.GetBytes(entity.Position.Y);
                memoryStream.Write(positionXBytes);
                memoryStream.Write(positionYBytes);

                var velocityXBytes = BitConverter.GetBytes(entity.Velocity.X);
                var velocityYBytes = BitConverter.GetBytes(entity.Velocity.Y);
                memoryStream.Write(velocityXBytes);
                memoryStream.Write(velocityYBytes);

                var colourBytes = BitConverter.GetBytes(entity.Color.PackedValue);
                memoryStream.Write(colourBytes);
            }

            return memoryStream.ToArray();
        }
    }
}