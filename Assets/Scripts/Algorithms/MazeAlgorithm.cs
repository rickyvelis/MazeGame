using System.Collections;
using UnityEngine;

public abstract class MazeAlgorithm
{
    protected MazeCell[,] _cells;       // Every Cell.
    protected int _mazeRows, _mazeColumns; // Total rows and columns.
    public WaitForSeconds StepDelay;    // Amount of seconds the coroutine should be delayed for.
    public bool CourseComplete;         // Indicator for if the algorithm is done generating.

    protected MazeAlgorithm(MazeCell[,] mazeCells, float delay)
    {
        _cells = mazeCells;
        _mazeColumns = mazeCells.GetLength(0);
        _mazeRows = mazeCells.GetLength(1);
        StepDelay = new WaitForSeconds(delay);
    }

    public abstract IEnumerator Generate();
}
