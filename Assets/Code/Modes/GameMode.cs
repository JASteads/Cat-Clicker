using UnityEngine;

public abstract class GameMode
{
    public Transform transform;

    public GameMode()
    {
        transform = InterfaceTool.CanvasSetup("Main", null, out _);
    }   

    public void Reset()
    { Object.Destroy(transform.gameObject); }
}