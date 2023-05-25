using UnityEngine;

public class GameMode
{
    public Transform transform;
    public readonly Canvas canvas;

    public GameMode()
    {
        transform = InterfaceTool.CanvasSetup("Main",
            null, out canvas).transform;
    }
}