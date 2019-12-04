using System.Collections;
using UnityEngine;

public class HuntAndKillMazeAlgorithm : MazeAlgorithm
{

    private int _currRow = 0;
    private int _currColumn = 0;
    private Renderer _rend;

    public HuntAndKillMazeAlgorithm(MazeCell[,] mazeCells, float delay) : base(mazeCells, delay) { }

    public override IEnumerator Generate()
    {
        WaitForSeconds delay = new WaitForSeconds(_stepDelay);
        _cells[_currRow, _currColumn].visited = true;
        _rend = _cells[_currRow, _currColumn].GetComponent<Renderer>();
        _rend.material.color = Color.white;

        while (!CourseComplete)
        {

            //KILL: Will run until it hits a dead end.
            while (RouteStillAvailable(_currRow, _currColumn))
            {
                int direction = Random.Range(1, 5);
                //int direction = ProceduralNumberGenerator.GetNextNumber ();

                if (direction == 1 && CellIsAvailable(_currRow - 1, _currColumn))
                {
                    // North
                    DestroyWallIfItExists(_cells[_currRow - 1, _currColumn].southWall);
                    _currRow--;
                }
                else if (direction == 2 && CellIsAvailable(_currRow + 1, _currColumn))
                {
                    // South
                    DestroyWallIfItExists(_cells[_currRow, _currColumn].southWall);
                    _currRow++;
                }
                else if (direction == 3 && CellIsAvailable(_currRow, _currColumn + 1))
                {
                    // east
                    DestroyWallIfItExists(_cells[_currRow, _currColumn].eastWall);
                    _currColumn++;
                }
                else if (direction == 4 && CellIsAvailable(_currRow, _currColumn - 1))
                {
                    // west
                    DestroyWallIfItExists(_cells[_currRow, _currColumn - 1].eastWall);
                    _currColumn--;
                }

                _cells[_currRow, _currColumn].visited = true;
                _rend = _cells[_currRow, _currColumn].GetComponent<Renderer>();
                _rend.material.color = Color.white;
                yield return delay;
            }

            Hunt(); // Finds the next unvisited cell with an adjacent visited cell. If it can't find any, it sets courseComplete to true.
        }
    }

    private void Hunt()
    {
        CourseComplete = true; // Set it to this, and see if we can prove otherwise below!

        for (int r = 0; r < _mazeRows; r++)
        {
            for (int c = 0; c < _mazeColumns; c++)
            {
                if (!_cells[r, c].visited)
                {
                    CourseComplete = false; // Yep, we found something so definitely do another Kill cycle.
                    _currRow = r;
                    _currColumn = c;
                    DestroyAdjacentWall(_currRow, _currColumn);
                    _cells[_currRow, _currColumn].visited = true;
                    _rend = _cells[_currRow, _currColumn].GetComponent<Renderer>();
                    _rend.material.color = Color.white;
                    return; // Exit the function
                }
            }
        }
    }


    private bool RouteStillAvailable(int row, int column)
    {
        int availableRoutes = 0;

        if (row > 0 && !_cells[row - 1, column].visited)
        {
            availableRoutes++;
        }

        if (row < _mazeRows - 1 && !_cells[row + 1, column].visited)
        {
            availableRoutes++;
        }

        if (column > 0 && !_cells[row, column - 1].visited)
        {
            availableRoutes++;
        }

        if (column < _mazeColumns - 1 && !_cells[row, column + 1].visited)
        {
            availableRoutes++;
        }

        return availableRoutes > 0;
    }

    private bool CellIsAvailable(int row, int column)
    {
        if (row >= 0 && row < _mazeRows && column >= 0 && column < _mazeColumns && !_cells[row, column].visited)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DestroyWallIfItExists(GameObject wall)
    {
        if (wall != null)
        {
            GameObject.Destroy(wall);
        }
    }

    private void DestroyAdjacentWall(int row, int column)
    {
        bool wallDestroyed = false;

        while (!wallDestroyed)
        {
            int direction = Random.Range(1, 5);

            if (direction == 1 && row > 0 && _cells[row - 1, column].visited)
            {
                //North
                DestroyWallIfItExists(_cells[row - 1, column].southWall);
                wallDestroyed = true;
            }
            else if (direction == 2 && row < (_mazeRows - 2) && _cells[row + 1, column].visited)
            {
                //South
                DestroyWallIfItExists(_cells[row, column].southWall);
                wallDestroyed = true;
            }
            else if (direction == 3 && column > 0 && _cells[row, column - 1].visited)
            {
                //East
                DestroyWallIfItExists(_cells[row, column - 1].eastWall);
                wallDestroyed = true;
            }
            else if (direction == 4 && column < (_mazeColumns - 2) && _cells[row, column + 1].visited)
            {
                //West
                DestroyWallIfItExists(_cells[row, column].eastWall);
                wallDestroyed = true;
            }
        }
    }
}