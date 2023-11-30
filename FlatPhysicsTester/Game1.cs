using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Flat;
using Flat.Graphics;
using System.Drawing;
using Flat.Input;
using System;
using FlatPhysics;
using System.Collections.Generic;
using FlatMath = FlatPhysics.FlatMath;

namespace FlatPhysicsTester
{

    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private Sprites sprites;
        private Texture2D texture;
        private Screen screen;
        private Shapes shapes;
        private Camera camera;
        private SpriteFont fontConsolas18;

        private FlatWorld world;
        
        private List<FlatEntity> entityList; 
        private List<FlatEntity> entityRemovalList;
        
        private Stopwatch watch;

        private double totalWorldStepTime = 0;
        private int totalSampleCount = 0;
        private int totalBodyCount = 0;
        private Stopwatch sampleTimer = new Stopwatch();

        private string worldStepTimeString = string.Empty;
        private string bodyCountString = string.Empty;

        public Game1()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.SynchronizeWithVerticalRetrace = true;
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.IsFixedTimeStep = true;

            const double UpdatesPerSecond = 144d;
            this.TargetElapsedTime = TimeSpan.FromTicks((long)Math.Round((double)TimeSpan.TicksPerSecond / UpdatesPerSecond));
        }

        protected override void Initialize()
        {
            this.Window.Position = new Microsoft.Xna.Framework.Point(10, 40);

            FlatUtil.SetRelativeBackBufferSize(this.graphics, 0.85f);

            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 768;
            this.graphics.ApplyChanges();

            this.sprites = new Sprites(this);
            this.screen = new Screen(this, 1280, 720);
            this.shapes = new Shapes(this);
            this.camera = new Camera(this.screen);
            this.camera.Zoom = 20;

            this.camera.GetExtents(out float left, out float right, out float top, out float bottom);

            this.entityList = new List<FlatEntity>();
            this.entityRemovalList = new List<FlatEntity>();
            
            this.world = new FlatWorld();

            float padding = MathF.Abs(right - left) * 0.10f;
            
            if(!FlatBody.CreateBoxBody(right - left - padding * 2, 3f, 1f, true, 0.5f,
                                       out FlatBody groundBody, out string errorMessage))
            {
                throw new Exception(errorMessage);
            }

            groundBody.MoveTo(new FlatVector(0, -10));
            this.world.AddBody(groundBody);
            
            this.entityList.Add(new FlatEntity(groundBody, Color.DarkGreen));

            if (!FlatBody.CreateBoxBody(20f, 2f, 1, true, 0.5f, out FlatBody ledgeBody1, out errorMessage))
            {
                throw new Exception(errorMessage);
            }

            ledgeBody1.MoveTo(new FlatVector(-10, 0));
            ledgeBody1.Rotate(MathHelper.TwoPi / -20f);
            this.world.AddBody(ledgeBody1);
            
            this.entityList.Add(new FlatEntity(ledgeBody1, Color.DarkGray));

            if (!FlatBody.CreateBoxBody(20f, 2f, 1, true, 0.5f, out FlatBody ledgeBody2, out errorMessage))
            {
                throw new Exception(errorMessage);
            }
            ledgeBody2.MoveTo(new FlatVector(10, 7));
            ledgeBody2.Rotate(MathHelper.TwoPi / 20f);
            this.world.AddBody(ledgeBody2);
            
            this.entityList.Add(new FlatEntity(ledgeBody2, Color.Red));



            this.watch = new Stopwatch();
            this.sampleTimer.Start();

            base.Initialize();

        }

        protected override void LoadContent()
        {
            this.fontConsolas18 = this.Content.Load<SpriteFont>("Consolas18");
        }

        protected override void Update(GameTime gameTime)
        {
            FlatKeyboard keyboard = FlatKeyboard.Instance;
            keyboard.Update();

            FlatMouse mouse = FlatMouse.Instance;
            mouse.Update();

            if(mouse.IsLeftMouseButtonPressed())
            {
                float width = RandomHelper.RandomSingle(2f, 3f);
                float height = RandomHelper.RandomSingle(2f, 3f);

                FlatVector mouseWorldPosition = 
                    FlatConverter.ToFlatVector(mouse.GetMouseWorldPosition(this, this.screen, this.camera));

                this.entityList.Add(new FlatEntity(this.world, width, height, false, mouseWorldPosition));

            }

            if (mouse.IsRightMouseButtonPressed())
            {
                float radius = RandomHelper.RandomSingle(1.25f, 1.5f);

                FlatVector mouseWorldPosition =
                    FlatConverter.ToFlatVector(mouse.GetMouseWorldPosition(this, this.screen, this.camera));

                this.entityList.Add(new FlatEntity(this.world, radius, false, mouseWorldPosition));

            }
            if (keyboard.IsKeyAvailable)
            {

                if (keyboard.IsKeyClicked(Keys.OemTilde))
                {
                    Console.WriteLine($"Bodycount: {this.world.BodyCount - 1}");
                    Console.WriteLine();
                }
                if (keyboard.IsKeyClicked(Keys.F))
                {
                    FlatUtil.ToggleFullScreen(this.graphics);
                }

                if (keyboard.IsKeyClicked(Keys.Escape))
                {
                    this.Exit();
                }

                if (keyboard.IsKeyDown(Keys.A))
                {
                    this.camera.DecZoom();
                }

                if (keyboard.IsKeyDown(Keys.Z))
                {
                    this.camera.IncZoom();
                }
            }

            if(this.sampleTimer.Elapsed.TotalSeconds > 1d)
            {
                this.bodyCountString = "BodyCount: " + Math.Round(this.totalBodyCount / (double)this.totalSampleCount, 4).ToString();
                this.worldStepTimeString = "StepTime: " + Math.Round(this.totalWorldStepTime / (double)totalSampleCount, 4).ToString();
                
                this.totalBodyCount = 0;
                this.totalWorldStepTime = 0d;
                this.totalSampleCount = 0;
                this.sampleTimer.Restart();
            }

            

            this.watch.Restart();
            this.world.Step(FlatUtil.GetElapsedTimeInSeconds(gameTime), 20);
            this.watch.Stop();

            this.totalWorldStepTime += this.watch.Elapsed.TotalMilliseconds;
            this.totalBodyCount += this.world.BodyCount;
            this.totalSampleCount++;

            this.camera.GetExtents(out _, out _, out float viewBottom, out _);

            this.entityRemovalList.Clear();

            for (int i = 0; i < this.entityList.Count; i++)
            {
                FlatEntity entity = this.entityList[i];
                FlatBody body = entity.Body;

                if (body.IsStatic)
                {
                    continue;
                }

                FlatAABB box = body.GetAABB();

                if (box.Max.Y < viewBottom)
                {
                    this.entityRemovalList.Add(entity);
                    
                }
            }

            for(int i = 0;i < this.entityRemovalList.Count;i++) 
            {
                FlatEntity entity = this.entityRemovalList[i];
                this.world.RemoveBody(entity.Body);
                this.entityList.Remove(entity);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.screen.Set();
            this.GraphicsDevice.Clear(new Color(50, 60, 70));





            this.shapes.Begin(this.camera);
            for (int i = 0; i < this.entityList.Count; i++)
            {
                this.entityList[i].Draw(this.shapes);
            }




            this.shapes.End();




            Vector2 stringSize = this.fontConsolas18.MeasureString(this.bodyCountString);

            this.sprites.Begin();
            this.sprites.DrawString(this.fontConsolas18, this.bodyCountString, new Vector2(0, 0), Color.White);
            this.sprites.DrawString(this.fontConsolas18, this.worldStepTimeString, new Vector2(0, stringSize.Y), Color.White);
            this.sprites.End();

            this.screen.Unset();
            this.screen.Present(this.sprites);
            base.Draw(gameTime);
        }
        
    }
}