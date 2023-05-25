public class CLMode : GameMode
{
    CLSCSystem system;
    
    public CLMode()
    {
        system = transform.gameObject.AddComponent<CLSCSystem>();
    }
}