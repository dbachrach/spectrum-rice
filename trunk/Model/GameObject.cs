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

using Spectrum.Model.Convenience_Objects;

namespace Spectrum.Model
{
    enum Direction { Up, Down, Left, Right, None }

    class GameObject : IComparable<GameObject>
    {
        // instance variables
        protected GameTexture Texture;
        protected GameTexture InactiveTexture;

        // properties
        public string Id { get; set; }
        public Colors Visibility { get; set; } // whether object is viewable in each color
        public Colors Tangibility { get; set; } // whether physics engine will colide or not
        public Colors PlayerTangibility { get; set; } // whether physics engine will collide with a player or not
        public Colors Sensibility { get; set; } // whether events will be sent on a "would be" collision
        public Colors PlayerSensibility { get; set; } // whether events will be sent on a "would be collision with a player

        public string ImageName { get; set; }
        public List<GameObject> Parents { get; set; }
        public List<GameObject> Children { get; set; }
		public List<GameObject> CombinableWith { get; set; }
        public List<GameObject> CurrentlyCombined { get; set; }
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
        public int ZIndex { get; set; }
        public bool HasBecomeVisibleInAllColors { get; set; }
        public float Scale { get; set; }

        public Body body { get; set; }
        public Geom geom { get; set; }
        public FixedAngleJoint joint { get; set; }
        public float Mass { get; set; }
        public bool IsStatic { get; set; }
        public Vector2 Size { get; set; }
        public float InitialBounciness { get; set; }
        public float InitialLinearDrag { get; set; }
        public float InitialFriction { get; set; }

        // constants to figure out if objects are "close enough"
        private static int FUZZY_DX_TOLERANCE = 10; /* TODO: Readjust both parent blocks when we create a combine block */
        private static int FUZZY_DY_TOLERANCE = 2;

        private bool BBB = true;

		/* Default Constructor */
        public GameObject()
        {
            Visibility = Colors.AllColors;
            Tangibility = Colors.AllColors;
            PlayerTangibility = Colors.AllColors;
            Sensibility = Colors.AllColors;
            PlayerSensibility = Colors.AllColors;

            CombinableWith = new List<GameObject>();
            CurrentlyCombined = new List<GameObject>();

            Children = new List<GameObject>();
            Parents = new List<GameObject>();
            Pickupable = false;
            Inactive = false;
            
            DirectionFacing = Direction.Right;

            FrameCount = 1;
            FramesPerSec = 1;

            Mass = 1;
            IsStatic = false;
            Scale = 1.0f;

            InitialBounciness = 0;
            InitialLinearDrag = .001f;
            InitialFriction = .2f;

            ZIndex = -100;

            HasBecomeVisibleInAllColors = false;
        }

        // the default property for game objects is that the player can only collide with this
        // object if it is currently visible. if you wish to have objects with different properties,
        // override this method or set the tangibility explicitly.
        public virtual void setVisibility(Colors vis)
        {
            Visibility = vis;
            PlayerTangibility = vis;
            PlayerSensibility = vis;
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

        public bool currentlySensible()
        {
            return Container.CurrentColor.Contains(this.Sensibility);
        }

        public bool currentlyPlayerSensible()
        {
            return Container.CurrentColor.Contains(this.PlayerSensibility);
        }
       

        public bool hasChildren()
        {
            return ((Children != null) && (Children.Count > 0));
        }

        public bool hasParents()
        {
            return ((Parents != null) && (Parents.Count > 0));
        }

        public int CompareTo(GameObject obj)
        {
            return obj.ZIndex.CompareTo(ZIndex);
        }

        //Load the texture for the sprite using the Content Pipeline
        public virtual void LoadContent(ContentManager theContentManager, GraphicsDevice graphicsDevice)
        {
            LoadTexture();
            LoadInactiveTexture();
            LoadPhysicsBody(Size, IsStatic);
        }

        public void LoadTexture()
        {
            Texture = new GameTexture(0.0f, this.Scale, .5f);
            Texture.Load(Container.GameRef.Content, Container.GameRef.GraphicsDevice, ImageName, FrameCount, FramesPerSec);
            Texture.Pause();

            Size = Texture.TextureSize() * this.Scale;
        }
        public void LoadInactiveTexture()
        {
            if (InactiveImageName != null && !InactiveImageName.Equals(""))
            {
                InactiveTexture = new GameTexture(0.0f, this.Scale, .5f);
                InactiveTexture.Load(Container.GameRef.Content, Container.GameRef.GraphicsDevice, InactiveImageName, FrameCount, FramesPerSec);
                InactiveTexture.Pause();
            }
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
            body.LinearDragCoefficient = InitialLinearDrag;
            
            joint = JointFactory.Instance.CreateFixedAngleJoint(Container.Sim, body);

             //TODO: This is subdivide code. We might need it later
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

            geom.RestitutionCoefficient = InitialBounciness; // bounciness
            geom.FrictionCoefficient = InitialFriction;

            geom.Tag = this;
        }

        //Draw the sprite to the screen
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(currentlyVisible() || Container.allColorsMode()) {
                Colors col = Container.CurrentColor;
                if (Container.allColorsMode())
                {
                    col = this.Visibility;
                }
                Texture.Rotation = body.Rotation;

                /* TODO: Don't use 1280 & 720 hardcoded */
                if (InViewport())
                {
                        Texture.DrawFrame(spriteBatch, col, body.Position - Container.CameraPosition, DrawEffects(), HasBecomeVisibleInAllColors);
                }
                
            }
        }

        // removes the object and all of its children and cleans up references in its parents
        // devestation: true to remove the object from the level and the physics engine
        //              false to just remove the object from the level but not the engine
        public void Reap(bool devestation)
        {
            ReapChildren();

            // remove this object from its parents' list of children
            if (hasParents())
            {
                foreach (GameObject obj in Parents)
                {
                    obj.Children.Remove(this);
                    foreach(GameObject parent in Parents)
                    {
                        obj.CurrentlyCombined.Remove(parent);
                    }
                }
            }
            if (devestation)
            {
                Container.DeferObliterate(this);
            }
            else 
            {
                Container.DeferRemoveObjectFromLevel(this);
            }
        }

        // Calls reap on all of the object's children
        public void ReapChildren()
        {
            if (hasChildren())
            {
                foreach (GameObject obj in Children)
                {
                    obj.Parents.Remove(this);
                    obj.Reap(true);
                }
                Children.RemoveAll(x => true);
            }
        }

        //Update the Sprite and change it's position based on the passed in speed, direction and elapsed time.
        public virtual void Update(GameTime theGameTime)
        {
            float elapsed = (float)theGameTime.ElapsedGameTime.TotalSeconds;
            if (Texture != null)
            {
                Texture.UpdateFrame(elapsed);
            }

            // check if this object is close enough to combine with any of the possible objects
            if (CombinableWith != null && Children.Count == 0)
            {
                foreach (GameObject obj in CombinableWith)
                {
                    if (!CurrentlyCombined.Contains(obj) && this.PositionFuzzyEqual(obj))
                    {
                        GameObject g = this.CombineObjectWith(obj);
                        CurrentlyCombined.Add(obj);
                        obj.CurrentlyCombined.Add(this);
                        
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

            /* TODO: Get rid of this BBB thing */
            if (BBB)
            {
                if (Events != null)
                {
                    foreach (Event e in Events)
                    {
                        if (e.Type == EventType.Behavior)
                        {
                            e.Execute(Container.DeferFuture, theGameTime.TotalRealTime.TotalMilliseconds);
                            BBB = false;
                        }
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

        /* Subclasses should override this method to act on collisions with other objects */
        protected virtual void DidCollideWithObject(GameObject obj, ref ContactList contactList, bool physicsCollision)
        {
            // a generic game object doesn't do anything special on collision
        }

        protected virtual Body WillLoadPhysicsBody()
        {
            return null;
        }

        protected virtual Geom WillLoadPhysicsGeom()
        {
            return null;
        }

        /* Subclassees should override this method to allow the object to be combined with another object */
        public virtual GameObject CombineObjectWith(GameObject obj)
        {
            // a generic game object doesn't combine with other objects
            return null;
        }

        // Gets called by the physics engine to determine if two objects should colide.
        // Returns a boolean based on whether the objects are both tangible and should colide.
        // If two objects colide, this calls the DidCollideWithObject notification on both objects.
        private bool OnCollision(Geom g1, Geom g2, ContactList contactList)
        {
            GameObject o1 = (GameObject) g1.Tag;
            GameObject o2 = (GameObject) g2.Tag;
            GameObject poss = Container.player.Possession;

            bool didHit = false;
            
            
            if (o1 is Player || o1 == poss)
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

            if (didHit == true)
            {

                /* Overriding checks on collision */
                if (o1.CombinableWith.Contains(o2) || o2.CombinableWith.Contains(o1))
                {
                    // Objects that can combine with each other do not collide
                    didHit = false;
                }
                else if (o1.hasParents() || o2.hasParents())
                {
                    foreach (GameObject a in o1.Parents)
                    {
                        if (a.CombinableWith.Contains(o2))
                        {
                            // object 2 combines with one of a's parents
                            didHit = false;
                            break;
                        }
                        else
                        {
                            foreach (GameObject b in o2.Parents)
                            {
                                if (a.CombinableWith.Contains(b)) {
                                    didHit = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (didHit == true)
                    {
                        foreach (GameObject b in o2.Parents)
                        {
                            if (b.CombinableWith.Contains(o1))
                            {
                                // object 2 combines with one of a's parents
                                didHit = false;
                                break;
                            }
                        }
                    }
                }
            }

            /* Find out if these two object send events based on collision */
            bool sendEvents = false;
            if (o1 is Player || o1 == poss)
            {
                if (o2.currentlyPlayerSensible())
                {
                    sendEvents = true;
                }
            }
            else if (o2 is Player || o2 == poss)
            {
                if (o1.currentlyPlayerSensible())
                {
                    sendEvents = true;
                }
            }
            else if (o1.currentlySensible() && o2.currentlySensible())
            {
                sendEvents = true;
            }

            if (sendEvents)
            {
                o1.DidCollideWithObject(o2, ref contactList, didHit);
                if (!didHit)
                {
                    // TODO: I think we need this to inform o2 of the collision. Looks like phys won't call onCollision to the o
                    // other object if we return false
                    o2.DidCollideWithObject(o1, ref contactList, didHit);
                }
            }

            return didHit;
        }

        private void OnSeparation(Geom g1, Geom g2)
        {
            /*
            GameObject o1 = (GameObject)g1.Tag;
            GameObject o2 = (GameObject)g2.Tag;

            foreach (Event e in o1.Events)
            {
                if (e.Type == EventType.Separation)
                {
                    if (o2.col
                }
            }
             */
        }
        protected bool InViewport()
        {
            return (body.Position.X + (Size.X / 2) >= Container.CameraPosition.X && body.Position.X - (Size.X / 2) <= Container.CameraPosition.X + 1280 &&
                   body.Position.Y + (Size.Y / 2) >= Container.CameraPosition.Y && body.Position.Y - (Size.Y / 2) <= Container.CameraPosition.Y + 720);
        }

    }
}
