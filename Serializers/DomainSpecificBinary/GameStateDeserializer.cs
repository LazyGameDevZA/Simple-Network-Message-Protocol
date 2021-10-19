namespace Serializers.DomainSpecificBinary
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using BasicGame;
    using Microsoft.Xna.Framework;
    
    public class GameStateDeserializer: IGameStateDeserializer
    {
        public Entity[] Deserialize(byte[] data)
        {
            using var memoryStream = new MemoryStream(data);
            var entities = new List<Entity>(BasicGame.MaxEntities);
            var shortBytes = new byte[2];

            while (memoryStream.Position < memoryStream.Length - 1)
            {
                var entity = new Entity();

                entity.ID = (byte)memoryStream.ReadByte();

                memoryStream.Read(shortBytes);
                entity.Position.X = BitConverter.ToInt16(shortBytes);

                memoryStream.Read(shortBytes);
                entity.Position.Y = BitConverter.ToInt16(shortBytes);

                entity.Color = new Color(
                    memoryStream.ReadByte(),
                    memoryStream.ReadByte(),
                    memoryStream.ReadByte()
                );

                entities.Add(entity);
            }

            return entities.ToArray();
        }
    }
}