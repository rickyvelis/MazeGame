using UnityEngine;

public class MazeCell : MonoBehaviour
{
    public bool visited = false;
    public GameObject northWall, southWall, eastWall, westWall;
    public Color startColor;
}
