using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
using Spectrum.View;

namespace Spectrum
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Level level;

        private PauseMenu pauseMenu;
        public bool Paused { get; set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            level = Parser.Parse("cloudy.txt");
            level.GameRef = this;

            // TODO: Move this to load level when we make that function
            graphics.PreferredBackBufferHeight = (int)level.Height;
            graphics.PreferredBackBufferWidth = (int)level.Width;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsFixedTimeStep = true;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 10);

            pauseMenu = new PauseMenu(this);
            Paused = false;

            base.Initialize();
        }
        

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pauseMenu.LoadContent(Content, GraphicsDevice);

            level.LoadContent(Content, GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Globals.Keyboard = Keyboard.GetState();
            Globals.Gamepad = GamePad.GetState(PlayerIndex.One);

            if (Globals.UserInputPress(Keys.P, Buttons.Back))
            {
                level.DebugMode = !level.DebugMode;
            }

            if (Globals.UserInputPress(Keys.Escape, Buttons.Start))
            {
                Paused = !Paused;
            }

            if (Paused)
            {
                pauseMenu.Update(gameTime);
            }
            else
            {
                level.Update(gameTime);
            }

            base.Update(gameTime);

            Globals.PreviousKeyboard = Globals.Keyboard;
            Globals.PreviousGamepad = Globals.Gamepad;

            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(level.CurrentColor.SystemColor());

            spriteBatch.Begin();
            level.Draw(spriteBatch);

            if (Paused)
            {
                pauseMenu.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
