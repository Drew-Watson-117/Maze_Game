using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{

    public enum State
    {
        Menu,
        HighScores,
        Game5x5,
        Game10x10,
        Game15x15,
        Game20x20,
        Credits
    }
    internal interface IGameState
    {
        public void Initialize();

        public void LoadContent(GraphicsDevice graphicsDevice);

        public void ProcessInput(GameTime gameTime);
        public State Update(GameTime gameTime);

        public void Draw(GameTime gameTime);
 
    }
}
