using Ship;
using System;
using System.Collections.Generic;

namespace GameSystems
{
    public class ObjectsStorage : GameSystemBase
    {
        public readonly FastList<ShipData> ShipsData = new(200);
        public readonly FastList<ShipView> ShipsView = new(200);
        public int PlayerShipIndex { get; private set; } = -1;

        private readonly List<ShipInstance> _removeQueue = new(50);
        private readonly List<ShipInstance> _addQueue = new(50);
        protected override void Init()
        {

        }

        protected override void Subscribe()
        {
            EventBus.SpawnShip += OnSpawnShip;
            EventBus.RemoveShip += OnDestroyShip;

            GameFlow.PostUpdateTick += UpdateCollections;
        }

        protected override void Unsubscribe()
        {
            EventBus.SpawnShip -= OnSpawnShip;
            EventBus.RemoveShip -= OnDestroyShip;

            GameFlow.PostUpdateTick -= UpdateCollections;
        }

        private void UpdateCollections()
        {
            foreach (var ship in _removeQueue)
            {
                RemoveShip(ship.ShipData.Index);
            }

            _removeQueue.Clear();

            foreach (var ship in _addQueue)
            {
                AddShip(ship);
            }

            _addQueue.Clear();
        }

        private void OnSpawnShip(ShipInstance shipInstance)
        {
            _addQueue.Add(shipInstance);
        }

        private void OnDestroyShip(ShipInstance shipInstance)
        {
            _removeQueue.Add(shipInstance);
        }

        private void AddShip(ShipInstance shipInstance)
        {
            ShipsData.Add(shipInstance.ShipData);
            ShipsView.Add(shipInstance.View);
            var lastIndex = ShipsData.Count - 1;
            ref var shipData = ref shipInstance.ShipData;
            shipData.Index = lastIndex;

            if (shipInstance.ShipData.IsPlayer)
            {
                PlayerShipIndex = lastIndex;
            }
        }

        private void RemoveShip(int index)
        {
            int lastIndex = ShipsData.Count - 1;

            if (index != lastIndex)
            {
                ref var movedData = ref ShipsData[lastIndex];
                var movedView = ShipsView[lastIndex];

                ShipsData[index] = movedData;
                ShipsView[index] = movedView;

                movedData.Index = index;
            }

            if (PlayerShipIndex == lastIndex)
            {
                PlayerShipIndex = index;
            }
            else if (PlayerShipIndex == index)
            {
                PlayerShipIndex = -1;
            }

            ShipsData.RemoveAt(lastIndex);
            ShipsView.RemoveAt(lastIndex);
        }
    }
}


// быстрый потому что может возвращать ref для структур и даже использовать foreach (ref var item in collection)
public class FastList<T> where T : struct
{
    private T[] _items;

    public int Count { get; private set; }

    public int Capacity => _items.Length;

    public FastList(int capacity = 4)
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
        int last = Count - 1;

        if (index != last)
        {
            _items[index] = _items[last];
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

