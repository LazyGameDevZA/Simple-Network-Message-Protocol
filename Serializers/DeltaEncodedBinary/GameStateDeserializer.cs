namespace Serializers.DeltaEncodedBinary
{
    using System;
    using System.IO;
    using BasicGame;
    using Microsoft.Xna.Framework;
    
    public class GameStateDeserializer: IGameStateDeserializer
    {
        private readonly Entity[] entities = new Entity[BasicGame.MaxEntities];
        
        public Entity[] Deserialize(byte[] data)
        {
            using var memoryStream = new MemoryStream(data);
            var shortBytes = new byte[2];
            var boolBytes = new byte[1];
            
            // Spawned Entities
            var spawnCount = memoryStream.ReadByte();

            for (int i = 0; i < spawnCount; i++)
            {
                var spawnedEntity = new Entity();
                spawnedEntity.ID = (byte)memoryStream.ReadByte();

                memoryStream.Read(shortBytes);
                spawnedEntity.Position.X = BitConverter.ToInt16(shortBytes);

                memoryStream.Read(shortBytes);
                spawnedEntity.Position.Y = BitConverter.ToInt16(shortBytes);

                spawnedEntity.Color = new Color(
                    memoryStream.ReadByte(),
                    memoryStream.ReadByte(),
                    memoryStream.ReadByte()
                );

                this.entities[spawnedEntity.ID] = spawnedEntity;
            }

            // Destroyed Entities
            var destroyedCount = memoryStream.ReadByte();

            for (int i = 0; i < destroyedCount; i++)
            {
                var destroyedId = memoryStream.ReadByte();

                this.entities[destroyedId] = null;
            }

            // Updated entities
            while (memoryStream.Position < memoryStream.Length - 1)
            {
                var id = memoryStream.ReadByte();

                var diffX = (sbyte)(byte)memoryStream.ReadByte();
                var diffY = (sbyte)(byte)memoryStream.ReadByte();
                this.entities[id].Position += new Point(diffX, diffY);

                memoryStream.Read(boolBytes);
                var shouldInvertColor = BitConverter.ToBoolean(boolBytes);

                if (shouldInvertColor)
                {
                    this.entities[id].Color.Invert();
                }
            }

            return entities;
        }
    }
}