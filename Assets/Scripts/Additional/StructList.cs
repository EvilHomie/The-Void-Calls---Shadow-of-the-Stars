using System;

//может возвращать ref для структур и даже использовать foreach (ref var item in collection)
public class StructList<T> where T : struct
{
    private T[] _items;

    public int Count { get; private set; }

    public int Capacity => _items.Length;

    public StructList(int capacity = 4)
    {
        if (capacity <= 4)
        {
            capacity = 4;
        }

        _items = new T[capacity];
        Count = 0;
    }

    public ref T this[int index]
    {
        get
        {
            CheckIndex(index);
            return ref _items[index];
        }
    }

    public void Add(in T item)
    {
        if (Count >= _items.Length)
            Resize(_items.Length * 2);

        _items[Count] = item;
        Count++;
    }

    // по сути вернуть пустышку из коллекции для заполнения
    public ref T Emplace()
    {
        if (Count >= _items.Length)
        {
            Resize(_items.Length * 2);
        }

        return ref _items[Count++];
    }

    public void RemoveAt(int index)
    {
        int lastIndex = Count - 1;

        if (index != lastIndex)
        {
            ref var movedItem = ref _items[lastIndex];
            _items[index] = movedItem;
        }

        Count--;
    }

    private void CheckIndex(int index)
    {
        if (index < 0 || index >= Count)
        {
            throw new IndexOutOfRangeException();
        }
    }

    public void Clear()
    {
        Count = 0;
    }

    private void Resize(int newCapacity)
    {
        var newArray = new T[newCapacity];
        Array.Copy(_items, newArray, Count);
        _items = newArray;
    }

    public RefEnumerator GetEnumerator()
    {
        return new RefEnumerator(_items, Count);
    }

    public ref struct RefEnumerator
    {
        private readonly T[] _items;
        private readonly int _count;
        private int _index;

        public RefEnumerator(T[] items, int count)
        {
            _items = items;
            _count = count;
            _index = -1;
        }

        public bool MoveNext()
        {
            _index++;
            return _index < _count;
        }

        public ref T Current => ref _items[_index];
    }
}
