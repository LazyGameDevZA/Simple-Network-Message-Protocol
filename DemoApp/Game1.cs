using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LazyGameDevZA.SimpleNetworkMessageProtocol.Server
{
    using System;
    using System.Linq;
    using BasicGame;
    public class Game1 : Game
    {
        private static readonly string[] SizeSuffixes =
            { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        
        private readonly IGameStateSerializer gameStateSerializer;
        private readonly IGameStateDeserializer gameStateDeserializer;
        
        private Texture2D dot;
        private Texture2D spawnZone;
        private Texture2D destroyZone;
        private SpriteFont font;
        
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private BasicGame basicGame;

        private int stateSize;
        private byte[] frameData = Array.Empty<byte>();

        private Entity[] entitiesToRender = Array.Empty<Entity>();

        public Game1(IGameStateSerializer gameStateSerializer, IGameStateDeserializer gameStateDeserializer)
        {
            this.gameStateSerializer = gameStateSerializer;
            this.gameStateDeserializer = gameStateDeserializer;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            this.basicGame = new BasicGame(this._graphics.GraphicsDevice.PresentationParameters.Bounds);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            this.dot = this.Content.Load<Texture2D>("dot");

            this.spawnZone = new Texture2D(GraphicsDevice, this.basicGame.SpawnZone.Width, this.basicGame.SpawnZone.Height);
            this.destroyZone = new Texture2D(GraphicsDevice, this.basicGame.DestroyZone.Width, this.basicGame.DestroyZone.Height);
            
            Color[] zoneData = new Color[this.spawnZone.Width * this.spawnZone.Height];

            for (int x = 0; x < this.spawnZone.Width; x++)
            {
                for (int y = 0; y < this.spawnZone.Height; y++)
                {
                    if (x == 0 || x == this.spawnZone.Width - 1 || y == 0 || y == this.spawnZone.Height - 1)
                    {
                        zoneData[x +  this.spawnZone.Width * y] = Color.White;
                    }
                    else
                    {
                        zoneData[x +  this.spawnZone.Width * y] = Color.Black;
                    }
                }
            }

            spawnZone.SetData(zoneData);
            
            zoneData = new Color[this.destroyZone.Width * this.destroyZone.Height];

            for (int x = 0; x < this.destroyZone.Width; x++)
            {
                for (int y = 0; y < this.destroyZone.Height; y++)
                {
                    if (x == 0 || x == this.destroyZone.Width - 1 || y == 0 || y == this.destroyZone.Height - 1)
                    {
                        zoneData[x +  this.spawnZone.Width * y] = Color.White;
                    }
                    else
                    {
                        zoneData[x +  this.spawnZone.Width * y] = Color.Black;
                    }
                }
            }

            this.destroyZone.SetData(zoneData);

            this.font = this.Content.Load<SpriteFont>("Consolas");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            this.basicGame.Update(gameTime.ElapsedGameTime);

            var frameBytes = this.gameStateSerializer.Serialize(this.basicGame);

            this.stateSize = frameBytes.Length;

            this.entitiesToRender = this.gameStateDeserializer.Deserialize(frameBytes);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            this._spriteBatch.Begin();
            
            this._spriteBatch.Draw(this.spawnZone, this.basicGame.SpawnZone.Location.ToVector2(), Color.Aqua);
            this._spriteBatch.Draw(this.destroyZone, this.basicGame.DestroyZone.Location.ToVector2(), Color.OrangeRed);

            var halfDot = dot.Bounds.Size.ToVector2() / 2;
            var aliveEntities = 0;

            foreach (var entity in this.entitiesToRender.Where(x => x != null))
            {
                this._spriteBatch.Draw(dot, entity.Position.ToVector2() - halfDot, entity.Color);
                aliveEntities++;
            }

            var entityCountText = $"Entities: {aliveEntities}";
            
            this._spriteBatch.DrawString(this.font, entityCountText, new Vector2(10, this._graphics.PreferredBackBufferHeight - 60), Color.White);
      
            int decimalPlaces = 1;
            int i = 0;
            decimal dValue = this.stateSize;
            while (Math.Round(dValue, decimalPlaces) >= 1000)
            {
                dValue /= 1024;
                i++;
            }

            var sizeText = string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[i]);

            this._spriteBatch.DrawString(this.font, sizeText, new Vector2(10, this._graphics.PreferredBackBufferHeight - 30), Color.White);
            
            this._spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
