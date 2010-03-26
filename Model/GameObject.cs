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
        protected List<GameTexture> Textures;

        /// <summary>
        /// String reference to this object
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Colors this object is visible in
        /// </summary>
        public Colors Visibility { get; set; }

        /// <summary>
        /// Colors this object will collide in (for physics engine)
        /// </summary>
        public Colors Tangibility { get; set; }

        /// <summary>
        /// Colors this object will collide with the player in
        /// </summary>
        public Colors PlayerTangibility { get; set; }

        /// <summary>
        /// Colors in which this object will be sent a "would be" collision
        /// </summary>
        public Colors Sensibility { get; set; }

        /// <summary>
        /// Colors in which this object will be sent a "would be" collision with the player
        /// </summary>
        public Colors PlayerSensibility { get; set; }

        /// <summary>
        /// Names of the image file (without file extension) to draw for this object
        /// </summary>
        public List<string> ImageNames { get; set; }

        /// <summary>
        /// List of objects that were combined together to make this object. 
        /// Empty if this object was not made from a combination.
        /// </summary>
        public List<GameObject> Parents { get; set; }

        /// <summary>
        /// List of objects that this parent has made via combination.
        /// Empty if this object has made no objects.
        /// </summary>
        public List<GameObject> Children { get; set; }

        /// <summary>
        /// List of objects that this object may combine with
        /// </summary>
		public List<GameObject> CombinableWith { get; set; }

        /// <summary>
        /// List of objects this object is currently combined with
        /// </summary>
        public List<GameObject> CurrentlyCombined { get; set; }

        /// <summary>
        /// Whether this object may be picked up by the player
        /// </summary>
        public bool Pickupable { get; set; }

        /// <summary>
        /// List of events that this object may fire
        /// </summary>
        public List<Event> Events { get; set; }

        /// <summary>
        /// Level that this object is contained in
        /// </summary>
        public Level Container { get; set; }

        /// <summary>
        /// Number of frames in the animation to draw this object
        /// </summary>
        public int FrameCount { get; set; }

        /// <summary>
        /// Speed to render the animation at
        /// </summary>
        public int FramesPerSec { get; set; }

        /// <summary>
        /// Direction this object is currently facing
        /// </summary>
        public Direction DirectionFacing { get; set; }

        /// <summary>
        /// Original coordinates for this object when it is initially loaded
        /// </summary>
        public Vector2 OriginalPosition { get; set; }

        /// <summary>
        /// Original velocity to start the object at when it is loaded
        /// </summary>
        public Vector2 OriginalVelocity { get; set; }

        /// <summary>
        /// Index to order this object when drawn to the screen.
        /// Positive numbers draw behind the player.
        /// Negative numbers draw in front of the player.
        /// </summary>
        public int ZIndex { get; set; }

        /// <summary>
        /// Whether this object should now be displayed in White Mode
        /// </summary>
        public bool HasBecomeVisibleInAllColors { get; set; }

        /// <summary>
        /// Scale at which to render the image to the screen
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// Physics body representing this object
        /// </summary>
        public Body body { get; set; }

        /// <summary>
        /// Physics geometry representing this object
        /// </summary>
        public Geom geom { get; set; }

        /// <summary>
        /// Physics joint associated with this object
        /// </summary>
        public FixedAngleJoint joint { get; set; }

        /// <summary>
        /// Physics mass this object has
        /// </summary>
        public float Mass { get; set; }

        /// <summary>
        /// Whether this object appears in a static location on the screen
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Size of this object in screen pixels
        /// </summary>
        public Vector2 Size { get; set; }

        /// <summary>
        /// Initial physical bounciness
        /// </summary>
        public float InitialBounciness { get; set; }

        /// <summary>
        /// Initial physical linear drag
        /// </summary>
        public float InitialLinearDrag { get; set; }

        /// <summary>
        /// Initial physical friction
        /// </summary>
        public float InitialFriction { get; set; }

        /// <summary>
        /// Object that this object will follow
        /// </summary>
        public GameObject Leader { get; set; }
        public Vector2 PathVector { get; set; }

        // constants to figure out if objects are "close enough"
        private static int FUZZY_DX_TOLERANCE = 10; /* TODO: Readjust both parent blocks when we create a combine block */
        private static int FUZZY_DY_TOLERANCE = 2;

        /// <summary>
        /// If the player stands on this object, he jumps higher.
        /// False by default.
        /// </summary>
        public bool SuperJumpable { get; set; }


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

            DirectionFacing = Direction.Right;
            Scale = 1.0f;
            ZIndex = -100;

            FrameCount = 1;
            FramesPerSec = 1;

            Mass = 1;
            IsStatic = false;
            InitialBounciness = 0;
            InitialLinearDrag = .001f;
            InitialFriction = .2f;

            HasBecomeVisibleInAllColors = false;

            Leader = null;

            SuperJumpable = false;

            Textures = new List<GameTexture>();
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
            //return Container.CurrentColor.Contains(this.Visibility);
            return this.Visibility.Contains(Container.CurrentColor);
        }

        public bool currentlyTangible()
        {
            //return Container.CurrentColor.Contains(this.Tangibility);
            return this.Tangibility.Contains(Container.CurrentColor);
        }

        public bool currentlyPlayerTangible()
        {
            //return Container.CurrentColor.Contains(this.PlayerTangibility);
            return this.PlayerTangibility.Contains(Container.CurrentColor);
        }

        public bool currentlySensible()
        {
            //return Container.CurrentColor.Contains(this.Sensibility);
            return this.Sensibility.Contains(Container.CurrentColor);
        }

        public bool currentlyPlayerSensible()
        {
            //return Container.CurrentColor.Contains(this.PlayerSensibility);
            return this.PlayerSensibility.Contains(Container.CurrentColor);
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

        /// <summary>
        /// Load the texture for the sprite using the Content Pipeline
        /// </summary>
        public virtual void LoadContent(ContentManager theContentManager, GraphicsDevice graphicsDevice)
        {
            LoadTextures();
            LoadPhysicsBody(Size, IsStatic);

            if (Events != null)
            {
                foreach (Event e in Events)
                {
                    e.LoadContent(theContentManager, graphicsDevice);
                }
            }
        }

        public virtual void LoadTextures()
        {
            foreach (string imgName in ImageNames)
            {
                GameTexture t = new GameTexture(0.0f, this.Scale, .5f);
                t.Load(Container.GameRef.Content, Container.GameRef.GraphicsDevice, imgName, FrameCount, FramesPerSec);
                t.Pause();

                Size = t.TextureSize() * this.Scale;

                Textures.Add(t);
            }
        }

        /// <summary>
        /// Load the Farseer body and geometry objects for this object
        /// </summary>
        /// <param name="size">Size of the object</param>
        /// <param name="isStatic">Whether the object should be rendered at a static position</param>
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
            Draw(spriteBatch, body.Position);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Vector2 posn)
        {
            if (currentlyVisible() || Container.allColorsMode())
            {
                Colors col = Container.CurrentColor;
                if (Container.allColorsMode())
                {
                    col = this.Visibility;
                }

                foreach (GameTexture t in Textures) 
                {
                    t.Rotation = body.Rotation;
                }

                if (InViewport(posn))
                {
                    foreach (GameTexture t in Textures)
                    {
                        t.DrawFrame(spriteBatch, col, posn - Container.CameraPosition, DrawEffects(), HasBecomeVisibleInAllColors);
                    }
                }
            }
        }

 

        /// <summary>
        /// Removes the object and all of its children and cleans up references in its parents
        /// </summary>
        /// <param name="devestation">true to remove the object from the level and the physics engine
        ///                           false to just remove the object from the level but not the engine </param>
        public void Reap(bool devestation)
        {
            Console.WriteLine("Reaping " + this.Visibility.ToString());
            Console.WriteLine("hasChildren: {0}", hasChildren());
            ReapChildren();
            Console.WriteLine("2 hasChildren: {0}", hasChildren());

            // remove this object from its parents' list of children
            if (hasParents())
            {
                Console.WriteLine("Has parents: " + Parents);
                foreach (GameObject parent in Parents)
                {
                    Console.WriteLine("parent: " + parent);
                    //parent.Children.Remove(this);
                    foreach(GameObject parent2 in Parents)
                    {
                        parent.CurrentlyCombined.Remove(parent2);
                        Console.WriteLine("Remove {0} from {1}", parent2, parent);
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

        /// <summary>
        /// Calls reap on all of the object's children
        /// </summary>
        public void ReapChildren()
        {
            if (hasChildren())
            {
                foreach (GameObject child in Children)
                {
                    child.Reap(true);
                    child.Parents.Remove(this);
                }
                Children.RemoveAll(x => true);
            }
        }

        public virtual void Update(GameTime theGameTime)
        {
            float elapsed = (float)theGameTime.ElapsedGameTime.TotalSeconds;
            if (Textures != null)
            {
                foreach (GameTexture t in Textures)
                {
                    t.UpdateFrame(elapsed);
                }
            }

            // check if this object is close enough to combine with any of the possible objects
            if (CombinableWith != null && !hasChildren() /*Children.Count == 0*/)
            {
                foreach (GameObject obj in CombinableWith)
                {
                    if (!CurrentlyCombined.Contains(obj) && this.PositionFuzzyEqual(obj))
                    {
                        GameObject g = this.CombineObjectWith(obj);
                        this.CurrentlyCombined.Add(obj);
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

            if (Leader != null)
            {
                //this.body.LinearVelocity.X = Leader.body.LinearVelocity.X;
                this.body.Position = this.body.position + Leader.PathVector;
            }

            if (Events != null)
            {
                List<Event> toDelete = new List<Event>();
                foreach (Event e in Events)
                {
                    if (e.Type == EventType.Behavior)
                    {
                        Console.WriteLine("Executing behavior {0}", e);
                        e.Execute(Container.DeferFuture, theGameTime.TotalRealTime.TotalMilliseconds);
                        toDelete.Add(e);
                    }
                }

                Events.RemoveAll(item => toDelete.Contains(item));
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

        /* Subclassees should override this method to allow the object to be combined with another object */
        public virtual GameObject CombineObjectWith(GameObject obj)
        {
            // a generic game object doesn't combine with other objects
            return null;
        }

        /// <summary>
        /// Gets called by the physics engine to determine if two objects should collide.
        /// Returns a boolean based on whether the objects are both tangible and should collide.
        /// If two objects colide, this calls the DidCollideWithObject() notification on both objects.
        /// </summary>
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

            if ((o1 is Player && o2 == poss) || (o2 is Player && o1 == poss))
            {
                didHit = false;
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

        
        protected virtual void DidSeparateWithObject(GameObject other)
        {
            /* generic game objects don't do anything on separation */
        }

        /// <summary>
        /// Gets called by the physics engine when two objects separate.
        /// This calls the DidSeparateWith() notification on both objects.
        /// </summary>
        /// <returns></returns>
        protected virtual void OnSeparation(Geom g1, Geom g2)
        {
            ((GameObject)g1.Tag).DidSeparateWithObject((GameObject)g2.Tag);
            ((GameObject)g2.Tag).DidSeparateWithObject((GameObject)g1.Tag);
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

        /// <summary>
        /// Whether any part of this object is currently within the borders of the window.
        /// </summary>
        protected bool InViewport()
        {
            return InViewport(this.body.Position);
        }

        protected bool InViewport(Vector2 posn)
        {
            if (this is Ground && Id == "ground")
            {
                Console.WriteLine(posn + " " + Size + " " + Container.CameraPosition);
            }
            return (posn.X + (Size.X / 2) >= Container.CameraPosition.X && posn.X - (Size.X / 2) <= Container.CameraPosition.X + Globals.GameWidth &&
                   posn.Y + (Size.Y / 2) >= Container.CameraPosition.Y && posn.Y - (Size.Y / 2) <= Container.CameraPosition.Y + Globals.GameHeight);
        }

        /// <summary>
        /// Makes player lose if he collides with this object
        /// </summary>
        public void MakeDeadly()
        {
            if (Events == null)
            {
                Events = new List<Event>();
            }
            Event e = new Event();
            e.Type = EventType.Collision;
            e.CollisionTarget = Container.player;
            e.Actions = new List<EventAction>();
            EventAction a = new EventAction();
            a.Special = Globals.LoseSpecial;
            a.Receiver = Container.player;
            e.Actions.Add(a);
            Events.Add(e);
        }

    }
}
