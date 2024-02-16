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

        protected virtual void DrawControls(float x, float initialY, int offset = 50)
        {
            _spriteBatch.DrawString(
                        font,
                        "Controls:",
                        new Vector2(
                             x,
                             initialY + offset
                             ),
                        Color.Black
                        );
            _spriteBatch.DrawString(
                    font,
                    "F1: New 5x5 Game",
                    new Vector2(
                         x,
                         initialY + 3 * offset / 2
                         ),
                    Color.Black
                    );
            _spriteBatch.DrawString(
                    font,
                    "F2: New 10x10 Game",
                    new Vector2(
                         x,
                         initialY + 4 * offset / 2
                         ),
                    Color.Black
                    );
            _spriteBatch.DrawString(
                    font,
                    "F3: New 15x15 Game",
                    new Vector2(
                         x,
                         initialY + 5 * offset / 2
                         ),
                    Color.Black
                    );
            _spriteBatch.DrawString(
                    font,
                    "F4: New 20x20 Game",
                    new Vector2(
                         x,
                         initialY + 6 * offset / 2
                         ),
                    Color.Black
                    );
            _spriteBatch.DrawString(
                    font,
                    "F5: View High Scores",
                    new Vector2(
                         x,
                         initialY + 7 * offset / 2
                         ),
                    Color.Black
                    );
            _spriteBatch.DrawString(
                    font,
                    "F6: View Credits",
                    new Vector2(
                         x,
                         initialY + 8 * offset / 2
                         ),
                    Color.Black
                    );
            _spriteBatch.DrawString(font, "WASD/IJKL/Arrow Keys: Moving", new Vector2(x, initialY + 9 * offset / 2), Color.Black);
            _spriteBatch.DrawString(font, "B: Breadcrumbs", new Vector2(x, initialY + 10 * offset / 2), Color.Black);
            _spriteBatch.DrawString(font, "P: Shortest Path", new Vector2(x, initialY + 11 * offset / 2), Color.Black);
            _spriteBatch.DrawString(font, "H: Hint", new Vector2(x, initialY + 12 * offset / 2), Color.Black);
            _spriteBatch.DrawString(font, "F: Fog of War", new Vector2(x, initialY + 13 * offset / 2), Color.Black);
        }
    }
}
