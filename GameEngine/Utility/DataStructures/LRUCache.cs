using System.Collections.Generic;

namespace GameEngine.Utility.DataStructures {
    public sealed class LRUCache<K, V> {

        public readonly int Capacity;

        private readonly Dictionary<K, LinkedListNode<(K, V)>> cacheMap;
        private readonly LinkedList<(K, V)> lruList;

        public LRUCache(int capacity) {
            Capacity = capacity;

            this.cacheMap = new Dictionary<K, LinkedListNode<(K, V)>>();
            this.lruList = new LinkedList<(K, V)>();
        }

        public V Get(K key) {
            if (!cacheMap.TryGetValue(key, out LinkedListNode < (K key, V value) > node))
                return default(V);

            V value = node.Value.value;
            lruList.Remove(node);
            lruList.AddLast(node);

            return value;
        }

        public void Add(K key, V value) {
            if (cacheMap.Count >= Capacity)
                RemoveFirst();

            (K key, V value) cacheItem = (key, value);
            LinkedListNode<(K, V)> node = new LinkedListNode<(K, V)>(cacheItem);
            lruList.AddLast(node);
            cacheMap.Add(key, node);
        }

        private void RemoveFirst() {
            LinkedListNode<(K key, V value)> node = lruList.First;
            lruList.RemoveFirst();

            cacheMap.Remove(node.Value.key);
        }

        public int Size => cacheMap.Count;
    }
}