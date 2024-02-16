using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    internal class MazeView : GameView
    {

        Maze maze;
        Player player;
        Stack<Cell> shortestPath;
        Cell hint;
        bool showShortest = false;
        bool showBreadcrumbs = false;
        bool showHint = false;
        bool showFog = false;
        int xDim;
        int yDim;
        List<Score> highScores;

        Texture2D backgroundTile;
        Texture2D leftWallTexture;
        Texture2D rightWallTexture;
        Texture2D topWallTexture;
        Texture2D bottomWallTexture;
        Texture2D playerTexture;
        Texture2D breadcrumbTexture;
        Texture2D shortestTexture;
        Texture2D endTexture;
        Texture2D fogTexture;
        public MazeView(int x, int y, State currentState, List<Score> highScores, GraphicsDeviceManager graphics, ContentManager content) : base(graphics, content, currentState)
        {
            nextState = currentState;
            xDim = x;
            yDim = y;
            this.highScores = highScores;
        }
        public override void Initialize()
        {

            keyboard = new KeyboardInput();
            this.RegisterInputs();

            maze = new Maze(xDim, yDim);
            shortestPath = maze.shortestPath(maze.getCell(0, 0), maze.getCell(xDim - 1, yDim - 1));
            hint = shortestPath.Peek();
            showShortest = false;
            showBreadcrumbs = false;
            showHint = false;
            showFog = false;
            // Set start and end of maze
            maze.setStart(0, 0);
            maze.setEnd(xDim - 1, yDim - 1);
            maze.start.visitedByPlayer = true;

            player = new Player(maze);
        }

        protected override void RegisterInputs()
        {
            keyboard.registerCommand(Keys.W, true, new IInputDevice.CommandDelegate(moveUp));
            keyboard.registerCommand(Keys.S, true, new IInputDevice.CommandDelegate(moveDown));
            keyboard.registerCommand(Keys.A, true, new IInputDevice.CommandDelegate(moveLeft));
            keyboard.registerCommand(Keys.D, true, new IInputDevice.CommandDelegate(moveRight));

            keyboard.registerCommand(Keys.I, true, new IInputDevice.CommandDelegate(moveUp));
            keyboard.registerCommand(Keys.J, true, new IInputDevice.CommandDelegate(moveLeft));
            keyboard.registerCommand(Keys.K, true, new IInputDevice.CommandDelegate(moveDown));
            keyboard.registerCommand(Keys.L, true, new IInputDevice.CommandDelegate(moveRight));

            keyboard.registerCommand(Keys.Up, true, new IInputDevice.CommandDelegate(moveUp));
            keyboard.registerCommand(Keys.Left, true, new IInputDevice.CommandDelegate(moveLeft));
            keyboard.registerCommand(Keys.Down, true, new IInputDevice.CommandDelegate(moveDown));
            keyboard.registerCommand(Keys.Right, true, new IInputDevice.CommandDelegate(moveRight));

            keyboard.registerCommand(Keys.H, true, new IInputDevice.CommandDelegate(toggleHint));
            keyboard.registerCommand(Keys.B, true, new IInputDevice.CommandDelegate(toggleBreadcrumbs));
            keyboard.registerCommand(Keys.P, true, new IInputDevice.CommandDelegate(toggleShortestPath));
            keyboard.registerCommand(Keys.F, true, new IInputDevice.CommandDelegate(toggleFog));


            keyboard.registerCommand(Keys.F1, true, new IInputDevice.CommandDelegate(new5x5));
            keyboard.registerCommand(Keys.F2, true, new IInputDevice.CommandDelegate(new10x10));
            keyboard.registerCommand(Keys.F3, true, new IInputDevice.CommandDelegate(new15x15));
            keyboard.registerCommand(Keys.F4, true, new IInputDevice.CommandDelegate(new20x20));
            keyboard.registerCommand(Keys.F5, true, new IInputDevice.CommandDelegate(GotoHighScores));
            keyboard.registerCommand(Keys.F6, true, new IInputDevice.CommandDelegate(GotoCredits));
        }

        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
            _spriteBatch = new SpriteBatch(graphicsDevice);

            backgroundTile = Content.Load<Texture2D>("background");
            leftWallTexture = Content.Load<Texture2D>("wallLeft");
            rightWallTexture = Content.Load<Texture2D>("wallRight");
            topWallTexture = Content.Load<Texture2D>("wallTop");
            bottomWallTexture = Content.Load<Texture2D>("wallBottom");
            playerTexture = Content.Load<Texture2D>("player");
            breadcrumbTexture = Content.Load<Texture2D>("breadcrumb");
            shortestTexture = Content.Load<Texture2D>("shortest");
            endTexture = Content.Load<Texture2D>("end");
            fogTexture = Content.Load<Texture2D>("fog");
            font = Content.Load<SpriteFont>("robotoFont");
        }

        public override void ProcessInput(GameTime gameTime)
        {
            keyboard.Update(gameTime);
            player.addTime(gameTime.ElapsedGameTime.TotalSeconds);

            if (player.hasMoved && player.cell != maze.end)
            {
                // Add/Remove from shortest path and change player score
                Cell bestCell = shortestPath.Peek();
                if (player.cell == bestCell) // If part of shortest path, remove from shortest path. If first time visited, add to score

                {
                    shortestPath.Pop();
                    if (!player.cell.visitedByPlayer)
                    {
                        player.changeScore(5);
                    }
                }
                else // Else, add to shortest path and subtract score if first time visiting
                {
                    // Add to shortest path
                    shortestPath.Push(player.lastCell);
                    if (!player.cell.visitedByPlayer)
                    {
                        // Adjacent to shortest path, -1, else -2
                        if (player.cell.above == bestCell || player.cell.below == bestCell || player.cell.left == bestCell || player.cell.right == bestCell)
                        {
                            player.changeScore(-1);
                        }
                        else
                        {
                            player.changeScore(-2);
                        }
                    }
                }

                //Update hint cell
                hint = shortestPath.Peek();

                // Add cell to breadcrumbs
                if (!player.cell.visitedByPlayer)
                {
                    player.cell.visitedByPlayer = true;
                }
                player.hasMoved = false;
            }
            else if (player.cell == maze.end && player.hasMoved)
            {
                this.UpdateHighScores();
                nextState = State.Menu;
            }
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

        void toggleFog(GameTime gameTime, float value)
        {
            showFog = !showFog;
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

        void moveUp(GameTime gameTime, float value)
        {
            if (player.cell.above != null)
            {
                player.lastCell = player.cell;
                player.cell = player.cell.above;
                player.hasMoved = true;
            }
        }
        void moveDown(GameTime gameTime, float value)
        {
            if (player.cell.below != null)
            {
                player.lastCell = player.cell;
                player.cell = player.cell.below;
                player.hasMoved = true;
            }
        }
        void moveLeft(GameTime gameTime, float value)
        {
            if (player.cell.left != null)
            {
                player.lastCell = player.cell;
                player.cell = player.cell.left;
                player.hasMoved = true;
            }
        }
        void moveRight(GameTime gameTime, float value)
        {
            if (player.cell.right != null)
            {
                player.lastCell = player.cell;
                player.cell = player.cell.right;
                player.hasMoved = true;
            }
        }

        void toggleBreadcrumbs(GameTime gameTime, float value)
        {
            showBreadcrumbs = !showBreadcrumbs;
        }

        void toggleHint(GameTime gameTime, float value)
        {
            showHint = !showHint;
        }

        void toggleShortestPath(GameTime gameTime, float value)
        {
            showShortest = !showShortest;
        }

        #endregion

        public override State Update(GameTime gameTime)
        {
            nextState = myState;
            this.ProcessInput(gameTime);
            // If the next state is not the same as the current state, reinitialize the game
            if (nextState != myState)
            {
                this.Initialize();
            }
            return nextState;
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            Rectangle cellRect;
            Rectangle wallTileRect;

            int offset = 50;
            if (yDim >= 20 || xDim >= 20) offset = 40;
            int mazeStartX = _graphics.PreferredBackBufferWidth / 2 - offset * xDim / 2;
            int mazeStartY = 100;
            // Draw each cell
            for (int row = 0; row < xDim; row++)
            {
                for (int col = 0; col < yDim; col++)
                {
                    Cell cell = maze.getCell(row, col);
                    cellRect = new Rectangle(mazeStartX + row * offset, mazeStartY + col * offset, offset + 1, offset + 1);

                    _spriteBatch.Draw(backgroundTile, cellRect, Color.White);

                    // Draw Walls
                    wallTileRect = new Rectangle(mazeStartX + row * offset, mazeStartY + col * offset, offset, offset);
                    if (cell.above == null)
                    {
                        _spriteBatch.Draw(topWallTexture, wallTileRect, Color.White);
                    }
                    if (cell.left == null)
                    {
                        _spriteBatch.Draw(leftWallTexture, wallTileRect, Color.White);
                    }
                    // Fill in bottom and right boundaries
                    if (cell.below == null)
                    {
                        _spriteBatch.Draw(bottomWallTexture, wallTileRect, Color.White);
                    }
                    if (cell.right == null)
                    {
                        _spriteBatch.Draw(rightWallTexture, wallTileRect, Color.White);
                    }
                    // Draw Breadcrumbs
                    if (showBreadcrumbs && cell.visitedByPlayer && cell != player.cell)
                    {
                        _spriteBatch.Draw(breadcrumbTexture, cellRect, Color.White);
                    }
                    // Draw Shortest Path
                    if (showShortest && shortestPath.Contains(cell) && cell != maze.end)
                    {
                        int rectSize = offset / 2;
                        Rectangle shortestRect = new Rectangle(mazeStartX + row * offset + rectSize / 2, mazeStartY + col * offset + rectSize / 2, rectSize, rectSize);
                        _spriteBatch.Draw(shortestTexture, shortestRect, Color.White);
                    }
                    // Draw Hint
                    if (showHint && hint == cell && cell != maze.end)
                    {
                        int rectSize = offset / 2;
                        Rectangle shortestRect = new Rectangle(mazeStartX + row * offset + rectSize / 2, mazeStartY + col * offset + rectSize / 2, rectSize, rectSize);
                        _spriteBatch.Draw(shortestTexture, shortestRect, Color.White);
                    }
                    // Draw Player
                    if (cell == player.cell)
                    {
                        _spriteBatch.Draw(playerTexture, cellRect, Color.White);
                    }
                    // Draw End Texture
                    if (cell == maze.end)
                    {
                        _spriteBatch.Draw(endTexture, cellRect, Color.White);
                    }
                    // Draw Fog
                    if (showFog && (cell.x > player.cell.x + 1 || cell.x < player.cell.x - 1 || cell.y > player.cell.y + 1 || cell.y < player.cell.y - 1) && cell != maze.end)
                    {
                        _spriteBatch.Draw(fogTexture, cellRect, Color.White);
                    }
                }
            }

            // Draw current score
            _spriteBatch.DrawString(
                    font,
                    "Score: " + player.getScore().value,
                    new Vector2(
                         mazeStartX + offset * xDim - font.MeasureString("Score: " + player.getScore().value).X,
                         mazeStartY / 2
                         ),
                    Color.Black
                    );
            // Draw time
            _spriteBatch.DrawString(
                    font,
                    "Time: " + player.getScore().time.ToString("0.00"),
                    new Vector2(
                         mazeStartX,
                         mazeStartY / 2
                         ),
                    Color.Black
                    );
            // Draw Controls
            DrawControls(mazeStartX + offset * xDim + 10, mazeStartY);
            _spriteBatch.End();
        }

        private void UpdateHighScores()
        {
            if (highScores.Count < 5 && !highScores.Contains(player.getScore()))
            {
                highScores.Add(player.getScore());
                highScores.Sort();
            }
            else if (!highScores.Contains(player.getScore()))
            {
                for (int i = highScores.Count - 1; i >= 0; i--)
                {
                    if (player.getScore().value > highScores[i].value)
                    {
                        highScores[i] = player.getScore();
                        break;
                    }
                }
            }
        }

    }
}
