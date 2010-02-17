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
    class ColorBar : IColorIndicator
    {
        private const string contentPath = "rainbow/";
        private const float scale = .5f;
        private Texture2D borderImg;
        private Texture2D barImg;
        private Texture2D wedgeImg;

        private double curPosn;
        private double finalPosn;

        private double startTime;

        private const double moveDuration = 200.0; // in milliseconds

        private bool newMove;
        private bool toRight;

        public bool MoveBG { get; set; }

        public ColorBar()
        {
            newMove = false;
            curPosn = 0;
            finalPosn = 0;
            startTime = 0;
            toRight = false;

            MoveBG = true;
        }

        public void LoadContent(ContentManager manager, GraphicsDevice graphicsDevice)
        {
            borderImg = manager.Load<Texture2D>(contentPath + "border");
            barImg = manager.Load<Texture2D>(contentPath + "Bar");
            wedgeImg = manager.Load<Texture2D>(contentPath + "wedge");
        }

        public void Update(GameTime gameTime)
        {
            if (newMove)
            {
                newMove = false;
                startTime = gameTime.TotalRealTime.TotalMilliseconds;
            }

            if (curPosn != finalPosn)
            {

                // Add dx to curRotation
                double time = gameTime.TotalRealTime.TotalMilliseconds - startTime;
                if (time >= moveDuration)
                {
                    curPosn = finalPosn;
                }
                else
                {
                    double percentage = time / moveDuration;

                    double dx = (1.0 - percentage) * ((barImg.Width * scale) / 6);
                    if (toRight)
                    {
                        curPosn = finalPosn - dx;
                    }
                    else
                    {
                        curPosn = finalPosn + dx;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(barImg, new Vector2((float)curPosn,50), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(barImg, new Vector2((float)curPosn - (barImg.Width * scale), 50), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(barImg, new Vector2((float)curPosn + (barImg.Width * scale), 50), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 1.0f);

            spriteBatch.Draw(borderImg, new Vector2(0, (84*scale)), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(wedgeImg, new Vector2(((barImg.Width * scale) / 2) - ((wedgeImg.Width*scale) / 2), (80*scale)), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 1.0f);
        }

        public void SetColor(Colors colors)
        {
            finalPosn = (colors.Index() * ((barImg.Width * scale) / 6));
        }

        // delegate
        public void DidChangeColor(Colors colors, bool rightward)
        {
            if (colors.IsSingularColor())
            {
                SetColor(colors);
                newMove = true;
                toRight = rightward;
            }
            else
            {
                // TODO: What to do when it's all colors?
            }
        }
    }
}
