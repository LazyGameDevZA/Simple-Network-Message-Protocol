namespace GameBenchmark
{
    using System;
    using BasicGame;
    using BenchmarkDotNet.Attributes;
    using Microsoft.Xna.Framework;

    [SimpleJob(targetCount: 1000)]
    public class SerializerBenchmark
    {
        private Serializers.Json.GameStateSerializer jsonSerializer;
        private Serializers.BasicBinary.GameStateSerializer basicBinarySerializer;
        private Serializers.DomainSpecificBinary.GameStateSerializer domainSpecificSerializer;
        private Serializers.DeltaEncodedBinary.GameStateSerializer deltaEncodedSerializer;

        private BasicGame basicGame;

        [GlobalSetup]
        public void GlobalSetup()
        {
            this.jsonSerializer = new Serializers.Json.GameStateSerializer();
            this.basicBinarySerializer = new Serializers.BasicBinary.GameStateSerializer();
            this.domainSpecificSerializer = new Serializers.DomainSpecificBinary.GameStateSerializer();
            this.deltaEncodedSerializer = new Serializers.DeltaEncodedBinary.GameStateSerializer();
            
            this.basicGame = new BasicGame(new Rectangle(0, 0, 800, 480), new Random());
        }

        [IterationSetup]
        public void IterationSetup()
        {
            this.basicGame.Update(TimeSpan.FromMilliseconds(16));
        }

        [Benchmark(Baseline = true)]
        public void Json()
        {
            this.jsonSerializer.Serialize(this.basicGame);
        }

        [Benchmark]
        public void BasicBinary()
        {
            this.basicBinarySerializer.Serialize(this.basicGame);
        }

        [Benchmark]
        public void DomainSpecific()
        {
            this.domainSpecificSerializer.Serialize(this.basicGame);
        }

        [Benchmark]
        public void DeltaEncoded()
        {
            this.deltaEncodedSerializer.Serialize(this.basicGame);
        }
    }
}