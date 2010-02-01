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

using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Factories;

namespace Spectrum.Model
{
    enum Direction { Up, Down, Left, Right, None }

    class GameObject
    {
        // instance variables
        protected GameTexture Texture;
        protected GameTexture InactiveTexture;

        // properties
        public string Id { get; set; }
        public Colors Visibility { get; set; }
        public Colors Tangibility { get; set; }
        public Colors PlayerTangibility { get; set; }

        public string ImageName { get; set; }
        public List<GameObject> Parents { get; set; }
        public List<GameObject> Children { get; set; }
		public List<GameObject> CombinableWith { get; set; }
        public bool Pickupable { get; set; }
        public bool Inactive { get; set; }
        public string InactiveImageName { get; set; }
        public List<Event> Events { get; set; }
        public Level Container { get; set; }
        public int FrameCount { get; set; }
        public int FramesPerSec { get; set; }
        public Direction DirectionFacing { get; set; }
        public Vector2 OriginalPosition { get; set; }
        public Vector2 OriginalVelocity { get; set; }

        public Body body { get; set; }
        public Geom geom { get; set; }
        public FixedAngleJoint joint { get; set; }
        public float Mass { get; set; }
        public bool IsStatic { get; set; }
        public Vector2 Size { get; set; }

        // constants to figure out if objects are "close enough"
        private static int FUZZY_DX_TOLERANCE = 5;
        private static int FUZZY_DY_TOLERANCE = 0;

		/* Default Constructor */
        public GameObject()
        {
            Visibility = Colors.AllColors;
            Tangibility = Colors.AllColors;
            PlayerTangibility = Colors.AllColors;

            CombinableWith = new List<GameObject>();

            Children = new List<GameObject>();
            Parents = new List<GameObject>();
            Pickupable = false;
            Inactive = false;
            
            DirectionFacing = Direction.Right;

            FrameCount = 1;
            FramesPerSec = 1;

            Mass = 1;
            IsStatic = false;
        }

        // the default property for game objects is that the player can only collide with this
        // object if it is currently visible. if you wish to have objects with different properties,
        // override this method or set the tangibility explicitly.
        public virtual void setVisibility(Colors vis)
        {
            Visibility = vis;
            PlayerTangibility = vis;
        }

        public bool currentlyVisible()
        {
            return Container.CurrentColor.Contains(this.Visibility);
        }

        public bool currentlyTangible()
        {
            return Container.CurrentColor.Contains(this.Tangibility);
        }

        public bool currentlyPlayerTangible()
        {
            return Container.CurrentColor.Contains(this.PlayerTangibility);
        }

        public bool hasChildren()
        {
            return ((Children != null) && (Children.Count > 0));
        }

        public bool hasParents()
        {
            return ((Parents != null) && (Parents.Count > 0));
        }

        //Load the texture for the sprite using the Content Pipeline
        public virtual void LoadContent(ContentManager theContentManager, GraphicsDevice graphicsDevice)
        {

            LoadTexture();

            if (InactiveImageName != null && !InactiveImageName.Equals(""))
            {
                InactiveTexture = new GameTexture(0.0f, 1.0f, .5f);
                InactiveTexture.Load(theContentManager, graphicsDevice, InactiveImageName, FrameCount, FramesPerSec);
                InactiveTexture.Pause();
            }

            LoadPhysicsBody(Size, IsStatic);
        }

        public void LoadTexture()
        {
            Texture = new GameTexture(0.0f, 1.0f, .5f);
            Texture.Load(Container.GameRef.Content, Container.GameRef.GraphicsDevice, ImageName, FrameCount, FramesPerSec);
            Texture.Pause();

            Size = Texture.TextureSize();
        }

        // load the Farseer body and geometry objects for this GameObject
        public virtual void LoadPhysicsBody(Vector2 size, bool isStatic)
        {
            LoadPhysicsBody(new Vector2(OriginalPosition.X + (size.X / 2), OriginalPosition.Y + (size.Y / 2)), size, isStatic);
        }

        public virtual void LoadPhysicsBody(Vector2 position, Vector2 size, bool isStatic)
        {
            if (isStatic)
            {
                Mass = float.MaxValue;
            }
            body = BodyFactory.Instance.CreateRectangleBody(Container.Sim, size.X, size.Y, Mass);
            body.Position = position;
            body.isStatic = isStatic;
            
            joint = JointFactory.Instance.CreateFixedAngleJoint(Container.Sim, body);

            // TODO: This is subdivide code. We might need it later
            //float xv = size.X / 2;
            //float yv = size.Y / 2;
            //Vertices vertices = new Vertices();
            //vertices.Add(new Vector2(-xv, -yv));
            //vertices.Add(new Vector2(xv, -yv));
            //vertices.Add(new Vector2(xv, yv));
            //vertices.Add(new Vector2(-xv, yv));
            //vertices.SubDivideEdges(10);
            //geom = new Geom(body, vertices, 0);
            //Container.Sim.Add(geom);

            geom = GeomFactory.Instance.CreateRectangleGeom(Container.Sim, body, size.X, size.Y);
            
            geom.OnCollision += OnCollision;
            geom.OnSeparation += OnSeparation;

            geom.RestitutionCoefficient = 0; // bounciness

            geom.Tag = this;

            this.DidLoadPhysicsBody();
        }

        //Draw the sprite to the screen
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(currentlyVisible()) {
                Colors col = Container.CurrentColor;
                if (Container.CurrentColor == Colors.AllColors) 
                {
                    col = this.Visibility;
                }
                Texture.Rotation = body.Rotation;
                Texture.DrawFrame(spriteBatch, col, body.Position, DrawEffects());
            }
        }

        // removes the object from the level but does not deallocate it
        public void Reap()
        {
            ReapChildren();

            // remove this object from its parents' list of children
            if (hasParents())
            {
                foreach (GameObject obj in Parents)
                {
                    obj.Children.Remove(this);
                }
            }

            Container.DeferRemoveGameObject(this);
        }

        public void ReapChildren()
        {
            if (hasChildren())
            {
                foreach (GameObject obj in Children)
                {
                    obj.Reap();
                }
            }
        }

        //Update the Sprite and change it's position based on the passed in speed, direction and elapsed time.
        public virtual void Update(GameTime theGameTime)
        {
            float elapsed = (float)theGameTime.ElapsedGameTime.TotalSeconds;
            Texture.UpdateFrame(elapsed);

            // check if this object is close enough to combine with any of the possible objects
            if (CombinableWith != null && Children.Count == 0)
            {
                foreach (GameObject obj in CombinableWith)
                {
                    if (this.PositionFuzzyEqual(obj))
                    {
                        GameObject g = this.CombineObjectWith(obj);
                        
                        this.Children.Add(g);
                        obj.Children.Add(g);

                        g.Parents.Add(this);
                        g.Parents.Add(obj);

                        g.LoadContent(Container.GameRef.Content, Container.GameRef.GraphicsDevice);
                        Console.WriteLine("Created obj");
                        Container.DeferAddGameObject(g);
                    }
                }
            }
        }

        public bool PositionFuzzyEqual(GameObject other)
        {
            double dx = Math.Abs(other.body.Position.X - this.body.Position.X);
            double dy = Math.Abs(other.body.Position.Y - this.body.Position.Y);

            return ((dy <= FUZZY_DY_TOLERANCE) && (dx <= FUZZY_DX_TOLERANCE));
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

        /*
        protected virtual void DidCollideWithObjects(List<GameObject> objs)
        {
            foreach (GameObject obj in objs)
            {
                if (obj.Events != null && obj.Events.Count > 0)
                {
                    // TODO: Do we need to go through the event list in THIS also? 
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
         */

        protected virtual void DidCollideWithObject(GameObject obj, ref ContactList contactList)
        {
            // a generic game object doesn't do anything special on collision
        }

        protected virtual void DidHitGround()
        {
            // TODO: Add code in collision notification to call this method
        }

        /* Subclasses should override this method to modify physics body object after it is created */
        protected virtual void DidLoadPhysicsBody() 
        {
            // do nothing
        }

        public virtual GameObject CombineObjectWith(GameObject obj)
        {
            return null;
        }

        private bool OnCollision(Geom g1, Geom g2, ContactList contactList)
        {
            
            GameObject o1 = (GameObject) g1.Tag;
            GameObject o2 = (GameObject) g2.Tag;
            GameObject poss = Container.player.Possession;

            bool didHit = false;
            
            if (o1.CombinableWith.Contains(o2) || o2.CombinableWith.Contains(o1)) 
            {
                // Objects that can combine with each other do not collide
                didHit = false;
            }
            else if (o1 is Player || o1 == poss)
            {
                if(o2.currentlyPlayerTangible())
                {
                    didHit = true;
                }
            }
            else if (o2 is Player || o2 == poss)
            {
                if (o1.currentlyPlayerTangible())
                {
                    didHit = true;
                }
            }
            else if(o1.currentlyTangible() && o2.currentlyTangible())
            {
                didHit = true;
            }


            if (didHit)
            {
                this.DidCollideWithObject(o2, ref contactList);
            }

            return didHit;
            /*
            if (!o1.currentlyVisible() || !o2.currentlyVisible())
            {
                return false;
            }
            //Console.WriteLine("On colision {0}", this);
            return true;
             * */
        }

        private void OnSeparation(Geom g1, Geom g2)
        {
            //Console.WriteLine("On seperation {0}", this);
        
        }


    }
}
