using BlockStackerLibrary;
using Microsoft.Xna.Framework;

namespace BlockStacker
{
    public class Game1 : Game
    {
        private BlockStackerGame game;

        private GraphicsDeviceManager graphics;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            game = new BlockStackerGame(graphics);
            Window.Title = "Block Stacker";
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            game.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            game.LoadContent(GraphicsDevice, Content);
        }

        protected override void Update(GameTime gameTime)
        {
            game.Update(this, gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            game.Draw();

            base.Draw(gameTime);
        }
    }
}
