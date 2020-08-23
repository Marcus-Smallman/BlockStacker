using BlockStackerLibrary.Entities;
using BlockStackerLibrary.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BlockStackerLibrary.Managers
{
    public class BlockManager
    {
        public Stack<Block> Blocks { get; }

        public bool HasDied { get; private set; }
        
        private Block activeBlock;
        private Texture2D blockTexture;
        private Floor floor;

        public BlockManager(Texture2D blockTexture, Floor floor)
        {
            Blocks = new Stack<Block>();
            HasDied = false;

            this.blockTexture = blockTexture;
            this.floor = floor;

            activeBlock = CreateBlock();
        }

        public void Update(GameTime gameTime)
        {
            if (activeBlock.Landed == true)
            {
                if (activeBlock.OnTop == false)
                {
                    HasDied = true;
                }
                else
                {
                    Blocks.Push(activeBlock);
                    activeBlock = CreateBlock();
                }
            }

            activeBlock.Update(gameTime);
            foreach (var block in Blocks)
            {
                if (block.Position.Y <= Resolution.VirtualHeight)
                {
                    block.Update(gameTime);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            activeBlock.Draw(spriteBatch);
            foreach (var block in Blocks)
            {
                if (block.Position.Y <= Resolution.VirtualHeight)
                {
                    block.Draw(spriteBatch);
                }
            }
        }

        public void Reset(Floor floor)
        {
            this.floor = floor;
            Blocks.Clear();
            activeBlock = CreateBlock();
            HasDied = false;
        }

        private Block CreateBlock()
        {
            return new Block(blockTexture, new Vector2(new Random().Next(0, Resolution.VirtualWidth - blockTexture.Width), blockTexture.Height * 3), floor, Blocks);
        }
    }
}
