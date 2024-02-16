using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Encodings.Web;

namespace Maze
{
    public enum Screen
    {
        Menu,
        HighScores,
        Game,
        Credits
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Meta Data
        List<Score> highScores = new List<Score>();
        IGameState gameState;
        Dictionary<State, IGameState> stateDict;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            stateDict = new Dictionary<State, IGameState>()
            {
                {State.Menu, new MenuView(_graphics, Content, State.Menu) },
                {State.Credits, new CreditsView(_graphics, Content, State.Credits) },
                {State.HighScores, new HighScoreView(highScores, _graphics, Content,State.HighScores) },
                {State.Game5x5, new MazeView(5,5,State.Game5x5,highScores,_graphics, Content) },
                {State.Game10x10, new MazeView(10,10,State.Game10x10,highScores,_graphics, Content) },
                {State.Game15x15, new MazeView(15,15,State.Game15x15,highScores,_graphics, Content) },
                {State.Game20x20, new MazeView(20,20,State.Game20x20,highScores,_graphics, Content) },
            };

            gameState = stateDict[State.Menu];

        }

        protected override void Initialize()
        {
            foreach (var state in stateDict.Values)
            {
                state.Initialize();
            }

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            foreach (var state in stateDict.Values)
            {
                state.LoadContent(GraphicsDevice);
            }

        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Note to Grader: Each game state has a ProcessInput function which is called within its Update function. 
            State nextState = gameState.Update(gameTime);
            gameState = stateDict[nextState];

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            gameState.Draw(gameTime);
            
            base.Draw(gameTime);
        }

    }
}