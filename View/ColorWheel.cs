﻿using System;
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

        private double curRotation;
        private double finalRotation;

        private double startTime;

        private const double rotationDuration = 400.0; // in milliseconds

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
                startTime = gameTime.TotalRealTime.TotalMilliseconds;
            }

            if (curRotation != finalRotation)
            {

                // Add dx to curRotation
                double time = gameTime.TotalRealTime.TotalMilliseconds - startTime;
                Console.WriteLine("time ({0} - {1}) {2}", gameTime.TotalRealTime.TotalMilliseconds, startTime, time);
                if (time > rotationDuration)
                {
                    curRotation = finalRotation;
                }
                else
                {
                    double percentage = time / rotationDuration;

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
            Console.WriteLine("{0} {1}", curRotation, finalRotation);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(wheelImg, new Vector2(10 + wheelImg.Width / 2, 10 + wheelImg.Height / 2), null, Color.White, 0, new Vector2(wheelImg.Width / 2, wheelImg.Height / 2), 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(overlayImg, new Vector2(10 + wheelImg.Width / 2, 10 + wheelImg.Height / 2), null, Color.White, (float)curRotation, new Vector2(wheelImg.Width / 2, wheelImg.Height / 2), 1.0f, SpriteEffects.None, 1.0f);

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
