namespace Serializers.DomainSpecificBinary
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

                var positionXBytes = BitConverter.GetBytes((short)entity.Position.X);
                var positionYBytes = BitConverter.GetBytes((short)entity.Position.Y);
                memoryStream.Write(positionXBytes);
                memoryStream.Write(positionYBytes);

                memoryStream.WriteByte(entity.Color.R);
                memoryStream.WriteByte(entity.Color.G);
                memoryStream.WriteByte(entity.Color.B);
            }

            return memoryStream.ToArray();
        }
    }
}