using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    internal class HighScoreView : GameView
    {
        List<Score> highScores;
        public HighScoreView(List<Score> highScores, GraphicsDeviceManager graphics, ContentManager content, State currentState) : base(graphics, content, currentState)
        {
            this.highScores = highScores;
        }

        public override void Initialize()
        {
            keyboard = new KeyboardInput();

            this.RegisterInputs();
        }

        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);
            font = Content.Load<SpriteFont>("robotoFont");
        }
        protected override void RegisterInputs()
        {
            keyboard.registerCommand(Keys.F1, true, new IInputDevice.CommandDelegate(new5x5));
            keyboard.registerCommand(Keys.F2, true, new IInputDevice.CommandDelegate(new10x10));
            keyboard.registerCommand(Keys.F3, true, new IInputDevice.CommandDelegate(new15x15));
            keyboard.registerCommand(Keys.F4, true, new IInputDevice.CommandDelegate(new20x20));
            keyboard.registerCommand(Keys.F5, true, new IInputDevice.CommandDelegate(GotoHighScores));
            keyboard.registerCommand(Keys.F6, true, new IInputDevice.CommandDelegate(GotoCredits));
            keyboard.registerCommand(Keys.F7, true, new IInputDevice.CommandDelegate(newExplore));

        }

        #region Input Handler Functions
        void GotoHighScores(GameTime gameTime, float value)
        {
            nextState = State.HighScores;
        }

        void GotoCredits(GameTime gameTime, float value)
        {
            nextState = State.Credits;
        }

        void new5x5(GameTime gameTime, float value)
        {
            nextState = State.Game5x5;
        }

        void new10x10(GameTime gameTime, float value)
        {
            nextState = State.Game10x10;
        }

        void new15x15(GameTime gameTime, float value)
        {
            nextState = State.Game15x15;
        }

        void new20x20(GameTime gameTime, float value)
        {
            nextState = State.Game20x20;
        }

        void newExplore(GameTime gameTime, float value)
        {
            nextState = State.Explore;
        }
        #endregion

        public override void ProcessInput(GameTime gameTime)
        {
            keyboard.Update(gameTime);
        }

        public override State Update(GameTime gameTime)
        {
            nextState = myState;
            this.ProcessInput(gameTime);
            return nextState;
        }
        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            int offset = 50;
            int scoresX = 100;
            _spriteBatch.DrawString(font, "High Scores: ", new Vector2(scoresX, offset), Color.Black);
            for (int i = highScores.Count - 1; i >= 0; i--)
            {
                Score score = highScores[i];
                string text = "Score: " + score.value.ToString() + ",  Time: " + score.time.ToString("0.00") +
                    "\nMaze Size: " + score.mazeX.ToString() + "x" + score.mazeY.ToString();
                _spriteBatch.DrawString(
                    font,
                    text,
                    new Vector2(
                         scoresX,
                         offset * (highScores.Count - i + 1)
                         ),
                    Color.Black
                    );
            }
            DrawControls(scoresX + 200, 0);
            _spriteBatch.End();
        }
    }
}
