using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using BlockStackerLibrary.Utilities;
using System;
using System.IO;
using BlockStackerLibrary.Entities;
using BlockStackerLibrary.Managers;

namespace BlockStackerLibrary
{
    public class BlockStackerGame
    {
        private const int GAME_WINDOW_WIDTH = 720;
        private const int GAME_WINDOW_HEIGHT = 1280;

        private SpriteBatch spriteBatch;

        public SpriteFont font;
        public SpriteFont fontSmall;

        public Texture2D blockTexture;
        private BlockManager blockManager;

        public Texture2D floorTexture;
        private Floor floor;

        private string scorePath;
        private bool started;
        private int highScore;

        public BlockStackerGame(GraphicsDeviceManager graphics)
        {
            Resolution.Init(ref graphics);
        }

        public void Initialize(int gameWindowWidth = GAME_WINDOW_WIDTH, int gameWindowHeight = GAME_WINDOW_HEIGHT)
        {
            Resolution.SetVirtualResolution(GAME_WINDOW_WIDTH, GAME_WINDOW_HEIGHT);
            Resolution.SetResolution(gameWindowWidth, gameWindowHeight, false);

            started = false;

            var dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BlockStacker");
            scorePath = $@"{dataPath}\Score.txt";
            if (Directory.Exists(dataPath) == false)
            {
                Directory.CreateDirectory(dataPath);
                SaveScore();
            }
            else
            {
                highScore = int.Parse(File.ReadAllText(scorePath));
            }
        }

        public void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            font = contentManager.Load<SpriteFont>("font");
            fontSmall = contentManager.Load<SpriteFont>("fontsmall");
            blockTexture = contentManager.Load<Texture2D>("block");
            floorTexture = contentManager.Load<Texture2D>("floor");

            spriteBatch = new SpriteBatch(graphicsDevice);

            floor = CreateFloor();
            blockManager = new BlockManager(blockTexture, floor);
        }

        public void Update(Game game, GameTime gameTime)
        {
            HandleInput(game);

            if (started == true)
            {
                blockManager.Update(gameTime);
                floor.Update(gameTime);

                if (blockManager.HasDied == true)
                {
                    Reset();
                }
            }
        }

        private void HandleInput(Game game)
        {
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                game.Exit();
            }
            else if (keyboardState.IsKeyDown(Keys.Back) &&
                     started == true)
            {
                Reset();

                started = false;
            }

            if (started == false)
            {
                var mouseState = Mouse.GetState();
                var touchCollection = TouchPanel.GetState();
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    started = true;
                }
                else if (touchCollection.Count > 0)
                {
                    if (touchCollection[0].State == TouchLocationState.Pressed)
                    {
                        started = true;
                    }
                }
            }
        }

        public void Draw()
        {
            Resolution.BeginDraw();

            spriteBatch.Begin(transformMatrix: Resolution.getTransformationMatrix());

            if (started == false)
            {
                DrawStart();
            }
            else
            {
                DrawGame();
            }

            spriteBatch.End();
        }

        private void DrawStart()
        {
            var startText = "TAP TO START";
            var startTextSize = font.MeasureString(startText);
            spriteBatch.DrawString(font, startText, new Vector2((Resolution.VirtualWidth / 2) - (startTextSize.X / 2), (Resolution.VirtualHeight / 2) - (startTextSize.Y / 2)), Color.White);

            var scoreText = highScore.ToString();
            var scoreTextSize = font.MeasureString(scoreText);
            spriteBatch.DrawString(font, scoreText, new Vector2((Resolution.VirtualWidth / 2) - (scoreTextSize.X / 2), Resolution.VirtualHeight - scoreTextSize.Y - 25), Color.White);

            var highScoreText = "HIGH SCORE";
            var highScoreTextSize = fontSmall.MeasureString(highScoreText);
            spriteBatch.DrawString(fontSmall, highScoreText, new Vector2((Resolution.VirtualWidth / 2) - (highScoreTextSize.X / 2), Resolution.VirtualHeight - highScoreTextSize.Y - scoreTextSize.Y - 25), Color.White);
        }

        private void DrawGame()
        {
            blockManager.Draw(spriteBatch);
            floor.Draw(spriteBatch);

            var scoreText = blockManager.Blocks.Count.ToString();
            var scoreTextSize = font.MeasureString(scoreText);
            spriteBatch.DrawString(font, scoreText, new Vector2((Resolution.VirtualWidth / 2) - (scoreTextSize.X / 2), Resolution.VirtualHeight - scoreTextSize.Y - 25), Color.White);
        }

        private void Reset()
        {
            if (blockManager.Blocks.Count > highScore)
            {
                highScore = blockManager.Blocks.Count;
                SaveScore();
            }

            floor = CreateFloor();
            blockManager.Reset(floor);
        }

        private Floor CreateFloor()
        {
            return new Floor(floorTexture, new Vector2(0, Resolution.VirtualHeight - floorTexture.Height));
        }

        private void SaveScore()
        {
            File.WriteAllText(scorePath, highScore.ToString());
        }
    }
}
