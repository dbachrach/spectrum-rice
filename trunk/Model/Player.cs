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
    enum PlayerState { Walking, Jumping }

    class Player : GameObject
    {
        public int TimesDied { get; set; }

        public TimeSpan PlayTime { get; set; }

        public GameObject Possession { get; set; }

        private KeyboardState PreviousKeyboardState { get; set; }

        private PlayerState State { get; set; }

        private Vector2 StartingPosition { get; set; }

        private const float MaxJumpHeight = 90.0f;

        public Player()
            : base()
        {
            Id = -1;
            ImageName = "dude";
            TimesDied = 0;
            PlayTime = TimeSpan.Zero;
            Possession = null;
            State = PlayerState.Walking;
        }

        public override void Update(GameTime theGameTime)
        {
            KeyboardState aCurrentKeyboardState = Keyboard.GetState();
            UpdateMovement(aCurrentKeyboardState);
            UpdateJump(aCurrentKeyboardState);
            UpdateColor(aCurrentKeyboardState);
            UpdatePickup(aCurrentKeyboardState);
            PreviousKeyboardState = aCurrentKeyboardState;
            base.Update(theGameTime);
        }

        private void UpdateMovement(KeyboardState aCurrentKeyboardState)
        {
            Vector2 v1 = Velocity;
            v1.X = 0;
            Velocity = v1;

            DrawEffects = SpriteEffects.None;

            if (Keyboard.GetState().IsKeyDown(Keys.Left) == true)
            {
                Vector2 v = Velocity;
                v.X = -3;
                Velocity = v;
                DrawEffects = SpriteEffects.FlipHorizontally;
            }
            else if (aCurrentKeyboardState.IsKeyDown(Keys.Right) == true)
            {
                Vector2 v = Velocity;
                v.X = 3;
                Velocity = v;
            }
        }

        private void UpdateJump(KeyboardState aCurrentKeyboardState)
        {
            if (State == PlayerState.Walking)
            {
                if (aCurrentKeyboardState.IsKeyDown(Keys.Space) == true && PreviousKeyboardState.IsKeyDown(Keys.Space) == false)
                {
                    Jump();
                }
            }

            if (State == PlayerState.Jumping)
            {
                if (StartingPosition.Y - Position.Y > MaxJumpHeight)
                {
                    Vector2 v = Velocity;
                    v.Y = 3;
                    Velocity = v;
                }

                if (Position.Y > StartingPosition.Y)
                {
                    Vector2 p = Position;
                    p.Y = StartingPosition.Y;
                    Position = p;

                    State = PlayerState.Walking;

                    Vector2 v = Velocity;
                    v.Y = 0;
                    Velocity = v;
                }
            }
        }

        private void UpdateColor(KeyboardState aCurrentKeyboardState)
        {
            if (aCurrentKeyboardState.IsKeyDown(Keys.A) == true && PreviousKeyboardState.IsKeyDown(Keys.A) == false)
            {
                Container.CurrentColor = Container.CurrentColor.ForwardColor();
            }
            else if (aCurrentKeyboardState.IsKeyDown(Keys.Z) == true && PreviousKeyboardState.IsKeyDown(Keys.Z) == false)
            {
                Container.CurrentColor = Container.CurrentColor.BackwardColor();
            }
        }

        private void UpdatePickup(KeyboardState aCurrentKeyboardState)
        {
            if (aCurrentKeyboardState.IsKeyDown(Keys.X) == true && PreviousKeyboardState.IsKeyDown(Keys.X) == false)
            {
                /* TODO: find object near that is pickupable and pick it up. */
                Possession = null;
            }
        }

        private void Jump()
        {
            if (State != PlayerState.Jumping)
            {
                State = PlayerState.Jumping;
                StartingPosition = Position;
                Vector2 v = Velocity;
                v.Y = -3;
                Velocity = v;
            }
        }
    }
}
