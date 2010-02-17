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
        private Texture2D wheelImg;
        private Texture2D overlayImg;

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
            wheelImg = manager.Load<Texture2D>("color-wheel");
            overlayImg = manager.Load<Texture2D>("color-wheel-overlay");
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
            spriteBatch.Draw(wheelImg, new Vector2(10 + wheelImg.Width / 2, 10 + wheelImg.Height / 2), null, Color.White, MoveBG ? (float)-curRotation : 0, new Vector2(wheelImg.Width / 2, wheelImg.Height / 2), 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(overlayImg, new Vector2(10 + wheelImg.Width / 2, 10 + wheelImg.Height / 2), null, Color.White, MoveBG ? 0 : (float)curRotation, new Vector2(wheelImg.Width / 2, wheelImg.Height / 2), 1.0f, SpriteEffects.None, 1.0f);
        }

        public void SetWheelColor(Colors colors)
        {
            finalRotation = (2 * Math.PI / 6 * (colors.Index()));
        }

        // delegate
        public void DidChangeColor(Colors colors, bool clockwise)
        {
            if (colors.IsSingularColor())
            {
                SetWheelColor(colors);
                newRotation = true;
                Clockwise = clockwise;
            }
            else
            {
                // TODO: What to do when it's all colors?
            }
        }
    }
}
