using Microsoft.Xna.Framework.Input;
using System;

namespace Flat.input
{
    public sealed class FlatKeyboard
    {
        private static readonly Lazy<FlatKeyboard> Lazy = new Lazy<FlatKeyboard>(() => new FlatKeyboard());

        public static FlatKeyboard Instance
        {
            get { return Lazy.Value; }
        }

        private KeyboardState prevKeyboardState;
        private KeyboardState currentKeyboardState; 

        public FlatKeyboard() 
        {
            this.prevKeyboardState = Keyboard.GetState();
            this.currentKeyboardState = prevKeyboardState;
        }

        public void Update()
        {
            this.prevKeyboardState = this.currentKeyboardState;
            this.currentKeyboardState = Keyboard.GetState();
        }

        public bool IsKeyDown(Keys key) 
        {
            return this.currentKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyClicked(Keys key) 
        {
            return this.currentKeyboardState.IsKeyDown(key) && !this.prevKeyboardState.IsKeyDown(key);
        }
    }
}
