using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObservableProperty<T>
{
    [SerializeField] private T _value;
    private List<Action<T>> _subscribers = new List<Action<T>>();

    public ObservableProperty() { }

    public ObservableProperty(T initialValue)
    {
        _value = initialValue;
    }

    public T Value
    {
        get => _value;
        set
        {
            if (!Equals(_value, value))
            {
                _value = value;
                NotifySubscribers();
            }
        }
    }

    public void Subscribe(Action<T> callback, bool notifyImmediately = true)
    {
        if (!_subscribers.Contains(callback))
            _subscribers.Add(callback);

        if (notifyImmediately)
        {
            callback?.Invoke(_value);
        }
    }

    public void Unsubscribe(Action<T> callback)
    {
        _subscribers.Remove(callback);
    }

    private void NotifySubscribers()
    {
        for (int i = 0; i < _subscribers.Count; i++)
        {
            _subscribers[i]?.Invoke(_value);
        }
    }
}

[Serializable]
public class ObservableList<T>
{
    [SerializeField] private List<T> _list = new List<T>();
    private List<Action<List<T>>> _subscribers = new List<Action<List<T>>>();

    public IReadOnlyList<T> List => _list;

    public void Add(T item)
    {
        _list.Add(item);
        NotifySubscribers();
    }

    public bool Remove(T item)
    {
        bool removed = _list.Remove(item);
        if (removed)
            NotifySubscribers();
        return removed;
    }

    public void Clear()
    {
        _list.Clear();
        NotifySubscribers();
    }

    /// <summary>
    /// Clears the list and copies the elements from another collection onto this list
    /// </summary>
    public void ClearAndCopyFrom(IEnumerable<T> collection)
    {
        _list.Clear();
        if (collection != null)
            _list.AddRange(collection);
        NotifySubscribers();
    }

    /// <summary>
    /// Returns a new List<T> that is a copy of the internal list.
    /// Modifying this list does not affect the ObservableList.
    /// </summary>
    public List<T> GetCopy()
    {
        return new List<T>(_list);
    }

    public void Subscribe(Action<List<T>> callback, bool notifyImmediately = true)
    {
        if (!_subscribers.Contains(callback))
            _subscribers.Add(callback);

        if (notifyImmediately)
            callback?.Invoke(_list);
    }

    public void Unsubscribe(Action<List<T>> callback)
    {
        _subscribers.Remove(callback);
    }

    private void NotifySubscribers()
    {
        for (int i = 0; i < _subscribers.Count; i++)
        {
            _subscribers[i]?.Invoke(_list);
        }
    }
}