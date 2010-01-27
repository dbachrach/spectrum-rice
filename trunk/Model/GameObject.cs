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
        public Colors ViewableColors { get; set; }
        public string ImageName { get; set; }
        public bool AffectedByGravity { get; set; }
        public List<GameObject> CombineObjects { get; set; }
		public List<GameObject> CombinableWith { get; set; }
        public bool Pickupable { get; set; }
        public bool Inactive { get; set; }
        public string InactiveImageName { get; set; }
        public List<Event> Events { get; set; }
        public bool ExistsWhenNotViewed { get; set; }
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
        

        

		/* Default Constructor */
        public GameObject()
        {
            ViewableColors = Colors.AllColors;
            AffectedByGravity = true;
            CombineObjects = new List<GameObject>();
            CombinableWith = null;
            Pickupable = false;
            Inactive = false;
            ExistsWhenNotViewed = false;
            DirectionFacing = Direction.Right;

            FrameCount = 1;
            FramesPerSec = 1;

            Mass = 1;
            IsStatic = false;
        }

        public bool currentlyVisible()
        {
            return Container.CurrentColor.Contains(this.ViewableColors);
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

            this.DidLoadPhysicsBody();
        }

        //Draw the sprite to the screen
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if(currentlyVisible()) {
                Colors col = Container.CurrentColor;
                if (Container.CurrentColor == Colors.AllColors) 
                {
                    col = this.ViewableColors;
                }
                Texture.Rotation = body.Rotation;
                Texture.DrawFrame(spriteBatch, col, body.Position, DrawEffects());
            }
            
        }

        //Update the Sprite and change it's position based on the passed in speed, direction and elapsed time.
        public virtual void Update(GameTime theGameTime)
        {
            float elapsed = (float)theGameTime.ElapsedGameTime.TotalSeconds;
            Texture.UpdateFrame(elapsed);

            /* TODO: Reimplement 
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
            */
        }


        // TODO: This line of code is how we check for whether two objs collide (taking into account things like color
        //       if (obj != this && this.currentlyVisible() && (obj.currentlyVisible() || (obj.ExistsWhenNotViewed && !(this is Player))))


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

        protected bool PositionFuzzyEqual(Vector2 p)
        {
            /*if (Position().Y != p.Y)
            {
                return false;
            }

            if (Math.Abs(Position().X - p.X) < 10)
            {
                return true;
            }
            */
            return false;
        }

        private bool OnCollision(Geom g1, Geom g2, ContactList contactList)
        {
            //Console.WriteLine("On colision {0}", this);
            return true;
        }

        private void OnSeparation(Geom g1, Geom g2)
        {
            //Console.WriteLine("On seperation {0}", this);
        
        }


    }
}
