using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Flat;
using Flat.Graphics;
using System.Drawing;
using Flat.input;
using System;

namespace FlatPhysicsTester
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private Sprites sprites;
        private Texture2D texture;
        private ScreenStrecher screen;

        private Shapes shapes;


        public Game1()
        {
            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.SynchronizeWithVerticalRetrace = true;
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.IsFixedTimeStep = true;
        }

        protected override void Initialize()
        {
            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 768;
            this.graphics.ApplyChanges();

            this.sprites = new Sprites(this);
            this.screen = new ScreenStrecher(this, 1280, 720);
            this.shapes = new Shapes(this);
            

            base.Initialize();

        }

        protected override void LoadContent()
        {

            this.texture = this.Content.Load<Texture2D>("Box");

        }

        protected override void Update(GameTime gameTime)
        {
            FlatKeyboard keyboard = FlatKeyboard.Instance;
            keyboard.Update();

            FlatMouse mouse = FlatMouse.Instance;
            mouse.Update();

            if(keyboard.IsKeyClicked(Keys.OemTilde))
            {
                Console.WriteLine("Mouse Window Pos" + mouse.WindowPosition);
                Console.WriteLine("Mouse Screen Pos" + mouse.GetScreenPosition(this.screen));
            }

            if (keyboard.IsKeyClicked(Keys.Escape))
            {
                this.Exit();
            }
            
            if(keyboard.IsKeyClicked(Keys.Right)) 
            {
            }

           

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.screen.Set();
            this.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            Viewport viewport = this.GraphicsDevice.Viewport;

            this.sprites.Begin(false);
            this.sprites.Draw(texture, null, new Microsoft.Xna.Framework.Rectangle(32, 32, 256, 256), Microsoft.Xna.Framework.Color.White);
            this.sprites.End();

            this.shapes.Begin();
            
            this.shapes.End();

            this.screen.Unset();
            this.screen.Present(this.sprites);
            base.Draw(gameTime);
        }
    }
}