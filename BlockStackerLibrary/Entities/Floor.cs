using BlockStackerLibrary.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlockStackerLibrary.Entities
{
    public class Floor
    {
        private Texture2D texture;

        public Vector2 Velocity;

        public Vector2 NextPosition;

        public Vector2 Position
        {
            get
            {
                return position;
            }
        }

        private Vector2 position;

        public Vector2 Size
        {
            get
            {
                return size;
            }
        }

        private Vector2 size;

        public Floor(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
            this.NextPosition = position;
            size = new Vector2(texture.Width, texture.Height);
        }

        public void Update(GameTime gameTime)
        {
            if (position.Y <= Resolution.VirtualHeight)
            {
                position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (this.position.Y < this.NextPosition.Y)
                {
                    Velocity.Y = 1000f;
                }

                if (this.position.Y >= this.NextPosition.Y)
                {
                    this.position.Y = this.NextPosition.Y;
                    Velocity.Y = 0f;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (position.Y <= Resolution.VirtualHeight)
            {
                spriteBatch.Draw(texture, position, Color.White);
            }
        }
    }
}
