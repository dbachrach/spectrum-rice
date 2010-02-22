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
using Spectrum.Model;

namespace Spectrum.View
{
    class ColorWheel : IColorIndicator
    {
        private const string contentPath = "rainbow/";

        private Texture2D wheelImg;
        private Texture2D overlayImg;

        private float scale = .25f;

        private double curRotation;
        private double finalRotation;

        private double startTime;

        private const double rotationDuration = 200.0; // in milliseconds

        private bool newRotation;
        private bool Clockwise;

        public bool MoveBG { get; set; }

        public ColorWheel()
        {
            newRotation = false;
            curRotation = 0;
            finalRotation = 0;
            startTime = 0;
            Clockwise = false;

            MoveBG = true;
        }

        public void LoadContent(ContentManager manager, GraphicsDevice graphicsDevice)
        {
            wheelImg = manager.Load<Texture2D>(contentPath + "circle");
            overlayImg = manager.Load<Texture2D>(contentPath + "WedgeCircle");
        }

        public void Update(GameTime gameTime)
        {
            if (newRotation) 
            {
                newRotation = false;
                startTime = gameTime.TotalRealTime.TotalMilliseconds;
            }

            if (curRotation != finalRotation)
            {

                // Add dx to curRotation
                double time = gameTime.TotalRealTime.TotalMilliseconds - startTime;
                if (time >= rotationDuration)
                {
                    curRotation = finalRotation;
                }
                else
                {
                    double percentage = time / rotationDuration;
                    
                    double dx = (1.0 - percentage) * (2 * Math.PI / 6);
                    if (Clockwise)
                    {
                        curRotation =  finalRotation - dx;
                    }
                    else
                    {
                        curRotation = finalRotation + dx;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 cent = new Vector2((wheelImg.Width * scale) / 2, (wheelImg.Height * scale) / 2);
            Vector2 srcCenter = new Vector2(wheelImg.Width / 2, wheelImg.Height / 2);

            spriteBatch.Draw(wheelImg, cent, null, Color.White, MoveBG ? (float)-curRotation : 0, srcCenter, scale, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(overlayImg, new Vector2(cent.X - (overlayImg.Width*scale/2), 0), null, Color.White, MoveBG ? 0 : (float)curRotation, Vector2.Zero, scale, SpriteEffects.None, 1.0f);
        }

        public void SetColor(Colors colors)
        {
            finalRotation = (2 * Math.PI / 6 * (colors.Index()));
        }

        // delegate
        public void DidChangeColor(Colors colors, bool rightward)
        {
            if (colors.IsSingularColor())
            {
                SetColor(colors);
                newRotation = true;
                Clockwise = rightward;
            }
            else
            {
                // TODO: What to do when it's all colors?
            }
        }
    }
}
