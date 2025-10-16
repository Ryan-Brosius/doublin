/// <summary>
/// Lazy wrapper for grabbing singletons from the dependency container.
/// MonoBehaviors must already exist in the scene, regular classes are created and stored on first resolve
/// </summary>
public struct LazyDependency<T>
{
    private T _value;
    private int _version;

    public T Value
    {
        get
        {
            var container = DependencyContainer.Instance;

            if (_value == null || _version != container.VersionNumber)
            {
                _value = container.Resolve<T>();
                _version = container.VersionNumber;
            }

            return _value;
        }
    }

    public bool IsResolvable()
    {
        try
        {
            return Value != null;
        }
        catch
        {
            return false;
        }
    }
}