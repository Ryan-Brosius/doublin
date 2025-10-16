using System;
using System.Collections.Generic;

public class DependencyContainer
{
    private static DependencyContainer _instance;
    public static DependencyContainer Instance => _instance ??= new DependencyContainer();

    private readonly Dictionary<Type, object> _singletons = new();

    public int VersionNumber { get; private set; } = 0;

    public void Register<T>(T instance)
    {
        var type = typeof(T);

        // if _singletons[type] is null most likely was a monobehavior that was deleted during a scene change
        // though theoretically this could be a bad way to try to solve this in the future
        if (_singletons.ContainsKey(type) && _singletons[type] != null)
            throw new Exception($"{type.Name} is already registered");

        _singletons[type] = instance;
    }

    public T Resolve<T>()
    {
        var type = typeof(T);
        if (_singletons.TryGetValue(type, out var obj))
        {
            // Should be throwing error here, this shouldnt be happening and something has gone wrong (kinda)
            if (obj == null)
                throw new Exception($"{type.Name} is registered but the instance is null");

            return (T)obj;
        }

        // If it's a MonoBehaviour, we cannot auto-create
        // it must be registered manually in the scene
        if (typeof(UnityEngine.MonoBehaviour).IsAssignableFrom(type))
            throw new Exception($"{type.Name} is a MonoBehaviour and must be in the scene before resolving");

        // Otherwise, we can just create a new instance new()
        if (type.GetConstructor(Type.EmptyTypes) != null)
        {
            var instance = (T)Activator.CreateInstance(type)!;
            Register(instance);
            return instance;
        }

        // Is either not MonoBehavior or needs parameters
        // TODO: maybe fix this in the future to fill in parameters(?)
        throw new Exception($"{type.Name} does not exist in the container and cannot be auto-created (no parameterless constructor)");
    }

    public bool TryResolve<T>(out T instance)
    {
        if (_singletons.TryGetValue(typeof(T), out var obj))
        {
            if (obj == null)
            {
                instance = default;
                return false;
            }

            instance = (T)obj;
            return true;
        }

        instance = default;
        return false;
    }

    public void Clear()
    {
        _singletons.Clear();
        VersionNumber++;
    }
}
