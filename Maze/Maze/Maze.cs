using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    public class Maze
    {
        public int xDim;
        public int yDim;
        public int size;
        public Cell start;
        public Cell end;
        private List<Cell> frontier;
        private List<List<Cell>> maze;
        public Maze(int xDim, int yDim) {
            this.xDim = xDim;
            this.yDim = yDim;
            this.size = xDim * yDim;
            this.frontier= new List<Cell>();
            this.maze= new List<List<Cell>>();
            // Load maze data structure
            for (int i = 0; i < xDim; i++) 
            {
                List<Cell> row = new List<Cell>();
                for (int j = 0; j < yDim; j++)
                {
                    row.Add(new Cell(i, j));
                }
                maze.Add(row);
            }
            // Load initial values into maze and frontier
            maze[0][0].inMaze= true;
            frontier.Add(maze[0][1]);
            maze[0][1].inFrontier= true;
            frontier.Add(maze[1][0]);
            maze[1][0].inFrontier = true;

            string[] temp = { "above", "below", "left", "right" };
            while (frontier.Count> 0)
            {
                // Pick random frontier cell and add to maze
                Random rnd = new Random();
                int cellIndex = rnd.Next(0, frontier.Count);
                Cell curCell = frontier[cellIndex];
                
                curCell.inMaze = true;

                // Pick random wall adjacent to maze and remove it
                bool wallChosen = false;
                List<string> neighbors = new List<string>(temp);
                while (!wallChosen)
                {
                    int neighborIndex = rnd.Next(0, neighbors.Count);
                    string neighbor = neighbors[neighborIndex];
                    switch (neighbor)
                    {
                        case "above":
                            Cell aboveCell = getAbove(curCell);
                            if (aboveCell != null)
                            {
                                if (aboveCell.inMaze)
                                {
                                    curCell.above = aboveCell;
                                    aboveCell.below = curCell;
                                    wallChosen = true;
                                }
                            }
                            break;
                        case "below":
                            Cell belowCell = getBelow(curCell);
                            if (belowCell != null)
                            {
                                if (belowCell.inMaze)
                                {
                                    curCell.below = belowCell;
                                    belowCell.above = curCell;
                                    wallChosen = true;
                                }
                            }
                            break;
                        case "left":
                            Cell leftCell = getLeft(curCell);
                            if (leftCell != null)
                            {
                                if (leftCell.inMaze)
                                {
                                    curCell.left = leftCell;
                                    leftCell.right = curCell;
                                    wallChosen = true;
                                }
                            }
                            break;
                        case "right":
                            Cell rightCell = getRight(curCell);
                            if (rightCell != null)
                            {
                                if (rightCell.inMaze)
                                {
                                    curCell.right = rightCell;
                                    rightCell.left = curCell;
                                    wallChosen = true;
                                }
                            }
                            break;
                    }
                    if (!wallChosen) neighbors.Remove(neighbor);
                }



                // Add neighbors not in the maze or frontier to the frontier
                Cell neighborCell = getAbove(curCell);
                if (neighborCell != null && !neighborCell.inMaze && !neighborCell.inFrontier)
                {
                    neighborCell.inFrontier= true;
                    frontier.Add(neighborCell);
                }
                neighborCell = getBelow(curCell);
                if (neighborCell != null && !neighborCell.inMaze && !neighborCell.inFrontier)
                {
                    neighborCell.inFrontier = true;
                    frontier.Add(neighborCell);
                }
                neighborCell = getLeft(curCell);
                if (neighborCell != null && !neighborCell.inMaze && !neighborCell.inFrontier)
                {
                    neighborCell.inFrontier = true;
                    frontier.Add(neighborCell);
                }
                neighborCell = getRight(curCell);
                if (neighborCell != null && !neighborCell.inMaze && !neighborCell.inFrontier)
                { 
                    neighborCell.inFrontier = true;
                    frontier.Add(neighborCell);
                }

                frontier.Remove(curCell);
            }

            this.setStart(0, 0);
            this.setEnd(xDim - 1, yDim - 1);

        }

        private Cell getLeft(Cell cell)
        {
            if (cell.isLeftBound()) return null;
            else return maze[cell.x - 1][cell.y];
        }
        private Cell getAbove(Cell cell)
        {
            if (cell.isTopBound()) return null;
            else return maze[cell.x][cell.y - 1];
        }
        private Cell getRight(Cell cell)
        {
            if (cell.isRightBound(xDim)) return null;
            else return maze[cell.x + 1][cell.y];
        }
        private Cell getBelow(Cell cell)
        {
            if (cell.isBottomBound(yDim)) return null;
            else return maze[cell.x][cell.y + 1];
        }

        public Cell getCell(int x, int y)
        {
            if (x < 0 || y < 0 || x >= xDim || y >= yDim) return null;
            return maze[x][y];
        }

        public Stack<Cell> shortestPath(Cell start, Cell end)
        {
            Stack<Cell> path = new Stack<Cell>();
            Queue<Cell> bfsQueue = new Queue<Cell>();
            HashSet<Cell> visited = new HashSet<Cell>();
            bfsQueue.Enqueue(start);
            visited.Add(start);
            while (bfsQueue.Count > 0)
            {
                Cell cell = bfsQueue.Dequeue();
                // Visit neighbors of the cell
                Cell[] neighbors = { cell.above, cell.below, cell.left, cell.right };
                foreach (Cell neighbor in neighbors)
                {
                    // If the path exists and the cell has not been visited already
                    if (neighbor != null && !visited.Contains(neighbor))
                    {
                        neighbor.predicessor = cell;
                        bfsQueue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }
            // Construct shortest path from predecessor field
            Cell pathCell = end;
            while (pathCell != start)
            {
                path.Push(pathCell);
                pathCell = pathCell.predicessor;
            }
            return path;
        }

        public void setStart(int x, int y)
        {
            Cell cell = getCell(x, y);
            this.start = cell;
        }

        public void setEnd(int x, int y)
        {
            Cell cell = getCell(x, y);
            this.end = cell;
        }

        public bool neighborVisited(Cell cell)
        {
            // If top left
            if (cell.x == 0 && cell.y == 0)
            {
                // Check right neighbor, bottom neighbor, bottom right
                if (getRight(cell).visitedByPlayer || getBelow(cell).visitedByPlayer || getBelow(getRight(cell)).visitedByPlayer) return true;
            }
            // If top right
            else if (cell.x == xDim - 1 && cell.y == 0)
            {
                if (getLeft(cell).visitedByPlayer || getBelow(cell).visitedByPlayer || getBelow(getLeft(cell)).visitedByPlayer) return true;
            }
            // If bottom left
            else if (cell.x == 0 && cell.y == yDim - 1)
            {
                if (getRight(cell).visitedByPlayer || getAbove(cell).visitedByPlayer || getAbove(getRight(cell)).visitedByPlayer) return true;
            }
            // If bottom right
            else if (cell.x == xDim - 1 && cell.y == yDim - 1)
            {
                // Check right neighbor, bottom neighbor, bottom right
                if (getLeft(cell).visitedByPlayer || getAbove(cell).visitedByPlayer || getAbove(getLeft(cell)).visitedByPlayer) return true;
            }
            // If top
            else if (cell.y == 0)
            {
                // Check all bottom neighbors, left and right
                if (getLeft(cell).visitedByPlayer || getRight(cell).visitedByPlayer || getBelow(cell).visitedByPlayer || getBelow(getRight(cell)).visitedByPlayer || getBelow(getLeft(cell)).visitedByPlayer) return true;
            }
            // If bottom
            else if (cell.y == yDim - 1)
            {
                // Check all above neighbors, left and right
                if (getLeft(cell).visitedByPlayer || getRight(cell).visitedByPlayer || getAbove(cell).visitedByPlayer || getAbove(getRight(cell)).visitedByPlayer || getAbove(getLeft(cell)).visitedByPlayer) return true;
            }
            // If left
            else if (cell.x == 0)
            {
                // Check all right neighbors, above and below
                if (getAbove(cell).visitedByPlayer || getBelow(cell).visitedByPlayer || getRight(cell).visitedByPlayer || getAbove(getRight(cell)).visitedByPlayer || getBelow(getRight(cell)).visitedByPlayer) return true;
            }
            // If right
            else if (cell.x == xDim - 1)
            {
                // Check all left neighbors, above and below
                if (getAbove(cell).visitedByPlayer || getBelow(cell).visitedByPlayer || getLeft(cell).visitedByPlayer || getAbove(getLeft(cell)).visitedByPlayer || getBelow(getLeft(cell)).visitedByPlayer) return true;
            }
            // Else, check all
            else if (getAbove(cell).visitedByPlayer || getBelow(cell).visitedByPlayer || getLeft(cell).visitedByPlayer || getAbove(getLeft(cell)).visitedByPlayer || getBelow(getLeft(cell)).visitedByPlayer || getRight(cell).visitedByPlayer || getAbove(getRight(cell)).visitedByPlayer || getBelow(getRight(cell)).visitedByPlayer) return true;

            return false;
        }
    }

    public class Cell 
    {
        public int x;
        public int y;
        public Cell above;
        public Cell below;
        public Cell right;
        public Cell left;

        public bool inMaze;
        public bool inFrontier;

        public Cell predicessor;
        public bool visitedByPlayer;
        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.above = null;
            this.below = null;
            this.right = null;
            this.left = null;
            this.inMaze= false;
            this.inFrontier= false;
            this.predicessor = null;
            this.visitedByPlayer= false;
        }

        public bool isLeftBound()
        {
            return this.x == 0;
        }
        public bool isRightBound(int width)
        {
            return this.x == width - 1;
        }
        public bool isTopBound()
        {
            return this.y == 0;
        }
        public bool isBottomBound(int height) 
        {
            return this.y == height - 1;
        }

    }

    public class Player
    {
        public Cell cell;
        public Cell lastCell;
        private Score score;
        public bool atEnd;
        public bool hasMoved;
        public Player(Maze maze)
        {
            this.cell = maze.start;
            this.lastCell = maze.start;
            this.score = new Score(0, maze.xDim, maze.yDim);
            this.atEnd = false;
            this.hasMoved = false;
        }

        public void changeScore(int value)
        {
            score.value += value;
        }

        public void addTime(double time)
        {
            score.time += time;
        }

        public Score getScore()
        {
            return score;
        }

    }

    public class Score : IEquatable<Score>, IComparable<Score>
    {
        public int value;
        public int mazeX;
        public int mazeY;
        public double time;
        public Score(int value, int mazeX, int mazeY)
        {
            this.value = value;
            this.mazeX = mazeX;
            this.mazeY = mazeY;
            this.time = 0;
        }

        public bool Equals(Score other)
        {
            if (other == null) return false;
            return (this.value.Equals(other.value));
        }

        public int CompareTo(Score compareScore)
        {
            // A null value means that this object is greater.
            if (compareScore == null)
                return 1;

            else
                return this.value.CompareTo(compareScore.value);
        }
    }
}
