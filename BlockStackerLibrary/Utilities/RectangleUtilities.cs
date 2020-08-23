using Microsoft.Xna.Framework;
using System;

namespace BlockStackerLibrary.Utilities
{
    public static class RectangleUtilities
    {
        public static Rectangle GetHitbox(Vector2 position, Vector2 size)
        {
            return new Rectangle((int)Math.Round(position.X), (int)Math.Round(position.Y), (int)Math.Round(size.X), (int)Math.Round(size.Y));
        }
    }
}
