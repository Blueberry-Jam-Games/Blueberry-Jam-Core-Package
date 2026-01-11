using UnityEngine;
using System.Collections.Generic;

namespace BJ
{
    [System.Serializable]
    public class SerializableHashMap<K, V> : ISerializationCallbackReceiver
    {
        public static implicit operator Dictionary<K, V>(SerializableHashMap<K, V> serializable)
        {
            return serializable.dictionary;
        }

        [System.Serializable]
        protected struct StoredItem<Ki, Vi>
        {
            [SerializeField]
            public Ki key;

            [SerializeField]
            public Vi value;
        }

        [SerializeField]
        private List<StoredItem<K, V>> serialization;

        private Dictionary<K, V> dictionary;

        public void OnBeforeSerialize()
        {
            serialization.Clear();

            foreach (var kvp in dictionary)
            {
                serialization.Add (new StoredItem<K, V>{ key = kvp.Key, value = kvp.Value });
            }
        }

        public void OnAfterDeserialize()
        {
            dictionary.Clear();

            for (int i = 0, count = serialization.Count; i < count; i++)
            {
                // Prevent duplicate key crash
                if (!dictionary.ContainsKey(serialization[i].key))
                {
                    dictionary.Add(serialization[i].key, serialization[i].value);                    
                }
            }
        }
    }
}
