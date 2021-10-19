namespace BasicGame
{
    public interface IGameStateSerializer
    {
        byte[] Serialize(BasicGame game);
    }
}