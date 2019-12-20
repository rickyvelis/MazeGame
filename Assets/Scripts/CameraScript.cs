using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    /// <summary>
    /// Adjusts the Size and farClipPlane properties of the Camera, so the maze will fill the screen as required.
    /// </summary>
    public void ConfigureOrthCam(int x, int y)
    {
        float mazeRatio = (float)x / y;

        // If mazeRatio is bigger than the Camera's aspect ratio, base the Camera's Orthograpic size on the width (SizeX) of the Maze.
        if (mazeRatio < Camera.main.aspect)
            Camera.main.orthographicSize = (float)y / 2;
        else
            Camera.main.orthographicSize = (float)x / 2 / Camera.main.aspect;

        //Sets Camera's Far Clipping Plane depending on the biggest Dimension of the maze/
        if (x > y)
            Camera.main.farClipPlane = (float)x * 2;
        else
            Camera.main.farClipPlane = (float)y * 2;
    }
}
