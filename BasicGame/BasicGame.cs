namespace BasicGame
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    public class BasicGame
    {
        public const int MaxEntities = 16; 
        public Rectangle Bounds => this.bounds;
        public Rectangle SpawnZone => this.spawnZone;
        public Rectangle DestroyZone => this.destroyZone;

        public IEnumerable<Entity> Entities => this.entities;
        public IEnumerable<byte> SpawnedEntities => this.spawnedEntities;
        public IEnumerable<byte> DestroyedEntities => this.destroyedEntities;

        private Rectangle bounds;

        private Rectangle spawnZone;
        private Rectangle destroyZone;

        private TimeSpan timeToNextSpawn;

        private readonly Entity[] entities = new Entity[MaxEntities];
        private readonly Queue<byte> availableIds = new(MaxEntities);
        private readonly List<byte> spawnedEntities = new(MaxEntities);
        private readonly List<byte> destroyedEntities = new(MaxEntities);
        private readonly Random random = new();

        public BasicGame(Rectangle bounds, Random random = null)
        {
            this.bounds = bounds;

            var zoneWidth = bounds.Width / 10;
            var zoneHeight = bounds.Height / 10;
            this.spawnZone = new Rectangle(bounds.Left, bounds.Top, zoneWidth, zoneHeight);
            this.destroyZone = new Rectangle(bounds.Right - zoneWidth, bounds.Bottom - zoneHeight, zoneWidth, zoneHeight);

            this.timeToNextSpawn = TimeSpan.Zero;

            for (byte i = 0; i < MaxEntities; i++)
            {
                this.availableIds.Enqueue(i);
            }
            
            if (random != null)
            {
                this.random = random;
            }
        }

        public void Update(TimeSpan elapsedTime)
        {
            this.timeToNextSpawn -= elapsedTime;
            this.spawnedEntities.Clear();
            
            this.destroyedEntities.ForEach(this.availableIds.Enqueue);
            this.destroyedEntities.Clear();

            if (this.timeToNextSpawn <= TimeSpan.Zero)
            {
                if (this.availableIds.TryDequeue(out var id))
                {
                    this.timeToNextSpawn += TimeSpan.FromSeconds(1);
                
                    this.spawnedEntities.Add(id);
                    this.entities[id] = new Entity
                    {
                        ID = id
                    };
                    var spawnPositionX = this.random.Next(this.spawnZone.Left, this.spawnZone.Right + 1);
                    var spawnPositionY = this.random.Next(this.spawnZone.Top, this.spawnZone.Bottom + 1);
                    this.entities[id].Position = new Point(spawnPositionX, spawnPositionY);

                    var velocityX = this.random.Next(1, 7);
                    var velocityY = this.random.Next(1, 7);
                    this.entities[id].Velocity = new Point(velocityX, velocityY);

                    var r = this.random.Next(256);
                    var g = this.random.Next(256);
                    var b = this.random.Next(256);
                    this.entities[id].Color = new Color(r, g, b);
                } 
                else
                {
                    this.timeToNextSpawn = TimeSpan.Zero;
                }
            }

            for (byte i = 0; i < MaxEntities; i++)
            {
                if (this.entities[i] == null)
                {
                    continue;
                }
                this.entities[i].Position += this.entities[i].Velocity;

                if (this.entities[i].Position.X <= this.bounds.Left)
                {
                    this.entities[i].Velocity.X = Math.Abs(this.entities[i].Velocity.X);
                    this.entities[i].Color.Invert();
                }
                else if (this.entities[i].Position.X >= this.bounds.Right)
                {
                    this.entities[i].Velocity.X = -Math.Abs(this.entities[i].Velocity.X);
                    this.entities[i].Color.Invert();
                }

                if (this.entities[i].Position.Y <= this.bounds.Top)
                {
                    this.entities[i].Velocity.Y = Math.Abs(this.entities[i].Velocity.Y);
                    this.entities[i].Color.Invert();
                }
                else if (this.entities[i].Position.Y >= this.bounds.Bottom)
                {
                    this.entities[i].Velocity.Y = -Math.Abs(this.entities[i].Velocity.Y);
                    this.entities[i].Color.Invert();
                }

                if (this.destroyZone.Contains(this.entities[i].Position))
                {
                    this.entities[i] = null;
                    this.destroyedEntities.Add(i);
                }
            }
        }
    }
}