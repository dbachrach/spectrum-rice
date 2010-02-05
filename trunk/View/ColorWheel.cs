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
    class ColorWheel
    {
        private Texture2D wheelImg;
        private Texture2D overlayImg;

        private float curRotation;
        private float finalRotation;

        private float startTime;

        private const float rotationDuration = 2000.0f; // in milliseconds

        private bool newRotation;
        private bool Clockwise;

        public ColorWheel()
        {
            newRotation = false;
            curRotation = 0;
            finalRotation = 0;
            startTime = 0;
            Clockwise = false;
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
                startTime = gameTime.TotalGameTime.Milliseconds;
            }

            if (curRotation != finalRotation)
            {

                // Add dx to curRotation
                float time = gameTime.TotalGameTime.Milliseconds * 1.0f - startTime;
                if (time > rotationDuration)
                {
                    curRotation = finalRotation;
                }
                else
                {
                    float percentage = time / rotationDuration;

                    float dx = (float)(percentage * (2 * MathHelper.Pi / 6));

                    if (Clockwise)
                    {
                        curRotation += dx;

                        if (curRotation > finalRotation)
                        {
                            curRotation = finalRotation;
                        }
                    }
                    else
                    {
                        curRotation -= dx;
                        if (curRotation < finalRotation)
                        {
                            curRotation = finalRotation;
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(wheelImg, new Vector2(10 + wheelImg.Width / 2, 10 + wheelImg.Height / 2), null, Color.White, 0, new Vector2(wheelImg.Width / 2, wheelImg.Height / 2), 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(overlayImg, new Vector2(10 + wheelImg.Width / 2, 10 + wheelImg.Height / 2), null, Color.White, curRotation, new Vector2(wheelImg.Width / 2, wheelImg.Height / 2), 1.0f, SpriteEffects.None, 1.0f);

        }


        // delegate
        public void DidChangeColor(Colors colors, bool clockwise)
        {
            if (colors.IsSingularColor())
            {
                finalRotation = (float)(2 * Math.PI / 6 * (colors.Index()));
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
