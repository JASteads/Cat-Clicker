public class CLMode : GameMode
{
    CLSystem sys;


    public CLMode()
    {
        sys = transform.gameObject.AddComponent<CLSystem>();

        sys.Init(transform);
    }
}