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

namespace Spectrum.Model
{
    enum Direction { Up, Down, Left, Right, None }

    class GameObject
    {
        public string Id { get; set; }
        public Colors ViewableColors { get; set; }

        private Rectangle _boundary;
        private Vector2 _velocity;

        public Rectangle Boundary { get { return _boundary; } set { _boundary = value; } }
        public string ImageName { get; set; }
        public bool AffectedByGravity { get; set; }
        public Vector2 Velocity { get { return _velocity; } set { _velocity = value; } }
        public List<GameObject> CombineObjects { get; set; }
		public List<GameObject> CombinableWith { get; set; }
        public bool Pickupable { get; set; }
        public bool Inactive { get; set; }
        public Texture2D InactiveImage { get; set; }
        public List<Event> Events { get; set; }
        public bool ExistsWhenNotViewed { get; set; }
        public Level Container { get; set; }
        public int FrameCount { get; set; }
        public int FramesPerSec { get; set; }
        public Direction DirectionFacing { get; set; }

        public GameTexture Texture;

        private float GravityConstant = 1.3f;

		/* Default Constructor */
		public GameObject() {
			ViewableColors = Colors.AllColors;
			AffectedByGravity = true;
			Velocity = new Vector2(0,0);
            CombineObjects = new List<GameObject>();
			CombinableWith = null;
			Pickupable = false;
			Inactive = false;
			ExistsWhenNotViewed = false;
            DirectionFacing = Direction.Right;

            FrameCount = 1;
            FramesPerSec = 1;
		}

        public void SetPosition(int x, int y)
        {
            _boundary.Location = new Point(x, y);
        }

        public void SetPosition(Vector2 p)
        {
            SetPosition(new Point((int) p.X, (int) p.Y));
        }

        public void SetPosition(Point p)
        {
            _boundary.Location = p;
        }

        public void SetSize(int width, int height)
        {
            _boundary.Width = width;
            _boundary.Height = height;
        }
        public Vector2 Position()
        {
            return new Vector2(_boundary.Left, _boundary.Top);
        }
        public Vector2 Size()
        {
            return new Vector2(_boundary.Width, _boundary.Height);
        }

        public bool currentlyVisible()
        {
            return Container.CurrentColor.Contains(this.ViewableColors);
        }

        //Load the texture for the sprite using the Content Pipeline
        public void LoadContent(ContentManager theContentManager, GraphicsDevice graphicsDevice)
        {
            
            Texture = new GameTexture(Vector2.Zero, 0.0f, 1.0f, .5f);
            Texture.Load(theContentManager, graphicsDevice, ImageName, FrameCount, FramesPerSec);
            Texture.Pause();

            SetSize((int)Texture.TextureSize().X, (int)Texture.TextureSize().Y);
        }

        //Draw the sprite to the screen
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(currentlyVisible()) {
                Texture.DrawFrame(spriteBatch, Container.CurrentColor, new Vector2(Boundary.Left, Boundary.Top), DrawEffects());
            }
            
        }

        //Update the Sprite and change it's position based on the passed in speed, direction and elapsed time.
        public virtual void Update(GameTime theGameTime)
        {
            if (AffectedByGravity && currentlyVisible())
            {
                Velocity = new Vector2(Velocity.X, Velocity.Y + GravityConstant);
            }

            if (!Velocity.Equals(Vector2.Zero) && !CheckCollision())
            {
                SetPosition((int)(Position().X + Velocity.X), (int)(Position().Y + Velocity.Y));
            }

            float elapsed = (float)theGameTime.ElapsedGameTime.TotalSeconds;
            Texture.UpdateFrame(elapsed);

            if (CombinableWith != null && CombineObjects.Count == 0)
            {
                foreach (GameObject obj in CombinableWith)
                {
                    
                    if (this.PositionFuzzyEqual(obj.Position()))
                    {
                        GameObject g = this.CombineObjectWith(obj);
                        g.ViewableColors = this.ViewableColors.ColorByMixingWith(obj.ViewableColors);
                        this.CombineObjects.Add(g);
                        obj.CombineObjects.Add(g);

                        g.LoadContent(Container.GameRef.Content, Container.GameRef.GraphicsDevice);
                        Console.WriteLine("Created obj");
                        Container.DeferAddGameObject(g);
                    }
                }
            }

        }

        // If object collides, then the position & velocity of the object is updated
        private bool CheckCollision()
        {
            Rectangle bothRect = new Rectangle((int)(Position().X + Velocity.X), (int)(Position().Y + Velocity.Y), (int)Size().X, (int)Size().Y);

            List<GameObject> objects = CollisionWithRect(bothRect);

            // see if the object collides with anything
            if (objects.Count == 0)
            {
                return false;
            }

            
            Rectangle yRect = new Rectangle((int)(Position().X), (int)(Position().Y + Velocity.Y), (int)Size().X, (int)Size().Y);
            Rectangle xRect = new Rectangle((int)(Position().X + Velocity.X), (int)(Position().Y), (int)Size().X, (int)Size().Y);
            bool collidesY = false;
            bool collidesX = false;

            foreach (GameObject obj in objects)
            {
                if (obj.Boundary.Intersects(yRect))
                {
                    collidesY = true;   
                }

                if(obj.Boundary.Intersects(xRect))
                {
                    collidesX = true;
                }

                if (collidesX && collidesY)
                {
                    int signX = (Velocity.X >= 0 ? 1 : -1);
                    int signY = (Velocity.Y >= 0 ? 1 : -1);

                    int totalDX = Math.Abs((int)Velocity.X);
                    int totalDY = Math.Abs((int)Velocity.Y);

                    int dx = totalDX;
                    int dy = totalDY;
                    
                    if(totalDX > totalDY)
                    {
                        while (dx > 0)
                        {
                            dx--;
                            dy = (int)(totalDY * (float)dx / totalDX);
                            Rectangle interRect = new Rectangle((int)(Position().X + (signX * dx)), (int)(Position().Y + (signY * dy)), (int)Size().X, (int)Size().Y);
                            if (!interRect.Intersects(obj.Boundary))
                            {
                                SetPosition(interRect.Location);
                                Velocity = Vector2.Zero;
                                Console.WriteLine("Both: " + obj.Id);
                                //return true;
                            }
                        }
                    }
                    else
                    {
                        while (dy > 0)
                        {
                            dy--;
                            dx = (int)(totalDX * (float)dy / totalDY);
                            Rectangle interRect = new Rectangle((int)(Position().X + (signX * dx)), (int)(Position().Y + (signY * dy)), (int)Size().X, (int)Size().Y);
                            if (!interRect.Intersects(obj.Boundary))
                            {
                                SetPosition(interRect.Location);
                                Velocity = Vector2.Zero;
                                Console.WriteLine("Both: " + obj.Id);
                                //return true;
                            }
                        }
                    }
                }

                else if (collidesY)
                {
                    if (Velocity.Y == 0)
                    {
                        // do nothing
                    }
                    else if (Velocity.Y > 0) // if falling
                    {
                        SetPosition((int)(Position().X), (int)(obj.Position().Y - Size().Y));
                        this.DidHitGround();
                    }
                    else // if jumping
                    {
                        SetPosition((int)(Position().X), (int)(obj.Position().Y + obj.Size().Y));
                    }

                    Velocity = new Vector2(Velocity.X, 0);
                    Console.WriteLine("Y: " + obj.Id);
                }
                else if (collidesX)
                {
                    if (Velocity.X == 0)
                    {
                        // do nothing
                    }
                    else if (Velocity.X > 0) // if moving right
                    {
                        SetPosition((int)(obj.Position().X - Size().X), (int)(Position().Y));
                    }
                    else // if moving left
                    {
                        SetPosition((int)(obj.Position().X + obj.Size().X), (int)(Position().Y));
                    }

                    Velocity = new Vector2(0, Velocity.Y);
                    Console.WriteLine("X: " + obj.Id);
                }
            }

            return collidesY && collidesX;
        }

        private List<GameObject> CollisionWithRect(Rectangle rect)
        {
            List<GameObject> objList = new List<GameObject>();

            foreach (GameObject obj in Container.GameObjects)
            {
                /* Check for collisions with player to an obj */
                /* TODO: The line below is messy */
                if (obj != this && this.currentlyVisible() && (obj.currentlyVisible() || (obj.ExistsWhenNotViewed && !(this is Player))))
                {
                    if (rect.Intersects(obj.Boundary))
                    {
                        objList.Add(obj);
                    }
                }
            }

            this.DidCollideWithObjects(objList);

            return objList;
        }

        // Returns a SpriteEffects value based on the DirectionFacing property of this GameObject
        public SpriteEffects DrawEffects()
        {
            SpriteEffects effects = SpriteEffects.None;

            if (DirectionFacing == Direction.Left)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            return effects;
        }


        /* Notifications */

        protected virtual void DidCollideWithObjects(List<GameObject> objs)
        {
            foreach (GameObject obj in objs)
            {
                if (obj.Events != null && obj.Events.Count > 0)
                {
                    /* TODO: Do we need to go through the event list in THIS also? */
                    foreach (Event e in obj.Events)
                    {
                        if (e.Type == EventType.Collision && e.CollisionTarget == this)
                        {
                            e.Execute();
                        }
                    }
                }
            }
        }
        protected virtual void DidHitGround()
        {
            // nada
        }

        public virtual GameObject CombineObjectWith(GameObject obj)
        {
            return null;
        }

        protected bool PositionFuzzyEqual(Vector2 p)
        {
            if (Position().Y != p.Y)
            {
                return false;
            }

            if (Math.Abs(Position().X - p.X) < 10)
            {
                return true;
            }

            return false;
        }
    }
}
