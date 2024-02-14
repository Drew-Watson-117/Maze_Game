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
        Screen screen = Screen.Menu;
        int xDim;
        int yDim;
        SpriteFont font;

        // In-Game Data
        Maze maze;
        Player player;
        Stack<Cell> shortestPath;
        Cell hint;
        bool showShortest = false;
        bool showBreadcrumbs = false;
        bool showHint = false;
       
        

        KeyboardInput inputKeyboard;

        Texture2D backgroundTile;
        Texture2D leftWallTexture;
        Texture2D rightWallTexture;
        Texture2D topWallTexture;
        Texture2D bottomWallTexture;
        Texture2D playerTexture;
        Texture2D breadcrumbTexture;
        Texture2D shortestTexture;
        Texture2D endTexture;
        

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        private void InitializeGame(int x, int y)
        {
            screen = Screen.Game;
            xDim = x;
            yDim = y;
            maze = new Maze(xDim, yDim);
            shortestPath = maze.shortestPath(maze.getCell(0, 0), maze.getCell(xDim - 1, yDim - 1));
            hint = shortestPath.Peek();
            showShortest = false;
            showBreadcrumbs = false;
            showHint = false;
            // Set start and end of maze
            maze.setStart(0, 0);
            maze.setEnd(xDim - 1, yDim - 1);
            maze.start.visitedByPlayer = true;

            player = new Player(maze);
            
        }

        protected override void Initialize()
        { 
            inputKeyboard = new KeyboardInput();

            registerInputs(inputKeyboard);

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            backgroundTile = Content.Load<Texture2D>("background");
            leftWallTexture = Content.Load<Texture2D>("wallLeft");
            rightWallTexture = Content.Load<Texture2D>("wallRight");
            topWallTexture = Content.Load<Texture2D>("wallTop");
            bottomWallTexture = Content.Load<Texture2D>("wallBottom");
            playerTexture = Content.Load<Texture2D>("player");
            breadcrumbTexture = Content.Load<Texture2D>("breadcrumb");
            shortestTexture = Content.Load<Texture2D>("shortest");
            endTexture = Content.Load<Texture2D>("end");
            font = Content.Load<SpriteFont>("robotoFont");
        }

        private void processInput(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            inputKeyboard.Update(gameTime);

        }
        protected override void Update(GameTime gameTime)
        {

            processInput(gameTime);

            // Game Update Logic
            if (screen == Screen.Game)
            {
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

                    //Update hint cell -- should be second element
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
                    updateHighScores();
                    screen = Screen.Menu;
                }
                
                
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin();
            // Menu Drawing
            if (screen == Screen.Menu)
            {
                _spriteBatch.DrawString(
                        font,
                        "Hungry Minotaur",
                        new Vector2(
                             _graphics.PreferredBackBufferWidth / 2,
                             _graphics.PreferredBackBufferHeight / 4
                             ),
                        Color.Black
                        );
                DrawControls(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 4);
            }
            // High Score Drawing
            if (screen == Screen.HighScores) 
            {
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
            }
            // Credits Drawing
            if (screen == Screen.Credits)
            {
                int offset = 50;
                _spriteBatch.DrawString(
                        font,
                        "Producer: Drew Watson",
                        new Vector2(
                             _graphics.PreferredBackBufferWidth / 2,
                             _graphics.PreferredBackBufferHeight / 4
                             ),
                        Color.Black
                        );
                _spriteBatch.DrawString(
                        font,
                        "Programmer: Drew Watson",
                        new Vector2(
                             _graphics.PreferredBackBufferWidth / 2,
                             _graphics.PreferredBackBufferHeight / 4 + offset
                             ),
                        Color.Black
                        );
                _spriteBatch.DrawString(
                        font,
                        "Creative Director: Drew Watson",
                        new Vector2(
                             _graphics.PreferredBackBufferWidth / 2,
                             _graphics.PreferredBackBufferHeight / 4 + 2*offset
                             ),
                        Color.Black
                        );
                _spriteBatch.DrawString(
                        font,
                        "Assets: Google Images",
                        new Vector2(
                             _graphics.PreferredBackBufferWidth / 2,
                             _graphics.PreferredBackBufferHeight / 4 + 3*offset
                             ),
                        Color.Black
                        );
            }
            // Draw Game Logic
            if (screen == Screen.Game)
            {
                Rectangle cellRect;
                Rectangle wallTileRect;

                int offset = 50;
                if (yDim >= 20 || xDim >= 20) offset = 40;
                int mazeStartX = _graphics.PreferredBackBufferWidth / 2 - offset*xDim/2;
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


                        // Draw Player
                        if (cell == player.cell)
                        {
                            _spriteBatch.Draw(playerTexture, cellRect, Color.White);
                        }
                        else if (cell == maze.end)
                        {
                            _spriteBatch.Draw(endTexture, cellRect, Color.White);
                        }
                        // Draw Breadcrumbs
                        else if (showBreadcrumbs && cell.visitedByPlayer)
                        {
                            _spriteBatch.Draw(breadcrumbTexture, cellRect, Color.White);
                        }
                        // Draw Hint
                        else if (showHint && hint == cell && cell != maze.end)
                        {
                            int rectSize = offset / 2;
                            Rectangle shortestRect = new Rectangle(mazeStartX + row * offset + rectSize / 2, mazeStartY + col * offset + rectSize / 2, rectSize, rectSize);
                            _spriteBatch.Draw(shortestTexture, shortestRect, Color.White);
                        }
                        // Draw Shortest Path
                        else if (showShortest && shortestPath.Contains(cell) && cell != maze.end)
                        {
                            int rectSize = offset / 2;
                            Rectangle shortestRect = new Rectangle(mazeStartX + row * offset + rectSize / 2, mazeStartY + col * offset + rectSize / 2, rectSize, rectSize);
                            _spriteBatch.Draw(shortestTexture, shortestRect, Color.White);
                        }


                    }
                }

                // Draw current score
                _spriteBatch.DrawString(
                        font,
                        "Score: " + player.getScore().value,
                        new Vector2(
                             mazeStartX + offset * xDim - font.MeasureString("Score: " + player.getScore().value).X,
                             mazeStartY/2
                             ),
                        Color.Black
                        );
                // Draw time
                _spriteBatch.DrawString(
                        font,
                        "Time: " + player.getScore().time.ToString("0.00"),
                        new Vector2(
                             mazeStartX,
                             mazeStartY/2
                             ),
                        Color.Black
                        );
                // Draw Controls
                DrawControls(mazeStartX + offset * xDim + 10, mazeStartY + offset * yDim / 4);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        void DrawControls(int x, int initialY, int offset = 50)
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


        }

        #region Input Commands

        void registerInputs(KeyboardInput keyboard)
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

            keyboard.registerCommand(Keys.F1, true, new IInputDevice.CommandDelegate(new5x5));
            keyboard.registerCommand(Keys.F2, true, new IInputDevice.CommandDelegate(new10x10));
            keyboard.registerCommand(Keys.F3, true, new IInputDevice.CommandDelegate(new15x15));
            keyboard.registerCommand(Keys.F4, true, new IInputDevice.CommandDelegate(new20x20));
            keyboard.registerCommand(Keys.F5, true, new IInputDevice.CommandDelegate(GotoHighScores));
            keyboard.registerCommand(Keys.F6, true, new IInputDevice.CommandDelegate(GotoCredits));

        }

        void GotoHighScores(GameTime gameTime, float value)
        {
            screen = Screen.HighScores;
        }

        void GotoCredits(GameTime gameTime, float value) 
        {
            screen = Screen.Credits;
        }

        void new5x5(GameTime gameTime, float value)
        {
            InitializeGame(5, 5);
        }

        void new10x10(GameTime gameTime, float value)
        {
            InitializeGame(10, 10);
        }

        void new15x15(GameTime gameTime, float value)
        {
            InitializeGame(15, 15);
        }

        void new20x20(GameTime gameTime, float value)
        {
            InitializeGame(20, 20);
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
            showBreadcrumbs= !showBreadcrumbs;
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

        void updateHighScores()
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