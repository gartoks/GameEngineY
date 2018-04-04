using System;
using System.Collections.Generic;

namespace GameEngine.Utility.Extensions {
    public static class CollectionExtensions {

        public static Dictionary<K, V> AddToDictionary<K, V>(this (K, V)[] set, Dictionary<K, V> dictionary = null, Func<K, V, bool> dataFilter = null) {
            if (dictionary == null)
                dictionary = new Dictionary<K, V>();

            foreach ((K key, V value) data in set) {
                if (dictionary.ContainsKey(data.key))
                    continue;

                if (dataFilter != null && !dataFilter(data.key, data.value))
                    continue;

                dictionary[data.key] = data.value;
            }

            return dictionary;
        }

        public static (K, V) ToTuple<K, V>(this KeyValuePair<K, V> pair) => (pair.Key, pair.Value);

        public static void AddThreadSafely<T>(this ICollection<T> collection, T item) {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            lock (collection) {
                collection.Add(item);
            }
        }

        public static void RemoveThreadSafely<T>(this ICollection<T> collection, T item) {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            lock (collection) {
                collection.Remove(item);
            }
        }

        public static bool TryTake<T>(this ICollection<T> collection, out T item) {
            foreach (T t in collection) {
                item = t;
                return true;
            }

            item = default(T);
            return false;
        }

        public static void AddThreadSafely<K, V>(this IDictionary<K, V> collection, K key, V value) {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            lock (collection) {
                collection.Add(key, value);
            }
        }

        public static void RemoveThreadSafely<K, V>(this IDictionary<K, V> collection, K key) {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            lock (collection) {
                collection.Remove(key);
            }
        }

        public static V TakeThreadSafely<K, V>(this IDictionary<K, V> collection, K key) {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            V value;
            lock (collection) {
                value = collection[key];
                collection.Remove(key);
            }

            return value;
        }
    }
}