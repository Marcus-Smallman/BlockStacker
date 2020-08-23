using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using BlockStackerLibrary.Utilities;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System;
using BlockStackerLibrary.Managers;
using System.Collections.Generic;

namespace BlockStackerLibrary.Entities
{
    public class Block
    {
        public bool Landed { get; private set; }

        public bool OnTop { get; private set; }

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

        private bool fallen;

        private readonly Floor floor;

        private readonly Stack<Block> blocks;

        public Block(Texture2D texture, Vector2 position, Floor floor, Stack<Block> blocks)
        {
            this.texture = texture;
            this.position = position;
            this.floor = floor;
            this.blocks = blocks;
            this.NextPosition = position;
            size = new Vector2(texture.Width, texture.Height);

            Landed = false;
            OnTop = false;
            fallen = false;

            var rng = new Random().Next(0, 2);
            if (rng == 0)
            {
                Velocity.X = GetXVelocity(false);
            }
            else
            {
                Velocity.X = GetXVelocity(true);
            }
        }

        public void Update(GameTime gameTime)
        {
            var previousPosition = position;

            position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            var keyboardState = Keyboard.GetState();
            var touchCollection = TouchPanel.GetState();
            if (fallen == false)
            {
                if (position.X < 0)
                {
                    Velocity.X = GetXVelocity(false);
                }
                else if (position.X > Resolution.VirtualWidth - texture.Width)
                {
                    Velocity.X = GetXVelocity(true);
                }
            }

            if (Landed == true)
            {
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

            if (position.Y > Resolution.VirtualHeight)
            {
                Landed = true;
            }

            if (Landed == false)
            {
                ResolveCollisions();
            }

            if (keyboardState.IsKeyDown(Keys.Space) &&
                fallen == false)
            {
                fallen = true;
                Velocity.Y = 2000f;
                Velocity.X = 0f;
            }
            else if (touchCollection.Count > 0)
            {
                if (touchCollection.Any(touchLocation => touchLocation.State == TouchLocationState.Pressed) &&
                    fallen == false)
                {
                    fallen = true;
                    Velocity.Y = 2000f;
                    Velocity.X = 0f;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

        private bool IsCollided(out Block collidedBlock)
        {
            collidedBlock = null;
            foreach (var block in blocks.ToArray())
            {
                if (RectangleUtilities.GetHitbox(block.Position, block.Size).Intersects(RectangleUtilities.GetHitbox(position, size)) == true)
                {
                    collidedBlock = block;

                    return true;
                }
            }

            return false;
        }

        private bool IsOnFloor()
        {
            if (RectangleUtilities.GetHitbox(floor.Position, floor.Size).Intersects(RectangleUtilities.GetHitbox(position, size)) == true)
            {
                return true;
            }

            return false;
        }

        private void ResolveCollisions()
        {
            if (IsCollided(out Block collidedBlock) == true)
            {
                position.Y = collidedBlock.Position.Y - texture.Height;
                Landed = true;

                var topBlock = blocks.Peek();
                if (collidedBlock.Position == topBlock.Position)
                {
                    OnTop = true;

                    floor.NextPosition = new Vector2(floor.Position.X, floor.Position.Y + (texture.Height * GetNextPositionDistance()));
                    NextPosition = new Vector2(position.X, position.Y + (texture.Height * GetNextPositionDistance()));
                    foreach (var stackedBlock in blocks)
                    {
                        stackedBlock.NextPosition = new Vector2(stackedBlock.Position.X, stackedBlock.Position.Y + (texture.Height * GetNextPositionDistance()));
                    }
                }
            }
            else if (IsOnFloor() == true)
            {
                position.Y = floor.Position.Y - texture.Height;
                Landed = true;

                if (blocks.Count == 0)
                {
                    OnTop = true;
                    floor.NextPosition = new Vector2(floor.Position.X, floor.Position.Y + (texture.Height * GetNextPositionDistance()));
                    NextPosition = new Vector2(position.X, position.Y + (texture.Height * GetNextPositionDistance()));
                    foreach (var stackedBlock in blocks)
                    {
                        stackedBlock.NextPosition = new Vector2(stackedBlock.Position.X, stackedBlock.Position.Y + (texture.Height * GetNextPositionDistance()));
                    }
                }
            }
        }

        private int GetNextPositionDistance()
        {
            if (blocks.Count >= 20)
            {
                return 1;
            }

            return 2;
        }

        private float GetXVelocity(bool negative)
        {
            float speed = 250f;

            int multiplier = blocks.Count / 2;
            int additionalSpeed = multiplier * 20;
            if (additionalSpeed <= 300)
            {
                speed += additionalSpeed;
            }
            else
            {
                speed += 300;
            }

            return negative == true ? speed * -1 : speed;
        }
    }
}
