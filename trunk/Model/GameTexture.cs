using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Spectrum.Model
{
    class GameTexture
    {
        private int framecount;
        private Texture2D myTexture;
        private float TimePerFrame;
        private int Frame;
        private float TotalElapsed;
        private bool Paused;
        public float Rotation, Scale, Depth;
        public Vector2 Origin;
        private string Asset;

        public int FrameOffset;


        public Vector2 LocalOffset { get; set; }

        public int AssetCount { get; set; }

        public GameTexture(float Rotation, float Scale, float Depth)
        {

            this.Origin = Vector2.Zero;
            this.Rotation = Rotation;
            this.Scale = Scale;
            this.Depth = Depth;
            this.LocalOffset = Vector2.Zero;

            AssetCount = 7;
        }

        public void Load(Texture2D tex, int FrameCount, int FramesPerSec)
        {
            Asset = "";
            framecount = FrameCount;
            myTexture = tex;
            Console.WriteLine("Loaded tex: {0}", myTexture);

            this.Origin = new Vector2(this.TextureSize().X / 2, this.TextureSize().Y / 2);

            TimePerFrame = (float)1 / FramesPerSec;
            Frame = 0;
            TotalElapsed = 0;
            Paused = false;

        }
        public void Load(ContentManager content, GraphicsDevice graphicsDevice, string asset, int FrameCount, int FramesPerSec)
        {
            
            Load(content, graphicsDevice, asset, FrameCount, FramesPerSec, 0, 0);
        }

        public void Load(ContentManager content, GraphicsDevice graphicsDevice, string asset, int FrameCount, int FramesPerSec, int w, int h)
        {
            Load(content, graphicsDevice, asset, FrameCount, FramesPerSec, w, h, 150);
        }
        public void Load(ContentManager content, GraphicsDevice graphicsDevice, string asset, int FrameCount, int FramesPerSec, int w, int h, int alpha)
        {
            Asset = asset;
            framecount = FrameCount;
            if (asset == null || asset.Equals(""))
            {
                AssetCount = 1;
                myTexture = CreateRectangle(w, h * AssetCount, graphicsDevice, (byte)alpha);
            }
            else
            {
                // TODO: Get all images to have the white space at the bottom so that we don't need these special cases
                if (asset.Equals("pl"))
                {  
                    AssetCount = 6;
                }
                else if (asset.Equals("hint"))
                {
                    AssetCount = 1;
                }

                myTexture = content.Load<Texture2D>(asset);
            }

            this.Origin = new Vector2(this.TextureSize().X / 2, this.TextureSize().Y / 2);

            TimePerFrame = (float)1 / FramesPerSec;
            Frame = 0;
            TotalElapsed = 0;
            Paused = false;
        }

        public void SetFrameData(int count, int offset, int fps)
        {
            framecount = count;
            FrameOffset = offset;
            Frame = 0;
            TotalElapsed = 0;
            TimePerFrame = (float)1 / fps;
        }

        // class AnimatedTexture
        public void UpdateFrame(float elapsed)
        {

            if (Paused)
                return;

            TotalElapsed += elapsed;

            if (TotalElapsed > TimePerFrame)
            {

                Frame++;

                // Keep the Frame between 0 and the total frames, minus one.
                Frame = Frame % framecount;
                TotalElapsed -= TimePerFrame;

            }

        }

        public void DrawFrame(SpriteBatch Batch, Colors color, Vector2 screenpos, SpriteEffects drawEffects, bool whiteMode)
        {
            if (Paused)
            {
                Frame = 0;
            }
            DrawFrame(Batch, Frame, color, screenpos, drawEffects, whiteMode);

        }

        public void DrawFrame(SpriteBatch Batch, int Frame, Colors color, Vector2 screenpos, SpriteEffects drawEffects, bool whiteMode)
        {
            int FrameWidth = myTexture.Width / framecount;

            int divisor = 0;

            if (whiteMode) 
            {
                divisor = 6;
            }
            else if (color.Equals(Colors.RedColor))
            {
                divisor = 0;
            }
            else if (color.Equals(Colors.OrangeColor))
            {
                divisor = 1;
            }
            else if (color.Equals(Colors.YellowColor))
            {
                divisor = 2;
            }
            else if (color.Equals(Colors.GreenColor))
            {
                divisor = 3;
            }
            else if (color.Equals(Colors.BlueColor))
            {
                divisor = 4;
            }
            else if (color.Equals(Colors.PurpleColor))
            {
                divisor = 5;
            }
            else if (AssetCount == 7)
            {
                divisor = 6;
            }


            int FrameHeight = myTexture.Height / AssetCount;

            if (Asset == "tutorial/tutorialThumb") 
            {
                FrameHeight = myTexture.Height;
                divisor = 0;
            }

            if (AssetCount == 1)
            {
                divisor = 0;
            }

            Rectangle sourcerect = new Rectangle(FrameWidth * (Frame + FrameOffset), FrameHeight * divisor, FrameWidth, FrameHeight);
            Batch.Draw(myTexture, screenpos + LocalOffset, sourcerect, Color.White,Rotation, Origin, Scale, drawEffects, Depth);

        }

        public Vector2 TextureSize()
        {
            int FrameWidth = myTexture.Width / framecount;
            int FrameHeight = myTexture.Height / AssetCount;
            return new Vector2(FrameWidth, FrameHeight);
        }

        public bool IsPaused
        {

            get { return Paused; }

        }

        public void Reset()
        {

            Frame = 0;
            TotalElapsed = 0f;

        }

        public void Stop()
        {

            Pause();
            Reset();

        }

        public void Play()
        {

            Paused = false;

        }

        public void Pause()
        {

            Paused = true;

        }



        private Texture2D CreateRectangle(int width, int height, GraphicsDevice graphicsDevice)
        {
            return CreateRectangle(width, height, graphicsDevice, 150);
        }

        private Texture2D CreateRectangle(int width, int height, GraphicsDevice graphicsDevice, byte alpha)
        {
            Texture2D rectangleTexture = new Texture2D(graphicsDevice, width, height, 1, TextureUsage.None, SurfaceFormat.Color); // create the rectangle texture, ,but it will have no color! lets fix that
            Color[] color = new Color[width * height]; //set the color to the amount of pixels in the textures
            for (int i = 0; i < color.Length; i++) //loop through all the colors setting them to whatever values we want
            {
                color[i] = new Color(0, 0, 0, alpha);
            }
            rectangleTexture.SetData(color); //set the color data on the texture
            return rectangleTexture; //return the texture
        }
    }
}
