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
        
        //private Rectangle _boundary;
        //private Vector2 _velocity;
        protected GameTexture Texture;
        protected GameTexture InactiveTexture;
        //protected bool _isStatic;

        // properties
        public string Id { get; set; }
        public Colors ViewableColors { get; set; }
        //public Rectangle Boundary { get { return _boundary; } set { _boundary = value; } }
        public string ImageName { get; set; }
        public bool AffectedByGravity { get; set; }
        //public Vector2 Velocity { get { return _velocity; } set { _velocity = value; } }
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
        /*
            get { 
                return _isStatic; 
            } 
            set { 
                _isStatic = value; 
                if (IsStatic) { 
                    Mass = float.MaxValue; 
                } 
            } 
        }
        */

        

		/* Default Constructor */
		public GameObject() {
			ViewableColors = Colors.AllColors;
			AffectedByGravity = true;
			//Velocity = new Vector2(0,0);
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
        /*
        public void SetPosition(Vector2 p)
        {
            SetPosition(new Point((int) p.X, (int) p.Y));
        }


        public void SetPosition(int x, int y)
        {
            SetPosition(new Point(x, y));
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
        */
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

            LoadPhysicsBody(Texture.TextureSize(), IsStatic);
            //SetSize((int)Texture.TextureSize().X, (int)Texture.TextureSize().Y);
        }

        public void LoadTexture()
        {
            Texture = new GameTexture(0.0f, 1.0f, .5f);
            Texture.Load(Container.GameRef.Content, Container.GameRef.GraphicsDevice, ImageName, FrameCount, FramesPerSec);
            Texture.Pause();
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

            float xv = size.X / 2;
            float yv = size.Y / 2;

            Vertices vertices = new Vertices();
            vertices.Add(new Vector2(-xv, -yv));
            vertices.Add(new Vector2(xv, -yv));
            vertices.Add(new Vector2(xv, yv));
            vertices.Add(new Vector2(-xv, yv));
            vertices.SubDivideEdges(10);

            //geom = GeomFactory.Instance.CreateRectangleGeom(Container.Sim, body, size.X, size.Y);
            geom = new Geom(body, vertices, 0);
            Container.Sim.Add(geom);

            geom.OnCollision += OnCollision;
            geom.OnSeparation += OnSeparation;

            geom.RestitutionCoefficient = 0;

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
                Texture.DrawFrame(spriteBatch, col, body.Position /* TODO: REMOVE-- new Vector2(Boundary.Left, Boundary.Top)*/, DrawEffects());
            }
            
        }

        //Update the Sprite and change it's position based on the passed in speed, direction and elapsed time.
        public virtual void Update(GameTime theGameTime)
        {
            /* TODO: REMOVE 
             * 
            if (AffectedByGravity && currentlyVisible())
            {
                Velocity = new Vector2(Velocity.X, Velocity.Y + Container.Gravity);
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
            */
        }

        // If object collides, then the position & velocity of the object is updated
        private bool CheckCollision()
        {
            return false;
            /*Rectangle bothRect = new Rectangle((int)(Position().X + Velocity.X), (int)(Position().Y + Velocity.Y), (int)Size().X, (int)Size().Y);

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
                                //Console.WriteLine("Both: " + obj.Id);
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
                                //Console.WriteLine("Both: " + obj.Id);
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
                    //Console.WriteLine("Y: " + obj.Id);
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
                    //Console.WriteLine("X: " + obj.Id);
                }
            }

            return collidesY && collidesX;*/
        }

        private List<GameObject> CollisionWithRect(Rectangle rect)
        {
            List<GameObject> objList = new List<GameObject>();

            /*foreach (GameObject obj in Container.GameObjects)
            {
                 Check for collisions with player to an obj 
                TODO: The line below is messy 
                if (obj != this && this.currentlyVisible() && (obj.currentlyVisible() || (obj.ExistsWhenNotViewed && !(this is Player))))
                {
                    if (rect.Intersects(obj.Boundary))
                    {
                        objList.Add(obj);
                    }
                }
            }

            this.DidCollideWithObjects(objList);*/

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

        protected virtual void DidLoadPhysicsBody()
        {

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
