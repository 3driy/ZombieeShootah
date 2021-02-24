using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lazy {
    public class Singletone<T> : MonoBehaviour where T : MonoBehaviour {
        private static T _instance;
        public static T Get {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null) {
                        var singlton = new GameObject("[SINGL] " + typeof(T));
                        _instance = singlton.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }
    }
}