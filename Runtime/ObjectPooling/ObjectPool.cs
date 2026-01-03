using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

namespace BJ
{
    /**
     * @brief Object pools are collections of objects that can be activated and deactivated without the expense of Instantiate calls.
     */
    [ExecuteAlways]
    public class ObjectPool : MonoBehaviour
    {
        [Header("Object Configuration")]
        [SerializeField]
        private GameObject pooledObject;

        [Space]

        [Header("Creation Behavior")]
        [SerializeField]
        private CreateTime createTime;
        [SerializeField]
        private int initialCreationAmmount = 10;

        [Header("Expansion Behavior")]
        [SerializeField]
        private ExpansionPolicy expansionPolicy;

        [SerializeField]
        [Tooltip("Will serve as the ammount to increase the pool by if Linear is slected (must be int) or the multiplication factor if MULTIPLY is selected (rounded to an int, must be > 1)")]
        private float coefficient = 1.0f;

        [Header("Shrink Behavior")]
        [SerializeField]
        [Tooltip("Shrink Policy is only applied when an item is pooled.")]
        private ShrinkPolicy shrinkPolicy;

        [Tooltip("Only applies for SHRINK_LOW_DEMAND policy")]
        [SerializeField]
        private float lowDemandPercentage = 0.0f;

        private Queue<GameObject> pooledObjects;
        // Includes the theoretical number of active objects as well even though we don't track it
        private int capacity = 0;

#if UNITY_EDITOR
        private GameObject lastPrefab;
        private void Update()
        {
            if (!Application.isPlaying)
            {
                if (pooledObjects == null)
                {
                    pooledObjects = new Queue<GameObject>();
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        pooledObjects.Enqueue(transform.GetChild(i).gameObject);
                    }
                }
                if (createTime == CreateTime.EDITOR)
                {
                    if (lastPrefab == null)
                    {
                        lastPrefab = pooledObject;
                    }
                    if (lastPrefab != pooledObject)
                    {
                        DestroyPool();
                        ExpandPool(initialCreationAmmount);
                    }
                    if (createTime == CreateTime.EDITOR && pooledObjects.Count != initialCreationAmmount)
                    {
                        if (initialCreationAmmount > pooledObjects.Count)
                        {
                            ExpandPool(initialCreationAmmount - pooledObjects.Count);
                        }
                        else
                        {
                            while (pooledObjects.Count > initialCreationAmmount)
                            {
                                GameObject scrap = DePool();
                                DestroyImmediate(scrap);
                            }
                        }
                    }
                }
                else
                {
                    if (pooledObjects.Count > 0)
                    {
                        DestroyPool();
                    }
                }
            }
        }

        /**
         * @brief Helper for editor functionality to quckly destroy an entire object pool for recreation.
         */
        private void DestroyPool()
        {
            pooledObjects.Clear();
            capacity = 0;

            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
#endif

        private void Awake()
        {
            if (Application.isPlaying)
            {
                pooledObjects = new Queue<GameObject>();

                if (createTime == CreateTime.ON_AWAKE)
                {
                    ExpandPool(initialCreationAmmount);
                }
                else if (createTime == CreateTime.EDITOR)
                {
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        pooledObjects.Enqueue(transform.GetChild(i).gameObject);
                    }
                }
            }
        }

        private void Start()
        {
            if (Application.isPlaying)
            {
                if (createTime == CreateTime.ON_START)
                {
                    ExpandPool(initialCreationAmmount);
                }
            }
        }

        /**
         * @brief Expands a pool by the specified amount.
         * @param amount The number of new items to add to the pool.
         */
        private void ExpandPool(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogError("Tried to expand the pool by an impossible amount, this is a break");
                return;
            }
            capacity += amount;
            for (int i = 0; i < amount; i++)
            {
                GameObject newItem = GameObject.Instantiate(pooledObject);
                newItem.transform.SetParent(transform);
                pooledObjects.Enqueue(newItem);
                newItem.SetActive(false);
            }
        }

        /**
         * @brief Retrieves an object from the pool. It will be returned activated but with no further modifications.
         * @return The next GameObject in the pool to be used.
         */
        public GameObject DePool()
        {
            if (pooledObjects.Count == 0)
            {
                switch (expansionPolicy)
                {
                    case ExpansionPolicy.AS_NEEDED:
                        ExpandPool(1);
                        break;
                    case ExpansionPolicy.LINEAR:
                        ExpandPool(Mathf.RoundToInt(coefficient));
                        break;
                    case ExpansionPolicy.MULTIPLY:
                        float newCapacity = (float)capacity * (coefficient - 1);
                        // Happens when capacity = 0.
                        if (newCapacity == 0)
                        {
                            newCapacity = coefficient;
                        }
                        ExpandPool(Mathf.RoundToInt(newCapacity));
                        break;
                    case ExpansionPolicy.DENY:
                        return null;
                }
            }

            GameObject next = pooledObjects.Dequeue();
            next.SetActive(true);
            return next;
        }

        /**
         * @brief Returns an object to the pool. It will be deactvated and it's parent will be set to the local storage game object.
         * @param returned The game object returned to the pool.
         */
        public void RePool(GameObject returned)
        {
            if (shrinkPolicy == ShrinkPolicy.KEEP_TO_INITIAL && capacity > initialCreationAmmount)
            {
                Destroy(returned.gameObject);
                capacity--;
            }
            else if (shrinkPolicy == ShrinkPolicy.SHRINK_LOW_DEMAND && pooledObjects.Count / capacity > lowDemandPercentage)
            {
                Destroy(returned.gameObject);
                capacity--;
            }
            else
            {
                returned.transform.SetParent(transform);
                pooledObjects.Enqueue(returned);
                returned.SetActive(false);
            }
        }
    }
}

public enum CreateTime
{
    // An editor script will create the object pool as needed
    EDITOR,
    // The objects will be created in the Awake function
    ON_AWAKE,
    // The objects will be created in the Start function
    ON_START,
    // Objects will only be created when they are requested
    ON_DEMAND
}

public enum ExpansionPolicy
{
    // If an object is requested that exceeds the pool it will be created
    AS_NEEDED,
    // If an object is requested that exceeds the pool null will be returned
    DENY,
    // If an object is requested that exceeds the pool, 'coefficient' items will be created, then one will be returned
    LINEAR,
    // If an object is requested that exceeds the pool, the pool size will be multipleid to 'coefficient' times the current size, then one will be returned
    MULTIPLY
}

public enum ShrinkPolicy
{
    // If an object is repooled no action will be taken
    KEEP_ALL,
    // If an object is repooled and there are currently more than the initial number of items in the pool, it will be destroyed
    KEEP_TO_INITIAL,
    // If an object is repooled and less than lowDemandPercentage are in use, the object will be destroyed
    SHRINK_LOW_DEMAND
}
