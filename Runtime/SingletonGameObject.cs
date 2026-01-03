using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BJ
{
    public abstract class SingletonGameObject<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T instance;
        public static T Instance { get { return instance; } }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(this.gameObject);
                instance = this.gameObject.GetComponent<T>();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}
