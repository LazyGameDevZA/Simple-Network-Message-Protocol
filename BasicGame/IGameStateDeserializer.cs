namespace BasicGame
{
    public interface IGameStateDeserializer
    {
        Entity[] Deserialize(byte[] data);
    }
}