# Hints from Dr. Matthias

## Maze Algorithm

1. Create Graph of cells. Assume there are walls around all the cells
	- Frontier: A cell not yet in the maze which has a neighbor in the maze
2. Pick a starting point. Add it to the maze. Look at neighbors (not diagonally). If they are not yet in the maze or frontier add them to the frontier
3. Pick a random cell in the frontier.
	- Randomly select a wall in the cell adjacent to a cell IN THE MAZE
	- Remove the selected wall
4. New cell is the cell from Step 3
5. Repeat from Step 3 until Frontier is empty

Use Hash Table to maintain Frontier
Use Hash Table to maintain Maze

Represent Cells in a way that helps with navigation and finding neighbors

## Shortest Path

- Compute shortest path once, right afteer maze execution
- Make shortest path a stack
- Bottom of the stack is the goal
- If player moves along shortest path, pop from stack
- If player moves away from shortest path, push previous position to the top of the stack


## Rendering

- Make texture for each configuration of walls (all walls, just left wall, etc) OR
- Make a horizontal wall texture and a vertical wall texture and draw them at each point

## Order of Doing Assigment

- Code Maze Generation Algorithm First (may just want to print to console)
- Work on Rendering