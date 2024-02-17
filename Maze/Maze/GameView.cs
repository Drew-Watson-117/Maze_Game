using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    abstract class GameView : IGameState
    {

        protected KeyboardInput keyboard;
        protected GraphicsDeviceManager _graphics;
        protected SpriteBatch _spriteBatch;
        protected ContentManager Content;
        protected SpriteFont font;
        protected State nextState;
        protected State myState;
        private string[] strings = { 
            "Controls:", 
            "F1: New 5x5 Game", 
            "F2: New 10x10 Game", 
            "F3: New 15x15 Game", 
            "F4: New 20x20 Game", 
            "F5: View High Scores", 
            "F6: Credits", 
            "F7: Explore Mode",
            "WASD/IJKL/Arrow Keys: Moving",
            "B: Toggle Breadcrumbs",
            "P: Toggle Shortest Path",
            "H: Toggle Hint",
            "F: Toggle Fog of War"
        };
        protected GameView(GraphicsDeviceManager graphics, ContentManager content, State currentState) 
        {
            myState = currentState;
            nextState = currentState;
            this._graphics = graphics;
            this.Content = content;
        }
        public abstract void Initialize();

        protected abstract void RegisterInputs();
        public abstract void LoadContent(GraphicsDevice graphicsDevice);

        public abstract void ProcessInput(GameTime gameTime);
        public abstract State Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);

        protected virtual void DrawControls(float x, float initialY, int offset = 25)
        {

            this.DrawList(strings, x, initialY, offset);

        }

        private void DrawList(string[] strings, float x, float initialY, int offset = 25)
        {
            for (int i = 0; i < strings.Length; i++)
            {
                _spriteBatch.DrawString(font, strings[i], new Vector2(x, initialY + i * offset), Color.Black);
            }
        }
    }
}
