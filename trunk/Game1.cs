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

        private Level level;
        TimeSpan elapsedTime = TimeSpan.Zero;
        int frameCount = 0;
        int frameRate = 0;

        private PauseMenu pauseMenu;
        public bool Paused { get; set; }

        private string levelPath = "Levels/";
        private string levelExtension = ".txt";
        private string[] levels = { "TrainingDay", "DrEvil"};
        private int levelIndex;
        private const int GameWidth = 1280;
        private const int GameHeight = 720;

        public Game1()
        {
            levelIndex = 0;

            graphics = new GraphicsDeviceManager(this);
            // TODO: Move this to load level when we make that function
            graphics.PreferredBackBufferWidth = GameWidth;
            graphics.PreferredBackBufferHeight = GameHeight;

            LoadLevel(false);

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

            pauseMenu = new PauseMenu(this, GameWidth, GameHeight);
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

            // update frame rate
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCount;
                frameCount = 0;
            }


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

            frameCount++;
            string fps = string.Format("fps: {0}", frameRate);

            spriteBatch.Begin();
            level.Draw(spriteBatch);
            spriteBatch.DrawString(level.font, fps, Vector2.Zero, Color.Black);

            if (Paused)
            {
                pauseMenu.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void Restart()
        {
            // TODO: use reload current level
            LoadLevel(true);
        }

        public void Win()
        {
            levelIndex++;
            LoadLevel(true);
        }

        public void LoadLevel(bool loadContent)
        {
            level = Parser.Parse(levelPath + levels[levelIndex % levels.Count<string>()] + levelExtension);
            level.GameRef = this;
            if (loadContent)
            {
                level.LoadContent(Content, GraphicsDevice);
            }
        }
    }
}
