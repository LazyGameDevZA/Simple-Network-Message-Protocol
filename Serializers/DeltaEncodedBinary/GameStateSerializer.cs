namespace Serializers.DeltaEncodedBinary
{
    using System;
    using System.IO;
    using System.Linq;
    using BasicGame;

    public class GameStateSerializer: IGameStateSerializer
    {
        private readonly EntityState?[] previousState = new EntityState?[BasicGame.MaxEntities];

        public byte[] Serialize(BasicGame game)
        {
            using var memoryStream = new MemoryStream();
            var entities = game.Entities.ToArray();
            var spawnedEntities = game.SpawnedEntities.ToArray();
            var destroyedEntities = game.DestroyedEntities.ToArray();
            
            // Spawned Entities
            memoryStream.WriteByte((byte)spawnedEntities.Length);

            foreach (var spawnedEntity in spawnedEntities.Select(id => entities[id]))
            {
                this.previousState[spawnedEntity.ID] = new EntityState
                {
                    Position = spawnedEntity.Position,
                    Color = spawnedEntity.Color
                };
                memoryStream.WriteByte(spawnedEntity.ID);

                var positionXBytes = BitConverter.GetBytes((short)spawnedEntity.Position.X);
                var positionYBytes = BitConverter.GetBytes((short)spawnedEntity.Position.Y);
                memoryStream.Write(positionXBytes);
                memoryStream.Write(positionYBytes);

                memoryStream.WriteByte(spawnedEntity.Color.R);
                memoryStream.WriteByte(spawnedEntity.Color.G);
                memoryStream.WriteByte(spawnedEntity.Color.B);
            }

            // Destroyed Entities
            memoryStream.WriteByte((byte)destroyedEntities.Length);

            memoryStream.Write(destroyedEntities);

            foreach (var destroyedEntity in destroyedEntities)
            {
                this.previousState[destroyedEntity] = null;
            }

            // Updated entities
            foreach (var entity in entities.Where(entity => entity != null))
            {
                var optionalEntityState = this.previousState[entity.ID];

                if (optionalEntityState == null)
                {
                    continue;
                }

                var previousEntityState = optionalEntityState.Value;

                if (previousEntityState.Position == entity.Position && previousEntityState.Color == entity.Color)
                {
                    continue;
                }

                memoryStream.WriteByte(entity.ID);

                memoryStream.WriteByte((byte)entity.Velocity.X);
                memoryStream.WriteByte((byte)entity.Velocity.Y);

                var shouldInvertColor = entity.Color != previousEntityState.Color;
                var shouldInvertBytes = BitConverter.GetBytes(shouldInvertColor);
                memoryStream.Write(shouldInvertBytes);

                previousEntityState.Position = entity.Position;
                previousEntityState.Color = entity.Color;

                this.previousState[entity.ID] = previousEntityState;
            }

            return memoryStream.ToArray();
        }
    }
}