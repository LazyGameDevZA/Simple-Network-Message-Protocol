namespace Serializers.BasicBinary
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

            while (memoryStream.Position < memoryStream.Length - 1)
            {
                var entity = new Entity();

                entity.ID = (byte)memoryStream.ReadByte();

                var intBytes = new byte[4];
                memoryStream.Read(intBytes);
                entity.Position.X = BitConverter.ToInt32(intBytes);

                memoryStream.Read(intBytes);
                entity.Position.Y = BitConverter.ToInt32(intBytes);
                
                memoryStream.Read(intBytes);
                entity.Velocity.X = BitConverter.ToInt32(intBytes);

                memoryStream.Read(intBytes);
                entity.Velocity.Y = BitConverter.ToInt32(intBytes);

                memoryStream.Read(intBytes);
                entity.Color = new Color(BitConverter.ToUInt32(intBytes));

                entities.Add(entity);
            }

            return entities.ToArray();
        }
    }
}