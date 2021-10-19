namespace Serializers.Json
{
    using System.Linq;
    using System.Text.Json;
    using BasicGame;

    public class GameStateSerializer: IGameStateSerializer
    {
        private readonly JsonSerializerOptions options = new()
        {
            IncludeFields = true
        };
        
        public byte[] Serialize(BasicGame game)
        {
            var entities = game.Entities.Select(entity =>
            {
                if (entity == null)
                {
                    return null;
                }

                return new EntityDTO
                {
                    ID = entity.ID,
                    PositionX = entity.Position.X,
                    PositionY = entity.Position.Y,
                    VelocityX = entity.Velocity.X,
                    VelocityY = entity.Velocity.Y,
                    PackedColour = entity.Color.PackedValue
                };
            });
            
            var bytes = JsonSerializer.SerializeToUtf8Bytes(entities);
            return bytes;
        }
    }

}