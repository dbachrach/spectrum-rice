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

namespace Spectrum
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;
        MenuItem[] menuItem = new MenuItem[4];
        int selectedItem = 0;
        private GameTexture pauseBackground;

        Level level;

        public bool Paused { get; set; }
        private KeyboardState PreviousKeyboardState { get; set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.PreferredBackBufferWidth = 1000;
            //graphics.PreferredBackBufferHeight = 700;
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
            level = Parser.Parse("demo.txt");
            level.GameRef = this;

            pauseBackground = new GameTexture(0, 1.0f, 1.0f);

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

            font = this.Content.Load<SpriteFont>("Pesca");

            Color baseColor = Color.White;
            Color selectedColor = Color.Red;

            menuItem[0] = new MenuItem(Globals.ResumeMenuItem, Globals.ResumeMenuItem, font, new Vector2(50f, 150f), baseColor, selectedColor, false);
            menuItem[1] = new MenuItem(Globals.RestartMenuItem, Globals.RestartMenuItem, font, new Vector2(50f, 200f), baseColor, selectedColor, false);
            menuItem[2] = new MenuItem(Globals.SettingsMenuItem, Globals.SettingsMenuItem, font, new Vector2(50f, 250f), baseColor, selectedColor, false);
            menuItem[3] = new MenuItem(Globals.ExitMenuItem, Globals.ExitMenuItem, font, new Vector2(50f, 300f), baseColor, selectedColor, false);

            pauseBackground.Load(Content, GraphicsDevice, null, 1, 1, 800, 600);
            pauseBackground.Pause();

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

            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.P) == true && PreviousKeyboardState.IsKeyDown(Keys.P) == false)
            {
                level.DebugMode = !level.DebugMode;
            }

            if (keyboard.IsKeyDown(Keys.Escape) == true && PreviousKeyboardState.IsKeyDown(Keys.Escape) == false)
            {
                Paused = !Paused;
            }





            if (Paused)
            {
                for (int i = 0; i < menuItem.Length; i++)
                {
                    menuItem[i].Selected = false;
                }

                if ((keyboard.IsKeyDown(Keys.Up)) && (PreviousKeyboardState.IsKeyUp(Keys.Up)))
                {
                    selectedItem -= 1;
                    if (selectedItem == -1)
                    {
                        selectedItem = menuItem.Length - 1;
                    }
                }

                if ((keyboard.IsKeyDown(Keys.Down)) && (PreviousKeyboardState.IsKeyUp(Keys.Down)))
                {
                    selectedItem += 1;
                    if (selectedItem == menuItem.Length)
                    {
                        selectedItem = 0;
                    }
                }

                if ((keyboard.IsKeyDown(Keys.Enter)) && (PreviousKeyboardState.IsKeyUp(Keys.Enter)))
                {
                    if (menuItem[selectedItem].Name.Equals(Globals.ResumeMenuItem))
                    {
                        Paused = false;
                    }
                    else if (menuItem[selectedItem].Name.Equals(Globals.RestartMenuItem))
                    {
                        // TODO: Restart
                    }
                    else if (menuItem[selectedItem].Name.Equals(Globals.SettingsMenuItem))
                    {
                        // TODO: Show settings
                    }
                    else if (menuItem[selectedItem].Name.Equals(Globals.ExitMenuItem))
                    {
                        Exit();
                    }
                }

                menuItem[selectedItem].Selected = true;
            }
            else
            {
                level.Update(gameTime);
            }


            PreviousKeyboardState = keyboard;

            base.Update(gameTime);
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
                pauseBackground.DrawFrame(spriteBatch, level.CurrentColor, new Vector2(400, 300), SpriteEffects.None);
                for (int i = 0; i < menuItem.Length; i++)
                {
                    menuItem[i].Draw(spriteBatch);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
