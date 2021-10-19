namespace LazyGameDevZA.SimpleNetworkMessageProtocol.Server
{
    using System;
    using BasicGame;
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            IGameStateSerializer serializer;
            IGameStateDeserializer deserializer;
            switch (args[0])
            {
                case "JSON":
                    serializer = new Serializers.Json.GameStateSerializer();
                    deserializer = new Serializers.Json.GameStateDeserializer();
                    break;
                case "Basic":
                    serializer = new Serializers.BasicBinary.GameStateSerializer();
                    deserializer = new Serializers.BasicBinary.GameStateDeserializer();
                    break;
                case "Domain":
                    serializer = new Serializers.DomainSpecificBinary.GameStateSerializer();
                    deserializer = new Serializers.DomainSpecificBinary.GameStateDeserializer();
                    break;
                case "Delta":
                    serializer = new Serializers.DeltaEncodedBinary.GameStateSerializer();
                    deserializer = new Serializers.DeltaEncodedBinary.GameStateDeserializer();
                    break;
                default:
                    throw new Exception("invalid args");
            }
            using var game = new Game1(serializer, deserializer);
            game.Run();
        }
    }
}