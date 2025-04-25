using System;
using UnityEngine;
using System.Collections.Generic;

namespace Anvil.Common
{
    [Serializable]
    public abstract class ObservableBase
    {
        public abstract void ForceNotify();
        public abstract void UnBind();
    }

    [Serializable]
    public class Observable<T> : ObservableBase
    {
        [SerializeField]
        private T _value;

        // Event triggered when the value changes
        public event Action<T> OnValueChanged;

        // Property to get/set the value
        public T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(value, _value)) return;
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }

        // Constructors
        public Observable() { }

        public Observable(T initialValue)
        {
            _value = initialValue;
        }

        // Force notify with a new value
        public void ForceNotify(T newValue)
        {
            _value = newValue;
            OnValueChanged?.Invoke(_value);
        }

        // Force notify without changing the value
        public override void ForceNotify()
        {
            OnValueChanged?.Invoke(_value);
        }

        // Unbind all subscribers
        public override void UnBind()
        {
            OnValueChanged = null;
        }

        // Implicit conversion from Observable<T> to T
        public static implicit operator T(Observable<T> observable)
        {
            return observable.Value;
        }
    }

    [Serializable]
    internal class ObservableBool : Observable<bool>
    {
    }


    [Serializable]
    internal sealed class ObservableString : Observable<string>
    {
    }

    [Serializable]
    internal sealed class ObservableInt : Observable<int>
    {
    }

    [Serializable]
    internal sealed class ObservableFloat : Observable<float>
    {
    }

    /// <summary>
    /// The original list never changes to ensure our references to the list are not changed.
    /// </summary>
    [Serializable]
    internal class ObservableList<T> : ObservableBase
    {
        private readonly List<T> _value = new();

        // Events for specific list changes
        public event Action<T> OnItemAdded;
        public event Action<T> OnItemRemoved;
        public event Action OnListCleared;
        public event Action<int, T> OnItemInserted;
        public event Action<int, T> OnItemRemovedAt;

        public List<T> Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<List<T>>.Default.Equals(value, _value)) return;
                // clear the list and add the new items to it
                _value.Clear();
                _value.AddRange(value);
                OnValueChanged?.Invoke(_value);
            }
        }

        public ObservableList() { }

        public ObservableList(List<T> initialList)
        {
            _value = initialList;
        }

        public void AddRange(IEnumerable<T> items)
        {
            _value.AddRange(items);
            OnValueChanged?.Invoke(_value);
        }

        public void Add(T item)
        {
            _value.Add(item);
            OnItemAdded?.Invoke(item);
            OnValueChanged?.Invoke(_value);
        }

        public bool Remove(T item)
        {
            bool removed = _value.Remove(item);
            if (removed)
            {
                OnItemRemoved?.Invoke(item);
                OnValueChanged?.Invoke(_value);
            }
            return removed;
        }

        public void Clear()
        {
            _value.Clear();
            OnListCleared?.Invoke();
            OnValueChanged?.Invoke(_value);
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > _value.Count) throw new ArgumentOutOfRangeException(nameof(index));
            _value.Insert(index, item);
            OnItemInserted?.Invoke(index, item);
            OnValueChanged?.Invoke(_value);
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _value.Count) throw new ArgumentOutOfRangeException(nameof(index));
            T removedItem = _value[index];
            _value.RemoveAt(index);
            OnItemRemovedAt?.Invoke(index, removedItem);
            OnValueChanged?.Invoke(_value);
        }

        public T this[int index]
        {
            get => _value[index];
            set
            {
                if (EqualityComparer<T>.Default.Equals(value, _value[index])) return;
                _value[index] = value;
                // Optionally, trigger an event for item replacement
                OnValueChanged?.Invoke(_value);
            }
        }

        // Optionally, implement other list methods as needed

        public override void ForceNotify()
        {
            OnValueChanged?.Invoke(_value);
        }

        // Implicit conversion from ObservableList<T> to List<T>
        public static implicit operator List<T>(ObservableList<T> observableList)
        {
            return observableList._value;
        }

        public override void UnBind()
        {
            OnItemAdded = null;
            OnItemRemoved = null;
            OnListCleared = null;
            OnItemInserted = null;
            OnItemRemovedAt = null;
            OnValueChanged = null;
        }

        // Event to notify subscribers when the entire list changes
        public event Action<List<T>> OnValueChanged;
    }



}