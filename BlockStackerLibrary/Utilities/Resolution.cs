using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlockStackerLibrary.Utilities
{
    /// <remarks>
    /// This class is used to stretch the game to match the resolution. This has been taken from:
    /// http://www.david-amador.com/2010/03/xna-2d-independent-resolution-rendering/
    /// </remarks>
    public static class Resolution
    {
        public static int Width;
        public static int Height;
        public static int VirtualWidth;
        public static int VirtualHeight;

        private static GraphicsDeviceManager device = null;

        private static Matrix scaleMatrix;
        private static bool fullscreen = false;
        private static bool dirtyMatrix = true;

        public static void Init(ref GraphicsDeviceManager device)
        {
            Width = device.PreferredBackBufferWidth;
            Height = device.PreferredBackBufferHeight;
            Resolution.device = device;
            dirtyMatrix = true;
            ApplyResolutionSettings();
        }


        public static Matrix getTransformationMatrix()
        {
            if (dirtyMatrix)
            {
                RecreateScaleMatrix();
            }

            return scaleMatrix;
        }

        public static void SetResolution(int width, int height, bool FullScreen)
        {
            Width = width;
            Height = height;

            fullscreen = FullScreen;

            ApplyResolutionSettings();
        }

        public static void SetVirtualResolution(int width, int height)
        {
            VirtualWidth = width;
            VirtualHeight = height;

            dirtyMatrix = true;
        }

        private static void ApplyResolutionSettings()
        {
            if (fullscreen == false)
            {
                if ((Width <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width) &&
                    (Height <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    device.PreferredBackBufferWidth = Width;
                    device.PreferredBackBufferHeight = Height;
                    device.IsFullScreen = fullscreen;
                    device.ApplyChanges();
                }
            }
            else
            {
                foreach (var displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    if ((displayMode.Width == Width) &&
                        (displayMode.Height == Height))
                    {
                        device.PreferredBackBufferWidth = Width;
                        device.PreferredBackBufferHeight = Height;
                        device.IsFullScreen = fullscreen;
                        device.ApplyChanges();
                    }
                }
            }

            dirtyMatrix = true;

            Width = device.PreferredBackBufferWidth;
            Height = device.PreferredBackBufferHeight;
        }

        public static void BeginDraw()
        {
            FullViewport();

            device.GraphicsDevice.Clear(Color.Black);

            ResetViewport();

            device.GraphicsDevice.Clear(Color.LightGreen);
        }

        private static void RecreateScaleMatrix()
        {
            dirtyMatrix = false;
            scaleMatrix = Matrix.CreateScale(
                           (float)device.GraphicsDevice.Viewport.Width / VirtualWidth,
                           (float)device.GraphicsDevice.Viewport.Width / VirtualWidth,
                           1f);
        }


        public static void FullViewport()
        {
            var viewport = new Viewport();
            viewport.X = viewport.Y = 0;
            viewport.Width = Width;
            viewport.Height = Height;
            device.GraphicsDevice.Viewport = viewport;
        }

        public static float getVirtualAspectRatio()
        {
            return (float)VirtualWidth / (float)VirtualHeight;
        }

        public static void ResetViewport()
        {
            float targetAspectRatio = getVirtualAspectRatio();
            int width = device.PreferredBackBufferWidth;
            int height = (int)(width / targetAspectRatio + .5f);
            bool changed = false;

            if (height > device.PreferredBackBufferHeight)
            {
                height = device.PreferredBackBufferHeight;
                width = (int)(height * targetAspectRatio + .5f);
                changed = true;
            }

            var viewport = new Viewport();

            viewport.X = (device.PreferredBackBufferWidth / 2) - (width / 2);
            viewport.Y = (device.PreferredBackBufferHeight / 2) - (height / 2);
            viewport.Width = width;
            viewport.Height = height;
            viewport.MinDepth = 0;
            viewport.MaxDepth = 1;

            if (changed)
            {
                dirtyMatrix = true;
            }

            device.GraphicsDevice.Viewport = viewport;
        }

    }
}
