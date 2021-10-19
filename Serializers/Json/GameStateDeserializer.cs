namespace Serializers.Json
{
    using System.Text.Json;
    using BasicGame;
    using Microsoft.Xna.Framework;
    
    public class GameStateDeserializer: IGameStateDeserializer
    {
        public Entity[] Deserialize(byte[] data)
        {
            var reader = new Utf8JsonReader(data);
            var dtos = JsonSerializer.Deserialize<EntityDTO[]>(ref reader);

            var entities = new Entity[dtos.Length];

            for (int i = 0; i < dtos.Length; i++)
            {
                var entityDto = dtos[i];

                if (entityDto == null)
                {
                    continue;
                }
                
                entities[i] = new Entity()
                {
                    ID = entityDto.ID,
                    Position = new Point(entityDto.PositionX, entityDto.PositionY),
                    Color = new Color(entityDto.PackedColour)
                };
            }

            return entities;
        }
    }
}